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
    }

}
