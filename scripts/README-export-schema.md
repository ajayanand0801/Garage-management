# Schema Export Scripts

Exports all table schema (tables, columns, keys, indexes) from the SQL Server database.

## Files

| File | Purpose |
|------|--------|
| `export-schema.sql` | SQL script that queries system catalog and outputs schema (run via sqlcmd). |
| `run-export-schema.ps1` | PowerShell script for Cursor IDE terminal: runs the SQL and saves output to a file. |

## Run in Cursor IDE Terminal (recommended)

From repo root:

```powershell
.\scripts\run-export-schema.ps1
```

Defaults: server `localhost`, database `GarageManagement`, Windows auth, output `scripts\schema-export.txt`.

With parameters:

```powershell
.\scripts\run-export-schema.ps1 -Server ".\SQLEXPRESS" -Database "GarageManagement" -OutputFile "my-schema.txt"
```

SQL authentication:

```powershell
.\scripts\run-export-schema.ps1 -Server "localhost" -Database "GarageManagement" -Username "sa" -Password "YourPassword"
```

Trust server certificate (e.g. Docker SQL / self-signed cert):

```powershell
.\scripts\run-export-schema.ps1 -Server "localhost" -Database "GarageManagement" -Username "sa" -Password "YourPassword" -TrustServerCertificate
```

Environment variables (optional): `SQL_SERVER`, `SQL_DATABASE`, `SQL_USER`, `SQL_PASSWORD`, `SQL_TRUSTED=1`, `SQL_TRUST_SERVER_CERTIFICATE=1`.

## Run SQL file directly (any terminal)

Requires [sqlcmd](https://docs.microsoft.com/en-us/sql/tools/sqlcmd-utility) (SQL Server tools or SSMS).

From repo root:

```powershell
sqlcmd -S localhost -d GarageManagement -E -i scripts\export-schema.sql -o scripts\schema-export.txt -W
```

- `-S` server, `-d` database, `-E` Windows auth (use `-U` / `-P` for SQL auth).
- `-i` input SQL file, `-o` output file, `-W` trim trailing spaces.

Output is written to the file path given after `-o`.

## Troubleshooting: "sqlcmd failed" or "sqlcmd not found"

- **sqlcmd not found**  
  The script looks for `sqlcmd` in your PATH and in common install folders (e.g. `C:\Program Files\Microsoft SQL Server\Client SDK\ODBC\*\Tools\Binn`). Install [SQL Server Command Line Utilities](https://learn.microsoft.com/en-us/sql/tools/sqlcmd-utility) or SSMS (which includes sqlcmd), or add the sqlcmd folder to your PATH.

- **Export failed (errors above)**  
  Usually login or database name. Use the correct instance, e.g. `-Server '.\SQLEXPRESS'` or `-Server '(localdb)\MSSQLLocalDB'`, and a valid `-Database` name.
