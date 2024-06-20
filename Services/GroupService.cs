using AutoMapper;
using Core.DTOs;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Repo.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Services
{
    public class GroupService : IGroupService
    {
        #region Constructor and Dependencies

        private readonly IChatRepository _chatRepository;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _environment;
        private readonly AppIdentityDbContext _context;

        public GroupService(IChatRepository chatRepository, IMapper mapper, IWebHostEnvironment environment, AppIdentityDbContext context)
        {
            _chatRepository = chatRepository;
            _mapper = mapper;
            _environment = environment;
            _context = context;
        }

        #endregion

        #region Get Messages for Group

        public async Task<List<ChatMessage>> GetMessagesForGroupAsync(int groupId)
        {
            var messages = await _context.ChatMessages
                .Where(m => m.ChatGroupId == groupId)
                .OrderBy(m => m.Timestamp)
                .ToListAsync();

            return messages;
        }

        #endregion

        #region Create Group

        public async Task<bool> CreateGroupAsync(ChatGroupDto chatGroupDto)
        {
            var group = new ChatGroup
            {
                Name = chatGroupDto.Name,
                AdminUserId = chatGroupDto.AdminUserId,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _chatRepository.AddGroupAsync(group);
            if (result <= 0)
            {
                return false;
            }

            foreach (var username in chatGroupDto.MemberUsernames)
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);
                if (user != null)
                {
                    var groupMember = new GroupMember
                    {
                        ChatGroupId = group.Id,
                        AppUserId = user.Id
                    };
                    await _chatRepository.AddGroupMemberAsync(groupMember);
                }
            }

            return true;
        }

        #endregion

        #region Add User to Group

        public async Task<bool> AddUserToGroupAsync(int groupId, string userId)
        {
            var groupMember = new GroupMember
            {
                ChatGroupId = groupId,
                AppUserId = userId
            };

            var result = await _chatRepository.AddGroupMemberAsync(groupMember);
            return result > 0;
        }

        #endregion

        #region Get User Groups

        public async Task<IEnumerable<ChatGroupDto>> GetUserGroupsAsync(string userId)
        {
            var groups = await _chatRepository.GetUserGroupsAsync(userId);
            var groupDtos = _mapper.Map<IEnumerable<ChatGroup>, IEnumerable<ChatGroupDto>>(groups);
            return groupDtos;
        }

        #endregion

        #region Send Message to Group

        public async Task<bool> SendMessageToGroupAsync(int groupId, ChatMessage messageDto)
        {
            var group = await _chatRepository.GetGroupByIdAsync(groupId);
            if (group == null)
            {
                throw new Exception($"Group with id {groupId} not found.");
            }

            var userId = messageDto.SenderUserId; // Ensure you have the sender's ID properly set in ChatMessageDto

            var message = new ChatMessage
            {
                ChatGroupId = groupId,
                Content = messageDto.Content,
                Timestamp = DateTime.UtcNow,
                SenderUserId = userId, // Assign the authenticated user's ID here
                ReceiverUserId = group.AdminUserId // Example: Assign receiver's ID based on your logic
            };

            var result = await _chatRepository.AddMessageAsync(message);
            return result > 0;
        }

        #endregion

        #region Check if User is Group Admin

        public async Task<bool> IsGroupAdmin(int groupId, string userId)
        {
            var group = await _chatRepository.GetGroupByIdAsync(groupId);
            return group != null && group.AdminUserId == userId;
        }

        #endregion

        #region Get Group by ID

        public async Task<ChatGroup> GetGroupByIdAsync(int groupId)
        {
            return await _context.ChatGroups
                .Include(g => g.GroupMember)
                .FirstOrDefaultAsync(g => g.Id == groupId);
        }

        #endregion

        #region Get Friends' Usernames

        public async Task<List<string>> GetFriendsUsernamesAsync(string userId)
        {
            var friends = await _context.Friendships
                .Where(f => f.User1Id == userId || f.User2Id == userId)
                .Select(f => f.User1Id == userId ? f.User2.UserName : f.User1.UserName)
                .ToListAsync();

            return friends;
        }

        #endregion

        #region Get All Groups

        public async Task<IEnumerable<ChatGroupDto>> GetAllGroupsAsync()
        {
            var groups = await _chatRepository.GetAllGroupsAsync();
            var groupDtos = _mapper.Map<IEnumerable<ChatGroup>, IEnumerable<ChatGroupDto>>(groups);
            return groupDtos;
        }

        #endregion

        #region Get All Groups (Detailed)

        public async Task<List<ChatGroup>> GetAllGroupAsync()
        {
            return await _context.ChatGroups.Include(g => g.GroupMember).ToListAsync();
        }

        #endregion

        #region Save Media File

        public Tuple<int, string> SaveMediaFile(IFormFile mediaFile)
        {
            try
            {
                var contentPath = _environment.ContentRootPath;
                var uploadFolder = Path.Combine(contentPath, "wwwroot", "Uploads");

                if (!Directory.Exists(uploadFolder))
                {
                    Directory.CreateDirectory(uploadFolder);
                }

                var fileExtension = Path.GetExtension(mediaFile.FileName);
                var allowedExtensions = new string[] { ".jpg", ".png", ".jpeg", ".pdf", ".mp4", ".wav", ".mp3" };

                if (!allowedExtensions.Contains(fileExtension.ToLower()))
                {
                    string allowedExtensionsStr = string.Join(", ", allowedExtensions);
                    string errorMessage = $"Only {allowedExtensionsStr} extensions are allowed";
                    return new Tuple<int, string>(0, errorMessage);
                }

                string uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
                var filePath = Path.Combine(uploadFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    mediaFile.CopyTo(fileStream);
                }

                return new Tuple<int, string>(1, uniqueFileName);
            }
            catch (Exception ex)
            {
                return new Tuple<int, string>(0, $"Error occurred: {ex.Message}");
            }
        }

        #endregion
    }
}
