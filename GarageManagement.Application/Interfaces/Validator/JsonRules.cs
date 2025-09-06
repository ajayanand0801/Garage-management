using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarageManagement.Application.Interfaces.Validator
{
    public static class JsonRules
    {
        public static string VehicleRule = @"
{
  ""id"": { ""type"": ""int"", ""required"": false },
  ""vehicleID"": { ""type"": ""int"", ""required"": false },
  ""vin"": { ""type"": ""string"", ""required"": true, ""minLength"": 1 },
  ""color"": { ""type"": ""string"", ""required"": true },
  ""registrationNumber"": { ""type"": ""string"", ""required"": false, ""nullable"": true },
  ""engineNumber"": { ""type"": ""string"", ""required"": true },
  ""chassisNumber"": { ""type"": ""string"", ""required"": true },
  ""isActive"": { ""type"": ""bool"", ""required"": true },
  ""isDeleted"": { ""type"": ""bool"", ""required"": true },
  ""brand"": {
    ""type"": ""object"",
    ""required"": true,
    ""properties"": {
      ""brandID"": { ""type"": ""int"", ""required"": true },
      ""brandName"": { ""type"": ""string"", ""required"": true }
    }
  },
  ""model"": {
    ""type"": ""object"",
    ""required"": true,
    ""properties"": {
      ""modelID"": { ""type"": ""int"", ""required"": true },
      ""modelName"": { ""type"": ""string"", ""required"": true }
    }
  },
  ""modelYear"": {
    ""type"": ""object"",
    ""required"": true,
    ""properties"": {
      ""modelYearID"": { ""type"": ""int"", ""required"": true },
      ""modelYear"": { ""type"": ""int"", ""required"": true }
    }
  },
  ""owners"": {
    ""type"": ""array"",
    ""required"": true,
    ""items"": {
      ""type"": ""object"",
      ""properties"": {
        ""ownerName"": { ""type"": ""string"", ""required"": true },
        ""contactNumber"": { ""type"": ""string"", ""required"": true },
        ""email"": { ""type"": ""string"", ""required"": true },
        ""address"": { ""type"": ""string"", ""required"": true },
        ""ownershipStartDate"": { ""type"": ""datetime"", ""required"": true },
        ""ownershipEndDate"": { ""type"": ""datetime"", ""required"": false, ""nullable"": true }
      }
    }
  }
}";
        public static string VehicleLookupRule = @"
{
  ""lookupID"": { ""type"": ""int"", ""required"": false },
  ""lookupType"": { ""type"": ""string"", ""required"": true, ""minLength"": 1 },
  ""lookupValue"": { ""type"": ""string"", ""required"": true, ""minLength"": 1 },
  ""isActive"": { ""type"": ""bool"", ""required"": false },
  ""isDeleted"": { ""type"": ""bool"", ""required"": false },
  ""createdAt"": { ""type"": ""datetime"", ""required"": false },
  ""createdBy"": { ""type"": ""string"", ""required"": false, ""nullable"": true },
  ""modifiedAt"": { ""type"": ""datetime"", ""required"": false, ""nullable"": true },
  ""modifiedBy"": { ""type"": ""string"", ""required"": false, ""nullable"": true }
}";

        public static readonly string ServiceRequestSchema = @"
{
  ""$schema"": ""http://json-schema.org/draft-07/schema#"",
  ""title"": ""ServiceRequestDto"",
  ""type"": ""object"",
  ""properties"": {
    ""ServiceRequestID"": { ""type"": ""integer"" },
    ""TenantID"": { ""type"": ""integer"" },
    ""OrgID"": { ""type"": ""integer"" },
    ""DomainID"": { ""type"": ""integer"" },
    ""DomainType"": {
      ""type"": ""string"",
      ""minLength"": 3,
      ""maxLength"": 50
    },
    ""ServiceID"": { ""type"": ""integer"" },
    ""ServiceType"": {
      ""type"": ""string"",
      ""minLength"": 3,
      ""maxLength"": 50
    },
    ""Description"": { ""type"": ""string"" },
    ""Priority"": {
      ""type"": ""string"",
      ""enum"": [""Low"", ""Medium"", ""High"", ""Critical""]
    },
    ""CreatedBy"": {
      ""type"": ""string"",
      ""format"": ""email""
    },
    ""CreatedAt"": {
      ""type"": ""string"",
      ""format"": ""date-time""
    },
    ""Customer"": {
      ""type"": ""object"",
      ""properties"": {
        ""CustomerID"": { ""type"": ""integer"" },
        ""FirstName"": { ""type"": ""string"", ""minLength"": 1 },
        ""LastName"": { ""type"": ""string"", ""minLength"": 1 },
        ""Email"": { ""type"": ""string"", ""format"": ""email"" },
        ""Phone"": { ""type"": ""string"", ""minLength"": 5 },
        ""MobilePhone"": { ""type"": ""string"", ""minLength"": 5 },
        ""Address"": { ""type"": ""string"", ""minLength"": 5 },
        ""City"": { ""type"": ""string"", ""minLength"": 2 },
        ""State"": { ""type"": ""string"", ""minLength"": 2 },
        ""PostalCode"": { ""type"": ""string"", ""minLength"": 4 },
        ""Country"": { ""type"": ""string"", ""minLength"": 2 }
      },
      ""required"": [
        ""FirstName"", ""LastName"", ""Email"",
        ""Phone"", ""MobilePhone"", ""Address"",
        ""City"", ""State"", ""PostalCode"", ""Country""
      ]
    },
    ""Booking"": {
      ""type"": ""object"",
      ""properties"": {
        ""BookingID"": { ""type"": ""integer"" },
        ""BookingNo"": { ""type"": ""string"" },
        ""BookingType"": { ""type"": ""string"" },
        ""StartDate"": { ""type"": ""string"", ""format"": ""date-time"" },
        ""EndDate"": { ""type"": ""string"", ""format"": ""date-time"" },
        ""BookedBy"": { ""type"": ""string"", ""format"": ""email"" },
        ""Status"": { ""type"": ""string"" }
      }
    },
    ""Documents"": {
      ""type"": ""array"",
      ""items"": {
        ""type"": ""object"",
        ""properties"": {
          ""DocumentID"": { ""type"": ""integer"" },
          ""FileName"": { ""type"": ""string"", ""minLength"": 3 },
          ""FileType"": { ""type"": ""string"", ""minLength"": 3 },
          ""DocumentType"": { ""type"": ""string"", ""minLength"": 3 },
          ""FileUrl"": { ""type"": ""string"", ""format"": ""uri"" },
          ""UploadedBy"": { ""type"": ""string"", ""format"": ""email"" },
          ""UploadedAt"": { ""type"": ""string"", ""format"": ""date-time"" }
        },
        ""required"": [""FileName"", ""FileType"", ""DocumentType"", ""FileUrl""]
      }
    },
    ""DomainData"": {
      ""type"": ""object"",
      ""properties"": {
        ""Vehicle"": {
          ""type"": ""object"",
          ""properties"": {
            ""VehicleID"": { ""type"": ""integer"" },
            ""Make"": { ""type"": ""string"", ""minLength"": 2 },
            ""Model"": { ""type"": ""string"", ""minLength"": 1 },
            ""Year"": {
              ""type"": ""integer"",
              ""minimum"": 1900,
              ""maximum"": 2100
            },
            ""VIN"": { ""type"": ""string"", ""minLength"": 5 },
            ""LicensePlate"": { ""type"": ""string"", ""minLength"": 3 }
          },
          ""required"": [""VehicleID"", ""Make"", ""Model"", ""Year"", ""VIN"", ""LicensePlate""]
        },
        ""Quotation"": {
          ""type"": ""object"",
          ""properties"": {
            ""QuotationID"": { ""type"": ""integer"" },
            ""QuotationNo"": { ""type"": ""string"" },
            ""EstimatedTotal"": {
              ""type"": ""number"",
              ""minimum"": 0
            },
            ""Currency"": {
              ""type"": ""string"",
              ""minLength"": 3,
              ""maxLength"": 3
            },
            ""Discount"": {
              ""type"": ""number"",
              ""minimum"": 0
            },
            ""Tax"": {
              ""type"": ""number"",
              ""minimum"": 0
            },
            ""GrandTotal"": {
              ""type"": ""number"",
              ""minimum"": 0
            },
            ""Status"": { ""type"": ""string"" },
            ""CreatedAt"": { ""type"": ""string"", ""format"": ""date-time"" },
            ""QuotationItems"": {
              ""type"": ""array"",
              ""items"": {
                ""type"": ""object"",
                ""properties"": {
                  ""ItemID"": { ""type"": ""integer"" },
                  ""PartName"": { ""type"": ""string"", ""minLength"": 2 },
                  ""PartNumber"": { ""type"": ""string"", ""minLength"": 2 },
                  ""Description"": { ""type"": ""string"", ""minLength"": 5 },
                  ""Quantity"": {
                    ""type"": ""integer"",
                    ""minimum"": 1
                  },
                  ""UnitPrice"": {
                    ""type"": ""number"",
                    ""minimum"": 0
                  },
                  ""TotalPrice"": {
                    ""type"": ""number"",
                    ""minimum"": 0
                  }
                },
                ""required"": [
                  ""PartName"", ""PartNumber"",
                  ""Quantity"", ""UnitPrice"", ""TotalPrice""
                ]
              }
            }
          }
        }
      },
      ""required"": [""Vehicle""]
    }
  },
  ""required"": [
    ""DomainType"", ""ServiceType"", ""Priority"",
    ""Customer"", ""Booking"", ""Documents"", ""DomainData""
  ]
}";



    }

}
