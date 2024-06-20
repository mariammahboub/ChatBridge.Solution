using Core.DTOs;
using Core.Entities;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IGroupService
    {
      //  Task<bool> IsMemberOfGroup(int groupId, string userId);
        Task<List<ChatMessage>> GetMessagesForGroupAsync(int groupId);
        Task<bool> CreateGroupAsync(ChatGroupDto chatGroupDto);
        Task<bool> AddUserToGroupAsync(int groupId, string userId);
        Task<IEnumerable<ChatGroupDto>> GetUserGroupsAsync(string userId);
        Task<bool> SendMessageToGroupAsync(int groupId, ChatMessage messageDto);
        Task<bool> IsGroupAdmin(int groupId, string userId);
        Task<ChatGroup> GetGroupByIdAsync(int groupId);
        Task<List<string>> GetFriendsUsernamesAsync(string userId);
        Task<IEnumerable<ChatGroupDto>> GetAllGroupsAsync();
        Tuple<int, string> SaveMediaFile(IFormFile mediaFile);
    }
}
