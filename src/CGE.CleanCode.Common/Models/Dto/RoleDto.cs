using CGE.CleanCode.Common.Models.Dto.Interfaces;

namespace CGE.CleanCode.Common.Models.Dto
{
    public class RoleDto : IDto
    {
        public string Id { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
    }
}
