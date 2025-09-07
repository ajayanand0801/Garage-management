using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarageManagement.Domain.Entites.Request
{
    public class ServiceRequestDocument:BaseEntity
    {
       
        public Guid SvDocumentGuid { get; set; }
        public long RequestID { get; set; }
        public string DocumentType { get; set; }
        public string FileName { get; set; }
        public string FileURL { get; set; }
        public int? FileSizeKB { get; set; }
        public string MimeType { get; set; }
      
        public ServiceRequest ServiceRequest { get; set; }
    }

}
