using ComponentManagement.PaginationUtility;
using GarageManagement.Application.DTOs;
using GarageManagement.Application.Interfaces;
using GarageManagement.Application.Interfaces.Mapper;
using GarageManagement.Application.Interfaces.ServiceInterface;
using GarageManagement.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarageManagement.Application.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly IGenericRepository<Customer> _customerGenericRepo;
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapperUtility _mapperUtility;
        private readonly IPaginationService<Customer> _paginationService;

        public CustomerService(
            IGenericRepository<Customer> customerGenericRepo,
            ICustomerRepository customerRepository,
            IMapperUtility mapperUtility,
            IPaginationService<Customer> paginationService)
        {
            _customerGenericRepo = customerGenericRepo;
            _customerRepository = customerRepository;
            _mapperUtility = mapperUtility;
            _paginationService = paginationService;
        }

        public async Task<IEnumerable<CrmCustomerDTO>> GetAllCustomersAsync()
        {
            var customers = await _customerGenericRepo.GetAllAsync();
            return customers.Select(c => _mapperUtility.Map<Customer, CrmCustomerDTO>(c));
        }

        public async Task<CrmCustomerDTO?> GetCustomerByIdAsync(long id)
        {
            var customer = await _customerGenericRepo.GetByIdAsync(id);
            if (customer == null)
                return null;

            return _mapperUtility.Map<Customer, CrmCustomerDTO>(customer);
        }

        public async Task<CrmCustomerDTO?> CreateCustomerAsync(CrmCustomerDTO customerDto)
        {
            var customer = _mapperUtility.Map<CrmCustomerDTO, Customer>(customerDto);
            
            // Set system fields
            customer.CustomerGuid = Guid.NewGuid();
            customer.CreatedAt = DateTime.UtcNow;
            customer.CreatedBy = customerDto.CreatedBy ?? "System";
            customer.IsActive = true;
            customer.IsDeleted = false;

            var result = await _customerGenericRepo.AddAsync(customer);
            if (!result)
                return null;

            // Return the created customer with its ID
            return _mapperUtility.Map<Customer, CrmCustomerDTO>(customer);
        }

        public async Task<bool> UpdateCustomerAsync(long id, CrmCustomerDTO customerDto)
        {
            var existing = await _customerGenericRepo.GetByIdAsync(id);
            if (existing == null)
                return false;

            // Map DTO properties to the existing tracked entity (don't create a new entity)
            _mapperUtility.Map(customerDto, existing);
            
            // Preserve system fields that shouldn't be updated from DTO
            existing.Id = id; // Ensure Id is preserved
            existing.CustomerGuid = existing.CustomerGuid; // Preserve GUID
            existing.CreatedAt = existing.CreatedAt; // Preserve original creation date
            existing.CreatedBy = existing.CreatedBy; // Preserve original creator
            
            // Update audit fields
            existing.ModifiedAt = DateTime.UtcNow;
            existing.ModifiedBy = customerDto.ModifiedBy ?? "System";

            // Since the entity is already tracked, just save changes
            return await _customerGenericRepo.UpdateAsync(existing);
        }

        public async Task<bool> DeleteCustomerAsync(long id)
        {
            return await _customerGenericRepo.SoftDelete(id);
        }

        public async Task<PaginationResult<CrmCustomerDTO>> GetAllCustomersAsync(PaginationRequest request, CancellationToken cancellationToken)
        {
            IQueryable<Customer> query = _customerRepository.GetAll();

            var pagedResult = await _paginationService.PaginateAsync(
                query,
                request,
                cancellationToken);

            // Map PaginationResult<Customer> to PaginationResult<CustomerDto>
            var mappedItems = pagedResult.Items.Select(c => _mapperUtility.Map<Customer, CrmCustomerDTO>(c)).ToList();

            var result = new PaginationResult<CrmCustomerDTO>
            {
                Items = mappedItems,
                TotalCount = pagedResult.TotalCount
            };

            return result;
        }
    }
}

