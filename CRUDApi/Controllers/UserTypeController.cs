using CRUD.BLL.Services.UserTypeService;
using CRUD.DTO.Models;
using Microsoft.AspNetCore.Mvc;

namespace CRUD.Api.Controllers
{
    public class UserTypeController
    {
        private IUserTypeService _userTypeService;

        public UserTypeController(IUserTypeService userTypeService)
        {
            _userTypeService = userTypeService;
        }

        [HttpPost("add")]
        public async Task<ResponseDto<bool>> Add(CreateUserTypeDto userTypeDto)
        {
            return await _userTypeService.Add(userTypeDto);
        }

        [HttpGet("get-all")]
        public async Task<ResponseDto<List<UserTypeDto>>> GetAll()
        {
            return await _userTypeService.GetAll();
        }

        [HttpDelete("delete")]
        public async Task<ResponseDto<bool>> Delete(long id)
        {
            return await _userTypeService.Delete(id);
        }
    }
}
