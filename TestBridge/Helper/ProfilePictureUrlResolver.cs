using AutoMapper;
using Core.DTOs;
using Core.Entities;
using Microsoft.Extensions.Configuration;
using ProfileEntity = Core.Entities.Profile;

namespace TestBridge.Helpers
{
    public class ProfilePictureUrlResolver : IValueResolver<ProfileEntity, ResponseProfileDto, string?>
    {
        private readonly IConfiguration _configuration;

        public ProfilePictureUrlResolver(IConfiguration configuration)
        
        {
            _configuration = configuration;
        }

        public string Resolve(ProfileEntity source, ResponseProfileDto destination, string? destMember, ResolutionContext context)
        {
            if (!string.IsNullOrEmpty(source.ProfilePicture))
            {
                return $"{_configuration["ApiBaseUrl"]}/{source.ProfilePicture}";
            }
            return string.Empty;
        }
    }
}
