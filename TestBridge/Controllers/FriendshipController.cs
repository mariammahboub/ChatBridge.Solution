// FriendshipController.cs

using Core.DTOs;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace TestBridge.Controllers
{
    [Authorize]
    public class FriendshipController : ApiBaceController
    {
        #region Parameters
        private readonly UserManager<AppUser> _userManager;
        private readonly IFriendshipService _friendshipService; 
        #endregion

        #region Constructor
        public FriendshipController(UserManager<AppUser> userManager, IFriendshipService friendshipService)
        {
            _userManager = userManager;
            _friendshipService = friendshipService;
        } 
        #endregion

        #region GetFriends
        [HttpGet("friends")]
        public async Task<IActionResult> GetFriends()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { Message = "User is not authenticated." });
            }
            var friends = await _friendshipService.GetFriendsAsync(userId);
            var friendDtos = friends.Select(f => new UserDto
            {
                UserName = f.UserName,
            }).ToList();

            return Ok(friendDtos);
        } 
        #endregion

        #region are-friends
        [HttpGet("are-friends")]
        public async Task<IActionResult> AreFriends(string userId1, string userId2)
        {
            var areFriends = await _friendshipService.AreFriendsAsync(userId1, userId2);
            return Ok(new { AreFriends = areFriends });
        } 
        #endregion

        #region GetFriendsUsernames
        [HttpGet("friends-usernames")]
        public async Task<IActionResult> GetFriendsUsernames(string userId)
        {
            var friendsUsernames = await _friendshipService.GetFriendsUsernamesAsync(userId);
            return Ok(friendsUsernames);
        } 
        #endregion

        #region Getfriends
        [HttpGet("Getfriends")]
        public async Task<IActionResult> GetFriends(string userId)
        {
            var friends = await _friendshipService.GetFriendsAsync(userId);
            return Ok(friends);
        } 
        #endregion
    }
}
