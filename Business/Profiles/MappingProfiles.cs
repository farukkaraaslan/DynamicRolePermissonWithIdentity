using AutoMapper;
using Business.Dto;
using Core.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Profiles;

public class MappingProfiles :Profile
{
    public MappingProfiles()
    {
        CreateMap<User, UserLoginDto>().ReverseMap();
        CreateMap<User, UserRegisterDto>().ReverseMap();
        CreateMap<User, UserUpdateDto>().ReverseMap();
        
    }
}
