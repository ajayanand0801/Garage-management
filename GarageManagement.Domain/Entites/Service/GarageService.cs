namespace GarageManagement.Domain.Entites.Service
{
    /// <summary>
    /// Garage service entity. Maps to [rpa].[GarageServices].
    /// </summary>
    public class GarageService : BaseEntity
    {
        public string Code { get; set; } = string.Empty;
        public string ServiceName { get; set; } = string.Empty;
        public long CategoryId { get; set; }
        public long? ParentId { get; set; }
        public long ServiceDomainId { get; set; }
        public decimal? EstimatedCost { get; set; }
        public bool QuotationRequired { get; set; }
        public string? Description { get; set; }

        public GarageService? Parent { get; set; }
        public ICollection<GarageService>? Children { get; set; }
    }
}
