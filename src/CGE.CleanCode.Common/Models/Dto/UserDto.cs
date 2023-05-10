using CGE.CleanCode.Common.Models.Dto.Interfaces;
using StackExchange.Redis;
using System.Collections.Generic;
using System;

namespace CGE.CleanCode.Common.Models.Dto
{
    public class UserDto : IDto
    {
        public string Id { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public DateTime DOB { get; set; }
        public string Email { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string IdentityProvider { get; set; } = string.Empty;
        public List<Role> Roles { get; set; } = new List<Role>();
    }
}
