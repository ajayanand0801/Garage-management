using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarageManagement.Domain.Entites
{
    public abstract class BaseEntity
       
    {
       // [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public DateTime CreatedAt { get; set; }= DateTime.UtcNow;
        public string? CreatedBy { get; set; } = "System";
        public DateTime? ModifiedAt { get; set; }
        public string? ModifiedBy { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;

    }

}
