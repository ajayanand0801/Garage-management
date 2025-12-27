using ComponentManagement.PaginationUtility;
using GarageManagement.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarageManagement.Application.Interfaces.ServiceInterface
{
    public interface ICustomerService
    {
        Task<IEnumerable<CrmCustomerDTO>> GetAllCustomersAsync();
        Task<CrmCustomerDTO?> GetCustomerByIdAsync(long id);
        Task<bool> CreateCustomerAsync(CrmCustomerDTO customerDto);
        Task<bool> UpdateCustomerAsync(long id, CrmCustomerDTO customerDto);
        Task<bool> DeleteCustomerAsync(long id);
        Task<PaginationResult<CrmCustomerDTO>> GetAllCustomersAsync(PaginationRequest request, CancellationToken cancellationToken);
    }
}

