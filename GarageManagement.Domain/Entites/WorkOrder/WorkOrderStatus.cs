namespace GarageManagement.Domain.Entites.WorkOrder
{
    /// <summary>
    /// Lookup entity for work order status (Draft, Open, Scheduled, etc.).
    /// Maps to [rpa].[WorkOrderStatus].
    /// </summary>
    public class WorkOrderStatus : BaseEntity
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }
}
