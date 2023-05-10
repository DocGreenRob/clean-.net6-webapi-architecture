using CGE.CleanCode.Common.Models.Dto.Interfaces;

namespace CGE.CleanCode.Common.Models.Dto
{
    public class RouteDto : IDto
    {
        public string Id { get; set; } = string.Empty;
        public string RouteName { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
    }
}
