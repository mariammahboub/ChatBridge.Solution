using AutoMapper;
using Core.DTOs;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace TestBridge.Controllers
{
    [Authorize]
    public class ChatController : ApiBaceController
    {
        #region Parameters
        private readonly UserManager<AppUser> _userManager;
        private readonly IChatService _chatService;
        private readonly IFriendshipService _friendshipService;
        private readonly IMapper _mapper;
        private readonly ILogger<ChatController> _logger;
        private readonly IChatRepository _chatRepository;
        #endregion

        #region Constructor
        public ChatController(UserManager<AppUser> userManager, IChatService chatService, IFriendshipService friendshipService, IMapper mapper, ILogger<ChatController> logger, IChatRepository chatRepository)
        {
            _userManager = userManager;
            _chatService = chatService;
            _friendshipService = friendshipService;
            _mapper = mapper;
            _logger = logger;
            _chatRepository = chatRepository;
        }
        #endregion

        #region SendMessage
        [HttpPost("send-message")]
        public async Task<IActionResult> SendMessage([FromForm] ChatMessageDto messageDto)
        {
            var senderUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var senderUser = await _userManager.FindByIdAsync(senderUserId);
            if (senderUser == null)
            {
                return Unauthorized(new { Message = "User is not authenticated." });
            }
            var receiverUser = await _userManager.FindByNameAsync(messageDto.UserName);

            if (receiverUser == null)
            {
                return NotFound(new { Message = "Receiver not found." });
            }
            var chatMessage = _mapper.Map<ChatMessage>(messageDto);
            chatMessage.SenderUserId = senderUserId;
            chatMessage.ReceiverUserId = receiverUser.Id; // Set ReceiverUserId
            chatMessage.AppUserId = senderUserId;
            chatMessage.Timestamp = DateTime.UtcNow;
            if (messageDto.MediaFile != null)
            {
                var (status, filePath) = _chatService.SaveMediaFile(messageDto.MediaFile);

                if (status == 1)
                {
                    chatMessage.MediaUrl = filePath; // Assuming MediaUrl is where you store the file path
                }
            }
            var result = await _chatService.SendMessageAsync(chatMessage);
            if (result)
            {
                return Ok(new { Message = "Message sent successfully." });
            }
            else
            {
                return BadRequest(new { Message = "Failed to send message." });
            }
        }
        #endregion

        #region GetMessages
        [HttpGet("get-messages")]
        public async Task<IActionResult> GetMessages(string username)
        {
            var senderUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (senderUserId == null)
            {
                return Unauthorized(new { Message = "User is not authenticated." });
            }

            var receiverUser = await _userManager.FindByNameAsync(username);
            if (receiverUser == null)
            {
                return NotFound(new { Message = "Receiver not found." });
            }
            var areFriends = await _friendshipService.AreFriendsAsync(senderUserId, receiverUser.Id);

            var messages = await _chatService.GetMessagesAsync(senderUserId, receiverUser.Id);

            return Ok(messages);
        }
        #endregion

        #region GetAllMessages
        [HttpGet("GetAllMessages")]
        public async Task<IActionResult> GetAllMessages()
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(currentUserId))
            {
                return Unauthorized(new { Message = "User is not authenticated." });
            }
            var messages = await _chatService.GetAllMessagesAsync(currentUserId);
            if (messages == null)
            {
                return NotFound(new { Message = "No messages found." });
            }

            return Ok(messages);
        } 
        #endregion

    }
}
