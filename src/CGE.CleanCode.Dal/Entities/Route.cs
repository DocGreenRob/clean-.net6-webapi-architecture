namespace CGE.CleanCode.Dal.Entities
{
    public class Route : BaseEntity
    {
        public string RouteName { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
    }
}
