using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Repo.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services
{
    public class FriendshipService : IFriendshipService
    {
        #region Constructor and Dependencies

        private readonly AppIdentityDbContext _context;

        public FriendshipService(AppIdentityDbContext context)
        {
            _context = context;
        }

        #endregion

        #region Check if Users are Friends

        public async Task<bool> AreFriendsAsync(string userId1, string userId2)
        {
            var friendship = await _context.Friendships
                .FirstOrDefaultAsync(f => (f.User1Id == userId1 && f.User2Id == userId2 && f.Status == Friendship.FriendshipStatus.Accepted) ||
                                          (f.User1Id == userId2 && f.User2Id == userId1 && f.Status == Friendship.FriendshipStatus.Accepted));

            return friendship != null;
        }

        #endregion

        #region Get Friends' Usernames

        public async Task<List<string>> GetFriendsUsernamesAsync(string userId)
        {
            var friends = await _context.Friendships
                .Where(f => f.User1Id == userId || f.User2Id == userId) // Check both sides of the friendship
                .Select(f => f.User1Id == userId ? f.User2.UserName : f.User1.UserName) // Select the username of the friend
                .ToListAsync();

            return friends;
        }

        #endregion

        #region Get Friends' Details

        public async Task<List<AppUser>> GetFriendsAsync(string userId)
        {
            var friends1 = await _context.Friendships
                .Where(f => f.User1Id == userId && f.Status == Friendship.FriendshipStatus.Accepted)
                .Select(f => f.User2)
                .ToListAsync();

            var friends2 = await _context.Friendships
                .Where(f => f.User2Id == userId && f.Status == Friendship.FriendshipStatus.Accepted)
                .Select(f => f.User1)
                .ToListAsync();

            return friends1.Concat(friends2).ToList();
        }

        #endregion
    }
}
