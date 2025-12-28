using ComponentManagement.PaginationUtility;
using GarageManagement.Application.DTOs;
using GarageManagement.Application.Interfaces;
using GarageManagement.Application.Interfaces.Mapper;
using GarageManagement.Application.Interfaces.ServiceInterface;
using GarageManagement.Application.Interfaces.Validator;
using GarageManagement.Domain.Entites.Vehicles;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GarageManagement.Application.Services
{
    public class VehicleService : IVehicleService
    {
        private readonly IGenericRepository<Vehicle> _vehicleGenricRepo;
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IJsonValidator _jsonValidator;
        private readonly IMapperUtility _mapperUtility;
        private readonly IPaginationService<Vehicle> _paginationService;
        private readonly IGenericRepository<VehicleLookup> _vehicleLookupRepository;
        private readonly IGenericRepository<VehicleBrand> _vehicleBrandRepository;
        private readonly IGenericRepository<VehicleModel> _vehicleModelRepository;
        private readonly IGenericRepository<VehicleModelYear> _vehicleModelYearRepository;




        public VehicleService(IGenericRepository<Vehicle> vehicleGenricRepo, IVehicleRepository vehicleRepository, IJsonValidator jsonValidator, IMapperUtility mapperUtility, IPaginationService<Vehicle> paginationService, IGenericRepository<VehicleLookup> vehicleLookupRepository, IGenericRepository<VehicleBrand> vehicleBrandRepository, IGenericRepository<VehicleModel> vehicleModelRepository, IGenericRepository<VehicleModelYear> vehicleModelYearRepository)
        {
            _vehicleGenricRepo = vehicleGenricRepo;
            _vehicleRepository = vehicleRepository;
            _jsonValidator = jsonValidator;
            _mapperUtility = mapperUtility;
            _paginationService = paginationService;
            _vehicleLookupRepository = vehicleLookupRepository;
            _vehicleBrandRepository = vehicleBrandRepository;
            _vehicleModelRepository = vehicleModelRepository;
            _vehicleModelYearRepository = vehicleModelYearRepository;
        }

        public async Task<IEnumerable<Vehicle>> GetAllVehiclesAsync() => await _vehicleRepository.GetAllVehiclesWithOwnersAsync();

        public async Task<VehicleDto?> GetVehicleByIdAsync(long id) => await _vehicleRepository.GetVehicleWithDetailsAsync(id);

        public async Task<bool> CreateVehicleAsync(Vehicle vehicle) => await _vehicleGenricRepo.AddAsync(vehicle);

        public async Task<bool> UpdateVehicleAsync(long id, Vehicle updated)
        {
            var existing = await _vehicleGenricRepo.GetByIdAsync(id);
            if (existing == null) return false;

            updated.Id = existing.Id;
            return await _vehicleGenricRepo.UpdateAsync(updated);
        }

        public async Task<bool> DeleteVehicleAsync(long id) => await _vehicleGenricRepo.DeleteAsync(id);
        public bool ValidateVehicle(VehicleDto vehicle, out List<string> errors)
        {
            // Use the injected validator to validate with the vehicle rules
            return _jsonValidator.Validate(vehicle, JsonRules.VehicleRule, out errors);
        }

        public async Task<List<string>> IsVehicleExists(VehicleDto vehicleRequest)
        {
            var errors = new List<string>();

            if (vehicleRequest == null)
            {
                errors.Add("Vehicle request cannot be null.");
                return errors;
            }

            // Assuming _vehicleRepository.VehicleExistsAsync returns true if any of them exist
            bool isVehicleExist = await _vehicleRepository.VehicleExistsAsync(
                vehicleRequest.EngineNumber,
                vehicleRequest.RegistrationNumber,
                vehicleRequest.ChassisNumber
            );

            if (isVehicleExist)
            {
                errors.Add("Vehicle with the same Engine Number or Registration Number or Chassis Number already exists.");
            }

            return errors;
        }

        public async Task<bool> CreateVehicle(VehicleDto vehicleRequest)
        {
           
            var isValid = ValidateVehicle(vehicleRequest, out List<string> errors);

            List<string> existErrors = await IsVehicleExists(vehicleRequest);
           
            if(existErrors.Count>0)
            return false; 
            

             var vehicle = _mapperUtility.Map<VehicleDto, Vehicle>(vehicleRequest);
            vehicle.VehicleID = await _vehicleRepository.GetMaxVehicleIdAsync();
            //var vehicle = new Vehicle
            //{
            //    VehicleID = 10006,
            //    VIN = "JT123456765018841",
            //    Color = "Silver",
            //    RegistrationNumber = "rJ383732",
            //    EngineNumber = "ENG82888",
            //    ChassisNumber = "CHS1289",
            //    IsActive = true,
            //    IsDeleted = false,
            //    CreatedAt = DateTime.UtcNow,
            //    CreatedBy = "System",

            //    BrandID = 1,
            //    ModelID = 100,
            //    ModelYearID = 1000,


            //    //Brand = new VehicleBrand
            //    //{
            //    //    BrandID = 1,
            //    //    BrandName = "Toyota",

            //    //},
            //    //Model = new VehicleModel
            //    //{
            //    //    ModelID = 100,
            //    //    ModelName = "Grand Highlander"
            //    //},
            //    //ModelYear = new VehicleModelYear
            //    //{
            //    //    ModelYearID = 1000,
            //    //    ModelYear = 2025
            //    //},

            //   // Owners = new List<VehicleOwner>() // Empty owners list
            //};

            //if (!isValid)
            //    return false;

            // Use AutoMapper to map DTO to Entity
            // var vehicle = _mapperUtility.Map<VehicleDto, Vehicle>(vehicleRequest);

            // // Set any additional system-level values
            vehicle.CreatedAt = DateTime.UtcNow;
            
            vehicle.CreatedBy = "System"; // or from current user context
            //var maxVehicleId= await _vehicleRepository.GetMaxVehicleIdAsync();
            // vehicle.Owners = null;
            //// var maxVehicleId= await _vehicleRepository.GetMaxVehicleIdAsync();
            // vehicle.VehicleID = maxVehicleId+1;
            //if (vehicle.Owners != null)
            //{
            //    foreach (var owner in vehicle.Owners)
            //    {
            //        owner.CreatedAt = DateTime.UtcNow;
            //        owner.CreatedBy = "system";
            //    }
            //}

            try
            {
                // Save to database
                Console.WriteLine("VehicleID: " + vehicle.VehicleID); // Should print 10005
                await _vehicleGenricRepo.AddAsync(vehicle);
            }
            catch (Exception ex)
            {
                throw ex; 
            }

           

            return true;
        }

        public async Task<VinSearchResponse> GetVehicleBYVin(string vin)
        {
            var vinresponse = await _vehicleRepository.GetVinSearchResponseAsync(vin);
            if (vinresponse != null)
                 vinresponse.DecodeStatus = "success";
            
            return vinresponse;
        }

        public async Task<bool> UpdateVehicleOwnersAsync(long vehicleId, List<VehicleOwnerDto> owners)
        {
            if (owners == null || owners.Count == 0)
                return false;

            return await _vehicleRepository.UpdateVehicleOwnersAsync(vehicleId, owners);
        }

        public async Task<PaginationResult<VehicleDto>> GetAllVehiclesAsync(PaginationRequest request, CancellationToken cancellationToken)
        {
            // Map lookup field names to their corresponding ID fields and lookup types
            var lookupFieldMap = new Dictionary<string, (string IdField, string LookupType)>(StringComparer.OrdinalIgnoreCase)
            {
                { "bodyType", ("BodyTypeID", "BodyType") },
                { "fuelType", ("FuelTypeID", "FuelType") },
                { "transmissionType", ("TransmissionID", "TransmissionType") },
                { "driveTrain", ("DrivetrainID", "Drivetrain") }
            };

            // Map navigation property field names to their corresponding ID fields and search properties
            var navigationFieldMap = new Dictionary<string, (string IdField, string SearchProperty)>(StringComparer.OrdinalIgnoreCase)
            {
                { "brand", ("BrandID", "BrandName") },
                { "brandName", ("BrandID", "BrandName") },
                { "model", ("ModelID", "ModelName") },
                { "modelName", ("ModelID", "ModelName") },
                { "modelYear", ("ModelYearID", "ModelYear") },
                { "year", ("ModelYearID", "ModelYear") }
            };

            // Create a copy of the request to modify filters and sorts if needed
            var modifiedRequest = new PaginationRequest
            {
                Skip = request.Skip,
                Take = request.Take,
                Filters = new List<FilterField>(),
                Sorts = new List<SortField>()
            };

            // Track lookup sort fields separately (we'll sort after mapping computed values)
            var lookupSortFields = new List<SortField>();

            // Process sort fields - convert computed field names to database sortable fields
            if (request.Sorts != null && request.Sorts.Any())
            {
                foreach (var sort in request.Sorts)
                {
                    // Check if it's a navigation property field
                    if (navigationFieldMap.TryGetValue(sort.Field, out var navMapping))
                    {
                        // Use dot notation for navigation properties (e.g., "Brand.BrandName", "Model.ModelName")
                        string sortField = navMapping.IdField switch
                        {
                            "BrandID" => "Brand.BrandName",
                            "ModelID" => "Model.ModelName",
                            "ModelYearID" => "ModelYear.ModelYear",
                            _ => sort.Field
                        };
                        modifiedRequest.Sorts.Add(new SortField
                        {
                            Field = sortField,
                            Direction = sort.Direction
                        });
                    }
                    // Check if it's a lookup field - these need special handling
                    else if (lookupFieldMap.TryGetValue(sort.Field, out var lookupMapping))
                    {
                        // Store lookup sort info to apply after mapping
                        lookupSortFields.Add(sort);
                        // Don't add to modifiedRequest.Sorts - we'll sort after mapping
                    }
                    else
                    {
                        // Regular field - keep as is
                        modifiedRequest.Sorts.Add(sort);
                    }
                }
            }

            // Process filters - convert lookup field filters and navigation property filters to ID filters
            if (request.Filters != null && request.Filters.Any())
            {
                foreach (var filter in request.Filters)
                {
                    // Handle lookup field filters (from VehicleLookup table)
                    if (lookupFieldMap.TryGetValue(filter.Field, out var mapping))
                    {
                        // This is a lookup field filter - need to find matching IDs from VehicleLookup
                        var filterValue = filter.Value?.ToString();
                        if (!string.IsNullOrWhiteSpace(filterValue))
                        {
                            var filterLookups = await _vehicleLookupRepository.GetAllAsync(q => q
                                .Where(l => !l.IsDeleted && l.IsActive && 
                                    l.LookupType == mapping.LookupType));

                            List<int> matchingIds;
                            var operation = filter.Operation?.ToLowerInvariant() ?? "eq";

                            switch (operation)
                            {
                                case "contains":
                                case "contain":
                                    matchingIds = filterLookups
                                        .Where(l => l.LookupValue.Contains(filterValue, StringComparison.OrdinalIgnoreCase))
                                        .Select(l => l.LookupID)
                                        .ToList();
                                    break;
                                case "eq":
                                case "equal":
                                case "equals":
                                    matchingIds = filterLookups
                                        .Where(l => l.LookupValue.Equals(filterValue, StringComparison.OrdinalIgnoreCase))
                                        .Select(l => l.LookupID)
                                        .ToList();
                                    break;
                                case "startswith":
                                case "starts":
                                    matchingIds = filterLookups
                                        .Where(l => l.LookupValue.StartsWith(filterValue, StringComparison.OrdinalIgnoreCase))
                                        .Select(l => l.LookupID)
                                        .ToList();
                                    break;
                                case "endswith":
                                case "ends":
                                    matchingIds = filterLookups
                                        .Where(l => l.LookupValue.EndsWith(filterValue, StringComparison.OrdinalIgnoreCase))
                                        .Select(l => l.LookupID)
                                        .ToList();
                                    break;
                                default:
                                    // For other operations, try contains as fallback
                                    matchingIds = filterLookups
                                        .Where(l => l.LookupValue.Contains(filterValue, StringComparison.OrdinalIgnoreCase))
                                        .Select(l => l.LookupID)
                                        .ToList();
                                    break;
                            }

                            if (matchingIds.Any())
                            {
                                // Replace filter with ID field filter
                                if (matchingIds.Count == 1)
                                {
                                    modifiedRequest.Filters.Add(new FilterField
                                    {
                                        Field = mapping.IdField,
                                        Operation = "eq",
                                        Value = matchingIds[0],
                                        LogicalOperator = filter.LogicalOperator
                                    });
                                }
                                else
                                {
                                    // For multiple IDs, we need to create OR conditions
                                    // The first one uses the original logical operator to connect with previous filters
                                    // All subsequent ones use "or" to group them together
                                    modifiedRequest.Filters.Add(new FilterField
                                    {
                                        Field = mapping.IdField,
                                        Operation = "eq",
                                        Value = matchingIds[0],
                                        LogicalOperator = filter.LogicalOperator
                                    });
                                    
                                    // Add remaining IDs with "or" operator
                                    for (int i = 1; i < matchingIds.Count; i++)
                                    {
                                        modifiedRequest.Filters.Add(new FilterField
                                        {
                                            Field = mapping.IdField,
                                            Operation = "eq",
                                            Value = matchingIds[i],
                                            LogicalOperator = "or"
                                        });
                                    }
                                }
                            }
                            else
                            {
                                // No matching lookups found - add a filter that will return no results
                                // Use a non-existent ID to ensure no matches
                                modifiedRequest.Filters.Add(new FilterField
                                {
                                    Field = mapping.IdField,
                                    Operation = "eq",
                                    Value = -1, // Non-existent ID
                                    LogicalOperator = filter.LogicalOperator
                                });
                            }
                        }
                    }
                    // Handle navigation property filters (Brand, Model, ModelYear)
                    else if (navigationFieldMap.TryGetValue(filter.Field, out var navMapping))
                    {
                        var filterValue = filter.Value?.ToString();
                        if (!string.IsNullOrWhiteSpace(filterValue))
                        {
                            List<long> matchingIds = new List<long>();
                            var operation = filter.Operation?.ToLowerInvariant() ?? "eq";

                            if (navMapping.IdField == "BrandID")
                            {
                                var brands = await _vehicleBrandRepository.GetAllAsync(q => q
                                    .Where(b => !b.IsDeleted && b.IsActive));

                                switch (operation)
                                {
                                    case "contains":
                                    case "contain":
                                        matchingIds = brands
                                            .Where(b => b.BrandName.Contains(filterValue, StringComparison.OrdinalIgnoreCase))
                                            .Select(b => b.BrandID)
                                            .ToList();
                                        break;
                                    case "eq":
                                    case "equal":
                                    case "equals":
                                        matchingIds = brands
                                            .Where(b => b.BrandName.Equals(filterValue, StringComparison.OrdinalIgnoreCase))
                                            .Select(b => b.BrandID)
                                            .ToList();
                                        break;
                                    case "startswith":
                                    case "starts":
                                        matchingIds = brands
                                            .Where(b => b.BrandName.StartsWith(filterValue, StringComparison.OrdinalIgnoreCase))
                                            .Select(b => b.BrandID)
                                            .ToList();
                                        break;
                                    case "endswith":
                                    case "ends":
                                        matchingIds = brands
                                            .Where(b => b.BrandName.EndsWith(filterValue, StringComparison.OrdinalIgnoreCase))
                                            .Select(b => b.BrandID)
                                            .ToList();
                                        break;
                                    default:
                                        matchingIds = brands
                                            .Where(b => b.BrandName.Contains(filterValue, StringComparison.OrdinalIgnoreCase))
                                            .Select(b => b.BrandID)
                                            .ToList();
                                        break;
                                }
                            }
                            else if (navMapping.IdField == "ModelID")
                            {
                                var models = await _vehicleModelRepository.GetAllAsync(q => q
                                    .Where(m => !m.IsDeleted && m.IsActive));

                                switch (operation)
                                {
                                    case "contains":
                                    case "contain":
                                        matchingIds = models
                                            .Where(m => m.ModelName.Contains(filterValue, StringComparison.OrdinalIgnoreCase))
                                            .Select(m => m.ModelID)
                                            .ToList();
                                        break;
                                    case "eq":
                                    case "equal":
                                    case "equals":
                                        matchingIds = models
                                            .Where(m => m.ModelName.Equals(filterValue, StringComparison.OrdinalIgnoreCase))
                                            .Select(m => m.ModelID)
                                            .ToList();
                                        break;
                                    case "startswith":
                                    case "starts":
                                        matchingIds = models
                                            .Where(m => m.ModelName.StartsWith(filterValue, StringComparison.OrdinalIgnoreCase))
                                            .Select(m => m.ModelID)
                                            .ToList();
                                        break;
                                    case "endswith":
                                    case "ends":
                                        matchingIds = models
                                            .Where(m => m.ModelName.EndsWith(filterValue, StringComparison.OrdinalIgnoreCase))
                                            .Select(m => m.ModelID)
                                            .ToList();
                                        break;
                                    default:
                                        matchingIds = models
                                            .Where(m => m.ModelName.Contains(filterValue, StringComparison.OrdinalIgnoreCase))
                                            .Select(m => m.ModelID)
                                            .ToList();
                                        break;
                                }
                            }
                            else if (navMapping.IdField == "ModelYearID")
                            {
                                var modelYears = await _vehicleModelYearRepository.GetAllAsync(q => q
                                    .Where(my => !my.IsDeleted && my.IsActive));

                                // For ModelYear, the search property is an int, so we need to handle it differently
                                if (int.TryParse(filterValue, out var yearValue))
                                {
                                    switch (operation)
                                    {
                                        case "eq":
                                        case "equal":
                                        case "equals":
                                            matchingIds = modelYears
                                                .Where(my => my.ModelYear == yearValue)
                                                .Select(my => my.ModelYearID)
                                                .ToList();
                                            break;
                                        case "gt":
                                        case "greaterthan":
                                            matchingIds = modelYears
                                                .Where(my => my.ModelYear > yearValue)
                                                .Select(my => my.ModelYearID)
                                                .ToList();
                                            break;
                                        case "lt":
                                        case "lessthan":
                                            matchingIds = modelYears
                                                .Where(my => my.ModelYear < yearValue)
                                                .Select(my => my.ModelYearID)
                                                .ToList();
                                            break;
                                        case "gte":
                                        case "greaterthanorequal":
                                            matchingIds = modelYears
                                                .Where(my => my.ModelYear >= yearValue)
                                                .Select(my => my.ModelYearID)
                                                .ToList();
                                            break;
                                        case "lte":
                                        case "lessthanorequal":
                                            matchingIds = modelYears
                                                .Where(my => my.ModelYear <= yearValue)
                                                .Select(my => my.ModelYearID)
                                                .ToList();
                                            break;
                                        default:
                                            matchingIds = modelYears
                                                .Where(my => my.ModelYear == yearValue)
                                                .Select(my => my.ModelYearID)
                                                .ToList();
                                            break;
                                    }
                                }
                                else
                                {
                                    // If not a valid year number, try contains on string representation
                                    matchingIds = modelYears
                                        .Where(my => my.ModelYear.ToString().Contains(filterValue))
                                        .Select(my => my.ModelYearID)
                                        .ToList();
                                }
                            }

                            if (matchingIds.Any())
                            {
                                // Replace filter with ID field filter
                                if (matchingIds.Count == 1)
                                {
                                    modifiedRequest.Filters.Add(new FilterField
                                    {
                                        Field = navMapping.IdField,
                                        Operation = "eq",
                                        Value = matchingIds[0],
                                        LogicalOperator = filter.LogicalOperator
                                    });
                                }
                                else
                                {
                                    // For multiple IDs, create OR conditions
                                    modifiedRequest.Filters.Add(new FilterField
                                    {
                                        Field = navMapping.IdField,
                                        Operation = "eq",
                                        Value = matchingIds[0],
                                        LogicalOperator = filter.LogicalOperator
                                    });

                                    // Add remaining IDs with "or" operator
                                    for (int i = 1; i < matchingIds.Count; i++)
                                    {
                                        modifiedRequest.Filters.Add(new FilterField
                                        {
                                            Field = navMapping.IdField,
                                            Operation = "eq",
                                            Value = matchingIds[i],
                                            LogicalOperator = "or"
                                        });
                                    }
                                }
                            }
                            else
                            {
                                // No matching records found - add a filter that will return no results
                                modifiedRequest.Filters.Add(new FilterField
                                {
                                    Field = navMapping.IdField,
                                    Operation = "eq",
                                    Value = -1, // Non-existent ID
                                    LogicalOperator = filter.LogicalOperator
                                });
                            }
                        }
                    }
                    else
                    {
                        // Not a lookup or navigation field - keep the original filter
                        modifiedRequest.Filters.Add(filter);
                    }
                }
            }

            IQueryable<Vehicle> query = _vehicleRepository.GetAll();

            // Apply custom sorting for navigation properties with dot notation
            if (modifiedRequest.Sorts.Any())
            {
                query = ApplyNavigationPropertySorting(query, modifiedRequest.Sorts);
            }

            // Apply filters and remaining sorts
            var pagedResult = await _paginationService.PaginateAsync(
                query,
                modifiedRequest,
                cancellationToken);

            // Map PaginationResult<Vehicle> to PaginationResult<VehicleDto>
            var mappedItems = pagedResult.Items.Select(v => _mapperUtility.Map<Vehicle, VehicleDto>(v)).ToList();

            // Get all unique lookup IDs from the mapped items
            var drivetrainIds = mappedItems.Where(v => v.DrivetrainID.HasValue).Select(v => (int)v.DrivetrainID.Value).Distinct().ToList();
            var transmissionIds = mappedItems.Where(v => v.TransmissionID.HasValue).Select(v => (int)v.TransmissionID.Value).Distinct().ToList();
            var bodyTypeIds = mappedItems.Where(v => v.BodyTypeID.HasValue).Select(v => (int)v.BodyTypeID.Value).Distinct().ToList();
            var fuelTypeIds = mappedItems.Where(v => v.FuelTypeID.HasValue).Select(v => (int)v.FuelTypeID.Value).Distinct().ToList();

            // Fetch lookup values efficiently with filtered queries
            var allLookups = await _vehicleLookupRepository.GetAllAsync(q => q
                .Where(l => !l.IsDeleted && l.IsActive && (
                    (l.LookupType == "Drivetrain" && drivetrainIds.Contains(l.LookupID)) ||
                    (l.LookupType == "TransmissionType" && transmissionIds.Contains(l.LookupID)) ||
                    (l.LookupType == "BodyType" && bodyTypeIds.Contains(l.LookupID)) ||
                    (l.LookupType == "FuelType" && fuelTypeIds.Contains(l.LookupID))
                )));

            // Create dictionaries for each lookup type
            var drivetrainLookups = allLookups
                .Where(l => l.LookupType == "Drivetrain")
                .ToDictionary(l => l.LookupID, l => l.LookupValue);

            var transmissionLookups = allLookups
                .Where(l => l.LookupType == "TransmissionType")
                .ToDictionary(l => l.LookupID, l => l.LookupValue);

            var bodyTypeLookups = allLookups
                .Where(l => l.LookupType == "BodyType")
                .ToDictionary(l => l.LookupID, l => l.LookupValue);

            var fuelTypeLookups = allLookups
                .Where(l => l.LookupType == "FuelType")
                .ToDictionary(l => l.LookupID, l => l.LookupValue);

            // Map lookup values to DTOs
            foreach (var item in mappedItems)
            {
                if (item.DrivetrainID.HasValue && drivetrainLookups.TryGetValue((int)item.DrivetrainID.Value, out var drivetrainValue))
                {
                    item.DriveTrain = drivetrainValue;
                }

                if (item.TransmissionID.HasValue && transmissionLookups.TryGetValue((int)item.TransmissionID.Value, out var transmissionValue))
                {
                    item.TransmissionType = transmissionValue;
                }

                if (item.BodyTypeID.HasValue && bodyTypeLookups.TryGetValue((int)item.BodyTypeID.Value, out var bodyTypeValue))
                {
                    item.BodyType = bodyTypeValue;
                }

                if (item.FuelTypeID.HasValue && fuelTypeLookups.TryGetValue((int)item.FuelTypeID.Value, out var fuelTypeValue))
                {
                    item.FuelType = fuelTypeValue;
                }
            }

            // Apply sorting by lookup fields if needed (after mapping computed values)
            if (lookupSortFields.Any())
            {
                IOrderedEnumerable<VehicleDto>? orderedItems = null;
                for (int i = 0; i < lookupSortFields.Count; i++)
                {
                    var sort = lookupSortFields[i];
                    var isDescending = sort.Direction?.ToLowerInvariant() == "desc";

                    if (i == 0)
                    {
                        orderedItems = sort.Field.ToLowerInvariant() switch
                        {
                            "bodytype" => isDescending ? mappedItems.OrderByDescending(x => x.BodyType ?? "") : mappedItems.OrderBy(x => x.BodyType ?? ""),
                            "fueltype" => isDescending ? mappedItems.OrderByDescending(x => x.FuelType ?? "") : mappedItems.OrderBy(x => x.FuelType ?? ""),
                            "transmissiontype" => isDescending ? mappedItems.OrderByDescending(x => x.TransmissionType ?? "") : mappedItems.OrderBy(x => x.TransmissionType ?? ""),
                            "drivetrain" => isDescending ? mappedItems.OrderByDescending(x => x.DriveTrain ?? "") : mappedItems.OrderBy(x => x.DriveTrain ?? ""),
                            _ => mappedItems.OrderBy(x => 0)
                        };
                    }
                    else
                    {
                        orderedItems = sort.Field.ToLowerInvariant() switch
                        {
                            "bodytype" => isDescending ? orderedItems!.ThenByDescending(x => x.BodyType ?? "") : orderedItems!.ThenBy(x => x.BodyType ?? ""),
                            "fueltype" => isDescending ? orderedItems!.ThenByDescending(x => x.FuelType ?? "") : orderedItems!.ThenBy(x => x.FuelType ?? ""),
                            "transmissiontype" => isDescending ? orderedItems!.ThenByDescending(x => x.TransmissionType ?? "") : orderedItems!.ThenBy(x => x.TransmissionType ?? ""),
                            "drivetrain" => isDescending ? orderedItems!.ThenByDescending(x => x.DriveTrain ?? "") : orderedItems!.ThenBy(x => x.DriveTrain ?? ""),
                            _ => orderedItems
                        };
                    }
                }
                if (orderedItems != null)
                {
                    mappedItems = orderedItems.ToList();
                }
            }

            var result = new PaginationResult<VehicleDto>
            {
                Items = mappedItems,
                TotalCount = pagedResult.TotalCount
            };

            return result;
        }

        private IQueryable<Vehicle> ApplyNavigationPropertySorting(IQueryable<Vehicle> query, List<SortField> sortFields)
        {
            IOrderedQueryable<Vehicle>? orderedQuery = null;

            for (int i = 0; i < sortFields.Count; i++)
            {
                var sort = sortFields[i];
                var isDescending = sort.Direction?.ToLowerInvariant() == "desc";

                if (sort.Field.Contains("."))
                {
                    // Handle nested property sorting (e.g., "Brand.BrandName", "Model.ModelName")
                    var parts = sort.Field.Split('.');
                    if (parts.Length == 2)
                    {
                        var navigationProperty = parts[0];
                        var propertyName = parts[1];

                        if (i == 0)
                        {
                            orderedQuery = navigationProperty switch
                            {
                                "Brand" when propertyName == "BrandName" => isDescending
                                    ? query.OrderByDescending(v => v.Brand != null ? v.Brand.BrandName : "")
                                    : query.OrderBy(v => v.Brand != null ? v.Brand.BrandName : ""),
                                "Model" when propertyName == "ModelName" => isDescending
                                    ? query.OrderByDescending(v => v.Model != null ? v.Model.ModelName : "")
                                    : query.OrderBy(v => v.Model != null ? v.Model.ModelName : ""),
                                "ModelYear" when propertyName == "ModelYear" => isDescending
                                    ? query.OrderByDescending(v => v.ModelYear != null ? v.ModelYear.ModelYear : 0)
                                    : query.OrderBy(v => v.ModelYear != null ? v.ModelYear.ModelYear : 0),
                                _ => (IOrderedQueryable<Vehicle>)query
                            };
                        }
                        else
                        {
                            orderedQuery = navigationProperty switch
                            {
                                "Brand" when propertyName == "BrandName" => isDescending
                                    ? orderedQuery!.ThenByDescending(v => v.Brand != null ? v.Brand.BrandName : "")
                                    : orderedQuery!.ThenBy(v => v.Brand != null ? v.Brand.BrandName : ""),
                                "Model" when propertyName == "ModelName" => isDescending
                                    ? orderedQuery!.ThenByDescending(v => v.Model != null ? v.Model.ModelName : "")
                                    : orderedQuery!.ThenBy(v => v.Model != null ? v.Model.ModelName : ""),
                                "ModelYear" when propertyName == "ModelYear" => isDescending
                                    ? orderedQuery!.ThenByDescending(v => v.ModelYear != null ? v.ModelYear.ModelYear : 0)
                                    : orderedQuery!.ThenBy(v => v.ModelYear != null ? v.ModelYear.ModelYear : 0),
                                _ => orderedQuery
                            };
                        }
                    }
                }
            }

            return orderedQuery ?? query;
        }
    }

}
