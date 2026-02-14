using GarageManagement.Application.DTOs;
using GarageManagement.Application.Interfaces;
using GarageManagement.Application.Interfaces.Mapper;
using GarageManagement.Application.Interfaces.ServiceInterface;
using GarageManagement.Domain.Entites;
using GarageManagement.Domain.Entites.Quotation;
using GarageManagement.Domain.Entites.Vehicles;
using GarageManagement.Domain.Entites.WorkOrder;

namespace GarageManagement.Application.Services
{
    public class WorkOrderService : IWorkOrderService
    {
        private readonly IWorkOrderRepository _workOrderRepository;
        private readonly IGenericRepository<Customer> _customerRepository;
        private readonly IGenericRepository<Vehicle> _vehicleRepository;
        private readonly IGenericRepository<Quotation> _quotationRepository;
        private readonly IMapperUtility _mapperUtility;

        public WorkOrderService(
            IWorkOrderRepository workOrderRepository,
            IGenericRepository<Customer> customerRepository,
            IGenericRepository<Vehicle> vehicleRepository,
            IGenericRepository<Quotation> quotationRepository,
            IMapperUtility mapperUtility)
        {
            _workOrderRepository = workOrderRepository;
            _customerRepository = customerRepository;
            _vehicleRepository = vehicleRepository;
            _quotationRepository = quotationRepository;
            _mapperUtility = mapperUtility;
        }

        public async Task<WorkOrderDto?> CreateWorkOrderAsync(CreateWorkOrderRequestDto request, CancellationToken cancellationToken = default)
        {
            if (request.CustomerId.HasValue)
            {
                var customer = await _customerRepository.GetByIdAsync(request.CustomerId.Value);
                if (customer == null)
                    return null;
            }

            var vehicle = await _vehicleRepository.GetByIdAsync(request.VehicleId);
            if (vehicle == null)
                return null;

            var quotation = await _quotationRepository.GetByIdAsync(request.QuotationId);
            if (quotation == null)
                return null;

            var workOrder = _mapperUtility.Map<CreateWorkOrderRequestDto, WorkOrder>(request);
            workOrder.OrderGuid = Guid.NewGuid();
            workOrder.Status = "Created";
            workOrder.CreatedAt = DateTime.UtcNow;
            workOrder.CreatedBy = "System";
            workOrder.IsActive = true;
            workOrder.IsDeleted = false;
            workOrder.TenantID = 0;
            workOrder.OrgID = 0;

            var added = await _workOrderRepository.AddAsync(workOrder);
            if (!added)
                return null;

            return _mapperUtility.Map<WorkOrder, WorkOrderDto>(workOrder);
        }

        public async Task<WorkOrderDto?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
        {
            var workOrder = await _workOrderRepository.GetByIdAsync(id);
            if (workOrder == null)
                return null;
            return _mapperUtility.Map<WorkOrder, WorkOrderDto>(workOrder);
        }
    }
}
