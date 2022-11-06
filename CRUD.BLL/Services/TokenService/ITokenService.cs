

using CRUD.DTO.Models;

namespace CRUD.BLL.Services.TokenService
{
    public interface ITokenService
    {
        string GenerateJwtToken(UserDto user);
        string GenerateRefreshToken();
        string GenerateToken();
        string GetToken(string refreshToken);
        Task<ResponseDto<bool>> UpdateRefreshTokenExpired(long userId, string refreshToken);
        Task<ResponseDto<bool>> SaveRefreshToken(long userId, string newRefreshToken);
    }
}
