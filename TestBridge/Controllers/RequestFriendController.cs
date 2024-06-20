using Core.DTOs;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace TestBridge.Controllers
{
    [Authorize]
    public class RequestFriendController : ApiBaceController
    {
        #region Constructor and Dependencies

        private readonly UserManager<AppUser> _userManager;
        private readonly IFriendRequestService _friendRequestService;

        public RequestFriendController(UserManager<AppUser> userManager, IFriendRequestService friendRequestService)
        {
            _userManager = userManager;
            _friendRequestService = friendRequestService;
        }

        #endregion

        #region Search User

        [HttpGet("search")]
        public async Task<IActionResult> SearchUserByUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return BadRequest(new { Message = "Username must be provided." });
            }

            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                return NotFound(new { Message = "User not found." });
            }

            var userDto = new UserDto
            {
                UserName = user.UserName,
            };

            return Ok(userDto);
        }

        #endregion

        #region Send Friend Request

        [HttpPost("request")]
        public async Task<IActionResult> SendFriendRequest([FromBody] FriendRequestDto friendRequestDto)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(currentUserId))
            {
                return Unauthorized(new { Message = "User is not authenticated." });
            }
            if (currentUserId == friendRequestDto.AppUserId)
            {
                return BadRequest(new { Message = "You cannot send a friend request to yourself." });
            }
            var existingRequest = await _friendRequestService.GetFriendRequestAsync(currentUserId, friendRequestDto.AppUserId);
            if (existingRequest != null)
            {
                return BadRequest(new { Message = "You have already sent a friend request to this user." });
            }
            var targetUser = await _userManager.FindByNameAsync(friendRequestDto.Username);
            if (targetUser == null)
            {
                return NotFound(new { Message = "User not found." });
            }

            var result = await _friendRequestService.SendFriendRequestAsync(currentUserId, targetUser.Id);
            if (!result)
            {
                return BadRequest(new { Message = "Failed to send friend request." });
            }

            return Ok(new { Message = "Friend request sent successfully." });
        }

        #endregion

        #region Accept Friend Request

        [HttpPost("accept")]
        public async Task<IActionResult> AcceptFriendRequest(int requestId)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(currentUserId))
            {
                return Unauthorized(new { Message = "User is not authenticated." });
            }

            var friendRequest = await _friendRequestService.GetFriendRequestByIdAsync(requestId);
            if (friendRequest == null)
            {
                return NotFound(new { Message = "Friend request not found." });
            }

            if (friendRequest.ReceiverUserId != currentUserId)
            {
                return BadRequest(new { Message = "You are not authorized to accept this friend request." });
            }

            var result = await _friendRequestService.AcceptFriendRequestAsync(requestId);
            if (!result)
            {
                return BadRequest(new { Message = "Friend request could not be accepted." });
            }

            return Ok(new { Message = "Friend request accepted successfully." });
        }

        #endregion

        #region Reject Friend Request

        [HttpPost("reject")]
        public async Task<IActionResult> RejectFriendRequest(int requestId)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(currentUserId))
            {
                return Unauthorized(new { Message = "User is not authenticated." });
            }

            var friendRequest = await _friendRequestService.GetFriendRequestByIdAsync(requestId);
            if (friendRequest == null)
            {
                return NotFound(new { Message = "Friend request not found." });
            }

            if (friendRequest.ReceiverUserId != currentUserId)
            {
                return BadRequest(new { Message = "You are not authorized to reject this friend request." });
            }

            var result = await _friendRequestService.RejectFriendRequestAsync(requestId);
            if (!result)
            {
                return BadRequest(new { Message = "Friend request could not be rejected." });
            }

            return Ok(new { Message = "Friend request rejected successfully." });
        }

        #endregion

        #region Get Friend Requests

        [HttpGet("requests")]
        public async Task<IActionResult> GetFriendRequestsForUser()
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var friendRequests = await _friendRequestService.GetFriendRequestsForUserAsync(currentUserId);
            return Ok(friendRequests);
        }

        #endregion

        #region Get Accepted Friend Requests

        [HttpGet("accepted")]
        public async Task<IActionResult> GetAcceptedFriendRequestsForUser()
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var acceptedFriendRequests = await _friendRequestService.GetAcceptedFriendRequestsAsync(currentUserId);
            return Ok(acceptedFriendRequests);
        }

        #endregion
    }
}
