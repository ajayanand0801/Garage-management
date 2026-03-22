using ComponentManagement.PaginationUtility;
using GarageManagement.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarageManagement.Application.Interfaces.ServiceInterface
{
    public interface IServiceRequest
    {
        /// <summary>Creates a service request, assigns RequestNo (SR-######), and returns the persisted DTO or null if the transaction failed.</summary>
        Task<ServiceRequestDto?> Create(ServiceRequestDto request);

        Task<PaginationResult<ServiceListDto>> GetServiceRequestsAsync(PaginationRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets service request by id with full details (customer, vehicle, documents, metadata).
        /// </summary>
        Task<ServiceRequestDto?> GetByIdAsync(long id);

        /// <summary>
        /// Updates service request by id. Updates ServiceRequest, SRCustomerMetaData, SRVehicleMetaData and ServiceRequestMetadata (JSON) as provided.
        /// </summary>
        Task<bool> UpdateByServiceRequestId(long serviceRequestId, ServiceRequestDto request, string? modifiedBy = null);

        /// <summary>
        /// Soft delete a service request (sets IsActive = 0).
        /// </summary>
        Task<bool> DeleteServiceRequestAsync(long id);
    }
}
