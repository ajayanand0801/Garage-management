# Agent Executer — User Story Implementation Workflow

When this file is used in agent context (e.g. **@AgentExecuter.md**), follow the workflow below step by step.

---

## File paths (repository root)

| Purpose | Path |
|--------|------|
| User stories (DevOps) | **`output/userstories.json`** |
| Project structure | **`projectStructure.md`** |
| Senior .NET Engineer prompt | **`CURSOR_PROMPT_SENIOR_DOTNET_ENGINEER.md`** |
| Schema export (SQL tables) | **`scripts/schema-export.sql`** (or `scripts/schema-export.txt` if exported there) |
| Implementation output | **`output/implementation/<UserStoryId>-<Slug>.md`** (see step 6) |

---

## Workflow

### Step 1: Read or create `output/userstories.json`

1. **Try to read** `output/userstories.json`.
2. **If the file or folder does not exist:**
   - Create the **`output`** folder.
   - Create **`output/userstories.json`** with the **DevOps user-stories schema** below (one sample user story is enough so the user can add more).

**DevOps user-stories JSON schema** (use when creating the file):

```json
{
  "source": "DevOps",
  "project": "Garage Management",
  "lastUpdated": "YYYY-MM-DD",
  "userStories": [
    {
      "id": "US-001",
      "name": "Short title of the user story",
      "description": "As a [role], I want [goal] so that [benefit]. Optional longer description.",
      "acceptanceCriteria": [
        "Given ... When ... Then ...",
        "Scenario 2..."
      ],
      "priority": "High",
      "state": "New",
      "sprint": "",
      "areaPath": "",
      "tags": []
    }
  ]
}
```

**Fields to include:** `id`, `name`, `description`, `acceptanceCriteria` (array of strings). Optional: `priority`, `state`, `sprint`, `areaPath`, `tags`.

3. **If the file exists:** Parse it and continue to Step 2.

---

### Step 2: Ask the user which user story to process

1. List all user stories from `userstories.json` in a short table, e.g.:
   - **ID** | **Name** | **Description** (first 80 chars)
2. **Ask the user:**  
   *"Which user story do you want to process? (e.g. US-001 or the story ID)"*
3. Wait for the user to reply with a story **id** (e.g. `US-001`).

---

### Step 3: Load the selected user story

1. Find the user story in `userstories.json` by **id** (case-insensitive match).
2. If not found, tell the user and ask for a valid id; then retry from Step 2.
3. **Read and keep in context:**
   - **Name**
   - **Description**
   - **Acceptance criteria** (full list)

---

### Step 4: Switch to “ask agent” mode — analyze story

Act as the implementation agent and **analyze**:

1. **Description:** Extract roles, goal, benefit, and any implied behavior or constraints.
2. **Acceptance criteria:** For each criterion, identify:
   - **Given** (preconditions)
   - **When** (actions/triggers)
   - **Then** (expected outcomes)
   - Any **API**, **domain**, or **data** implications for the Garage Management solution.
3. **Scope:** Which layers are likely affected (Domain / Application / Infrastructure / API) and what new or changed components are needed (entities, DTOs, services, repositories, controllers).

Do **not** write code yet; only produce a short analysis (you will use it in Step 6).

---

### Step 5: Read project context

**Read** these files (in full) and keep them in context for the implementation document:

1. **`projectStructure.md`** — solution layout, layers, folder structure, patterns.
2. **`CURSOR_PROMPT_SENIOR_DOTNET_ENGINEER.md`** — architecture rules, Domain/Application/Infrastructure/API rules, naming, and conventions.
3. **Schema export** — **`scripts/schema-export.sql`** (or `scripts/schema-export.txt`). If the file exists, read it to get the list of SQL tables and their columns (e.g. `CREATE TABLE [schema].[TableName] (...)`). If it does not exist, note that entity design will be based only on the user story and project patterns.

Use this context to align the implementation plan with Onion architecture, existing patterns (e.g. application services, DTOs, repositories, `MappingProfile`, `RepairDbContext`), and the **current database schema** when available.

---

### Step 6: Create the user story implementation file

1. **Create** the folder **`output/implementation`** if it does not exist.
2. **Create** a markdown file named:  
   **`output/implementation/<UserStoryId>-<ShortSlug>.md`**  
   Example: `output/implementation/US-001-add-customer-export.md`
3. **Entity–schema alignment (before writing the plan):**
   - **Read** the schema export file (**`scripts/schema-export.sql`** or `scripts/schema-export.txt`). If it is missing, skip the alignment steps below and base entities only on the user story and project patterns.
   - **Identify** which SQL tables (e.g. `[schema].[TableName]`) are relevant to the user story (by table name, schema, or domain: e.g. `bkg`, `dbo`, `rpa`, `vhc`).
   - **For each relevant table:**
     - Check whether a **corresponding Domain entity** already exists in the project (e.g. under `GarageManagement.Domain/Entities/` or equivalent).
     - **If the entity exists:** Plan to **update** it. Analyse the schema columns vs current entity properties: add missing properties, align types (e.g. `bigint` → `long`, `nvarchar` → `string`, `bit` → `bool`, `datetime2` → `DateTime`), and note any columns to remove or rename. Document the intended changes so that the entity stays in sync with the table; Infrastructure Fluent API and migrations must be updated accordingly.
     - **If the entity does not exist:** Plan to **create** the entity from the schema. Map each column to a domain property (no EF attributes in Domain); decide table name and schema in Infrastructure (Fluent API). Use rich domain models and value objects where appropriate.
   - **Document** the outcome in the implementation file (see "Entity–schema alignment" subsection below).
4. **Write** the implementation document with at least these sections:

   - **User story**
     - Id, name, description (copy from JSON).
     - Acceptance criteria (bulleted list from JSON).
   - **Analysis summary**
     - Brief summary of Step 4 (roles, scenarios, scope, affected layers).
   - **Entity–schema alignment** (when schema export was used)
     - List of SQL tables relevant to this story.
     - For each table: **Create** (new entity) or **Update** (existing entity). If update, summarise property/type changes (add, remove, rename, type alignment). If create, note table/schema and key column-to-property mapping.
     - Ensures the implementation plan and tasks below are consistent with the database schema.
   - **Implementation plan**
     - **Domain:** New or changed entities/value objects; no EF attributes. Align with "Entity–schema alignment" (create vs update, property list).
     - **Application:** DTOs, interfaces (repos/services), application service changes, AutoMapper mappings.
     - **Infrastructure:** Repository implementations, DbContext/Fluent API changes, migrations if needed (new tables or schema changes).
     - **API:** Controllers, routes, request/response models, status codes.
   - **Tasks** (ordered checklist)
     - Concrete tasks the developer (or agent) can follow. For entities: either "**Create** entity X from table [schema].[TableName]" or "**Update** entity X to match schema (add PropertyY, change type of Z, …)". Then Application, Infrastructure, and API tasks as usual. “Add `CustomerExportDto` in Application/DTOs”, “Add `ExportCustomersAsync` to `ICustomerService` and `CustomerService`”).
   - **References**
     - Links or paths: `projectStructure.md`, `CURSOR_PROMPT_SENIOR_DOTNET_ENGINEER.md`, **`scripts/schema-export.sql`** (when used), and any existing similar feature (e.g. Customer, Vehicle) to mirror.

5. **Confirm** to the user that the file was created and give the path.
6. **Ask** the user if they want to proceed with implementation (code changes) based on this file, or to process another user story.

---

## Summary for the agent

| Step | Action |
|------|--------|
| 1 | Read `output/userstories.json`; if missing, create `output` and DevOps-style `userstories.json` with the schema above. |
| 2 | List user stories and ask which one to process (by id). |
| 3 | Load selected story (name, description, acceptance criteria). |
| 4 | Analyze description and acceptance criteria (Given/When/Then, scope, layers). |
| 5 | Read `projectStructure.md`, `CURSOR_PROMPT_SENIOR_DOTNET_ENGINEER.md`, and **schema export** (`scripts/schema-export.sql` or `.txt`) when available. |
| 6 | **Entity–schema alignment:** From schema export, identify relevant SQL tables; for each, decide **create** new entity or **update** existing entity (analyse columns vs current properties). Then create `output/implementation/<UserStoryId>-<Slug>.md` with user story, analysis, **entity–schema alignment**, implementation plan, tasks, references. |
| 7 | Tell the user the file path and ask if they want to implement or pick another story. |

---

## How to invoke

In Cursor, you can say for example:

- *"Run the user story workflow from @AgentExecuter.md"*
- *"Execute @AgentExecuter.md and process a user story"*

The agent will start at Step 1 and follow the workflow until Step 7.
