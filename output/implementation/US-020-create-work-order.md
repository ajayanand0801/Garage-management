# US-020: Create work order — Implementation

## User story

| Field | Value |
|-------|--------|
| **Id** | US-020 |
| **Name** | Create work order |
| **Description** | As a garage staff member, I want to create a work order so that vehicle service work can be tracked. |

### Acceptance criteria

- Given a valid customer and vehicle, When I create a work order, Then the work order is created with status 'Created'.
- Given service tasks are provided, When the work order is created, Then tasks are linked to the work order.

---

## Analysis summary

- **Role:** Garage staff member.
- **Goal:** Create a work order to track vehicle service work.
- **Benefit:** Service work is tracked and can be progressed (e.g. status, tasks) in later stories.

**Acceptance criteria (Given/When/Then):**

1. **AC1**
   - **Given:** Valid customer and vehicle exist.
   - **When:** User submits a create work order request.
   - **Then:** A new work order is created with status `'Created'`.
   - **Implications:** API must accept customer and vehicle (or references); service must validate they exist; new entity persisted with initial status.

2. **AC2**
   - **Given:** Service tasks are provided (e.g. from a quotation or as line items).
   - **When:** Work order is created.
   - **Then:** Tasks are linked to the work order.
   - **Implications:** Schema has no separate WorkOrderTask table; tasks are interpreted as the **QuotationItem** lines from the quotation linked via `QuotationId` on WorkOrder. Creating a work order from a quotation automatically “links” those items as the work order’s service tasks. If a dedicated WorkOrderTask table is required later, it would be a separate schema change.

**Scope:**

- **Domain:** New `WorkOrder` entity (rich model, no EF attributes).
- **Application:** DTOs (request/response), `IWorkOrderService`, `IWorkOrderRepository`, create use case, AutoMapper profile.
- **Infrastructure:** `WorkOrderRepository`, `RepairDbContext` and Fluent API for `WorkOrder`, no new tables (table exists in DB).
- **API:** `WorkOrderController` with POST create endpoint, validation and appropriate status codes.

---

## Entity–schema alignment

Schema export used: **`scripts/schema-export.sql`**.

### Relevant tables

| SQL table | Relevance | Action |
|-----------|-----------|--------|
| **[rpa].[WorkOrder]** | Core entity for this story | **Create** new entity |

### [rpa].[WorkOrder] → Create entity `WorkOrder`

- **Table:** `[rpa].[WorkOrder]`, schema **rpa**.
- **Domain entity:** **Create** new `WorkOrder` in `GarageManagement.Domain/Entites/` (e.g. under `WorkOrder/` or next to Quotation).
- No existing Domain entity; map from schema as below.

**Column → property mapping:**

| Column (schema) | Type | Domain property | Type |
|-----------------|------|------------------|------|
| ID | bigint | Id | long (from BaseEntity) |
| OrderGuid | uniqueidentifier | OrderGuid | Guid |
| TenantID | bigint | TenantID | long |
| OrgID | int | OrgID | int |
| VehicleId | bigint | VehicleId | long |
| QuotationId | bigint | QuotationId | long |
| Status | nvarchar(100) | Status | string |
| ScheduledStart | datetime2 | ScheduledStart | DateTime? |
| ScheduledEnd | datetime2 | ScheduledEnd | DateTime? |
| ActualStart | datetime2 | ActualStart | DateTime? |
| ActualEnd | datetime2 | ActualEnd | DateTime? |
| Notes | nvarchar | Notes | string? |
| IsActive | bit | IsActive | bool (BaseEntity) |
| CreatedAt | datetime2 | CreatedAt | DateTime (BaseEntity) |
| CreatedBy | nvarchar(200) | CreatedBy | string? (BaseEntity) |
| ModifiedAt | datetime2 | ModifiedAt | DateTime? (BaseEntity) |
| ModifiedBy | nvarchar(200) | ModifiedBy | string? (BaseEntity) |
| IsDeleted | bit | IsDeleted | bool (BaseEntity) |

- **Note:** Table does not have `CustomerID`; customer is implied via Quotation → ServiceRequest or Vehicle → Owner. Create API can accept `CustomerId` and `VehicleId` for validation only; WorkOrder stores `VehicleId` and `QuotationId`.
- **Tasks:** No `WorkOrderTask` table in schema. “Tasks” = quotation items from the linked `Quotation` (via `QuotationId`). No new table or entity for tasks in this story.

---

## Implementation plan

### Domain

- **Create** entity `WorkOrder` (e.g. `GarageManagement.Domain/Entites/WorkOrder/WorkOrder.cs`).
  - Inherit `BaseEntity`.
  - Properties: `OrderGuid`, `TenantID`, `OrgID`, `VehicleId`, `QuotationId`, `Status`, `ScheduledStart`, `ScheduledEnd`, `ActualStart`, `ActualEnd`, `Notes`.
  - Optional navigation: `Quotation` (for “tasks” as QuotationItems), `Vehicle` if needed for validation.
  - No EF/data annotations; table/schema in Infrastructure only.
  - Encapsulate initial status: e.g. set `Status = "Created"` when creating a new work order (in Application or Domain method).

### Application

- **DTOs:** `CreateWorkOrderRequestDto` (VehicleId, QuotationId, CustomerId for validation, optional ScheduledStart/ScheduledEnd, Notes), `WorkOrderDto` (response: Id, OrderGuid, VehicleId, QuotationId, Status, ScheduledStart/End, etc.).
- **Interfaces:** `IWorkOrderRepository` (e.g. `AddAsync`), `IWorkOrderService` (e.g. `CreateWorkOrderAsync(CreateWorkOrderRequestDto)` returning `WorkOrderDto`).
- **Service:** `WorkOrderService` — validate customer and vehicle exist (using existing `ICustomerRepository`/customer service and vehicle repo); optionally validate quotation exists and belongs to same context; create `WorkOrder` with status `"Created"`, set `OrderGuid`, audit fields; persist via `IWorkOrderRepository`; return DTO.
- **Mapping:** In `MappingProfile`, add `CreateWorkOrderRequestDto` → `WorkOrder` (where applicable) and `WorkOrder` → `WorkOrderDto`.

### Infrastructure

- **Repository:** Implement `IWorkOrderRepository` (e.g. `WorkOrderRepository`) with `AddAsync(WorkOrder)`; use `RepairDbContext`.
- **DbContext:** Add `DbSet<WorkOrder> WorkOrders`; in `OnModelCreating` map `WorkOrder` to table `WorkOrder`, schema `rpa`; configure `OrderGuid` default if desired; configure relationships to `Quotation` and `Vehicle` (FKs: `QuotationId`, `VehicleId`).
- **Migrations:** Table already exists; ensure Fluent API matches existing [rpa].[WorkOrder] (no new migration unless adding indexes/FKs that don’t exist yet).

### API

- **Controller:** `WorkOrderController` (e.g. `api/[controller]`).
  - **POST** create: accept `CreateWorkOrderRequestDto`; call `IWorkOrderService.CreateWorkOrderAsync`; return 201 with route to get by id and response body `WorkOrderDto`; 400 for validation/business rule failures (e.g. customer/vehicle not found).
- **Validation:** Validate required fields (e.g. VehicleId, QuotationId); business validation (customer/vehicle exist) in application service.

---

## Tasks (ordered checklist)

- [ ] **Domain:** Create entity `WorkOrder` from table [rpa].[WorkOrder]: add class under `GarageManagement.Domain/Entites/` (e.g. `WorkOrder/WorkOrder.cs`), inherit `BaseEntity`, add all properties per “Entity–schema alignment”, no EF attributes.
- [ ] **Application – DTOs:** Add `CreateWorkOrderRequestDto` (VehicleId, QuotationId, CustomerId optional for validation, ScheduledStart, ScheduledEnd, Notes). Add `WorkOrderDto` (Id, OrderGuid, VehicleId, QuotationId, Status, ScheduledStart/End, ActualStart/End, Notes, CreatedAt, etc.).
- [ ] **Application – Interface:** Add `IWorkOrderRepository` with `Task<WorkOrder?> GetByIdAsync(long id)` and `Task<bool> AddAsync(WorkOrder entity)`. Add `IWorkOrderService` with `Task<WorkOrderDto?> CreateWorkOrderAsync(CreateWorkOrderRequestDto request, CancellationToken cancellationToken = default)`.
- [ ] **Application – Service:** Implement `WorkOrderService`: validate customer exists (by id), validate vehicle exists, optionally validate quotation exists; create `WorkOrder` with status `"Created"`, set `OrderGuid = Guid.NewGuid()`, map request to entity; call repository `AddAsync`; return mapped `WorkOrderDto`.
- [ ] **Application – Mapping:** In `MappingProfile`, add `CreateWorkOrderRequestDto` → `WorkOrder` (VehicleId, QuotationId, Notes, ScheduledStart, ScheduledEnd; ignore Id, OrderGuid, Status, Created*). Add `WorkOrder` → `WorkOrderDto`.
- [ ] **Infrastructure – DbContext:** Add `DbSet<WorkOrder> WorkOrders`; in `OnModelCreating` configure `ToTable("WorkOrder", "rpa")`, property mappings if needed, and relationships (e.g. HasOne Quotation, HasOne Vehicle) with FKs QuotationId, VehicleId.
- [ ] **Infrastructure – Repository:** Add `WorkOrderRepository` implementing `IWorkOrderRepository` using `RepairDbContext`, `AddAsync` and `GetByIdAsync`.
- [ ] **Infrastructure – Registration:** Register `IWorkOrderRepository` → `WorkOrderRepository` and `IWorkOrderService` → `WorkOrderService` in API `Program.cs` (or Infrastructure `ServiceRegistration` if used for repos).
- [ ] **API – Controller:** Add `WorkOrderController` with POST action that accepts `CreateWorkOrderRequestDto`, calls `CreateWorkOrderAsync`, returns 201 Created with location header and `WorkOrderDto`; return 400 when validation or business rules fail (e.g. customer/vehicle not found).
- [ ] **API – Validation:** Ensure request validation (e.g. VehicleId and QuotationId required); document that “tasks” are the quotation items of the linked quotation (no separate task table in this story).

---

## References

- **Project structure and patterns:** `projectStructure.md`
- **Architecture and conventions:** `CURSOR_PROMPT_SENIOR_DOTNET_ENGINEER.md`
- **Schema (tables and columns):** `scripts/schema-export.sql`
- **Existing features to mirror:** Customer create (CustomerService, CustomerController), Quotation/QuotationItem (entity and Fluent API in RepairDbContext), Vehicle repositories and services
