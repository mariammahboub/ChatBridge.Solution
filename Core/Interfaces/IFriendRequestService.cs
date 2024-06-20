using Core.DTOs;
using Core.Entities;

namespace Core.Interfaces
{
    public interface IFriendRequestService
    {
        Task<FriendRequest> GetFriendRequestAsync(string senderUserId, string receiverUserId);
        Task<FriendRequest> GetFriendRequestByIdAsync(int requestId); // Renamed to have a unique signature
        Task<bool> SendFriendRequestAsync(string senderUserId, string receiverUserId);
        Task<bool> AcceptFriendRequestAsync(int requestId);
        Task<bool> RejectFriendRequestAsync(int requestId);
        Task<IEnumerable<FriendRequest>> GetFriendRequestsForUserAsync(string userId);
        Task<IEnumerable<FriendRequestDto>> GetAcceptedFriendRequestsAsync(string userId);

    }
}
