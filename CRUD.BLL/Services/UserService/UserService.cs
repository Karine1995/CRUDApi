using AutoMapper;
using CRUD.BLL.Filters;
using CRUD.BLL.Helpers;
using CRUD.BLL.Services.TokenService;
using CRUD.DAL;
using CRUD.DAL.Models;
using CRUD.DTO.Models;
using CRUD.DTO.UserModel;
using CryptoHelper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace CRUD.BLL.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _db;
        private readonly ErrorHelpers _errorHelpers;
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(AppDbContext db,
                            ErrorHelpers errorHelpers,
                            IMapper mapper,
                            ITokenService tokenService,
                            IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _errorHelpers = errorHelpers;
            _mapper = mapper;
            _tokenService = tokenService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ResponseDto<bool>> Register(UserRegisterDto registerDto)
        {
            var response = new ResponseDto<bool>();

            registerDto.UserName = registerDto.UserName.ToLower().Trim();

            if (await _db.Users.AnyAsync(x => x.UserName == registerDto.UserName))
            {
                return await _errorHelpers.SetError(response, "UserName alrady taken.");
            }

            var user = new User()
            {
                UserName = registerDto.UserName,
                Name = registerDto.UserName,
                UserTypeId = registerDto.TypeId,
                PasswordHash = Crypto.HashPassword(registerDto.Password)
            };

            _db.Users.Add(user);

            await _db.SaveChangesAsync();


            response.Data = true;
            return response;
        }

        public async Task<ResponseDto<UserDto>> Update(UpdateUserDto userDto)
        {
            var response = new ResponseDto<UserDto>();


            var user = await _db.Users
                                .FirstOrDefaultAsync(x => x.Id == userDto.Id);

            if (user == null)
            {
                return await _errorHelpers.SetError(response, "User not found");
            }

            user.Name = userDto.Name;
            user.PasswordHash = Crypto.HashPassword(userDto.Password);
            user.UserTypeId = userDto.TypeId;

            await _db.SaveChangesAsync();

            return await GetById(user.Id);
        }

        public async Task<ResponseDto<bool>> Delete(long id)
        {
            var response = new ResponseDto<bool>();

            var user = await _db.Users
                                    .FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted != true);

            if(user == null)
            {
                return await _errorHelpers.SetError(response, "Item Not Found.");
            }

            user.IsDeleted = true;

            await _db.SaveChangesAsync();

            response.Data = true;

            return response;
        }

        public async Task<ResponseDto<UserDto>> GetById(long id)
        {
            var response = new ResponseDto<UserDto>();
            var user = await _db.Users
                                .FirstOrDefaultAsync(x => x.Id == id);

            if (user == null)
            {
                return await _errorHelpers.SetError(response, "User not found.");
            }

            var mappedUser = _mapper.Map<UserDto>(user);
            response.Data = mappedUser;
            return response;
        }

        public async Task<ResponseDto<bool>> ChangePassword(ResetPasswordDto passwordDto)
        {
            var response = new ResponseDto<bool>();
            var user = await _db.Users.FirstOrDefaultAsync(x => x.Id == passwordDto.Id);

            if (user == null)
            {
                return await _errorHelpers.SetError(response, "User not found.");
            }

            user.PasswordHash = Crypto.HashPassword(passwordDto.Password);

            await _db.SaveChangesAsync();

            response.Data = true;
            return response;
        }

        public async Task<ResponseDto<UserLoginResponsetDto>> Login(UserLoginDto loginDto)
        {
            var response = new ResponseDto<UserLoginResponsetDto>();

            var user = await _db.Users
                            .FirstOrDefaultAsync(x => x.UserName.ToLower().Trim() == loginDto.UserName.ToLower().Trim());

            if (user == null || !Crypto.VerifyHashedPassword(user.PasswordHash, loginDto.Password))
            {
                return await _errorHelpers.SetError(response, "Incorrect UserName Or Password.");
            }

            var userDto = _mapper.Map<UserDto>(user);

            var token = _tokenService.GenerateJwtToken(userDto);
            var refreshToken = _tokenService.GenerateRefreshToken();


            var session = new UserSession()
            {
                Token = token,
                UserId = user.Id,
                RefreshToken = refreshToken
            };

            _db.UserSessions.Add(session);

            await _db.SaveChangesAsync();

            response.Data = new UserLoginResponsetDto()
            {
                Token = token,
                RefreshToken = refreshToken
            };

            return response;
        }

        public async Task<ResponseDto<bool>> LogOut()
        {
            var response = new ResponseDto<bool>();

            long currentUser = Convert.ToInt64(_httpContextAccessor.HttpContext.User.FindFirst("Id").Value);
            if (currentUser == 0)
            {
                return await _errorHelpers.SetError(response, "User Not Found");
            }

            var sessions = await _db.UserSessions
                                            .Where(x => x.UserId == currentUser && x.IsExpired == false)
                                            .ToListAsync();

            foreach (var x in sessions)
            {
                x.IsExpired = true;
            }

            await _db.SaveChangesAsync();

            response.Data = true;
            return response;
        }

        public async Task<ResponseDto<RefreshDto>> Refresh(RefreshDto refreshDto)
        {
            var response = new ResponseDto<RefreshDto>();

            var savedToken = _tokenService.GetToken(refreshDto.RefreshToken);

            if (savedToken != refreshDto.Token)
            {
                return await _errorHelpers.SetError(response, "Invalid Refresh Token");
            }

            long currentUser = Convert.ToInt64(_httpContextAccessor.HttpContext.User.FindFirst("Id").Value);

            var newJwtToken = _tokenService.GenerateToken();
            var newRefreshToken = _tokenService.GenerateRefreshToken();

            await _tokenService.UpdateRefreshTokenExpired(currentUser, refreshDto.RefreshToken);
            await _tokenService.SaveRefreshToken(currentUser, newRefreshToken);

            response.Data = new RefreshDto
            {
                Token = newJwtToken,
                RefreshToken = newRefreshToken
            };

            return response;

        }

        public async Task<ResponseDto<PagedModelDto<UserDto>>> GetAll(UserFilter filter)
        {
            var response = new ResponseDto<PagedModelDto<UserDto>>();

            var entities = await filter.FilterObjects(_db.Users).ToListAsync();
            var count = filter.Count(_db.Users);

            response.Data = new PagedModelDto<UserDto>()
            {
                Entities = _mapper.Map<List<UserDto>>(entities),
                Count = count
            };

            return response;
        }
    }
}
