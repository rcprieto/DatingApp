using API.DTOs;
using API.Entities;
using AutoMapper;

namespace API;

public class AutoMapperProfiles : Profile
{
	public AutoMapperProfiles()
	{
		CreateMap<AppUser, MemberDto>()
			.ForMember(dest => dest.PhotoUrl, opt => opt.MapFrom(src => src.Photos.FirstOrDefault(x => x.IsMain).Url));


		CreateMap<Photo, PhotoDto>();
		CreateMap<MemberUpdateDto, AppUser>();
		CreateMap<RegisterDto, AppUser>();
		CreateMap<Message, MessageDto>()
		.ForMember(c => c.SenderPhotoUrl, x => x.MapFrom(t => t.Sender.Photos.FirstOrDefault(y => y.IsMain).Url))
		.ForMember(c => c.RecipientPhotoUrl, x => x.MapFrom(t => t.Recipient.Photos.FirstOrDefault(y => y.IsMain).Url));

	}

}
