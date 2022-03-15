using AutoMapper;
using Plantilla.WebApi.Data.Contracts.Model;
using Plantilla.WebApi.Dto;

namespace Plantilla.WebApi.Busines.Impl.Mapping
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<UserDto, User>().ReverseMap();
        }
    }
}
