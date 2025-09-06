using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarageManagement.Application.DTOs
{
    public class DocumentDto
    {
      
        public long? DocumentID { get; set; }

        [Required, MinLength(3)]
        public string FileName { get; set; } = null!;

        [Required, MinLength(3)]
        public string FileType { get; set; } = null!;
        [Required, MinLength(3)]
        public string DocumentType { get; set; } = null!;

        [Required, Url]
        public string FileUrl { get; set; } = null!;

       
        public string? UploadedBy { get; set; } = null!;

        
        public DateTime? UploadedAt { get; set; }
    }
}
