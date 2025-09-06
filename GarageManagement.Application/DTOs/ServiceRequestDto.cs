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

       
        public int? DomainID { get; set; }

        [Required, MinLength(3), MaxLength(50)]
        public string DomainType { get; set; } = null!;

       
        public long? ServiceID { get; set; }

        [Required, MinLength(3), MaxLength(50)]
        public string ServiceType { get; set; } = null!;

        
        public string? Description { get; set; } = null!;

        [Required]
        [RegularExpression("Low|Medium|High|Critical")]
        public string Priority { get; set; } = null!;

       
        public string? CreatedBy { get; set; } = null!;

        
        public DateTime? CreatedAt { get; set; }

        [Required]
        public CustomerDto Customer { get; set; } = null!;

       
        public BookingDto? Booking { get; set; } = null!;

       
        public List<DocumentDto>? Documents { get; set; } = new();

        
        public DomainDataDto? DomainData { get; set; } = null!;
    }
}
