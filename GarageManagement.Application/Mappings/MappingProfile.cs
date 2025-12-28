using AutoMapper;
using GarageManagement.Application.DTOs;
using GarageManagement.Domain.Entites;
using GarageManagement.Domain.Entites.Quotation;
using GarageManagement.Domain.Entites.Request;
using GarageManagement.Domain.Entites.Vehicles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace GarageManagement.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Main DTO -> Entity mapping
            CreateMap<VehicleDto, Vehicle>()
                .ForMember(dest => dest.BrandID, opt => opt.MapFrom(src => src.Brand.BrandID))
                 .ForMember(dest => dest.VehicleID, opt => opt.MapFrom(src => src.VehicleID))
                .ForMember(dest => dest.ModelID, opt => opt.MapFrom(src => src.Model.ModelID))
                .ForMember(dest => dest.ModelYearID, opt => opt.MapFrom(src => src.ModelYear.ModelYearID))
                .ForMember(dest => dest.RegistrationNumber, opt => opt.MapFrom(src => src.RegistrationNumber))
                .ForMember(dest => dest.RegDate, opt => opt.MapFrom(src => src.RegDate))
                .ForMember(dest => dest.ManufactureDate, opt => opt.MapFrom(src => src.ManufactureDate))
                .ForMember(dest => dest.EngineSize, opt => opt.MapFrom(src => src.EngineSize))
                .ForMember(dest => dest.DrivetrainID, opt => opt.MapFrom(src => src.DrivetrainID))
                .ForMember(dest => dest.TransmissionID, opt => opt.MapFrom(src => src.TransmissionID))
                .ForMember(dest => dest.BodyTypeID, opt => opt.MapFrom(src => src.BodyTypeID))
                .ForMember(dest => dest.FuelTypeID, opt => opt.MapFrom(src => src.FuelTypeID))
                .ForMember(dest => dest.Owners, opt => opt.MapFrom(src => src.Owners))
                .ForMember(dest => dest.Brand, opt => opt.Ignore())
                .ForMember(dest => dest.Model, opt => opt.Ignore())
                .ForMember(dest => dest.ModelYear, opt => opt.Ignore())
                .AfterMap((src, dest) =>
                {
                    if (dest.Owners != null)
                    {
                        foreach (var owner in dest.Owners)
                        {
                           owner.VehicleID = src.VehicleID.HasValue? src.VehicleID.Value:0;
                            owner.VehicleVIN = src.VIN;
                        }
                    }
                });

            // Owner DTO -> Entity
            CreateMap<VehicleOwnerDto, VehicleOwner>();

            // VehicleBrand Entity -> DTO
            CreateMap<VehicleBrand, VehicleBrandDto>()
                .ForMember(dest => dest.BrandID, opt => opt.MapFrom(src => src.BrandID))
                .ForMember(dest => dest.BrandName, opt => opt.MapFrom(src => src.BrandName));

            // VehicleModel Entity -> DTO
            CreateMap<VehicleModel, VehicleModelDto>()
                .ForMember(dest => dest.ModelID, opt => opt.MapFrom(src => src.ModelID))
                .ForMember(dest => dest.ModelName, opt => opt.MapFrom(src => src.ModelName));

            // VehicleModelYear Entity -> DTO
            CreateMap<VehicleModelYear, VehicleModelYearDto>()
                .ForMember(dest => dest.ModelYearID, opt => opt.MapFrom(src => src.ModelYearID))
                .ForMember(dest => dest.ModelYear, opt => opt.MapFrom(src => src.ModelYear));

            // VehicleOwner Entity -> DTO (complete mapping)
            CreateMap<VehicleOwner, VehicleOwnerDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.OwnerID, opt => opt.MapFrom(src => src.OwnerID))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
                .ForMember(dest => dest.OwnerName, opt => opt.MapFrom(src => src.OwnerName))
                .ForMember(dest => dest.TradeLicenseNo, opt => opt.MapFrom(src => src.TradeLicenseNo))
                .ForMember(dest => dest.ContactNumber, opt => opt.MapFrom(src => src.ContactNumber))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.OwnershipStartDate, opt => opt.MapFrom(src => src.OwnershipStartDate))
                .ForMember(dest => dest.OwnershipEndDate, opt => opt.MapFrom(src => src.OwnershipEndDate));

            // Vehicle Entity -> DTO
            CreateMap<Vehicle, VehicleDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.VehicleID, opt => opt.MapFrom(src => src.VehicleID))
                .ForMember(dest => dest.VIN, opt => opt.MapFrom(src => src.VIN))
                .ForMember(dest => dest.Color, opt => opt.MapFrom(src => src.Color))
                .ForMember(dest => dest.RegistrationNumber, opt => opt.MapFrom(src => src.RegistrationNumber))
                .ForMember(dest => dest.EngineNumber, opt => opt.MapFrom(src => src.EngineNumber))
                .ForMember(dest => dest.ChassisNumber, opt => opt.MapFrom(src => src.ChassisNumber))
                .ForMember(dest => dest.RegDate, opt => opt.MapFrom(src => src.RegDate))
                .ForMember(dest => dest.ManufactureDate, opt => opt.MapFrom(src => src.ManufactureDate))
                .ForMember(dest => dest.EngineSize, opt => opt.MapFrom(src => src.EngineSize))
                .ForMember(dest => dest.DrivetrainID, opt => opt.MapFrom(src => src.DrivetrainID))
                .ForMember(dest => dest.TransmissionID, opt => opt.MapFrom(src => src.TransmissionID))
                .ForMember(dest => dest.BodyTypeID, opt => opt.MapFrom(src => src.BodyTypeID))
                .ForMember(dest => dest.FuelTypeID, opt => opt.MapFrom(src => src.FuelTypeID))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => src.IsDeleted))
                .ForMember(dest => dest.Brand, opt => opt.MapFrom(src => src.Brand))
                .ForMember(dest => dest.Model, opt => opt.MapFrom(src => src.Model))
                .ForMember(dest => dest.ModelYear, opt => opt.MapFrom(src => src.ModelYear))
                .ForMember(dest => dest.Owners, opt => opt.MapFrom(src => src.Owners));

            CreateMap<VehicleLookupDTO, VehicleLookup>()
     .ForMember(dest => dest.LookupID, opt => opt.MapFrom(src => src.ID));

            CreateMap<VehicleLookup, VehicleLookupDTO>()
                .ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.LookupID));

            //service Request

            // Dto -> Entity
            CreateMap<ServiceRequestDto, ServiceRequest>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ServiceRequestID))
                .ForMember(dest => dest.TenantID, opt => opt.MapFrom(src => src.TenantID ?? 0))
                .ForMember(dest => dest.OrgID, opt => opt.MapFrom(src => src.OrgID ?? 0))
                .ForMember(dest => dest.DomainID, opt => opt.MapFrom(src => src.DomainID ?? 0))
                .ForMember(dest => dest.ServiceID, opt => opt.MapFrom(src => src.ServiceID ?? 0))
                .ForMember(dest => dest.DomainType, opt => opt.MapFrom(src => src.DomainType))
                .ForMember(dest => dest.ServiceType, opt => opt.MapFrom(src => src.ServiceType))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => src.Priority))
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt ?? DateTime.UtcNow))
                .ForMember(dest => dest.CustomerID, opt => opt.MapFrom(src => src.Customer.CustomerID)) // assuming CustomerDto has this
                .ForMember(dest => dest.Documents, opt => opt.MapFrom(src => src.Documents))
                .ForMember(dest => dest.MetadataEntries, opt => opt.Ignore()) // Handle manually if needed
                .ForMember(dest => dest.VehicleID, opt => opt.MapFrom(src => src.DomainData.Vehicle.VehicleID));// assuming this is how it's stored
                //.ForMember(dest => dest.RealEstatePropertyID, opt => opt.MapFrom(src => src.DomainData.PropertyID)) // example mapping
                //.ForMember(dest => dest.HealthProfileID, opt => opt.MapFrom(src => src.DomainData.HealthProfileID)); // if available


            // Entity -> DTO
            CreateMap<ServiceRequest, ServiceRequestDto>()
                .ForMember(dest => dest.ServiceRequestID, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.TenantID, opt => opt.MapFrom(src => src.TenantID))
                .ForMember(dest => dest.OrgID, opt => opt.MapFrom(src => src.OrgID))
                .ForMember(dest => dest.DomainID, opt => opt.MapFrom(src => src.DomainID))
                .ForMember(dest => dest.ServiceID, opt => opt.MapFrom(src => src.ServiceID))
                .ForMember(dest => dest.DomainType, opt => opt.MapFrom(src => src.DomainType))
                .ForMember(dest => dest.ServiceType, opt => opt.MapFrom(src => src.ServiceType))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => src.Priority))
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.Documents, opt => opt.MapFrom(src => src.Documents))
                .ForMember(dest => dest.Customer, opt => opt.Ignore()) // You might join this manually via navigation or service
                .ForMember(dest => dest.Booking, opt => opt.Ignore()) // Populate externally
                .ForMember(dest => dest.DomainData, opt => opt.MapFrom(src => new DomainDataDto
                {
                    //VehicleID = src.VehicleID,
                    //PropertyID = src.RealEstatePropertyID,
                    //HealthProfileID = src.HealthProfileID
                }));

            CreateMap<DocumentDto, ServiceRequestDocument>()
    .ForMember(dest => dest.SvDocumentGuid, opt => opt.MapFrom(_ => Guid.NewGuid()))
    .ForMember(dest => dest.DocumentType, opt => opt.MapFrom(src => src.DocumentType))
    .ForMember(dest => dest.FileName, opt => opt.MapFrom(src => src.FileName))
    .ForMember(dest => dest.FileURL, opt => opt.MapFrom(src => src.FileUrl))             // Different naming
    .ForMember(dest => dest.MimeType, opt => opt.MapFrom(src => src.FileType))           // Different naming
    .ForMember(dest => dest.FileSizeKB, opt => opt.Ignore())                             // If not provided by DTO
    .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.UploadedAt ?? DateTime.UtcNow))
    .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.UploadedBy))
    .ForMember(dest => dest.IsActive, opt => opt.MapFrom(_ => true))
    .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(_ => false))
    .ForMember(dest => dest.ServiceRequest, opt => opt.Ignore())                         // Navigation
    .ForMember(dest => dest.RequestID, opt => opt.Ignore());                             // FK set manually


            CreateMap<ServiceRequestDocument, DocumentDto>()
    .ForMember(dest => dest.DocumentID, opt => opt.MapFrom(src => src.Id))
    .ForMember(dest => dest.FileName, opt => opt.MapFrom(src => src.FileName))
    .ForMember(dest => dest.FileUrl, opt => opt.MapFrom(src => src.FileURL))
    .ForMember(dest => dest.FileType, opt => opt.MapFrom(src => src.MimeType))
    .ForMember(dest => dest.DocumentType, opt => opt.MapFrom(src => src.DocumentType))
    .ForMember(dest => dest.UploadedAt, opt => opt.MapFrom(src => src.CreatedAt))
    .ForMember(dest => dest.UploadedBy, opt => opt.MapFrom(src => src.CreatedBy));


            CreateMap<CustomerDto, ServiceRequestVehicleMetaData>()
    .ForMember(dest => dest.OwnerName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}".Trim()))
    .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
    .ForMember(dest => dest.ContactNumber, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.MobilePhone) ? src.Phone : src.MobilePhone))
    .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
    //.ForMember(dest => dest.City, opt => opt.Ignore()) // Not present in entity, ignore or map to metadata
    //.ForMember(dest => dest.State, opt => opt.Ignore()) // Same
    //.ForMember(dest => dest.PostalCode, opt => opt.Ignore()) // Same
    //.ForMember(dest => dest.Country, opt => opt.Ignore()) // Same
    .ForMember(dest => dest.OwnerID, opt => opt.MapFrom(src => src.CustomerID))
    .ForMember(dest => dest.OwnerType, opt => opt.MapFrom(_ => "Individual")); // or "Company" if applicable
                                                                               // .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<VehicleDomainDTO, ServiceRequestVehicleMetaData>()
    .ForMember(dest => dest.VehicleId, opt => opt.MapFrom(src => src.VehicleID))
    .ForMember(dest => dest.Make, opt => opt.MapFrom(src => src.Make))
    .ForMember(dest => dest.Model, opt => opt.MapFrom(src => src.Model))
    .ForMember(dest => dest.Year, opt => opt.MapFrom(src => src.Year))
    .ForMember(dest => dest.VIN, opt => opt.MapFrom(src => src.VIN))
    .ForMember(dest => dest.LicensePlate, opt => opt.MapFrom(src => src.LicensePlate));
            //.ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<CustomerDto, ServiceRequestCustomerMetaData>()
                   .ForMember(dest => dest.CustomerID, opt => opt.MapFrom(src => src.CustomerID ?? 0))
                   .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                   .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
                   .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Phone))
                   .ForMember(dest => dest.MobileNo, opt => opt.MapFrom(src => src.MobilePhone))
                   .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                   .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                   .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.City))
                   .ForMember(dest => dest.State, opt => opt.MapFrom(src => src.State))
                   .ForMember(dest => dest.PostalCode, opt => opt.MapFrom(src => src.PostalCode))
                   .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.Country))
                   .ForMember(dest => dest.CountryCode, opt => opt.Ignore()) // You can map this separately if needed

                   // Fields not in CustomerDto — set default/null or map manually later
                   .ForMember(dest => dest.CustomerMetaGuid, opt => opt.Ignore())
                   .ForMember(dest => dest.TenantID, opt => opt.Ignore())
                   .ForMember(dest => dest.OrgID, opt => opt.Ignore())
                   //.ForMember(dest => dest.RequestID, opt => opt.Ignore())
                   .ForMember(dest => dest.CustomerType, opt => opt.Ignore())
                   .ForMember(dest => dest.CompanyName, opt => opt.Ignore())
                   .ForMember(dest => dest.TradeLicenseNo, opt => opt.Ignore())
                   .ForMember(dest => dest.TaxID, opt => opt.Ignore())
                   .ForMember(dest => dest.RegistrationNumber, opt => opt.Ignore())
                   .ForMember(dest => dest.ContactPerson, opt => opt.Ignore())
                   .ForMember(dest => dest.ContactPhone, opt => opt.Ignore())
                   .ForMember(dest => dest.ServiceRequest, opt => opt.Ignore());

            //quotationDTO 

            // DTO to Entity
            CreateMap<QuotationDTO, Quotation>()

                    .ForMember(dest => dest.Id, opt => opt.Ignore()) // ← This is critical

                    .ForMember(dest => dest.QuotationId, opt => opt.Ignore()) // ✅ Ignore identity column

                //.ForMember(dest => dest.QuotationId, opt => opt.MapFrom(src => src.QuotationID ?? 0))
                .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.EstimatedTotal ?? 0))
                .ForMember(dest => dest.CurrencyCode, opt => opt.MapFrom(src => src.Currency))
                .ForMember(dest => dest.DiscountAmount, opt => opt.MapFrom(src => src.Discount ?? 0))
                .ForMember(dest => dest.TaxAmount, opt => opt.MapFrom(src => src.Tax ?? 0))
                .ForMember(dest => dest.FinalAmount, opt => opt.MapFrom(src => src.GrandTotal ?? 0))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status ?? "pending"))
            .ForMember(dest => dest.QuotationItems, opt => opt.MapFrom(src => src.QuotationItems));
            //.ForAllOtherMembers(opt => opt.Ignore());

            // Entity to DTO
            CreateMap<Quotation, QuotationDTO>()

                 .ForMember(dest => dest.QuotationID, opt => opt.MapFrom(src => src.QuotationId))
                 .ForMember(dest => dest.QuotationNo, opt => opt.MapFrom(src => src.ReferenceNo))
                 .ForMember(dest => dest.QuoteGuid, opt => opt.MapFrom(src => src.QuoteGuid))
                  .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy))
                //.ForMember(dest => dest.QuotationID, opt => opt.MapFrom(src => src.QuotationId))
                .ForMember(dest => dest.EstimatedTotal, opt => opt.MapFrom(src => src.TotalAmount))
                .ForMember(dest => dest.Currency, opt => opt.MapFrom(src => src.CurrencyCode))
                .ForMember(dest => dest.Discount, opt => opt.MapFrom(src => src.DiscountAmount))
                .ForMember(dest => dest.Tax, opt => opt.MapFrom(src => src.TaxAmount))
                .ForMember(dest => dest.GrandTotal, opt => opt.MapFrom(src => src.FinalAmount))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.QuotationItems, opt => opt.MapFrom(src => src.QuotationItems));
            //.ForAllOtherMembers(opt => opt.Ignore());

            //CreateMap<QuotationItemDto, QuotationItem>()
            //.ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description ?? string.Empty))
            // .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name ?? string.Empty))
            //.ForMember(dest => dest.ItemType, opt => opt.MapFrom(src => src.ItemType ?? string.Empty))
            //.ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.code ?? string.Empty))
            //.ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity>0
            //))
            //.ForMember(dest => dest.Hours, opt => opt.MapFrom(src => src.Hours))
            //.ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.UnitPrice)).ReverseMap()
            //.ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.TotalPrice)).ReverseMap()
            //.ForMember(dest => dest.DiscountAmount, opt => opt.MapFrom(src => src.DiscountAmount ?? 0))
            //.ForMember(dest => dest.TaxAmount, opt => opt.MapFrom(src => src.TaxAmount ?? 0))
            //.ForMember(dest => dest.NetTotal, opt => opt.MapFrom(src => src.NetTotal ?? 0))
            //.ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt)).ReverseMap()
            //.ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy)).ReverseMap()
            //.ForMember(dest => dest.IsOptional, opt => opt.MapFrom(src => src.IsOptional ?? false))
            //.ForMember(dest => dest.IsApproved, opt => opt.MapFrom(src => src.IsApproved ?? false))
            //.ForMember(dest => dest.Quotation, opt => opt.Ignore()) // You can handle this manually
            //.ForMember(dest => dest.ItemGguid, opt => opt.Ignore()) // Auto-generated in DB or manually set
            //.ForMember(dest => dest.QuotationID, opt => opt.Ignore()); // Set manually during save

            CreateMap<QuotationItemDto, QuotationItem>()
           .ForMember(dest => dest.Id, opt => opt.Ignore())
           .ForMember(dest => dest.QuotationID, opt => opt.Ignore())
           .ForMember(dest => dest.Quotation, opt => opt.Ignore())
           .ForMember(dest => dest.ItemGguid, opt => opt.Ignore())
           .ForMember(dest => dest.IsActive, opt => opt.Ignore())
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Description ?? string.Empty))
           .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description ?? string.Empty))
           .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.code ?? string.Empty))
           .ForMember(dest => dest.ItemType, opt => opt.MapFrom(src => src.ItemType ?? string.Empty))
           .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
           .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.UnitPrice))
           .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.TotalPrice))
           .ForMember(dest => dest.DiscountAmount, opt => opt.MapFrom(src => src.DiscountAmount ?? 0))
           .ForMember(dest => dest.TaxAmount, opt => opt.MapFrom(src => src.TaxAmount ?? 0))
           .ForMember(dest => dest.NetTotal, opt => opt.MapFrom(src => src.NetTotal ?? 0))
           .ForMember(dest => dest.IsOptional, opt => opt.MapFrom(src => src.IsOptional ?? false))
           .ForMember(dest => dest.IsApproved, opt => opt.MapFrom(src => src.IsApproved ?? false))
           // ignore audit/created/modified that you set manually
           .ForMember(dest => dest.CreatedAt, opt => opt.Condition((src, dest, srcMember) => false))
            .ForMember(dest => dest.CreatedBy, opt => opt.Condition((src, dest, srcMember) => false))


          // .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
           //.ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
           //.ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))

          // .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy))

           .ForMember(dest => dest.ModifiedAt, opt => opt.Ignore())
           .ForMember(dest => dest.ModifiedBy, opt => opt.Ignore())
           ;

            // Entity ➡️ DTO
            CreateMap<QuotationItem, QuotationItemDto>()
                .ForMember(dest => dest.ItemID, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ItemGuid, opt => opt.MapFrom(src => src.ItemGguid))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.PartNumber, opt => opt.MapFrom(src => src.Code))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.ItemType, opt => opt.MapFrom(src => src.ItemType))
                .ForMember(dest => dest.code, opt => opt.MapFrom(src => src.Code))
                // .ForMember(dest => dest.cu, opt => opt.MapFrom(src => src.CurrencyCode))
                .ForMember(dest => dest.Hours, opt => opt.MapFrom(src => src.Hours ?? 0))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
                .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.UnitPrice))
                .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.TotalPrice))
                .ForMember(dest => dest.DiscountAmount, opt => opt.MapFrom(src => src.DiscountAmount ?? 0))
                .ForMember(dest => dest.TaxAmount, opt => opt.MapFrom(src => src.TaxAmount ?? 0))
                .ForMember(dest => dest.NetTotal, opt => opt.MapFrom(src => src.NetTotal))
                .ForMember(dest => dest.IsOptional, opt => opt.MapFrom(src => src.IsOptional))
                .ForMember(dest => dest.IsApproved, opt => opt.MapFrom(src => src.IsApproved))
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.ModifiedBy, opt => opt.MapFrom(src => src.ModifiedBy))
                .ForMember(dest => dest.ModifiedAt, opt => opt.MapFrom(src => src.ModifiedAt))
                 .ForMember(dest => dest.IsActive, opt => opt.Ignore());
                //.ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive));
            // .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<QuotationItemDto, QuotationItem>()
   //.ForAllMembers(opt => opt.Condition((src, dest, srcMember) =>
   //{
   //    // Skip nulls and empty strings
   //    if (srcMember == null) return false;
   //    if (srcMember is string str) return !string.IsNullOrWhiteSpace(str);
   //    return true;
   //}))
   .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ItemID))
    .ForMember(dest => dest.ItemGguid, opt => opt.MapFrom(src => src.ItemGuid))
    .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.PartNumber ?? string.Empty))
    .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Description))
    .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description ?? string.Empty))
    .ForMember(dest => dest.ItemType, opt => opt.MapFrom(src => src.ItemType ?? string.Empty))
    .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
    .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.UnitPrice))
    .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.TotalPrice))
    .ForMember(dest => dest.DiscountAmount, opt => opt.MapFrom(src => src.DiscountAmount ?? 0))
    .ForMember(dest => dest.TaxAmount, opt => opt.MapFrom(src => src.TaxAmount ?? 0))
    .ForMember(dest => dest.NetTotal, opt => opt.MapFrom(src => src.NetTotal ?? 0))
    .ForMember(dest => dest.IsOptional, opt => opt.MapFrom(src => src.IsOptional ?? false))
    .ForMember(dest => dest.IsApproved, opt => opt.MapFrom(src => src.IsApproved ?? false))
   
    .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src =>
        src.CreatedAt.HasValue ? src.CreatedAt.Value : DateTime.UtcNow))
    .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy))
    .ForMember(dest => dest.ModifiedAt, opt => opt.MapFrom(src => src.ModifiedAt))
    .ForMember(dest => dest.ModifiedBy, opt => opt.MapFrom(src => src.ModifiedBy))
    .ForMember(dest => dest.QuotationID, opt => opt.Ignore())
     .ForMember(dest => dest.IsActive, opt => opt.Ignore())
    .ForMember(dest => dest.Quotation, opt => opt.Ignore());



            // DTO ➡️ Entity (Only map values if they are not null / empty)
            // Optional: Reverse mapping
            //CreateMap<QuotationItem, QuotationItemDto>()
            //     .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.UnitPrice))
            //    .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.TotalPrice))
            //    .ForMember(dest => dest.DiscountAmount, opt => opt.MapFrom(src => src.DiscountAmount ?? 0))
            //    .ForMember(dest => dest.TaxAmount, opt => opt.MapFrom(src => src.TaxAmount ?? 0))
            //   // .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy))
            //    //.ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
            //    .ForMember(dest => dest.code, opt => opt.MapFrom(src => src.Code))
            //    // .ForMember(dest => dest.ItemGuid, opt => opt.MapFrom(src => src.ItemGguid))
            //    .ForMember(dest => dest.IsOptional, opt => opt.MapFrom(src => src.IsOptional))
            //    .ForMember(dest => dest.IsApproved, opt => opt.MapFrom(src => src.IsApproved))
            //    .ForMember(dest => dest.NetTotal, opt => opt.MapFrom(src => src.NetTotal))
            //    .ForMember(dest => dest.DiscountAmount, opt => opt.MapFrom(src => src.DiscountAmount))
            //    .ForMember(dest => dest.TaxAmount, opt => opt.MapFrom(src => src.TaxAmount))
            //    .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            //  //  .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
            ////.ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy))
            //.ForMember(dest => dest.ItemID, opt => opt.MapFrom(src => src.Id)) // Assuming BaseEntity has Id
            //    .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name)) // mapping description as part name
            //    .ForMember(dest => dest.PartNumber, opt => opt.MapFrom(src => src.Code))
            //    .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            //    .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity));
            ////.ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name ?? string.Empty));

            ///////////////////////////////////////
            //CreateMap<QuotationItemDto, QuotationItem>()
            //.ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description ?? src.PartName)) // fallback if no description
            //.ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.PartNumber))
            //.ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
            //.ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.UnitPrice))
            //.ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.TotalPrice)).ReverseMap();
            ////.ForAllOtherMembers(opt => opt.Ignore()); // ignore unused fields in the entity

            //CreateMap<QuotationItem, QuotationItemDto>()
            //    .ForMember(dest => dest.ItemID, opt => opt.MapFrom(src => src.Id)) // Assuming BaseEntity has Id
            //    .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name)) // mapping description as part name
            //    .ForMember(dest => dest.PartNumber, opt => opt.MapFrom(src => src.Code)).ReverseMap()
            //    .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description)).ReverseMap()
            //    .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity)).ReverseMap()
            //    .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.UnitPrice)).ReverseMap()
            //    .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.TotalPrice)).ReverseMap();
            // .ForAllOtherMembers(opt => opt.Ignore()); // ignore unused fields in the DTO

            //Customer mappings
            // CrmCustomerDTO -> Customer
            CreateMap<CrmCustomerDTO, Customer>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.CustomerGuid, opt => opt.MapFrom(src => src.CustomerGuid ?? Guid.NewGuid()))
                .ForMember(dest => dest.TenantID, opt => opt.MapFrom(src => src.TenantId ?? 0))
                .ForMember(dest => dest.OrgID, opt => opt.MapFrom(src => src.OrgId ?? 0))
                .ForMember(dest => dest.TaxID, opt => opt.MapFrom(src => src.TaxId))
                .ForMember(dest => dest.ReferralSourceID, opt => opt.MapFrom(src => src.ReferralSourceId))
                .ForMember(dest => dest.CountryID, opt => opt.MapFrom(src => src.CountryId))
                .ForMember(dest => dest.StateID, opt => opt.MapFrom(src => src.StateId))
                .ForMember(dest => dest.CityID, opt => opt.MapFrom(src => src.CityId))
                .ForMember(dest => dest.ReferralSourceID, opt => opt.MapFrom(src => src.ReferralSourceId))
                .ForMember(dest => dest.CreatedAt, opt => opt.Condition((src, dest, srcMember) => srcMember != default(DateTime)))
                .ForMember(dest => dest.ModifiedAt, opt => opt.MapFrom(src => src.ModifiedAt));

            // Customer -> CrmCustomerDTO
            CreateMap<Customer, CrmCustomerDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.CustomerGuid, opt => opt.MapFrom(src => src.CustomerGuid))
                .ForMember(dest => dest.TenantId, opt => opt.MapFrom(src => src.TenantID))
                .ForMember(dest => dest.OrgId, opt => opt.MapFrom(src => src.OrgID))
                .ForMember(dest => dest.TaxId, opt => opt.MapFrom(src => src.TaxID))
                .ForMember(dest => dest.ReferralSourceId, opt => opt.MapFrom(src => src.ReferralSourceID))
                .ForMember(dest => dest.CountryId, opt => opt.MapFrom(src => src.CountryID))
                .ForMember(dest => dest.ReferralSourceId, opt => opt.MapFrom(src => src.ReferralSourceID))
                .ForMember(dest => dest.StateId, opt => opt.MapFrom(src => src.StateID))
                .ForMember(dest => dest.CityId, opt => opt.MapFrom(src => src.CityID));

            // Vehicle Lookup DTOs mappings
            // VehicleBrand -> VehicleMakeDto
            CreateMap<VehicleBrand, VehicleMakeDto>()
                .ForMember(dest => dest.MakeID, opt => opt.MapFrom(src => src.BrandID))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.BrandName));

            // VehicleModel -> VehicleModelLookupDto
            CreateMap<VehicleModel, VehicleModelLookupDto>()
                .ForMember(dest => dest.MakeID, opt => opt.MapFrom(src => src.BrandID))
                .ForMember(dest => dest.ModelID, opt => opt.MapFrom(src => src.ModelID))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.ModelName));

            // VehicleModelYear -> VehicleModelYearLookupDto
            CreateMap<VehicleModelYear, VehicleModelYearLookupDto>()
                .ForMember(dest => dest.ModelID, opt => opt.MapFrom(src => src.ModelID))
                .ForMember(dest => dest.ModelYearID, opt => opt.MapFrom(src => src.ModelYearID))
                .ForMember(dest => dest.ModelYear, opt => opt.MapFrom(src => src.ModelYear));

        }
    }

}
