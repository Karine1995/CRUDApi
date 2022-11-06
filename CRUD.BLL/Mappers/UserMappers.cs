using AutoMapper;
using CRUD.DAL.Models;
using CRUD.DTO.Models;
using System.Linq;

namespace CRUD.BLL.Mappers
{
    public class UserMappers : Profile
    {
        public UserMappers()
        {
            CreateMap<User, UserDto>()
               .ReverseMap();

            CreateMap<UserType, UserTypeDto>()
                .ReverseMap();

        }
    }
}
