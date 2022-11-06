using CRUD.DTO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRUD.BLL.Services.UserTypeService
{
    public interface IUserTypeService
    {
        Task<ResponseDto<bool>> Add(CreateUserTypeDto usertypeDto);
        Task<ResponseDto<List<UserTypeDto>>> GetAll();
        Task<ResponseDto<bool>> Delete(long id);
    }
}
