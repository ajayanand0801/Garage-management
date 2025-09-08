using GarageManagement.Application.DTOs;
using GarageManagement.Application.Interfaces;
using GarageManagement.Application.Interfaces.Mapper;
using GarageManagement.Application.Interfaces.ServiceInterface;
using GarageManagement.Application.Interfaces.Validator;
using GarageManagement.Domain.Entites.Quotation;
using GarageManagement.Domain.Entites.Vehicles;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarageManagement.Application.Services.Quotations
{
    public class QuotationService : IQuotationService
    {
        private readonly IGenericRepository<Quotation> _quotationGenericRepo;
        private readonly IQuotationRepository _quotationRepository;
        private readonly IJsonValidator _jsonValidator;
        private readonly IMapperUtility _mapperUtility;
        private readonly IUnitOfWork _unitOfWork;
      //  private readonly string _jsonSchema = JsonRules.QuotationSchema; // Assume this exists


        public QuotationService(
            IGenericRepository<Quotation> quotationGenericRepo,
            IQuotationRepository quotationRepository,
            IJsonValidator jsonValidator,
            IMapperUtility mapperUtility,
            IUnitOfWork unitOfWork)
        {
            _quotationGenericRepo = quotationGenericRepo;
            _quotationRepository = quotationRepository;
             _jsonValidator = jsonValidator;
            _mapperUtility = mapperUtility;
            _unitOfWork = unitOfWork;
        }
        public async Task<bool> CreateQuotationAsync(long requestID,QuotationDTO quotationRequest)
        {
            //if (!_jsonValidator.ValidateJsonPayload(quotationDto, _jsonSchema, out List<string> errors))
            //    throw new ValidationException($"Quotation validation failed: {string.Join("; ", errors)}");

            var quotation = _mapperUtility.Map<QuotationDTO, Quotation>(quotationRequest);

            // Set required system fields
            quotation.QuoteGuid = Guid.NewGuid();
            quotation.CreatedAt = DateTime.UtcNow;
            quotation.IsActive = true;
            quotation.IsDeleted = false;
            quotation.Status = "draft";
            quotation.DomainID = 3100;
            quotation.OrgID = 1;
            quotation.TenantID = 1;
            quotation.ServiceID = 1;
            quotation.ReferenceNo = "323";
            quotation.RequestID= requestID;
           
          // quotation.QuotationId = 1;

           //quotation.QuotationItems = null;

          await _unitOfWork.Quotation.AddTransactionAsync(quotation);

          await CreateQuotationItemsAsync(requestID, quotation, quotationRequest);

            try
            {
                await _unitOfWork.CommitAsync();
            }
            catch (Exception ex) { }
            return true;

        }

        public async Task<bool> DeleteQuotationAsync(long id)
        {
            var result = await Task.Run(() => _quotationGenericRepo.SoftDelete(id));

            return result;
        }

        public Task<IEnumerable<QuotationDTO>> GetAllQuotationsAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<QuotationDTO?> GetQuotationByIdAsync(long id)
        {
            var result = await Task.Run(() => _quotationRepository.GetById(id).FirstOrDefault());

            if (result == null)
                return null;

            var quotation = _mapperUtility.Map<Quotation, QuotationDTO>(result);
            return quotation;

        }

        public Task<bool> UpdateQuotationAsync(long id, QuotationDTO updated)
        {
            throw new NotImplementedException();
        }

        public bool ValidateQuotation(QuotationDTO quotationDto, out List<string> errors)
        {
            throw new NotImplementedException();
        }
        private async Task CreateQuotationItemsAsync(long requestId, Quotation quotation, QuotationDTO quotationRequest)
        {
            if (quotationRequest.QuotationItems == null || !quotationRequest.QuotationItems.Any())
                return;

          //  var itemEntities = _mapperUtility.Map<List<QuotationItemDto>, List<QuotationItem>>(quotationRequest.QuotationItems);

            foreach (var itemEntity in quotation.QuotationItems)
            {
                //var itemEntity = _mapperUtility.Map<QuotationItemDto, QuotationItem>(itemDto);

                itemEntity.Quotation = quotation;
                itemEntity.QuotationID = quotation.Id; // FK association
                itemEntity.ItemGguid = Guid.NewGuid();
                itemEntity.CreatedAt = DateTime.UtcNow;
                itemEntity.CreatedBy = "System";
                itemEntity.IsActive = true;
                itemEntity.IsDeleted = false;
                itemEntity.ItemType = "Labour";
                itemEntity.Hours = 0;
                itemEntity.Name = itemEntity.Description;
               // itemEntity.Code = "23323";

                //await _unitOfWork.QuotationItem.AddTransactionAsync(itemEntity);
            }

           // quotation.QuotationItems = itemEntities;

        }

    }
}
