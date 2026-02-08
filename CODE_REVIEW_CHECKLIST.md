# Code Review Checklist — Pre-Commit (New Feature)

---

## Instructions for the Agent (AI)

When this file is used in agent context (e.g. @CODE_REVIEW_CHECKLIST.md or as part of a pre-commit review), **you must**:

1. **Perform the code review**
   - Focus on **changed and new files** (e.g. from `git status` / `git diff`).
   - Work through the checklist sections below (1–10) and verify the codebase against each item that applies to the change.
   - Use the codebase, `projectStructure.md`, and `CURSOR_PROMPT_SENIOR_DOTNET_ENGINEER.md` to assess compliance.

2. **Fix any issues found**
   - If a checklist item is **not satisfied**, **fix the code** (edit files) so it complies.
   - Prefer fixing over only reporting; make the minimum necessary changes to satisfy the checklist.
   - If an item cannot be fixed automatically (e.g. design decision), note it in the summary and leave the code as-is.

3. **Verify build and linters**
   - Run `dotnet build garageManagementSolution.sln` (or the solution path). If the build fails, fix errors until it succeeds.
   - Check linter/analyzer output for the changed files; resolve any new warnings in those files.

4. **Summarize and ask the user to commit**
   - Provide a short summary:
     - What was reviewed (which files/areas).
     - What issues were found and **what was fixed** (list fixes).
     - Any items skipped or not applicable and why.
   - End with a clear prompt: **ask the user to review the changes and commit them** (e.g. “Please review the changes above and commit when ready.”).

**Scope:** Apply checklist items only where they relate to the current change (e.g. no new API → skip API-specific items; no new entity → skip Domain entity checks). Do not refactor unrelated code.

---

## Checklist (use when performing the review)

Use this checklist **before committing** any new feature. Verify each item that applies to your changes.

---

## 1. Architecture & Layer Separation

| # | Check | ✓ |
|---|--------|---|
| 1.1 | **Dependencies point inward:** API → Application → Domain; Infrastructure → Application → Domain. Domain has no references to Application, Infrastructure, or API. |
| 1.2 | **Domain** contains only entities, value objects, and domain logic. No EF attributes, DbContext, or infrastructure types. |
| 1.3 | **Application** defines interfaces (repos, UoW, services); use cases and DTOs live here. No EF Core or infrastructure-specific code. |
| 1.4 | **Infrastructure** implements repository/service interfaces; DbContext and Fluent API configuration stay here. No business logic in repositories. |
| 1.5 | **API** is thin: controllers delegate to application services and use DTOs only. No direct repository or DbContext usage in controllers. |

---

## 2. Domain Layer

| # | Check | ✓ |
|---|--------|---|
| 2.1 | Entities are **rich domain models** (behavior + data), not anemic. |
| 2.2 | No data annotations or EF attributes on domain entities. |
| 2.3 | New entities inherit from **BaseEntity** where appropriate (Id, CreatedAt, CreatedBy, ModifiedAt, ModifiedBy, IsActive, IsDeleted). |
| 2.4 | Value objects used where applicable; business rules live in Domain or Application. |

---

## 3. Application Layer

| # | Check | ✓ |
|---|--------|---|
| 3.1 | New use cases implemented via **application services** (e.g. existing pattern: `ICustomerService`, `IQuotationService`). No new patterns (e.g. CQRS/MediatR) unless already in the project. |
| 3.2 | **Interfaces** for new repositories or external services are defined in Application; implementations live in Infrastructure. |
| 3.3 | All cross-layer data uses **DTOs**; no domain entities exposed from API. |
| 3.4 | **AutoMapper** used only for DTO ↔ Domain; new mappings added to `MappingProfile.cs` in Application/Mappings. |
| 3.5 | Validation follows project approach (e.g. **IJsonValidator** / JSON rules); FluentValidation only where already used. |
| 3.6 | Business exceptions are handled explicitly; no swallowed exceptions. |

---

## 4. Infrastructure Layer

| # | Check | ✓ |
|---|--------|---|
| 4.1 | New repositories implement interfaces from Application; use **RepairDbContext** and existing patterns (e.g. GenericRepository, UnitOfWork). |
| 4.2 | **Fluent API** / `OnModelCreating` used for entity configuration; no business logic in repository methods. |
| 4.3 | All data access is **async** (`async`/`await`). |
| 4.4 | New DbContext registrations or repository registrations are added in **ServiceRegistration.cs** or **Program.cs** as per existing pattern. |

---

## 5. API Layer

| # | Check | ✓ |
|---|--------|---|
| 5.1 | Controllers are **thin**: action methods call application service interfaces and return DTOs. |
| 5.2 | **RESTful** conventions followed; appropriate HTTP verbs and status codes used. |
| 5.3 | Request/response use **DTOs** or dedicated API models; domain entities are never returned directly. |
| 5.4 | List endpoints use **pagination** (e.g. `IPaginationService<T>`, `PaginationRequest` / `PaginationResult<T>`) where applicable. |
| 5.5 | Errors are handled via global exception-handling middleware where possible; no sensitive data in responses. |

---

## 6. Code Quality & Conventions

| # | Check | ✓ |
|---|--------|---|
| 6.1 | **Naming** matches project: `*Dto`, `*Repository`, `*Service`, existing folder layout (see `projectStructure.md`). |
| 6.2 | **Nullable reference types** respected; no unnecessary suppressions without reason. |
| 6.3 | No **magic strings/numbers**; use constants or configuration where appropriate. |
| 6.4 | **SOLID** and clean code principles followed; composition preferred over inheritance. |
| 6.5 | No **commented-out code** or debug leftovers; no unnecessary `using` statements. |

---

## 7. Security & Performance

| # | Check | ✓ |
|---|--------|---|
| 7.1 | All **external input** is validated; no raw user input used in queries or commands without validation. |
| 7.2 | **Sensitive data** (passwords, tokens, PII) is not logged or exposed in responses. |
| 7.3 | **Async** used for all I/O (DB, HTTP, file); no blocking calls in async code. |
| 7.4 | No **N+1** queries; use appropriate includes or projection where needed. |
| 7.5 | **Pagination** applied to list endpoints to avoid large result sets. |

---

## 8. Testing

| # | Check | ✓ |
|---|--------|---|
| 8.1 | **Unit tests** added/updated for new Domain logic, application services, and validators. |
| 8.2 | Tests use **mocks** for Infrastructure; no real DB or EF in unit tests. |
| 8.3 | Tests follow **Arrange–Act–Assert**; they test behavior, not implementation details. |
| 8.4 | All new tests **pass** locally. |

---

## 9. Project Consistency

| # | Check | ✓ |
|---|--------|---|
| 9.1 | **projectStructure.md** and **CURSOR_PROMPT_SENIOR_DOTNET_ENGINEER.md** were consulted; changes align with documented structure and rules. |
| 9.2 | New files are placed in the **correct project and folder** (e.g. DTOs in Application/DTOs, repos in Infrastructure/Repositories). |
| 9.3 | **Solution file** and **project references** are correct; solution builds without errors. |
| 9.4 | No new dependencies added without justification; existing libraries used where possible. |

---

## 10. Pre-Commit Sanity Checks

| # | Check | ✓ |
|---|--------|---|
| 10.1 | **Build** succeeds: `dotnet build garageManagementSolution.sln` (or your solution path). |
| 10.2 | **Linter/analyzers** show no new warnings in changed files. |
| 10.3 | **.cursorignore** and **.gitignore** respected; no committed secrets or ignored paths. |
| 10.4 | Commit message clearly describes the **feature** and scope of changes. |

---

## Quick Reference

- **Solution:** `garageManagementSolution.sln`
- **Architecture:** Onion — Domain (center) ← Application ← Infrastructure ← API
- **Key docs:** `projectStructure.md`, `CURSOR_PROMPT_SENIOR_DOTNET_ENGINEER.md`

---

---

**Agent reminder:** After completing the review and applying fixes, always **ask the user to review the changes and commit them**.
