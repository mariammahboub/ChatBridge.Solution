using Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IFriendshipService
    {
        Task<bool> AreFriendsAsync(string userId1, string userId2);
        Task<List<AppUser>> GetFriendsAsync(string userId); // New method for getting friends
        Task<List<string>> GetFriendsUsernamesAsync(string userId); // Add this method

    }
}
