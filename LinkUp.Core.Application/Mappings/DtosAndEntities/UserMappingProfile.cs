using AutoMapper;
using LinkUp.Core.Application.Dtos.User;
using LinkUp.Core.Application.Viewmodels.User;

namespace LinkUp.Core.Application.Mappings.DtosAndEntities;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<UserDto, UserSaveDto>().ReverseMap().ForMember(dest=> dest.IsVerified, opt=> opt.Ignore());
        CreateMap<RegisterUserViewModel, UserSaveDto>()
            .ForMember(dest => dest.PhotoPath, opt => opt.Ignore())
            .ReverseMap()
            .ForMember(dest => dest.ConfirmPassword, opt => opt.Ignore())
            .ForMember(dest => dest.ProfileImageFile, opt => opt.Ignore());

        CreateMap<LoginViewModel, LoginDto>().ReverseMap();
        CreateMap<UserDto, UserViewModel>().ReverseMap();

    }
}