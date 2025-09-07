using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarageManagement.Domain.Entites.Request
{
    public class ServiceRequestMetadata:BaseEntity
    {
      
        public Guid MetaDataGuid { get; set; }
        public long RequestID { get; set; }
        public string KeyName { get; set; }
        public string KeyValue { get; set; }
       

        public ServiceRequest ServiceRequest { get; set; }
    }

}
