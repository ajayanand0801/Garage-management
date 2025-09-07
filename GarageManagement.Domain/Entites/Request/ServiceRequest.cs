using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarageManagement.Domain.Entites.Request
{
    public class ServiceRequest:BaseEntity
    {

        public Guid RequestGuid { get; set; } = new Guid();
        public string? RequestNo { get; set; }
        public long TenantID { get; set; }
        public long OrgID { get; set; }
        public long DomainID { get; set; }
        public long ServiceID { get; set; }
        public long CustomerID { get; set; }
        public long? VehicleID { get; set; }
        public long? RealEstatePropertyID { get; set; }
        public long? HealthProfileID { get; set; }
        public string? Description { get; set; }
        public string Status { get; set; }
        public string? Priority { get; set; }
        public string? Metadata { get; set; }
       public string? DomainType { get; set; }
        public string? ServiceType { get; set; }

        public ICollection<ServiceRequestDocument>? Documents { get; set; }
        public ICollection<ServiceRequestMetadata>? MetadataEntries { get; set; }
        public ICollection<ServiceRequestVehicleMetaData>? vehicleMetaData { get; set; }
        public ServiceRequestCustomerMetaData? customerMetaData { get; set; }
    }

}
