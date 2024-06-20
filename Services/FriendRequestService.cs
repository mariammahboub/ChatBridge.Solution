using Core.DTOs;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Repo.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services
{
    public class FriendRequestService : IFriendRequestService
    {
        #region Constructor and Dependencies

        private readonly AppIdentityDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<FriendRequestService> _logger;

        public FriendRequestService(AppIdentityDbContext context, UserManager<AppUser> userManager, ILogger<FriendRequestService> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        #endregion

        #region Get Friend Request by Sender and Receiver

        public async Task<FriendRequest> GetFriendRequestAsync(string senderUserId, string receiverUserId)
        {
            return await _context.FriendRequests
                .FirstOrDefaultAsync(fr =>
                    fr.SenderUserId == senderUserId &&
                    fr.ReceiverUserId == receiverUserId);
        }

        #endregion

        #region Get Friend Request by Id

        public async Task<FriendRequest> GetFriendRequestByIdAsync(int requestId)
        {
            return await _context.FriendRequests
                .Include(fr => fr.AppUser)
                .FirstOrDefaultAsync(fr => fr.Id == requestId);
        }

        #endregion

        #region Send Friend Request

        public async Task<bool> SendFriendRequestAsync(string senderUserId, string receiverUserId)
        {
            if (string.IsNullOrEmpty(senderUserId))
            {
                throw new ArgumentException("SenderUserId cannot be null or empty", nameof(senderUserId));
            }

            var senderUser = await _context.Users.FindAsync(senderUserId);
            if (senderUser == null)
            {
                throw new ArgumentException("Invalid senderUserId");
            }

            var existingRequest = await _context.FriendRequests
                .FirstOrDefaultAsync(fr => fr.SenderUserId == senderUserId && fr.ReceiverUserId == receiverUserId);

            if (existingRequest != null)
            {
                return false;
            }

            var friendRequest = new FriendRequest
            {
                SenderUserId = senderUserId,
                ReceiverUserId = receiverUserId,
                AppUserId = senderUserId,
                Status = FriendRequestStatus.Pending,
            };

            _context.FriendRequests.Add(friendRequest);
            _logger.LogInformation("Adding FriendRequest: {@FriendRequest}", friendRequest);

            var result = await _context.SaveChangesAsync() > 0;

            return result;
        }

        #endregion

        #region Accept Friend Request

        public async Task<bool> AcceptFriendRequestAsync(int requestId)
        {
            var friendRequest = await GetFriendRequestByIdAsync(requestId);
            if (friendRequest == null || friendRequest.Status != FriendRequestStatus.Pending)
            {
                return false;
            }

            friendRequest.Status = FriendRequestStatus.Accepted;

            var friendship = new Friendship
            {
                User1Id = friendRequest.SenderUserId,
                User2Id = friendRequest.ReceiverUserId,
                Status = Friendship.FriendshipStatus.Accepted
            };
            _context.Friendships.Add(friendship);
            return await _context.SaveChangesAsync() > 0;
        }

        #endregion

        #region Reject Friend Request

        public async Task<bool> RejectFriendRequestAsync(int requestId)
        {
            var friendRequest = await _context.FriendRequests.FindAsync(requestId);
            if (friendRequest == null || friendRequest.Status != FriendRequestStatus.Pending)
            {
                return false;
            }

            friendRequest.Status = FriendRequestStatus.Rejected;
            _context.FriendRequests.Update(friendRequest);
            var result = await _context.SaveChangesAsync();

            return result > 0;
        }

        #endregion

        #region Get Friend Requests for User

        public async Task<IEnumerable<FriendRequest>> GetFriendRequestsForUserAsync(string userId)
        {
            return await _context.FriendRequests
                .Where(fr => fr.ReceiverUserId == userId && fr.Status == FriendRequestStatus.Pending)
                .ToListAsync();
        }

        #endregion

        #region Get Accepted Friend Requests for User

        public async Task<IEnumerable<FriendRequestDto>> GetAcceptedFriendRequestsAsync(string userId)
        {
            var acceptedRequests = await _context.FriendRequests
                .Where(fr => fr.ReceiverUserId == userId && fr.Status == FriendRequestStatus.Accepted)
                .Select(fr => new FriendRequestDto
                {
                    RequestId = fr.Id,
                    SenderId = fr.SenderUserId,
                    ReceiverId = fr.ReceiverUserId,
                    Status = fr.Status,
                })
                .ToListAsync();

            return acceptedRequests;
        }

        #endregion
    }
}
