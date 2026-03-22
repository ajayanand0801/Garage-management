using ComponentManagement.PaginationUtility;
using GarageManagement.Application.DTOs;
using GarageManagement.Application.Interfaces;
using GarageManagement.Application.Interfaces.Mapper;
using GarageManagement.Application.Interfaces.ServiceInterface;
using GarageManagement.Domain.Entites;
using GarageManagement.Domain.Entites.Quotation;
using GarageManagement.Domain.Entites.Vehicles;
using GarageManagement.Domain.Entites.WorkOrder;
using System.ComponentModel.DataAnnotations;

namespace GarageManagement.Application.Services
{
    public class WorkOrderService : IWorkOrderService
    {
        private readonly IWorkOrderRepository _workOrderRepository;
        private readonly IGenericRepository<Customer> _customerRepository;
        private readonly IGenericRepository<Vehicle> _vehicleRepository;
        private readonly IGenericRepository<Quotation> _quotationRepository;
        private readonly IMapperUtility _mapperUtility;
        private readonly IPaginationService<WorkOrder> _paginationService;

        public WorkOrderService(
            IWorkOrderRepository workOrderRepository,
            IGenericRepository<Customer> customerRepository,
            IGenericRepository<Vehicle> vehicleRepository,
            IGenericRepository<Quotation> quotationRepository,
            IMapperUtility mapperUtility,
            IPaginationService<WorkOrder> paginationService)
        {
            _workOrderRepository = workOrderRepository;
            _customerRepository = customerRepository;
            _vehicleRepository = vehicleRepository;
            _quotationRepository = quotationRepository;
            _mapperUtility = mapperUtility;
            _paginationService = paginationService;
        }

        public async Task<WorkOrderDto?> CreateWorkOrderAsync(CreateWorkOrderRequestDto request, CancellationToken cancellationToken = default)
        {
            await ValidateCreateRequestAsync(request, cancellationToken);

            var workOrder = _mapperUtility.Map<CreateWorkOrderRequestDto, WorkOrder>(request);
            workOrder.OrderGuid = Guid.NewGuid();
            workOrder.Status = "Created";
            workOrder.CreatedAt = DateTime.UtcNow;
            workOrder.CreatedBy = "System";
            workOrder.IsActive = true;
            workOrder.IsDeleted = false;
            workOrder.TenantID = 1;// this will be fetched from token or header.
            workOrder.OrgID = 1;// this will be fetched from token or header.

            var added = await _workOrderRepository.AddAsync(workOrder);
            if (!added)
                throw new InvalidOperationException("Work order could not be created.");

            return _mapperUtility.Map<WorkOrder, WorkOrderDto>(workOrder);
        }

        /// <summary>
        /// Validates the create work order request. Throws ValidationException or InvalidOperationException with a clear message on failure.
        /// </summary>
        private async Task ValidateCreateRequestAsync(CreateWorkOrderRequestDto? request, CancellationToken cancellationToken)
        {
            if (request == null)
                throw new ValidationException("Request body cannot be null.");

            if (request.VehicleId <= 0)
                throw new ValidationException("VehicleId is required and must be greater than zero.");

            if (request.QuotationId <= 0)
                throw new ValidationException("QuotationId is required and must be greater than zero.");

            if (request.CustomerId.HasValue && request.CustomerId.Value <= 0)
                throw new ValidationException("CustomerId, when provided, must be greater than zero.");

            if (request.CustomerId.HasValue)
            {
                var customer = await _customerRepository.GetByIdAsync(request.CustomerId.Value);
                if (customer == null)
                    throw new ValidationException($"Customer with ID {request.CustomerId.Value} not found or inactive.");
            }

            var vehicle = await _vehicleRepository.GetByVehicleIdAsync(request.VehicleId);
            if (vehicle == null)
                throw new ValidationException($"Vehicle with ID {request.VehicleId} not found or inactive.");

            var quotation = await _quotationRepository.GetByIdAsync(request.QuotationId);
            if (quotation == null)
                throw new ValidationException($"Quotation with ID {request.QuotationId} not found or inactive.");
        }

        public async Task<WorkOrderDto?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
        {
            var workOrder = await _workOrderRepository.GetByIdAsync(id);
            if (workOrder == null)
                return null;
            return _mapperUtility.Map<WorkOrder, WorkOrderDto>(workOrder);
        }

        public async Task<bool> UpdateAsync(long id, WorkOrderDto dto, string? modifiedBy = null)
        {
            var existing = await _workOrderRepository.GetByIdAsync(id);
            if (existing == null)
                return false;

            existing.Status = dto.Status ?? existing.Status;
            existing.ScheduledStart = dto.ScheduledStart;
            existing.ScheduledEnd = dto.ScheduledEnd;
            existing.ActualStart = dto.ActualStart;
            existing.ActualEnd = dto.ActualEnd;
            existing.Notes = dto.Notes;
            existing.ModifiedAt = DateTime.UtcNow;
            existing.ModifiedBy = modifiedBy ?? dto.ModifiedBy ?? "System";

            return await _workOrderRepository.UpdateAsync(existing);
        }

        public async Task<bool> DeleteAsync(long id)
        {
            return await _workOrderRepository.SoftDeleteAsync(id);
        }

        public async Task<PaginationResult<WorkOrderDto>> GetPaginatedAsync(PaginationRequest request, CancellationToken cancellationToken = default)
        {
            var query = _workOrderRepository.GetQueryableForList();
            var paged = await _paginationService.PaginateAsync(query, request, cancellationToken);
            var items = paged.Items.Select(w => _mapperUtility.Map<WorkOrder, WorkOrderDto>(w)).ToList();
            return new PaginationResult<WorkOrderDto> { Items = items, TotalCount = paged.TotalCount };
        }
    }
}
