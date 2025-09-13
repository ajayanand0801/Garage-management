using ComponentManagement.PaginationUtility;
using GarageManagement.Application.DTOs;
using GarageManagement.Domain.Entites.Quotation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarageManagement.Application.Interfaces.ServiceInterface
{
    public interface IQuotationService
    {
        Task<IEnumerable<QuotationDTO>> GetAllQuotationsAsync();
        Task<QuotationDTO?> GetQuotationByIdAsync(long id);
        Task<bool> CreateQuotationAsync(long requestID, QuotationDTO quotationRequest);
       Task<bool> UpdateQuotationAsync(long id, QuotationDTO updated);
        Task<bool> DeleteQuotationAsync(long id); // Soft delete

        Task<bool> UpdateQuotationItemByQuoteIDAsync(long quoteId, List<QuotationItemDto> quotationItems);
        bool ValidateQuotation(QuotationDTO quotationDto, out List<string> errors);

        Task<PaginationResult<QuotationDTO>> GetAllQuotationsAsync(PaginationRequest request, CancellationToken cancellationToken);


        // Optionally, methods for items
        //  Task<bool> AddQuotationItemAsync(long quotationId, QuotationItemDTO itemDto);
        //Task<bool> DeleteQuotationItemAsync(long quotationId, Guid itemGuid);

    }
}
