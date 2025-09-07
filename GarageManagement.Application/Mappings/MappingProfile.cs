using AutoMapper;
using GarageManagement.Application.DTOs;
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
                            owner.VehicleID = src.VehicleID.Value;
                            owner.VehicleVIN = src.VIN;
                        }
                    }
                });

            // Owner DTO -> Entity
            CreateMap<VehicleOwnerDto, VehicleOwner>();

            CreateMap<Vehicle, VehicleDto>()
            .ForMember(dest => dest.Brand, opt => opt.MapFrom(src => src.Brand))
            .ForMember(dest => dest.Model, opt => opt.MapFrom(src => src.Model))
            .ForMember(dest => dest.ModelYear, opt => opt.MapFrom(src => src.ModelYear))
            .ForMember(dest => dest.Owners, opt => opt.MapFrom(src => src.Owners));

            // Optional reverse mapping (Entity -> DTO)
            //CreateMap<Vehicle, VehicleDto>()
            //    .ForMember(dest => dest.Brand, opt => opt.MapFrom(src => new VehicleBrandDto { BrandID = src.BrandID }))
            //    .ForMember(dest => dest.Model, opt => opt.MapFrom(src => new VehicleModelDto { ModelID = src.ModelID }))
            //    .ForMember(dest => dest.ModelYear, opt => opt.MapFrom(src => new VehicleModelYearDto { ModelYearID = src.ModelYearID }))
            //    .ForMember(dest => dest.Owners, opt => opt.MapFrom(src => src.Owners));

            CreateMap<VehicleOwner, VehicleOwnerDto>();

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

        }
    }

}
