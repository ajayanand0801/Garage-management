namespace GarageManagement.Domain.Entites.Service
{
    /// <summary>
    /// Service category lookup. Maps to [rpa].[ServiceCategory].
    /// </summary>
    public class ServiceCategory : BaseEntity
    {
        public string Code { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
