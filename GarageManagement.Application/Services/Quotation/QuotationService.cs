﻿using ComponentManagement.PaginationUtility;
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
        private readonly IPaginationService<Quotation> _paginationService;
        //  private readonly string _jsonSchema = JsonRules.QuotationSchema; // Assume this exists


        public QuotationService(
            IGenericRepository<Quotation> quotationGenericRepo,
            IQuotationRepository quotationRepository,
            IJsonValidator jsonValidator,
            IMapperUtility mapperUtility,
            IUnitOfWork unitOfWork,
            IPaginationService<Quotation> paginationService)
        {
            _quotationGenericRepo = quotationGenericRepo;
            _quotationRepository = quotationRepository;
             _jsonValidator = jsonValidator;
            _mapperUtility = mapperUtility;
            _unitOfWork = unitOfWork;
            _paginationService = paginationService;
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

        public async Task<PaginationResult<QuotationDTO>> GetAllQuotationsAsync(PaginationRequest request, CancellationToken cancellationToken)
        {
            IQueryable<Quotation> query = _quotationRepository.GetAll();

            var pagedResult = await _paginationService.PaginateAsync(
                query,
                request,
                cancellationToken);

            // Map PaginationResult<Quotation> to PaginationResult<QuotationDTO>
            var mappedItems = pagedResult.Items.Select(q => _mapperUtility.Map<Quotation, QuotationDTO>(q)).ToList();

            var result = new PaginationResult<QuotationDTO>
            {
                Items = mappedItems,
                TotalCount = pagedResult.TotalCount
            };

            return result;
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

        public async Task<bool> UpdateQuotationItemByQuoteIDAsync(long quoteId, List<QuotationItemDto> quotationItems)
        {
            // Load existing items from DB with tracking
            var existingItems = _quotationRepository
                .GetQuotationByQuoteId(quoteId)
                .ToList();  // EF Core tracked entities

            if (!existingItems.Any() && !quotationItems.Any())
                return false; // Nothing to update

            var existingDict = existingItems
                .Where(e => e.Id != 0)
                .ToDictionary(e => e.Id);

            foreach (var dto in quotationItems)
            {
                if (dto.ItemID.HasValue && dto.ItemID.Value > 0
                    && existingDict.TryGetValue(dto.ItemID.Value, out var existingEntity))
                {
                    // Map only properties from dto to the existing entity
                    _mapperUtility.Map(dto, existingEntity);  // Don't reassign it

                    // Set audit fields
                    existingEntity.ModifiedAt = DateTime.UtcNow;
                    existingEntity.ModifiedBy = "System";

                    // No need to call AddTransactionAsync — entity is already tracked
                }
                else
                {
                    // New item — optionally uncomment if you want to support adding

                    var newEntity = _mapperUtility.Map<QuotationItemDto, QuotationItem>(dto);
                    newEntity.QuotationID = quoteId;
                    newEntity.ItemGguid = Guid.NewGuid();
                    newEntity.CreatedAt = DateTime.UtcNow;
                    newEntity.CreatedBy = "System";
                    newEntity.ModifiedAt = DateTime.UtcNow;
                    newEntity.ModifiedBy = "System";
                    newEntity.IsActive = true;
                    newEntity.IsDeleted = false;

                    await _unitOfWork.QuotationItem.AddTransactionAsync(newEntity);

                }
            }

            // Commit once, after all updates
            await _unitOfWork.CommitAsync();

            return true;
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
