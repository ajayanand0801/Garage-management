using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarageManagement.Application.DTOs
{
    public class QuotationItemDto
    {

        public long? ItemID { get; set; } = 0;

        [Required, MinLength(2)]
        public string Name { get; set; } = null!;

        public Guid? ItemGuid { get; set; }


        [Required, MinLength(2)]
        public string PartNumber { get; set; } = null!;

        [MinLength(5)]
        public string? Description { get; set; } = null!;

        [Required, Range(1, int.MaxValue)]
        public int Quantity { get; set; } = 0;

        [Required, Range(0, double.MaxValue)]
        public decimal UnitPrice { get; set; } = 0;

        [Required, Range(0, double.MaxValue)]
        public decimal TotalPrice { get; set; } = 0;
        public string? ItemType { get; set; }
        public string? code { get; set; } = "AED";
        public decimal? Hours { get; set; } = 0;
        public decimal? DiscountAmount { get; set; } = 0;
        public decimal? TaxAmount { get; set; } = 0;    
        public decimal? NetTotal { get; set; } = 0;
        public bool? IsOptional { get; set; } = false;
        public bool? IsApproved { get; set; } = false;
        public string? CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public bool? IsActive { get; set; } = false;
        
    }
}
