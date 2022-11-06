using CRUD.BLL.Filters;
using CRUD.DTO.Models;
using CRUD.DTO.UserModel;

namespace CRUD.BLL.Services.UserService
{
    public interface IUserService
    {
        Task<ResponseDto<bool>> Register(UserRegisterDto registerDto);
        Task<ResponseDto<UserDto>> Update(UpdateUserDto userDto);
        Task<ResponseDto<bool>> Delete(long id);
        Task<ResponseDto<UserDto>> GetById(long id);
        Task<ResponseDto<bool>> ChangePassword(ResetPasswordDto passwordDto);
        Task<ResponseDto<UserLoginResponsetDto>> Login(UserLoginDto loginDto);
        Task<ResponseDto<bool>> LogOut();
        Task<ResponseDto<RefreshDto>> Refresh(RefreshDto refreshDto);
        Task<ResponseDto<PagedModelDto<UserDto>>> GetAll(UserFilter filter);
    }
}
