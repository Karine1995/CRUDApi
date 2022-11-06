using System;
using System.Collections.Generic;
using System.Linq;

namespace CRUD.DAL.Models
{
    public class User : BaseEntity
    {
        public string UserName { get; set; }
        public string PasswordHash { get; set; }
        public string Name { get; set; }
        public int UserTypeId { get; set; }        
        public ICollection<UserSession> Sessions { get; set; }
        public UserType UserType { get; set; }
        
    }
}
