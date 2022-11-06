using AutoMapper;
using CRUD.BLL.Helpers;
using CRUD.DAL;
using CRUD.DAL.Models;
using CRUD.DTO.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRUD.BLL.Services.UserTypeService
{
    public class UserTypeService : IUserTypeService
    {
        private readonly AppDbContext _db;
        private readonly ErrorHelpers _errorHelpers;
        private readonly IMapper _mapper;

        public UserTypeService(AppDbContext db,
                                ErrorHelpers errorHelpers,
                                IMapper mapper)
        {
            _db = db;
            _errorHelpers = errorHelpers;
            _mapper = mapper;
        }

        public async Task<ResponseDto<bool>> Add(CreateUserTypeDto usertypeDto)
        {
            var response = new ResponseDto<bool>();

            if (await _db.UserTypes.AnyAsync(x => x.Name == usertypeDto.Name))
            {
                return await _errorHelpers.SetError(response, "Item already exist.");
            }

            var userGroup = new UserType()
            {
                Name = usertypeDto.Name,
            };

            _db.UserTypes.Add(userGroup);

            await _db.SaveChangesAsync();

            response.Data = true;
            return response;
        }

        public async Task<ResponseDto<List<UserTypeDto>>> GetAll()
        {
            var response = new ResponseDto<List<UserTypeDto>>();

            var userTypes = await _db.UserTypes
                                        .Where(x => x.IsDeleted != true)
                                        .ToListAsync();

            response.Data = _mapper.Map<List<UserTypeDto>>(userTypes);

            return response;
        }

        public async Task<ResponseDto<bool>> Delete(long id)
        {
            var response = new ResponseDto<bool>();

            var userType = await _db.Users
                                        .Include(x => x.UserType)
                                        .FirstOrDefaultAsync(x => x.UserTypeId == id && x.IsDeleted != true);

            if (userType != null)
            {
                return await _errorHelpers.SetError(response, "Cannot remove data with referance.");
            }

            userType.IsDeleted = true;

            await _db.SaveChangesAsync();

            response.Data = true;

            return response;
        }

    }
}
