using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarageManagement.Application.DTOs
{
    public class ServiceRequestDto
    {
        
        public long? ServiceRequestID { get; set; }

        
        public long? TenantID { get; set; }

       
        public long? OrgID { get; set; }

       
        public long? DomainID { get; set; }

        [MinLength(3), MaxLength(50)]
        public string? DomainType { get; set; }

        public long? ServiceID { get; set; }

        [MinLength(3), MaxLength(50)]
        public string? ServiceType { get; set; }

        public string? Description { get; set; }

        public string? Status { get; set; }

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
