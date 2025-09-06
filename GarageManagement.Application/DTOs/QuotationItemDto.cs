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
       
        public long? ItemID { get; set; }

        [Required, MinLength(2)]
        public string PartName { get; set; } = null!;

        [Required, MinLength(2)]
        public string PartNumber { get; set; } = null!;

        [MinLength(5)]
        public string? Description { get; set; } = null!;

        [Required, Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Required, Range(0, double.MaxValue)]
        public decimal UnitPrice { get; set; }

        [Required, Range(0, double.MaxValue)]
        public decimal TotalPrice { get; set; }
    }
}
