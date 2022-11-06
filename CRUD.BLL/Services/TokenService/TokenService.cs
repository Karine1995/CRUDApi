using CRUD.BLL.Helpers;
using CRUD.BLL.Services.TokenService;
using CRUD.DAL;
using CRUD.DAL.Models;
using CRUD.DTO.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace CRUD.Bll.Services.TokenService
{
    public class TokenService : ITokenService
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _configuration;
        private readonly ErrorHelpers _errorHelpers;

        public TokenService(AppDbContext db,
                            IConfiguration configuration,
                            ErrorHelpers errorHelpers)
        {
            _db = db;
            _configuration = configuration;
            _errorHelpers = errorHelpers;
        }

        public string GenerateJwtToken(UserDto user)
        {
            string key = _configuration["Jwt:Key"].ToString();
            string issuer = _configuration["Jwt:Issuer"].ToString();
            TimeSpan expiryDuration = TimeSpan.Parse(_configuration["UserOptions:ExpiryDuration"].ToString());
            var claims = new[]
            {
                new Claim(nameof(ClaimTypes.Name), user.UserName),
                new Claim("Id", user.Id.ToString()),
                new Claim(nameof(ClaimTypes.NameIdentifier), Guid.NewGuid().ToString())
             };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
            var tokenDescriptor = new JwtSecurityToken(issuer, issuer, claims,
                expires: DateTime.Now.Add(expiryDuration), signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        public string GenerateToken()
        {
            byte[] key = Guid.NewGuid().ToByteArray();
            string token = Convert.ToBase64String(key);
            return token;
        }
        public string GetToken(string refreshToken)
        {
            TimeSpan RefreshTokenExpiryDuration = TimeSpan.Parse(_configuration["Jwt:RefreshTokenExpiryDuration"].ToString());

            var session = _db.UserSessions
                                    .FirstOrDefault(x => x.RefreshToken == refreshToken && !x.IsExpired);

            if (session != null)
            {
                if (session.CreatedDate.Add(RefreshTokenExpiryDuration) < DateTime.Now)
                {
                    session.IsExpired = true;
                    return null;
                }
                return session.Token;
            }
            return null;
        }


        public async Task<ResponseDto<bool>> SaveRefreshToken(long userId, string newRefreshToken)
        {
            var response = new ResponseDto<bool>();
            var user = await _db.UserSessions.FirstOrDefaultAsync(x => x.UserId == userId);

            if (newRefreshToken == null)
            {
                return await _errorHelpers.SetError(response, "Wrong Authorization Token");
            }

            var session = new UserSession()
            {
                Token = user.Token,
                UserId = user.Id,
                RefreshToken = newRefreshToken
            };

            _db.UserSessions.Add(session);

            await _db.SaveChangesAsync();

            response.Data = true;
            return response;
        }

        public async Task<ResponseDto<bool>> UpdateRefreshTokenExpired(long userId, string refreshToken)
        {
            var response = new ResponseDto<bool>();

            var user = await _db.UserSessions.FirstOrDefaultAsync(x => x.UserId == userId && x.RefreshToken == refreshToken);

            if (user == null)
            {
                return await _errorHelpers.SetError(response, "Item Not Found");
            }

            user.IsExpired = true;

            await _db.SaveChangesAsync();

            response.Data = true;
            return response;
        }
    }
}
