using AutoMapper;
using Core.DTOs;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Services
{
    public class ChatService : IChatService
    {
        #region Constructor and Dependencies

        private readonly IChatRepository _chatRepository;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _environment;

        public ChatService(IChatRepository chatRepository, IMapper mapper, IWebHostEnvironment environment)
        {
            _chatRepository = chatRepository;
            _mapper = mapper;
            _environment = environment;

        }

        #endregion

        #region Send Message

        public async Task<bool> SendMessageAsync(ChatMessage message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            message.Timestamp = DateTime.UtcNow;

            var result = await _chatRepository.AddMessageAsync(message);
            return result > 0;
        }

        #endregion

        #region Get Messages

        public async Task<IEnumerable<ChatMessageDto>> GetMessagesAsync(string senderUserId, string receiverUserId)
        {
            var messages = await _chatRepository.GetChatMessagesAsync(senderUserId, receiverUserId);
            return _mapper.Map<IEnumerable<ChatMessageDto>>(messages);
        }

        #endregion

        #region Get All Messages

        public async Task<List<ChatMessageDto>> GetAllMessagesAsync(string currentUserId)
        {
            var messages = await _chatRepository.GetMessagesAsync(currentUserId);

            if (messages == null || !messages.Any())
            {
                return new List<ChatMessageDto>(); // Or handle as per your application logic
            }

            var messageDtos = messages.Select(m => new ChatMessageDto
            {
                Content = m.Content,
                SenderUserId = m.SenderUserId,
                ReceiverUserId = m.ReceiverUserId,
                MediaUrl = m.MediaUrl,
                Timestamp = m.Timestamp,
            }).ToList();

            return messageDtos;
        }

        #endregion

        #region Save Media File

        public Tuple<int, string> SaveMediaFile(IFormFile mediaFile)
        {
            try
            {
                var contentPath = _environment.ContentRootPath;
                var uploadFolder = Path.Combine(contentPath, "wwwroot", "Uploads");

                if (!Directory.Exists(uploadFolder))
                {
                    Directory.CreateDirectory(uploadFolder);
                }

                var fileExtension = Path.GetExtension(mediaFile.FileName);
                var allowedExtensions = new string[] { ".jpg", ".png", ".jpeg", ".pdf", ".mp4", ".wav", ".mp3" };

                if (!allowedExtensions.Contains(fileExtension.ToLower()))
                {
                    string allowedExtensionsStr = string.Join(", ", allowedExtensions);
                    string errorMessage = $"Only {allowedExtensionsStr} extensions are allowed";
                    return new Tuple<int, string>(0, errorMessage);
                }

                string uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
                var filePath = Path.Combine(uploadFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    mediaFile.CopyTo(fileStream);
                }

                return new Tuple<int, string>(1, uniqueFileName);
            }
            catch (Exception ex)
            {
                return new Tuple<int, string>(0, $"Error occurred: {ex.Message}");
            }
        }

        #endregion


    }
}
