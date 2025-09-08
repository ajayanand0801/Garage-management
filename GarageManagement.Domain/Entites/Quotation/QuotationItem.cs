using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarageManagement.Domain.Entites.Quotation
{
    public class QuotationItem:BaseEntity
    {
      

        public Guid ItemGguid { get; set; }

        public long QuotationID { get; set; }

        public string? Name { get; set; } = null!;

        public string Description { get; set; } = null!;

        [MaxLength(50)]
        public string? ItemType { get; set; }

        [MaxLength(50)]
        public string? Code { get; set; }

        [MaxLength(20)]
        public string? CurrencyCode { get; set; } = "AED";

        [Column(TypeName = "decimal(5, 2)")]
        public decimal? Hours { get; set; } = 0;

        public int Quantity { get; set; } = 0;

        [Column(TypeName = "decimal(14, 2)")]
        public decimal UnitPrice { get; set; } = 0;

        [Column(TypeName = "decimal(14, 2)")]
        public decimal TotalPrice { get; set; } = 0;

        [Column(TypeName = "decimal(14, 2)")]
        public decimal? DiscountAmount { get; set; } = 0;

        [Column(TypeName = "decimal(14, 2)")]
        public decimal? TaxAmount { get; set; } = 0;

       
        public decimal NetTotal { get; set; } = 0;

        public bool IsOptional { get; set; } = false;

        public bool IsApproved { get; set; } = false;

      

        // Navigation property
       // [ForeignKey("QuotationID")]
        public Quotation Quotation { get; set; } = null!;
    }

}
