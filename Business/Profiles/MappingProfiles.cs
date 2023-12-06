using AutoMapper;
using Business.Dto;
using Business.Dto.Claim;
using Business.Dto.Role;
using Core.Entities.Concrete;
using System.Security.Claims;

namespace Business.Profiles;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<User, UserLoginDto>().ReverseMap();
        CreateMap<User, UserRegisterDto>().ReverseMap();
        CreateMap<User, UserUpdateDto>().ReverseMap();

        CreateMap<UserRole, RoleDto>().ReverseMap();
        CreateMap<UserRole, RoleRequestDto>().ReverseMap();

        CreateMap<Claim, ClaimDto>().ReverseMap();

    }
}
