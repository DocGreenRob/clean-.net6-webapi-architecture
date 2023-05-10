using CGE.CleanCode.Common.Models.Dto.Interfaces;

namespace CGE.CleanCode.Common.Models.Dto
{
    public class RolePermissionDto : IDto
    {
        public string Id { get; set; } = string.Empty;
        public string RoleId { get; set; } = string.Empty;
        public string RouteId { get; set; } = string.Empty;
        public string[] HttpVerb { get; set; }
    }
}
