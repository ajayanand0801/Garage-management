using ComponentManagement.PaginationUtility;
using GarageManagement.Application.DTOs;
using GarageManagement.Application.Interfaces;
using GarageManagement.Application.Interfaces.Mapper;
using GarageManagement.Application.Interfaces.ServiceInterface;
using GarageManagement.Application.Interfaces.Validator;
using GarageManagement.Domain.Entites.Request;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GarageManagement.Application.Services.Request
{
    public class ServiceRequestService : IServiceRequest
    {
        private readonly IJsonValidator _jsonValidator;
        private readonly IGenericRepository<ServiceRequest> _serviceRequestRepo;
        private readonly IGenericRepository<ServiceRequestDocument> _documentRepo;
        private readonly IGenericRepository<ServiceRequestMetadata> _metadataRepo;
        private readonly IServiceRequestRepository _serviceRequestRepository;
        private readonly IPaginationService<ServiceRequest> _paginationService;
        private readonly IMapperUtility _mapperUtility;
        private readonly IUnitOfWork _unitOfWork;

        // Assume you inject the schema as well (static or from config)
        private readonly string _jsonSchema = JsonRules.ServiceRequestSchema;

        public ServiceRequestService(
         IJsonValidator jsonValidator,
         IGenericRepository<ServiceRequest> serviceRequestRepo,
         IGenericRepository<ServiceRequestDocument> documentRepo,
         IGenericRepository<ServiceRequestMetadata> metadataRepo,
         IServiceRequestRepository serviceRequestRepository,
         IPaginationService<ServiceRequest> paginationService,
         IMapperUtility mapperUtility,
         IUnitOfWork unitOfWork)
        {
            _jsonValidator = jsonValidator;
            _serviceRequestRepo = serviceRequestRepo;
            _documentRepo = documentRepo;
            _metadataRepo = metadataRepo;
            _serviceRequestRepository = serviceRequestRepository;
            _paginationService = paginationService;
            _mapperUtility = mapperUtility;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Create(ServiceRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.DomainType) || string.IsNullOrWhiteSpace(request.ServiceType) || string.IsNullOrWhiteSpace(request.Priority))
                throw new ValidationException("ServiceRequest create requires DomainType, ServiceType and Priority.");

            // Run schema validation
            if (!_jsonValidator.ValidateJsonPayload(request, _jsonSchema, out List<string> errors))
            {
                var errorMessage = string.Join("; ", errors);
                throw new ValidationException($"ServiceRequest validation failed: {errorMessage}");
            }

            // Map DTO to Entity
            var serviceRequest = _mapperUtility.Map<ServiceRequestDto, ServiceRequest>(request);

            // Set system fields
            serviceRequest.CreatedAt = DateTime.UtcNow;
            serviceRequest.CreatedBy = request.CreatedBy ?? "System";
            serviceRequest.RequestGuid= Guid.NewGuid();
            serviceRequest.Status = "draft";
            // Save ServiceRequest first to get generated Id
            await _unitOfWork.ServiceRequest.AddTransactionAsync(serviceRequest);

            if (request.Customer != null && request.DomainData?.Vehicle != null)
                await StoreVehicleMetadataAsync(serviceRequest, request.Customer, request.DomainData?.Vehicle);


            if (request.Customer != null)
            {
               await StoreSvCusomerMetadataAsync(serviceRequest, request.Customer);
               await StoreCustomerMetadataAsync(serviceRequest, request.Customer);
            }


            if (request.Booking != null)
                await StoreBookingMetadataAsync(serviceRequest, request.Booking);


            if (serviceRequest.CreatedBy != null)
                await StoreDomainDataAsync(serviceRequest, request.DomainData);


            if (request.Documents != null)
            {
                foreach (var docDto in request.Documents)
                {
                    var documentEntity = _mapperUtility.Map<DocumentDto, ServiceRequestDocument>(docDto);

                    documentEntity.RequestID = serviceRequest.Id;  // FK set here
                    documentEntity.CreatedAt = DateTime.UtcNow;
                    documentEntity.CreatedBy = docDto.UploadedBy ?? "System";

                    await _unitOfWork.ServiceRequestDocument.AddTransactionAsync(documentEntity);
                    //await _documentRepo.AddAsync(documentEntity);
                }
            }
            //Commit all
            try
            {
                await _unitOfWork.CommitAsync();
            }
            catch (Exception ex) { }
            // You can add more logic here (e.g., metadata) if needed

            return true;
        }

        public async Task<PaginationResult<ServiceListDto>> GetServiceRequestsAsync(PaginationRequest request, CancellationToken cancellationToken = default)
        {
            IQueryable<ServiceRequest> query = _serviceRequestRepository.GetQueryableForList();

            var normalizedRequest = NormalizeServiceRequestPaginationRequest(request);

            var pagedResult = await _paginationService.PaginateAsync(
                query,
                normalizedRequest,
                cancellationToken);

            var mappedItems = pagedResult.Items
                .Select(sr => _mapperUtility.Map<ServiceRequest, ServiceListDto>(sr))
                .ToList();

            return new PaginationResult<ServiceListDto>
            {
                Items = mappedItems,
                TotalCount = pagedResult.TotalCount
            };
        }

        public async Task<ServiceRequestDto?> GetByIdAsync(long id)
        {
            var entity = await _serviceRequestRepository.GetByIdWithCustomerVehicleAndMetadataAsync(id);
            if (entity == null)
                return null;

            var dto = _mapperUtility.Map<ServiceRequest, ServiceRequestDto>(entity);
            dto.Status = entity.Status;

            if (entity.customerMetaData != null)
                dto.Customer = _mapperUtility.Map<ServiceRequestCustomerMetaData, CustomerDto>(entity.customerMetaData);

            if (entity.vehicleMetaData != null && entity.vehicleMetaData.Any())
                dto.DomainData = new DomainDataDto
                {
                    Vehicle = _mapperUtility.Map<ServiceRequestVehicleMetaData, VehicleDomainDTO>(entity.vehicleMetaData.First())
                };

            if (entity.Documents != null && entity.Documents.Any())
                dto.Documents = entity.Documents.Select(d => _mapperUtility.Map<ServiceRequestDocument, DocumentDto>(d)).ToList();

            if (entity.MetadataEntries != null)
            {
                var bookingMeta = entity.MetadataEntries.FirstOrDefault(m => string.Equals(m.KeyName, "Booking", StringComparison.OrdinalIgnoreCase));
                if (bookingMeta != null && !string.IsNullOrEmpty(bookingMeta.KeyValue))
                {
                    try
                    {
                        dto.Booking = JsonSerializer.Deserialize<BookingDto>(bookingMeta.KeyValue);
                    }
                    catch { /* ignore */ }
                }

                var domainDataMeta = entity.MetadataEntries.FirstOrDefault(m => string.Equals(m.KeyName, "DomainData", StringComparison.OrdinalIgnoreCase));
                if (domainDataMeta != null && !string.IsNullOrEmpty(domainDataMeta.KeyValue) && dto.DomainData != null)
                {
                    try
                    {
                        var storedDomainData = JsonSerializer.Deserialize<DomainDataDto>(domainDataMeta.KeyValue);
                        if (storedDomainData?.Quotation != null)
                            dto.DomainData.Quotation = storedDomainData.Quotation;
                        if (storedDomainData?.Vehicle != null && dto.DomainData.Vehicle == null)
                            dto.DomainData.Vehicle = storedDomainData.Vehicle;
                    }
                    catch { /* ignore */ }
                }
            }

            return dto;
        }

        public async Task<bool> DeleteServiceRequestAsync(long id)
        {
            return await _serviceRequestRepo.SoftDelete(id);
        }

        /// <summary>
        /// Maps client/DTO-style field names to ServiceRequest entity property names so sorting and filtering work.
        /// </summary>
        private static PaginationRequest NormalizeServiceRequestPaginationRequest(PaginationRequest request)
        {
            // Map client/DTO field names to ServiceRequest entity paths (own properties + joined customerMetaData / vehicleMetaData)
            var fieldToEntityProperty = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                // ServiceRequest (and BaseEntity) own properties
                { "createdDate", "CreatedAt" },
                { "serviceRequestID", "Id" },
                { "id", "Id" },
                { "tenantId", "TenantID" },
                { "orgId", "OrgID" },
                { "domainId", "DomainID" },
                { "isActive", "IsActive" },
                { "serviceType", "ServiceType" },
                { "domainType", "DomainType" },
                { "status", "Status" },
                { "priority", "Priority" },
                { "createdBy", "CreatedBy" },
                { "requestNo", "RequestNo" },
                { "description", "Description" },
                { "modifiedAt", "ModifiedAt" },
                { "modifiedBy", "ModifiedBy" },
                // SRCustomerMetaData (customerMetaData) - joined table
                { "email", "customerMetaData.Email" },
                { "phone", "customerMetaData.Phone" },
                { "mobileNo", "customerMetaData.MobileNo" },
                { "firstName", "customerMetaData.FirstName" },
                { "lastName", "customerMetaData.LastName" },
                { "customerName", "customerMetaData.FirstName" },
                { "address", "customerMetaData.Address" },
                { "city", "customerMetaData.City" },
                { "state", "customerMetaData.State" },
                { "country", "customerMetaData.Country" },
                { "postalCode", "customerMetaData.PostalCode" },
                { "companyName", "customerMetaData.CompanyName" },
                // SRVehicleMetaData (vehicleMetaData) - joined collection
                { "make", "vehicleMetaData.Make" },
                { "model", "vehicleMetaData.Model" },
                { "year", "vehicleMetaData.Year" },
                { "vin", "vehicleMetaData.VIN" },
                { "licensePlate", "vehicleMetaData.LicensePlate" },
                { "vehicleVin", "vehicleMetaData.VIN" },
                { "ownerName", "vehicleMetaData.OwnerName" }
            };

            var modifiedRequest = new PaginationRequest
            {
                Skip = request.Skip,
                Take = request.Take,
                Filters = new List<FilterField>(),
                Sorts = new List<SortField>()
            };

            if (request.Sorts != null)
            {
                foreach (var sort in request.Sorts)
                {
                    if (string.IsNullOrWhiteSpace(sort.Field)) continue;
                    var entityField = fieldToEntityProperty.TryGetValue(sort.Field.Trim(), out var mapped)
                        ? mapped
                        : sort.Field;
                    modifiedRequest.Sorts.Add(new SortField
                    {
                        Field = entityField,
                        Direction = sort.Direction ?? "asc"
                    });
                }
            }

            if (request.Filters != null)
            {
                foreach (var filter in request.Filters)
                {
                    if (string.IsNullOrWhiteSpace(filter.Field)) continue;
                    var entityField = fieldToEntityProperty.TryGetValue(filter.Field.Trim(), out var mapped)
                        ? mapped
                        : filter.Field;
                    modifiedRequest.Filters.Add(new FilterField
                    {
                        Field = entityField,
                        Operation = filter.Operation ?? "eq",
                        Value = filter.Value,
                        LogicalOperator = filter.LogicalOperator ?? "and"
                    });
                }
            }

            return modifiedRequest;
        }

        public async Task<bool> UpdateByServiceRequestId(long serviceRequestId, ServiceRequestDto request, string? modifiedBy = null)
        {
            var existing = await _serviceRequestRepository.GetByIdWithCustomerVehicleAndMetadataAsync(serviceRequestId);
            if (existing == null)
                return false;

            var modifiedByUser = modifiedBy ?? request.CreatedBy ?? "System";
            var now = DateTime.UtcNow;

            // 1. Update ServiceRequest own fields when provided
            if (request.Description != null)
                existing.Description = request.Description;
            if (request.Status != null)
                existing.Status = request.Status;
            if (request.Priority != null)
                existing.Priority = request.Priority;
            if (request.DomainType != null)
                existing.DomainType = request.DomainType;
            if (request.ServiceType != null)
                existing.ServiceType = request.ServiceType;
            if (request.TenantID.HasValue)
                existing.TenantID = request.TenantID.Value;
            if (request.OrgID.HasValue)
                existing.OrgID = request.OrgID.Value;
            if (request.DomainID.HasValue)
                existing.DomainID = request.DomainID.Value;
            if (request.ServiceID.HasValue)
                existing.ServiceID = request.ServiceID.Value;

            existing.ModifiedAt = now;
            existing.ModifiedBy = modifiedByUser;

            await _unitOfWork.ServiceRequest.UpdateAsync(existing);

            // 2. Update or create customer section (SRCustomerMetaData + JSON metadata)
            if (request.Customer != null)
            {
                await UpdateOrCreateCustomerMetadataAsync(existing, request.Customer, modifiedByUser, now);
                await UpdateOrCreateJsonMetadataAsync(existing, "customer", request.Customer, modifiedByUser, now);
            }

            // 3. Update or create vehicle section (SRVehicleMetaData + DomainData JSON)
            if (request.DomainData?.Vehicle != null)
            {
                await UpdateOrCreateVehicleMetadataAsync(existing, request.Customer, request.DomainData.Vehicle, modifiedByUser, now);
                await UpdateOrCreateJsonMetadataAsync(existing, "DomainData", request.DomainData, modifiedByUser, now);
            }

            // 4. Update Booking JSON metadata if provided
            if (request.Booking != null)
                await UpdateOrCreateJsonMetadataAsync(existing, "Booking", request.Booking, modifiedByUser, now);

            try
            {
                await _unitOfWork.CommitAsync();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        private async Task UpdateOrCreateCustomerMetadataAsync(ServiceRequest serviceRequest, CustomerDto customer, string modifiedBy, DateTime now)
        {
            if (serviceRequest.customerMetaData != null)
            {
                var meta = serviceRequest.customerMetaData;
                _mapperUtility.Map(customer, meta);
                meta.ModifiedAt = now;
                meta.ModifiedBy = modifiedBy;
                await _unitOfWork.SRCustomerMetaData.UpdateAsync(meta);
            }
            else
            {
                var newMeta = _mapperUtility.Map<CustomerDto, ServiceRequestCustomerMetaData>(customer);
                newMeta.ServiceRequest = serviceRequest;
                newMeta.RequestID = serviceRequest.Id;
                newMeta.CustomerType = "Individual";
                newMeta.TenantID = serviceRequest.TenantID;
                newMeta.OrgID = serviceRequest.OrgID;
                newMeta.CustomerID = serviceRequest.CustomerID;
                newMeta.CustomerMetaGuid = Guid.NewGuid();
                newMeta.CreatedAt = now;
                newMeta.CreatedBy = modifiedBy;
                await _unitOfWork.SRCustomerMetaData.AddTransactionAsync(newMeta);
            }
        }

        private async Task UpdateOrCreateVehicleMetadataAsync(ServiceRequest serviceRequest, CustomerDto? customer, VehicleDomainDTO vehicle, string modifiedBy, DateTime now)
        {
            var firstVehicle = serviceRequest.vehicleMetaData?.FirstOrDefault();
            if (firstVehicle != null)
            {
                _mapperUtility.Map(vehicle, firstVehicle);
                if (customer != null)
                {
                    firstVehicle.OwnerName = string.Join(" ", new[] { customer.FirstName, customer.LastName }.Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s!.Trim())).Trim();
                    firstVehicle.Email = customer.Email;
                    firstVehicle.ContactNumber = string.IsNullOrEmpty(customer.MobilePhone) ? customer.Phone ?? "" : customer.MobilePhone;
                    firstVehicle.Address = customer.Address;
                }
                firstVehicle.ModifiedAt = now;
                firstVehicle.ModifiedBy = modifiedBy;
                await _unitOfWork.SRVehicleMetaData.UpdateAsync(firstVehicle);
            }
            else
            {
                await StoreVehicleMetadataAsync(serviceRequest, customer ?? new CustomerDto(), vehicle);
            }
        }

        private async Task UpdateOrCreateJsonMetadataAsync<T>(ServiceRequest serviceRequest, string keyName, T payload, string modifiedBy, DateTime now)
        {
            var jsonString = JsonSerializer.Serialize(payload, new JsonSerializerOptions
            {
                WriteIndented = false,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            });

            var existingMeta = serviceRequest.MetadataEntries?.FirstOrDefault(m => string.Equals(m.KeyName, keyName, StringComparison.OrdinalIgnoreCase));
            if (existingMeta != null)
            {
                existingMeta.KeyValue = jsonString;
                existingMeta.ModifiedAt = now;
                existingMeta.ModifiedBy = modifiedBy;
                await _unitOfWork.ServiceRequestMetadata.UpdateAsync(existingMeta);
            }
            else
            {
                var newMeta = new ServiceRequestMetadata
                {
                    MetaDataGuid = Guid.NewGuid(),
                    RequestID = serviceRequest.Id,
                    KeyName = keyName,
                    KeyValue = jsonString,
                    CreatedAt = now,
                    CreatedBy = modifiedBy,
                    IsActive = true,
                    IsDeleted = false
                };
                await _unitOfWork.ServiceRequestMetadata.AddTransactionAsync(newMeta);
            }
        }

        public async Task StoreCustomerMetadataAsync(ServiceRequest serviceRequest, CustomerDto customer)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            // Serialize customer DTO to dictionary (key-value pairs)
            // var jsonString = JsonSerializer.Serialize(customer);
            //  var keyValues = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonString);



            //if (keyValues == null)
            //    throw new InvalidOperationException("Failed to serialize customer data.");

            var jsonString = JsonSerializer.Serialize(customer, new JsonSerializerOptions
            {
                WriteIndented = false,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            });


            var metadataEntries = new List<ServiceRequestMetadata>();

            var metadata = new ServiceRequestMetadata
            {
                MetaDataGuid = Guid.NewGuid(),
                ServiceRequest = serviceRequest,
                KeyName = "customer",
                KeyValue = jsonString,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System", // Adjust accordingly
                IsActive = true,
                IsDeleted = false
            };
            await _unitOfWork.ServiceRequestMetadata.AddTransactionAsync(metadata);

            //foreach (var kvp in keyValues)
            //{
            //    if (kvp.Value == null) continue;

            //    var metadata = new ServiceRequestMetadata
            //    {
            //        MetaDataGuid = Guid.NewGuid(),
            //        RequestID = requestId,
            //        KeyName = kvp.Key,
            //        KeyValue = kvp.Value.ToString() ?? string.Empty,
            //        CreatedAt = DateTime.UtcNow,
            //        CreatedBy = "System", // Adjust accordingly
            //        IsActive = true,
            //        IsDeleted = false
            //    };

            //    metadataEntries.Add(metadata);
            //}

            // Save all metadata entries to the repository
            //foreach (var entry in metadataEntries)
            //{
            //    await _unitOfWork.ServiceRequestMetadata.AddAsync(entry);
            //    //await _metadataRepo.AddAsync(entry);
            //}

        }

        public async Task StoreSvCusomerMetadataAsync(ServiceRequest serviceRequest, CustomerDto customer)
        {
            if (customer == null)
                return;

            var svCustomerMetaData = new ServiceRequestCustomerMetaData();

            // Map CustomerDto into the metadata (if present)
            if (customer != null)
            {
                svCustomerMetaData = _mapperUtility.Map<CustomerDto, ServiceRequestCustomerMetaData>(customer);

             
            }

         


            // Attach relationship and audit info
            svCustomerMetaData.ServiceRequest = serviceRequest;
         

            svCustomerMetaData.CustomerType = "Individual";
            svCustomerMetaData.TenantID = serviceRequest.TenantID;
            svCustomerMetaData.OrgID = serviceRequest.OrgID;
            svCustomerMetaData.CustomerID = serviceRequest.CustomerID;
            svCustomerMetaData.CustomerMetaGuid = Guid.NewGuid();
            svCustomerMetaData.CreatedAt = DateTime.UtcNow;
            svCustomerMetaData.CreatedBy = "System";

            // Save if there's any data populated
            await _unitOfWork.SRCustomerMetaData.AddTransactionAsync(svCustomerMetaData);
        }
        public async Task StoreVehicleMetadataAsync(ServiceRequest serviceRequest, CustomerDto customer, VehicleDomainDTO vehicle)
        {
            if (customer == null && vehicle == null)
                return;

            var svVehicleMetaData = new ServiceRequestVehicleMetaData();

            // Map CustomerDto into the metadata (if present)
            if (customer != null)
            {
                var customerMeta = _mapperUtility.Map<CustomerDto, ServiceRequestVehicleMetaData>(customer);

                svVehicleMetaData.OwnerName = customerMeta.OwnerName;
                svVehicleMetaData.Email = customerMeta.Email;
                svVehicleMetaData.ContactNumber = customerMeta.ContactNumber;
                svVehicleMetaData.Address = customerMeta.Address;
                svVehicleMetaData.OwnerID = customerMeta.OwnerID;
                svVehicleMetaData.OwnerType = customerMeta.OwnerType;
            }

            // Map VehicleDomainDTO into the metadata (if present)
            if (vehicle != null)
            {
                var vehicleMeta = _mapperUtility.Map<VehicleDomainDTO, ServiceRequestVehicleMetaData>(vehicle);

                svVehicleMetaData.VehicleId = vehicleMeta.VehicleId;
                svVehicleMetaData.Make = vehicleMeta.Make;
                svVehicleMetaData.Model = vehicleMeta.Model;
                svVehicleMetaData.Year = vehicleMeta.Year;
                svVehicleMetaData.VIN = vehicleMeta.VIN;
                svVehicleMetaData.LicensePlate = vehicleMeta.LicensePlate;
            }

            // Attach relationship and audit info
            svVehicleMetaData.ServiceRequest = serviceRequest;
            svVehicleMetaData.CreatedAt = DateTime.UtcNow;
            svVehicleMetaData.CreatedBy = "System";

            // Save if there's any data populated
            await _unitOfWork.SRVehicleMetaData.AddTransactionAsync(svVehicleMetaData);
        }


        public async Task StoreBookingMetadataAsync(ServiceRequest serviceRequest, BookingDto booking)
        {
            if (booking == null)
                throw new ArgumentNullException(nameof(booking));

            // Serialize BookingDto to dictionary
            var jsonString = JsonSerializer.Serialize(booking, new JsonSerializerOptions
            {
                WriteIndented = false,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            });

           // var jsonString = JsonSerializer.Serialize(booking);
           // var keyValues = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonString);

            //if (keyValues == null)
            //    throw new InvalidOperationException("Failed to serialize booking data.");

            var metadataEntries = new List<ServiceRequestMetadata>();

            var metadata = new ServiceRequestMetadata
            {
                MetaDataGuid = Guid.NewGuid(),
                ServiceRequest = serviceRequest,
                KeyName = "Booking",
                KeyValue = jsonString,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System", // or your current user context
                IsActive = true,
                IsDeleted = false
            };
            await _unitOfWork.ServiceRequestMetadata.AddAsync(metadata);
            //foreach (var kvp in keyValues)
            //{
            //    if (kvp.Value == null) continue;


            //    metadataEntries.Add(metadata);
            //}

            //foreach (var entry in metadataEntries)
            //{
            //    await _unitOfWork.ServiceRequestMetadata.AddAsync(entry);

            //   // await _metadataRepo.AddAsync(entry);
            //}
        }

        public async Task StoreDomainDataAsync(ServiceRequest serviceRequest, DomainDataDto domainData)
        {
            if (domainData == null)
                throw new ArgumentNullException(nameof(domainData));

            // Serialize entire DomainDataDto to JSON string
            var jsonString = JsonSerializer.Serialize(domainData, new JsonSerializerOptions
            {
                WriteIndented = false,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            });

            var metadata = new ServiceRequestMetadata
            {
                MetaDataGuid = Guid.NewGuid(),
                ServiceRequest = serviceRequest, // ✅ Use navigation property instead of RequestID
                KeyName = "DomainData",
                KeyValue = jsonString,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System",
                IsActive = true,
                IsDeleted = false
            };
            await _unitOfWork.ServiceRequestMetadata.AddTransactionAsync(metadata);
            //await _metadataRepo.AddAsync(metadata);
        }


    }


}
