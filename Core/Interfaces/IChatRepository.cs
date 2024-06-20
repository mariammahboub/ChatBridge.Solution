using Core.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IChatRepository
    {

        Task<int> AddMessageAsync(ChatMessage message);
        Task<IEnumerable<ChatMessage>> GetChatMessagesAsync(string senderUserId, string receiverUserId); 
        Task<List<ChatMessage>> GetMessagesAsync(string currentUserId);
        Task<int> AddGroupAsync(ChatGroup group);
        Task<int> AddGroupMemberAsync(GroupMember groupMember);
        Task<IEnumerable<ChatGroup>> GetUserGroupsAsync(string userId);
        Task<ChatGroup> GetGroupByIdAsync(int groupId); 
        Task<IEnumerable<ChatGroup>> GetAllGroupsAsync();
        Task<bool> ArchiveChatAsync(int groupId);
        Task<bool> ArchiveGroupAsync(int groupId);
        Task<ChatMessage> GetChatByIdAsync(int chatId);
        Task<List<ChatMessage>> GetUserChatsAsync(string userId);



    }
}
