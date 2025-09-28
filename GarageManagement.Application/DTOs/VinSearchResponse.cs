using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarageManagement.Application.DTOs
{
    public class VinSearchResponse
    {
        public string Vin { get; set; }
        public string DecodeStatus { get; set; }
        public VehicleInfo? Vehicle { get; set; }
        public MarketInfo? MarketInfo { get; set; }
        public RecallInfo? RecallInfo { get; set; }
        public bool HistoryCheckAvailable { get; set; }
        public DateTime? Timestamp { get; set; }
    }

    public class VehicleInfo
    {
        public LookupValue? Make { get; set; }
        public LookupValue? Model { get; set; }
        public LookupValue? Trim { get; set; }
        public LookupValue? ModelYear { get; set; }

        public string? BodyStyle { get; set; }
        public EngineInfo? Engine { get; set; }
        public TransmissionInfo? Transmission { get; set; }
        public string? Drivetrain { get; set; }
        public PlantInfo? Plant { get; set; }
        public string? RestraintType { get; set; }
        public string? GrossVehicleWeightRating { get; set; }
        public long Doors { get; set; } = 0;
        public MpgInfo?   Mpg { get; set; }
        public Dimensions? Dimensions { get; set; }

       public string? ChassisNumber {  get; set; }
       public string? RegistrationNumber { get; set; }

        public string? EngineNumber { get; set; }
    }

    public class LookupValue
    {
        public string? RawValue { get; set; }            // From VIN decoding
        public string? ResolvedValue { get; set; }       // From lookup (normalized or enriched)
        public string? LookupId { get; set; }            // Optional: ID from your system
    }

    public class EngineInfo
    {
        public string? Type { get; set; }
        public string? Displacement { get; set; }
        public string? FuelType { get; set; }
        public int Horsepower { get; set; } = 0;
    }

    public class TransmissionInfo
    {
        public string? Type { get; set; }
        public int Speeds { get; set; } = 0;
    }

    public class PlantInfo
    {
        public string? Country { get; set; }
        public string? PlantCode { get; set; }
        public string? PlantName { get; set; }
    }

    public class MpgInfo
    {
        public int City { get; set; } = 0;
        public int Highway { get; set; } = 0;
    }

    public class Dimensions
    {
        public double LengthIn { get; set; } 
        public double WidthIn { get; set; }
        public double HeightIn { get; set; }
        public double WheelbaseIn { get; set; }
    }

    public class MarketInfo
    {
        public string? Manufacturer { get; set; }
        public string? Wmi { get; set; }
        public string? VehicleType { get; set; }
        public string? CountryOfOrigin { get; set; }
    }

    public class RecallInfo
    {
        public bool HasRecalls { get; set; } = false;
        public int RecallCount { get; set; } = 0;
        public List<RecallDetail>? Recalls { get; set; }
    }

    public class RecallDetail
    {
        public string? RecallId { get; set; }
        public string? Component { get; set; }
        public string? Summary { get; set; }
        public DateTime? RecallDate { get; set; }
        public string? Remedy { get; set; }
    }



}
