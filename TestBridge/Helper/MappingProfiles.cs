using Profile=AutoMapper.Profile;
using Core.DTOs;
using ProfileEntity = Core.Entities.Profile;
using Microsoft.Extensions.Configuration;
using System;
using Core.Entities;

namespace TestBridge.Helpers
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<ProfileEntity, ResponseProfileDto>()
                .ForMember(d => d.ProfilePictureUrl, o => o.MapFrom<ProfilePictureUrlResolver>());

            CreateMap<ProfileFormDataDto, ProfileEntity>()
                .ForMember(dest => dest.ProfilePicture, opt => opt.Ignore()) 
                .ForMember(dest => dest.AppUserId, opt => opt.MapFrom(src => src.AppUserId)); 

            CreateMap<ChatMessage, ChatMessageDto>()
                .ForMember(d => d.MediaUrl, o => o.MapFrom<ChatMediaUrlResolver>())
                .ForMember(d => d.AppUserId, opt => opt.MapFrom(src => src.AppUserId))
                .ForMember(d => d.AppUser, opt => opt.MapFrom(src => src.AppUser.UserName));

            CreateMap<ChatMessageDto, ChatMessage>()
                .ForMember(dest => dest.MediaUrl, opt => opt.MapFrom(src => src.MediaUrl))
                .ForMember(dest => dest.AppUserId, opt => opt.MapFrom(src => src.AppUserId))
                .ForMember(dest => dest.AppUser, opt => opt.Ignore()); 

            CreateMap<ChatGroup, ChatGroupDto>()
                .ForMember(dest => dest.AdminUserId, opt => opt.MapFrom(src => src.AdminUserId))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.GroupMember, opt => opt.MapFrom(src => src.GroupMember));

            CreateMap<ChatGroupDto, ChatGroup>()
                .ForMember(dest => dest.AdminUserId, opt => opt.MapFrom(src => src.AdminUserId))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.GroupMember, opt => opt.Ignore()); 

            CreateMap<GroupMember, GroupMemberDto>()
                .ForMember(dest => dest.AppUserId, opt => opt.MapFrom(src => src.AppUserId))
                .ForMember(dest => dest.ChatGroupId, opt => opt.MapFrom(src => src.ChatGroupId));

            CreateMap<GroupMemberDto, GroupMember>()
                .ForMember(dest => dest.AppUserId, opt => opt.MapFrom(src => src.AppUserId))
                .ForMember(dest => dest.ChatGroupId, opt => opt.MapFrom(src => src.ChatGroupId))
                .ForMember(dest => dest.AppUser, opt => opt.Ignore()) 
                .ForMember(dest => dest.ChatGroup, opt => opt.Ignore());
        
        }
    }
}
