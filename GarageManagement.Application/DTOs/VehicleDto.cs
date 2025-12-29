using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarageManagement.Application.DTOs
{
    public class VehicleDto
    {
        public long? Id { get; set; }
        public long? VehicleID { get; set; }
        public string VIN { get; set; }
        public string? Color { get; set; }
        public string? RegistrationNumber { get; set; }
        public string? EngineNumber { get; set; }
        public string? ChassisNumber { get; set; }
        public string? BodyType { get; set; } = "-";
        public DateTime? RegDate { get; set; }
        public DateTime? ManufactureDate { get; set; }
        public decimal? EngineSize { get; set; }
        public long? DrivetrainID { get; set; }
        public string ? DriveTrain { get; set; }
        public long? TransmissionID { get; set; }
        public string? TransmissionType { get; set; }
        public long? BodyTypeID { get; set; }
        public long? FuelTypeID { get; set; }
        public string? FuelType { get; set; }
        public bool IsActive { get; set; } = true; // Default to true
        public bool IsDeleted { get; set; } = false; // Default to false
        public long? Mileage { get; set; }

        public VehicleBrandDto? Brand { get; set; }
        public VehicleModelDto ?Model { get; set; }
        public VehicleModelYearDto? ModelYear { get; set; }
        public List<VehicleOwnerDto>? Owners { get; set; }
    }

    public class VehicleBrandDto
    {
        public long BrandID { get; set; }
        public string BrandName { get; set; }
    }

    public class VehicleModelDto
    {
        public long ModelID { get; set; }
        public string ModelName { get; set; }
    }

    public class VehicleModelYearDto
    {
        public long ModelYearID { get; set; }
        public int ModelYear { get; set; }
    }

    public class VehicleOwnerDto
    {
        public long? Id { get; set; }
        public long OwnerID { get; set; }
        public int Type { get; set; }
        public string OwnerName { get; set; }
        public string? TradeLicenseNo { get; set; }
        public string ContactNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public DateTime OwnershipStartDate { get; set; }
        public DateTime? OwnershipEndDate { get; set; }
       
    }

    // Vehicle Lookup DTOs
    public class VehicleMakeDto
    {
        public long MakeID { get; set; }
        public string Name { get; set; }
    }

    public class VehicleModelLookupDto
    {
        public long MakeID { get; set; }
        public long ModelID { get; set; }
        public string Name { get; set; }
    }

    public class VehicleModelYearLookupDto
    {
        public long ModelID { get; set; }
        public long ModelYearID { get; set; }
        public int ModelYear { get; set; }
    }

}
