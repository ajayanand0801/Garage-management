# Garage Management – Project Structure & Architecture

This document describes the solution structure, layer responsibilities, and main patterns used in the **Garage Management** .NET solution. Configuration files such as `appsettings.json` are excluded from this overview.

---

## 1. Solution Overview

| Project | Purpose | Target Framework |
|--------|---------|------------------|
| **GarageManagement.Domain** | Core entities and business domain | .NET 9.0 |
| **GarageManagement.Application** | Use cases, DTOs, interfaces, services, mappings | .NET 9.0 |
| **GarageManagement.Infrastructure** | EF Core, DbContext, repositories, validators | .NET 9.0 |
| **GarageManagement.API** | ASP.NET Core Web API, controllers | .NET 9.0 |
| **GarageManagement.Shared** | Shared utilities (standalone; not referenced by other projects) | .NET 9.0 |
| **ComponentManagement.PaginationUtility** | Reusable pagination service and request/result types | .NET 8.0 |

**Solution file:** `garageManagementSolution.sln`

---

## 2. Architecture: Dependency Flow (Onion)

Dependencies point **inward**:

```
                    ┌─────────────────────────────────────┐
                    │           GarageManagement.API       │
                    │  (Controllers, Program.cs, HTTP)    │
                    └─────────────────┬───────────────────┘
                                      │
              ┌───────────────────────┼───────────────────────┐
              │                       │                       │
              ▼                       ▼                       ▼
┌─────────────────────┐   ┌─────────────────────┐   ┌─────────────────────┐
│  Infrastructure     │   │   Application        │   │   Domain            │
│  (EF, Repos,       │──▶│  (Services, DTOs,   │──▶│  (Entities only)    │
│   DbContext)        │   │   Interfaces, Maps)  │   │                     │
└─────────────────────┘   └──────────┬──────────┘   └─────────────────────┘
                                     │
                                     │  + ComponentManagement.PaginationUtility
                                     ▼
                    ┌─────────────────────────────────────┐
                    │           Domain (again)             │
                    └─────────────────────────────────────┘
```

- **Domain**: No project references. Contains only entities and base types.
- **Application**: References **Domain** and **ComponentManagement.PaginationUtility**.
- **Infrastructure**: References **Domain** and **Application** (implements repositories and validators).
- **API**: References **Domain**, **Application**, and **Infrastructure**; composes DI in `Program.cs`.

---

## 3. Project Structure (Excluding appsettings)

### 3.1 GarageManagement.Domain

**Path:** `GarageManagement.Domain\`  
**Role:** Persistence-agnostic domain model.

```
GarageManagement.Domain/
├── GarageManagement.Domain.csproj
└── Entites/                    # Note: folder name is "Entites"
    ├── BaseEntity.cs           # Id, CreatedAt, CreatedBy, ModifiedAt, ModifiedBy, IsActive, IsDeleted
    ├── Customer.cs
    ├── Quotation/
    │   ├── Quotation.cs
    │   └── QuotationItem.cs
    ├── Request/
    │   ├── ServiceRequest.cs
    │   ├── ServiceRequestCustomerMetaData.cs
    │   ├── ServiceRequestDocument.cs
    │   ├── ServiceRequestMetadata.cs
    │   └── ServiceRequestVehicleMetaData.cs
    └── Vehicles/
        ├── Vehicle.cs
        ├── VehicleBrand.cs
        ├── VehicleLookup.cs
        ├── VehicleModel.cs
        ├── VehicleModelYear.cs
        ├── VehicleOwner.cs
        └── VehicleVariant.cs
```

- All entities inherit from **BaseEntity** (audit and soft-delete fields).
- Some entities use `[Table]` / `[Schema]` and data annotations; table/schema configuration is also done in **RepairDbContext** via Fluent API.
- Domain has no references to Application, Infrastructure, or API.

---

### 3.2 GarageManagement.Application

**Path:** `GarageManagement.Application\`  
**Role:** Use cases, contracts, DTOs, validation contracts, and DTO ↔ Entity mapping.

```
GarageManagement.Application/
├── GarageManagement.Application.csproj   # References: Domain, PaginationUtility; AutoMapper
├── DTOs/
│   ├── BookingDto.cs
│   ├── CrmCustomerDTO.cs
│   ├── CustomerDto.cs
│   ├── DocumentDto.cs
│   ├── DomainDataDto.cs
│   ├── QuotationDTO.cs
│   ├── QuotationItemDto.cs
│   ├── ServiceRequestDto.cs
│   ├── VehicleDomainDTO.cs
│   ├── VehicleDto.cs
│   ├── VehicleLookupDTO.cs
│   └── VinSearchResponse.cs
├── Interfaces/
│   ├── ICustomerRepository.cs
│   ├── IGenericRepository.cs
│   ├── IQuotationRepository.cs
│   ├── IServiceRequestRepository.cs
│   ├── IUnitOfWork.cs
│   ├── IVehicleLookupRepository.cs
│   ├── IVehicleRepository .cs
│   ├── Mapper/
│   │   └── IMapperUtility.cs
│   ├── ServiceInterface/
│   │   ├── ICustomerService.cs
│   │   ├── IQuotationService.cs
│   │   ├── IServiceRequest.cs
│   │   ├── IVehicleLookupService.cs
│   │   └── IVehicleService.cs
│   └── Validator/
│       ├── IJsonValidator.cs
│       └── JsonRules.cs
├── Mappings/
│   ├── MappingProfile.cs      # AutoMapper Profile: DTO ↔ Domain entities
│   └── MapperUtility .cs
└── Services/
    ├── CustomerService.cs
    ├── Quotation/
    │   └── QuotationService.cs
    ├── Request/
    │   └── ServiceRequestService.cs
    ├── VehicleLookupService.cs
    └── VehicleService.cs
```

- **Repositories:** `IGenericRepository<T>`, `IUnitOfWork`, and feature-specific interfaces (`IVehicleRepository`, `ICustomerRepository`, etc.) are defined here; implemented in Infrastructure.
- **Services:** Application services implement `ICustomerService`, `IVehicleService`, `IVehicleLookupService`, `IServiceRequest`, `IQuotationService` and orchestrate repositories + mapping.
- **Mapping:** Central AutoMapper profile in `MappingProfile.cs` for Entity ↔ DTO (Vehicle, Customer, Quotation, ServiceRequest, VehicleLookup, etc.).
- **Pagination:** Application uses `IPaginationService<T>` and `PaginationRequest` / `PaginationResult<T>` from **ComponentManagement.PaginationUtility**.

---

### 3.3 GarageManagement.Infrastructure

**Path:** `GarageManagement.Infrastructure\`  
**Role:** Persistence (EF Core), repository implementations, and JSON validation.

```
GarageManagement.Infrastructure/
├── GarageManagement.Infrastructure.csproj   # References: Domain, Application; EF Core, Dapper, Newtonsoft.Json.Schema
├── DbContext/
│   └── RepairDbContext .cs     # EF Core DbContext, Fluent API, table/schema mapping
├── Repositories/
│   ├── CustomerRepository.cs
│   ├── GenericRepository.cs
│   ├── QuotationRepository.cs
│   ├── ServiceRequestRepository.cs
│   ├── UnitOfWork.cs
│   ├── VehicleLookupRepository.cs
│   └── VehicleRepository.cs
├── Validator/
│   └── JsonValidator.cs        # Implements IJsonValidator (JSON schema validation)
└── ServiceRegistration.cs     # AddInfrastructure(connectionString): DbContext + generic repo
```

- **RepairDbContext:** Single DbContext; maps entities to tables in schemas **dbo**, **vhc**, **rpa** (e.g. Vehicle in `vhc`, Quotation in `rpa`, ServiceRequest/Customer in `dbo`). All entity configuration is in `OnModelCreating`.
- **Repositories:** Implement Application interfaces; use `RepairDbContext` and, where used, `IUnitOfWork`.
- **JsonValidator:** Implements `IJsonValidator` from Application; used for request payload validation against JSON rules.
- **ServiceRegistration:** Registers DbContext and generic repository; additional registrations (specific repos, UoW, etc.) are done in **API** `Program.cs`.

---

### 3.4 GarageManagement.API

**Path:** `GarageManagement.API\`  
**Role:** HTTP API, DI composition, and pipeline configuration.

```
GarageManagement.API/
├── GarageManagement.API.csproj   # References: Domain, Application, Infrastructure
├── Program.cs                    # DI: repos, UoW, services, AutoMapper, validators, DbContext
├── GarageManagement.API.http     # HTTP client / manual testing
├── Properties/
│   └── launchSettings.json
└── Controllers/
    ├── CustomerController.cs
    ├── QuotationController.cs
    ├── ServiceRequestController .cs
    ├── VehicleController .cs
    └── VehicleLookupController.cs
```

- **Program.cs:** Registers all repositories, `IUnitOfWork`, application services, `IPaginationService<>`, `IJsonValidator`, AutoMapper (`MappingProfile`), `IMapperUtility`, and calls `AddInfrastructure(connectionString)`.
- **Controllers:** Thin; depend on application service interfaces (e.g. `ICustomerService`, `IVehicleService`) and DTOs. No direct repository or DbContext usage.
- **Pipeline:** CORS (e.g. React app), Swagger/OpenAPI, HTTPS redirection, authorization, MapControllers. No `appsettings` content is documented here.

---

### 3.5 GarageManagement.Shared

**Path:** `GarageManagement.Shared\`  
**Role:** Shared utilities (currently **not referenced** by Domain, Application, Infrastructure, or API).

```
GarageManagement.Shared/
├── GarageManagement.Shared.csproj
└── Utilities/
    ├── JsonRules.cs
    ├── JsonValidator.cs
    └── ValidatorUtility.cs
```

- Contains its own JSON-related utilities; Infrastructure implements validation via its own `JsonValidator` and Application’s `IJsonValidator`. Consider consolidating or explicitly referencing Shared if these utilities should be reused.

---

### 3.6 ComponentManagement.PaginationUtility

**Path:** `ComponentManagement.PaginationUtility\`  
**Role:** Reusable pagination for any `IQueryable<T>`.

```
ComponentManagement.PaginationUtility/
├── ComponentManagement.PaginationUtility.csproj   # .NET 8.0
├── IPaginationService.cs
├── PaginationService.cs
├── PaginationRequest.cs / PaginationResult<T>     # (inferred from usage)
├── PaginationUtility.cs
├── QueryableExtensions.cs
└── ExpressionBuilder.cs
```

- **Application** references this project and uses `IPaginationService<T>` and pagination request/result types in services and APIs (e.g. customer list with pagination).

---

## 4. Key Patterns & Conventions

| Pattern | Location | Description |
|--------|-----------|-------------|
| **Repository** | Application (interfaces), Infrastructure (implementations) | `IGenericRepository<T>` plus feature repos (Vehicle, Customer, Quotation, ServiceRequest, VehicleLookup). Async CRUD and optional query delegates. |
| **Unit of Work** | Application (`IUnitOfWork`), Infrastructure (`UnitOfWork`) | Groups repositories and provides `BeginTransactionAsync`, `CommitAsync`, `RollbackAsync`, `SaveChangesAsync`. |
| **Application services** | Application | One service per aggregate/feature; use repos + AutoMapper; expose DTOs. |
| **DTOs** | Application | All cross-layer data transfer via DTOs; no domain entities exposed from API. |
| **AutoMapper** | Application (`MappingProfile`, `MapperUtility`) | Used only for DTO ↔ Domain mapping; profiles live in Application. |
| **Dependency injection** | API `Program.cs` + Infrastructure `ServiceRegistration` | Scoped registrations for DbContext, repos, UoW, services, validators, pagination. |

---

## 5. Database Schemas (from RepairDbContext)

- **dbo:** ServiceRequest, ServiceRequestDocument, ServiceRequestMetadata, SRVehicleMetaData, SRCustomerMetaData, Customer.
- **vhc:** vehicle, vehicleOwner, VehicleModel, vehicleModelYear, vehicleVariant, VehicleBrand, VehicleLookup.
- **rpa:** Quotation, QuotationItem.

Entities use a mix of `[Table]`/`[Schema]` on domain classes and Fluent API in `RepairDbContext` for relationships, indexes, and defaults.

---

## 6. Request Flow (Typical)

1. **API:** Controller receives HTTP request, uses DTOs and application service interface (e.g. `ICustomerService`).
2. **Application:** Service uses repositories (and optionally `IUnitOfWork`), maps between DTOs and domain entities via AutoMapper, may use `IPaginationService<T>` for list endpoints.
3. **Infrastructure:** Repositories use `RepairDbContext` to read/write entities; `IJsonValidator` used where JSON schema validation is required.
4. **Response:** Service returns DTOs; controller returns them with appropriate HTTP status codes.

---

## 7. Summary

- **Onion architecture** is respected: Domain is central; Application defines use cases and contracts; Infrastructure implements persistence and validation; API is the composition and HTTP host.
- **Separation of concerns:** Controllers are thin; business logic lives in Application services; persistence and EF details are in Infrastructure.
- **Reusable building blocks:** Pagination is in a separate component; validation is behind an interface in Application and implemented in Infrastructure.
- **GarageManagement.Shared** is part of the solution but not referenced by other projects; **ComponentManagement.PaginationUtility** targets .NET 8.0 while the rest of the solution uses .NET 9.0. These are minor points to align if you want a single shared utility strategy and a uniform TFM.
