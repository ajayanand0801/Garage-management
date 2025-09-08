using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarageManagement.Domain.Entites.Quotation
{
    public class Quotation:BaseEntity
    {
       

        public long? QuotationId { get; set; }

        public Guid QuoteGuid { get; set; }

        [MaxLength(200)]
        public string? ReferenceNo { get; set; }

        public long RequestID { get; set; }

        public long TenantID { get; set; }

        public long OrgID { get; set; }

        public long? ServiceID { get; set; }

        public long? DomainID { get; set; }

        [Column(TypeName = "decimal(14, 2)")]
        public decimal TotalAmount { get; set; }

        [Column(TypeName = "char(3)")]
        public string CurrencyCode { get; set; } = null!;

        [Column(TypeName = "decimal(18, 2)")]
        public decimal? DiscountAmount { get; set; } = 0;

        [Column(TypeName = "decimal(18, 2)")]
        public decimal? TaxAmount { get; set; } = 0;

        [Column(TypeName = "decimal(18, 2)")]
        public decimal? FinalAmount { get; set; } = 0;

        public string? Notes { get; set; }

        [MaxLength(50)]
        public string Status { get; set; } = "pending";

    

        // Navigation property
        public ICollection<QuotationItem>? QuotationItems { get; set; } = new List<QuotationItem>();
    }
}
