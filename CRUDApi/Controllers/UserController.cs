using CRUD.BLL.Filters;
using CRUD.BLL.Services.UserService;
using CRUD.DTO.Models;
using CRUD.DTO.UserModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRUD.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/user")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ResponseDto<bool>> Register(UserRegisterDto registerDto)
        {
            return await _userService.Register(registerDto);
        }

        [HttpPut("edit")]
        public async Task<ResponseDto<UserDto>> Update(UpdateUserDto userDto)
        {
            return await _userService.Update(userDto);
        }

        [HttpDelete("delete")]
        public async Task<ResponseDto<bool>> Delete(long id)
        {
            return await _userService.Delete(id);
        }

        [HttpGet("{id}")]
        public async Task<ResponseDto<UserDto>> GetById(long id)
        {
            return await _userService.GetById(id);
        }

        [HttpGet]
        public async Task<ResponseDto<PagedModelDto<UserDto>>> GetAll([FromQuery] UserFilter filter)
        {
            return await _userService.GetAll(filter);
        }

        [HttpPut("change-password")]
        public async Task<ResponseDto<bool>> ChangePassword(ResetPasswordDto passwordDto)
        {
            return await _userService.ChangePassword(passwordDto);
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ResponseDto<UserLoginResponsetDto>> Login(UserLoginDto loginDto)
        {
            return await _userService.Login(loginDto);
        }

        [HttpPost("log-out")]
        public async Task<ResponseDto<bool>> LogOut()
        {
            return await _userService.LogOut();
        }

        [HttpPost("refresh")]
        public async Task<ResponseDto<RefreshDto>> Refresh(RefreshDto refreshDto)
        {
            return await _userService.Refresh(refreshDto);

        }
    }
}


