namespace GarageManagement.Application.DTOs
{
    /// <summary>
    /// Standard payload for lookup endpoints. Use for any lookup type (GarageService, etc.).
    /// </summary>
    public class LookupDto
    {
        public long Id { get; set; }
        public string? Code { get; set; }
        public string DisplayName { get; set; } = string.Empty;
    }
}
