using System;
using System.Collections.Generic;

namespace CGE.CleanCode.Dal.Entities
{
    public class User : BaseEntity
    {
        public string Username { get; set; } = string.Empty;
        public DateTime DOB { get; set; }
        public string Email { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string IdentityProvider { get; set; } = string.Empty;
        public List<Role> Roles { get; set; }= new List<Role>();
    }
}
