using Core.DTOs;
using Core.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IChatService
    {
        Task<bool> SendMessageAsync(ChatMessage message);
        Task<IEnumerable<ChatMessageDto>> GetMessagesAsync(string senderUserId, string receiverUserId);
        Task<List<ChatMessageDto>> GetAllMessagesAsync(string currentUserId);
        Tuple<int, string> SaveMediaFile(IFormFile mediaFile);

    


    }
}
