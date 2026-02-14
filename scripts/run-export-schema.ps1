# ============================================================
# Run in Cursor IDE Terminal: .\scripts\run-export-schema.ps1
# Exports all table schema from SQL Server into a single SQL DDL file
# Uses sqlcmd with Encrypt + TrustServerCertificate
# ============================================================

param(
    [string]$Server,
    [string]$Database,
    [string]$OutputFile = "schema-export.sql",
    [switch]$TrustedConnection,
    [string]$Username,
    [string]$Password
)

# -----------------------------
# Defaults from env or fallback
# -----------------------------
if (-not $Server)   { $Server   = if ($env:SQL_SERVER)   { $env:SQL_SERVER }   else { "localhost" } }
if (-not $Database) { $Database = if ($env:SQL_DATABASE) { $env:SQL_DATABASE } else { "GarageManagement" } }

if ($env:SQL_TRUSTED -eq "1") { $TrustedConnection = $true }
if (-not $Username) { $Username = $env:SQL_USER }
if (-not $Password) { $Password = $env:SQL_PASSWORD }

# -----------------------------
# Resolve paths
# -----------------------------
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$SqlFile   = Join-Path $ScriptDir "export-schema.sql"

if (-not [System.IO.Path]::IsPathRooted($OutputFile)) {
    $OutputFile = Join-Path $ScriptDir $OutputFile
}

# -----------------------------
# Create export-schema.sql dynamically
# -----------------------------
$ddlQuery = @"
-- ============================================================
-- Auto-generated SQL DDL export for database [$Database]
-- ============================================================

SET NOCOUNT ON;

-- Generate CREATE TABLE statements for all tables
SELECT 
    'CREATE TABLE [' + s.name + '].[' + t.name + '] (' + CHAR(13) +
    STRING_AGG(
        '    [' + c.name + '] ' +
        TYPE_NAME(c.user_type_id) +
        CASE WHEN c.max_length > 0 AND TYPE_NAME(c.user_type_id) IN ('varchar','nvarchar','char','nchar') 
             THEN '(' + CASE WHEN c.max_length = -1 THEN 'MAX' ELSE CAST(c.max_length AS NVARCHAR(10)) END + ')' ELSE '' END +
        CASE WHEN c.is_nullable = 0 THEN ' NOT NULL' ELSE ' NULL' END
        , ',' + CHAR(13)
    ) WITHIN GROUP (ORDER BY c.column_id)
    + CHAR(13) + ');' AS CreateTableSQL
FROM sys.tables t
JOIN sys.schemas s ON t.schema_id = s.schema_id
JOIN sys.columns c ON t.object_id = c.object_id
GROUP BY s.name, t.name
ORDER BY s.name, t.name;
"@

Set-Content -Path $SqlFile -Value $ddlQuery -Encoding UTF8

# -----------------------------
# Locate sqlcmd
# -----------------------------
function Find-Sqlcmd {
    $exe = Get-Command sqlcmd -ErrorAction SilentlyContinue
    if ($exe) { return $exe.Source }

    $paths = @(
        "C:\Program Files\Microsoft SQL Server\Client SDK\ODBC\180\Tools\Binn\sqlcmd.exe",
        "C:\Program Files\Microsoft SQL Server\Client SDK\ODBC\170\Tools\Binn\sqlcmd.exe",
        "C:\Program Files\Microsoft SQL Server\Client SDK\ODBC\160\Tools\Binn\sqlcmd.exe",
        "C:\Program Files\Microsoft SQL Server\150\Tools\Binn\sqlcmd.exe",
        "C:\Program Files\Microsoft SQL Server\140\Tools\Binn\sqlcmd.exe"
    )

    foreach ($p in $paths) { if (Test-Path $p) { return $p } }
    return $null
}

# -----------------------------
# Execute export
# -----------------------------
function Export-WithSqlcmd {
    $sqlcmdExe = Find-Sqlcmd
    if (-not $sqlcmdExe) {
        Write-Host "sqlcmd not found." -ForegroundColor Red
        return $false
    }

    $sqlcmdArgs = @(
        "-S", $Server,
        "-d", $Database,
        "-i", $SqlFile,
        "-o", $OutputFile,
        "-W",
        "-N",
        "-C",
        "-h", "-1"  # suppress headers
    )

    if ($TrustedConnection) { $sqlcmdArgs += "-E" }
    elseif ($Username) { 
        $sqlcmdArgs += "-U", $Username
        if ($Password) { $sqlcmdArgs += "-P", $Password }
    }
    else { $sqlcmdArgs += "-E" }

    Write-Host "Running sqlcmd:" -ForegroundColor Gray
    Write-Host "  $sqlcmdExe $($sqlcmdArgs -join ' ')" -ForegroundColor DarkGray

    $stderrFile = Join-Path $env:TEMP "sqlcmd-stderr.txt"
    if (Test-Path $stderrFile) { Remove-Item $stderrFile -Force }

    $process = Start-Process `
        -FilePath $sqlcmdExe `
        -ArgumentList $sqlcmdArgs `
        -Wait `
        -NoNewWindow `
        -PassThru `
        -RedirectStandardError $stderrFile

    if (Test-Path $stderrFile) {
        $err = Get-Content $stderrFile -Raw -ErrorAction SilentlyContinue
        if ($err) { Write-Host $err -ForegroundColor Red }
    }

    return ($process.ExitCode -eq 0)
}

# -----------------------------
# Run
# -----------------------------
Write-Host "Exporting SQL DDL from $Server / $Database" -ForegroundColor Cyan
Write-Host "Output file: $OutputFile" -ForegroundColor Cyan

$ok = Export-WithSqlcmd

if ($ok) {
    Write-Host "Schema exported successfully:" -ForegroundColor Green
    Write-Host "  $OutputFile" -ForegroundColor Green
} else {
    Write-Host "Schema export failed." -ForegroundColor Red
    exit 1
}
