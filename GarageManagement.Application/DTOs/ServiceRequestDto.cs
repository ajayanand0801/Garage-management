using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GarageManagement.Application.DTOs
{
    public class ServiceRequestDto
    {
        
        public long? ServiceRequestID { get; set; }

        /// <summary>Server-generated display number, e.g. SR-000101.</summary>
        public string? RequestNo { get; set; }

        
        public long? TenantID { get; set; }

       
        public long? OrgID { get; set; }

       
        public long? DomainID { get; set; }

        [MinLength(3), MaxLength(50)]
        public string? DomainType { get; set; }

        public long? ServiceID { get; set; }

        [MinLength(3), MaxLength(50)]
        public string? ServiceType { get; set; }

        public string? Description { get; set; }

        /// <summary>From ServiceRequest.Status when non-empty; omitted from JSON when null.</summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Status { get; set; }

        /// <summary>From ServiceRequestMetadata KeyName &quot;Employee&quot; JSON object; omitted when absent or empty.</summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Dictionary<string, string>? Employee { get; set; }

        [RegularExpression("Low|Medium|High|Critical")]
        public string? Priority { get; set; }

       
        public string? CreatedBy { get; set; } = null!;

        
        public DateTime? CreatedAt { get; set; }

        public CustomerDto? Customer { get; set; } = null!;

       
        public BookingDto? Booking { get; set; } = null!;

       
        public List<DocumentDto>? Documents { get; set; } = new();

        
        public DomainDataDto? DomainData { get; set; } = null!;
    }
}
