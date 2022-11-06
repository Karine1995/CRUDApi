using CRUD.DTO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRUD.BLL.Helpers
{
    public class ErrorHelpers
    {
        public async Task<ResponseDto<T>> SetError<T>(ResponseDto<T> response, string message)
        {
            response.Data = default;
            response.Errors = new List<ErrorModelDto>() { new ErrorModelDto()
            {
                Message = message
            } };

            return await Task.FromResult(response);
        }
    }
}
