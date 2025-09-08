using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarageManagement.Application.DTOs
{
    public class QuotationDTO
    {
        
        public long? QuotationID { get; set; }
        public string? QuotationNo { get; set; }

        [ Range(0, double.MaxValue)]
        public decimal? EstimatedTotal { get; set; }

        [ MinLength(3), MaxLength(3)]
        public string? Currency { get; set; } = null!;

        [Range(0, double.MaxValue)]
        public decimal? Discount { get; set; } = 0;

        [ Range(0, double.MaxValue)]
        public decimal ?Tax { get; set; }

        [ Range(0, double.MaxValue)]
        public decimal? GrandTotal { get; set; }

        
        public string? Status { get; set; } = null!;

     
        public DateTime? CreatedAt { get; set; }
        public List<QuotationItemDto>? QuotationItems { get; set; } = new();

    }
}
