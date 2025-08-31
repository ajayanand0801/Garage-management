using GarageManagement.Application.DTOs;
using GarageManagement.Domain.Entites.Vehicles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using AutoMapper;


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
        }
    }

}
