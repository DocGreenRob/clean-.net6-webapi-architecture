namespace CGE.CleanCode.Dal.Entities
{
    public class RolePermission : BaseEntity
    {
        public string RoleId { get; set; } = string.Empty;
        public string RouteId { get; set; } = string.Empty;
        public string[] HttpVerb { get; set; }
    }
}
