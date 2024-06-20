using AutoMapper;
using Core.DTOs;
using Core.Entities;
using Microsoft.Extensions.Configuration;

namespace TestBridge.Helpers
{
    public class ChatMediaUrlResolver : IValueResolver<ChatMessage, ChatMessageDto, string?>
    {
        private readonly IConfiguration _configuration;

        public ChatMediaUrlResolver(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string Resolve(ChatMessage source, ChatMessageDto destination, string? destMember, ResolutionContext context)
        {
            if (!string.IsNullOrEmpty(source.MediaUrl))
            {
                return $"{_configuration["ApiBaseUrl"]}/{source.MediaUrl}";
            }
            return string.Empty;
        }
    }
}
