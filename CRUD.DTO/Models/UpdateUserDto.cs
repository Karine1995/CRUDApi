﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRUD.DTO.Models
{
    public class UpdateUserDto : BaseDto
    {
        public string Password { get; set; }
        public string Name { get; set; }
        public int TypeId { get; set; }
    }
}
