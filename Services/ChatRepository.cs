using Core.DTOs;
using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Repo.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repo.Repositories
{
    public class ChatRepository : IChatRepository
    {
        #region Constructor and Dependencies

        private readonly AppIdentityDbContext _context;

        public ChatRepository(AppIdentityDbContext context)
        {
            _context = context;
        }

        #endregion

        #region Add Message

        public async Task<int> AddMessageAsync(ChatMessage message)
        {
            _context.ChatMessages.Add(message);
            return await _context.SaveChangesAsync();
        }

        #endregion

        #region Get Chat Messages

        public async Task<IEnumerable<ChatMessage>> GetChatMessagesAsync(string senderUserId, string receiverUserId)
        {
            return await _context.ChatMessages
                .Where(m => (m.SenderUserId == senderUserId && m.ReceiverUserId == receiverUserId) ||
                            (m.SenderUserId == receiverUserId && m.ReceiverUserId == senderUserId))
                .OrderBy(m => m.Timestamp)
                .ToListAsync();
        }

        #endregion

        #region Get Messages

        public async Task<List<ChatMessage>> GetMessagesAsync(string currentUserId)
        {
            var messages = await _context.ChatMessages
                .Where(m => m.SenderUserId == currentUserId || m.ReceiverUserId == currentUserId)
                .ToListAsync();

            return messages; // Ensure messages is not null
        }

        #endregion

        #region Add Group

        public async Task<int> AddGroupAsync(ChatGroup group)
        {
            if (group == null)
            {
                throw new ArgumentNullException(nameof(group));
            }

            _context.ChatGroups.Add(group);
            return await _context.SaveChangesAsync();
        }

        #endregion

        #region Add Group Member

        public async Task<int> AddGroupMemberAsync(GroupMember groupMember)
        {
            if (groupMember == null)
            {
                throw new ArgumentNullException(nameof(groupMember));
            }

            _context.GroupMembers.Add(groupMember);
            return await _context.SaveChangesAsync();
        }

        #endregion

        #region Get All Groups

        public async Task<IEnumerable<ChatGroup>> GetAllGroupsAsync()
        {
            return await _context.ChatGroups
                                 .Include(g => g.GroupMember) 
                                 .ToListAsync();
        }

        #endregion

        #region Get User Groups

        public async Task<IEnumerable<ChatGroup>> GetUserGroupsAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException(nameof(userId));
            }

            return await _context.GroupMembers
                                 .Where(gm => gm.AppUserId == userId)
                                 .Select(gm => gm.ChatGroup)
                                 .ToListAsync();
        }

        #endregion

        #region Archive Chat

        public async Task<bool> ArchiveChatAsync(int groupId)
        {
            var chat = await _context.ChatMessages.FirstOrDefaultAsync(c => c.Id == groupId);

            if (chat == null)
                return false; 

            chat.IsArchived = true; 
            _context.Entry(chat).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException)
            {
                return false; 
            }
        }

        #endregion

        #region Archive Group

        public async Task<bool> ArchiveGroupAsync(int groupId)
        {
            var group = await _context.ChatGroups.FirstOrDefaultAsync(g => g.Id == groupId);

            if (group == null)
                return false; 

            group.IsArchived = true; 
            _context.Entry(group).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException)
            {
                return false; 
            }
        }

        #endregion

        #region Get Group By Id

        public async Task<ChatGroup> GetGroupByIdAsync(int groupId)
        {
            return await _context.ChatGroups
                .Include(g => g.GroupMember)
                .FirstOrDefaultAsync(g => g.Id == groupId);
        }

        #endregion

        #region GetChatByIdAsync
        public async Task<ChatMessage> GetChatByIdAsync(int chatId)
        {
            return await _context.ChatMessages.FindAsync(chatId);
        }
        #endregion

        #region GetChatByIdAsync
        public async Task<List<ChatMessage>> GetUserChatsAsync(string userId)
        {
            return await _context.ChatMessages
                .Where(c => c.AppUserId == userId)
                .ToListAsync();
        } 
        #endregion
    }
}
