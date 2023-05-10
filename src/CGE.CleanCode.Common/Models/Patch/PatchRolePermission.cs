namespace CGE.CleanCode.Common.Models.Patch
{
    public class PatchRolePermission
    {
        public string RoleId { get; set; } = string.Empty;
        public string RouteId { get; set; } = string.Empty;
        public string[] HttpVerb { get; set; }
    }
}
