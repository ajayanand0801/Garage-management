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

        private readonly IMapperUtility _mapperUtility;
        private readonly IUnitOfWork _unitOfWork;

        // Assume you inject the schema as well (static or from config)
        private readonly string _jsonSchema = JsonRules.ServiceRequestSchema;

        public ServiceRequestService(
         IJsonValidator jsonValidator,
         IGenericRepository<ServiceRequest> serviceRequestRepo,
         IGenericRepository<ServiceRequestDocument> documentRepo,
         IGenericRepository<ServiceRequestMetadata> metadataRepo,
         IMapperUtility mapperUtility,
         IUnitOfWork unitOfWork)
        {
            _jsonValidator = jsonValidator;
            _serviceRequestRepo = serviceRequestRepo;
            _documentRepo = documentRepo;
            _metadataRepo = metadataRepo;
            _mapperUtility = mapperUtility;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Create(ServiceRequestDto request)
        {
            // Run schema validation
            if (!_jsonValidator.ValidateJsonPayload(request, _jsonSchema, out List<string> errors))
            {
                // 🔥 Validation failed, map errors
                var errorMessage = string.Join("; ", errors);
                // Throw a custom exception (or return a result object instead)
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
