using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace GarageManagement.Application.DTOs
{
    /// <summary>
    /// DTO for paginated service request listing.
    /// </summary>
    public class ServiceListDto
    {
        public long ServiceRequestID { get; set; }
        /// <summary>Display number from ServiceRequest.RequestNo, e.g. SR-000101.</summary>
        public string? RequestNo { get; set; }
        /// <summary>From ServiceRequest.Status when non-empty; omitted from JSON when null.</summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Status { get; set; }
        /// <summary>From ServiceRequestMetadata KeyName &quot;Employee&quot; JSON object as key/value pairs; omitted when absent or empty.</summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Dictionary<string, string>? Employee { get; set; }
        public string? ServiceType { get; set; }
        public string? DomainType { get; set; }
        /// <summary>Combination of FirstName and LastName.</summary>
        public string? CustomerName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? MobileNo { get; set; }
        /// <summary>Vehicle display format: Make/Model/Year (e.g. Nissan/Xtrail/2025).</summary>
        public string? Vehicle { get; set; }
        public string? Make { get; set; }
        public string? Model { get; set; }
        public int? Year { get; set; }
        public string? Vin { get; set; }
        public string? LicensePlate { get; set; }
        public string? CreatedBy { get; set; }
        public long TenantId { get; set; }
        public long OrgID { get; set; }
        public long DomainId { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }
    }
}
