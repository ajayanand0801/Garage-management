using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarageManagement.Application.DTOs
{
    public class BookingDto
    {
      
        public long? BookingID { get; set; }
        public string? BookingNo { get; set; }
        public string?  BookingType { get; set; }

       
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

       
        public string? BookedBy { get; set; } = null!;

        
        public string? Status { get; set; } = null!;
    }
}
