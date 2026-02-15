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
        Task<long?> CreateQuotationAsync(long requestID, QuotationDTO quotationRequest);
       Task<bool> UpdateQuotationAsync(long id, QuotationDTO updated);
        Task<bool> DeleteQuotationAsync(long id); // Soft delete

        Task<bool> UpdateQuotationItemByQuoteIDAsync(long quoteId, List<QuotationItemDto> quotationItems);
        bool ValidateQuotation(QuotationDTO quotationDto, out List<string> errors);

        Task<PaginationResult<QuotationDTO>> GetAllQuotationsAsync(PaginationRequest request, CancellationToken cancellationToken);

        Task<bool> DeleteQuotationItemAsync(long quotationId, long id); // Soft delete

        /// <summary>
        /// Updates quotation status to Approved or Rejected. RejectionNotes required when Rejected.
        /// </summary>
        Task<(bool Success, string? ErrorMessage)> UpdateQuotationStatusAsync(long id, UpdateQuotationStatusRequest request);

        // Optionally, methods for items
        //  Task<bool> AddQuotationItemAsync(long quotationId, QuotationItemDTO itemDto);
        //Task<bool> DeleteQuotationItemAsync(long quotationId, Guid itemGuid);

    }
}
