using AutoMapper;
using AuthApi.Models;
using AuthApi.DTOs;


namespace AuthApi.Mappings
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<ApplicationUser, UserDto>();
        }
    }
}
