using AutoMapper;
using Core.DTOs;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Services;
using System.Linq;
using System.Security.Claims;
namespace TestBridge.Controllers
{
[Authorize]
    public class GroupController : ApiBaceController
    {
        #region Parameters
        private readonly UserManager<AppUser> _userManager;
        private readonly IGroupService _groupService;
        private readonly IMapper _mapper;
        private readonly IFriendshipService _friendshipService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        #endregion

        #region Constructor
        public GroupController(UserManager<AppUser> userManager, IGroupService groupService, IMapper mapper, IFriendshipService friendshipService, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _groupService = groupService;
            _mapper = mapper;
            _friendshipService = friendshipService;
            _httpContextAccessor = httpContextAccessor;
        }
        #endregion

        #region CreateGroup
        [HttpPost("create-group")]
        public async Task<IActionResult> CreateGroup([FromBody] ChatGroupDto chatGroupDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var currentUser = await _userManager.FindByIdAsync(userId);
                if (currentUser == null)
                {
                    return Unauthorized(new { Message = "User is not authenticated." });
                }
                chatGroupDto.AdminUserId = userId;
                if (chatGroupDto.MemberUsernames == null || !chatGroupDto.MemberUsernames.Any())
                {
                    return BadRequest(new { Message = "Member usernames are required." });
                }
                if (string.IsNullOrEmpty(chatGroupDto.Name))
                {
                    return BadRequest(new { Message = "Group name is required." });
                }
                var friends = await _friendshipService.GetFriendsUsernamesAsync(userId);
                foreach (var username in chatGroupDto.MemberUsernames)
                {
                    if (!friends.Contains(username))
                    {
                        return BadRequest(new { Message = $"User '{username}' is not your friend." });
                    }
                }
                var result = await _groupService.CreateGroupAsync(chatGroupDto);
                if (result)
                {
                    return Ok(new { Message = "Group created successfully." });
                }
                else
                {
                    return BadRequest(new { Message = "Failed to create group." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = ex.Message });
            }
        }
        #endregion

        #region AddUserToGroup
        [HttpPost("AddUserToGroup/{groupId}")]
        public async Task<IActionResult> AddUserToGroup(int groupId, [FromForm] AddUserToGroupDto addUserToGroupDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var currentUser = await _userManager.FindByIdAsync(userId);
                if (currentUser == null)
                {
                    return Unauthorized(new { Message = "User is not authenticated." });
                }
                var isGroupAdmin = await _groupService.IsGroupAdmin(groupId, userId);
                if (!isGroupAdmin)
                {
                    return Unauthorized(new { Message = "You do not have permission to add users to this group." });
                }
                var userToAdd = await _userManager.FindByNameAsync(addUserToGroupDto.username);
                if (userToAdd == null)
                {
                    return BadRequest(new { Message = $"User with username '{addUserToGroupDto.username}' not found." });
                }
                var friends = await _friendshipService.GetFriendsAsync(userId);
                if (!friends.Any(f => f.UserName == userToAdd.UserName))
                {
                    return BadRequest(new { Message = $"User '{userToAdd.UserName}' is not your friend." });
                }

                var result = await _groupService.AddUserToGroupAsync(groupId, userToAdd.Id);
                if (result)
                {
                    return Ok(new { Message = "User added to group successfully." });
                }
                else
                {
                    return BadRequest(new { Message = "Failed to add user to group." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = ex.Message });
            }
        }
        #endregion

        #region SendMessageToGroup
        [HttpPost("SendMessageToGroup/{groupId}")]
        public async Task<IActionResult> SendMessageToGroup(int groupId, [FromForm] ChatMessageDto messageDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentUser = await _userManager.FindByIdAsync(userId);
            if (currentUser == null)
            {
                return Unauthorized(new { Message = "User is not authenticated." });
            }
            messageDto.SenderUserId = userId;
            messageDto.AppUserId = userId;
            var chatMessage = _mapper.Map<ChatMessage>(messageDto);
            chatMessage.Timestamp = DateTime.UtcNow;
            if (messageDto.MediaFile != null)
            {
                var (status, filePath) = _groupService.SaveMediaFile(messageDto.MediaFile);
                if (status == 1)
                {
                    chatMessage.MediaUrl = filePath;
                }
            }
            chatMessage.AppUserId = userId;
            var result = await _groupService.SendMessageToGroupAsync(groupId, chatMessage);
            if (result)
            {
                return Ok(new { Message = "Message sent to group successfully." });
            }
            else
            {
                return BadRequest(new { Message = "Failed to send message to group." });
            }
        }
        #endregion

        #region GetAllGroups
        [HttpGet("all-groups")]
        public async Task<IActionResult> GetAllGroups()
        {
            try
            {
                var groups = await _groupService.GetAllGroupsAsync();
                return Ok(groups);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = ex.Message });
            }
        }
        #endregion

        #region GetGroupMessages
        [HttpGet("GetMessages")]
        public async Task<IActionResult> GetGroupMessages(int groupId)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var messages = await _groupService.GetMessagesForGroupAsync(groupId);
                var messageDtos = _mapper.Map<List<ChatMessageDto>>(messages);
                return Ok(messageDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = ex.Message });
            }
        } 
        #endregion
    }
}
