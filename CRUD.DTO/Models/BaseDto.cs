using System;

namespace CRUD.DTO.Models
{
    public class BaseDto
    {
        public long Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifyDate { get; set; }
    }
}
