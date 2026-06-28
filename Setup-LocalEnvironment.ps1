<#
  One-shot local setup for Property Rental Management (SQL Server LocalDB).

  - Creates database PropertyRentalDB if missing
  - Applies schema from prjRentalManagement\Models\Model1.edmx.sql (recreates tables)
  - Runs Database\SeedPropertyRentalDB.sql (README sample users + demo building/apartments)

  Usage (from this folder, in PowerShell):
    .\Setup-LocalEnvironment.ps1
    .\Setup-LocalEnvironment.ps1 -SeedOnly    # schema already applied; only seed/missing rows
    .\Setup-LocalEnvironment.ps1 -ServerInstance "localhost\SQLEXPRESS"   # full SQL Server (SSMS)

  Requires: sqlcmd (SQL Server Command Line Tools)
#>
param(
    [switch]$SeedOnly,
    [string]$ServerInstance = '(localdb)\mssqllocaldb'
)

$ErrorActionPreference = 'Stop'
$DatabaseName = 'PropertyRentalDB'

$ScriptRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$SchemaFile = Join-Path $ScriptRoot 'prjRentalManagement\Models\Model1.edmx.sql'
$UpgradeFile = Join-Path $ScriptRoot 'Database\UpgradePakistanBuildingImage.sql'
$UpgradeUnitsFile = Join-Path $ScriptRoot 'Database\UpgradeBuildingUnits.sql'
$UpgradeBookingsFile = Join-Path $ScriptRoot 'Database\UpgradeBookings.sql'
$UpgradeBookingWorkflowFile = Join-Path $ScriptRoot 'Database\UpgradeBookingWorkflowStatus.sql'
$SeedFile = Join-Path $ScriptRoot 'Database\SeedPropertyRentalDB.sql'

$sqlcmd = @(
    "${env:ProgramFiles}\Microsoft SQL Server\Client SDK\ODBC\170\Tools\Binn\SQLCMD.EXE",
    "${env:ProgramFiles(x86)}\Microsoft SQL Server\Client SDK\ODBC\170\Tools\Binn\SQLCMD.EXE"
) | Where-Object { Test-Path $_ } | Select-Object -First 1

if (-not $sqlcmd) {
    $found = Get-ChildItem -Path "${env:ProgramFiles}\Microsoft SQL Server" -Recurse -Filter 'SQLCMD.EXE' -ErrorAction SilentlyContinue |
        Select-Object -First 1 -ExpandProperty FullName
    if ($found) { $sqlcmd = $found }
}

if (-not $sqlcmd) {
    throw 'sqlcmd.exe not found. Install SQL Server Command Line Tools or SQL Server (with sqlcmd).'
}

function Invoke-SqlCmdFile {
    param([string]$InputFile)
    & $sqlcmd -S $ServerInstance -d $DatabaseName -b -i $InputFile
    if ($LASTEXITCODE -ne 0) { throw "sqlcmd failed on: $InputFile (exit $LASTEXITCODE)" }
}

function Invoke-SqlCmdQuery {
    param([string]$Query)
    & $sqlcmd -S $ServerInstance -Q $Query -b
    if ($LASTEXITCODE -ne 0) { throw "sqlcmd failed: $Query (exit $LASTEXITCODE)" }
}

Write-Host "Using sqlcmd: $sqlcmd"
Write-Host "Server: $ServerInstance"

if (-not $SeedOnly) {
    if (-not (Test-Path $SchemaFile)) { throw "Schema file not found: $SchemaFile" }
    Write-Host "Creating database $DatabaseName if needed..."
    Invoke-SqlCmdQuery "IF DB_ID(N'$DatabaseName') IS NULL CREATE DATABASE [$DatabaseName];"
    Write-Host "Applying schema (Model1.edmx.sql) - this drops and recreates tables..."
    Invoke-SqlCmdFile $SchemaFile
    Write-Host "Applying optional DB upgrades (Pakistan phone columns, building photos)..."
    if (Test-Path $UpgradeFile) { Invoke-SqlCmdFile $UpgradeFile }
    if (Test-Path $UpgradeUnitsFile) { Invoke-SqlCmdFile $UpgradeUnitsFile }
    if (Test-Path $UpgradeBookingsFile) { Invoke-SqlCmdFile $UpgradeBookingsFile }
    if (Test-Path $UpgradeBookingWorkflowFile) { Invoke-SqlCmdFile $UpgradeBookingWorkflowFile }
}

if ($SeedOnly) {
    Write-Host "Seed-only: applying optional upgrades then seed..."
    if (Test-Path $UpgradeFile) { Invoke-SqlCmdFile $UpgradeFile }
    if (Test-Path $UpgradeUnitsFile) { Invoke-SqlCmdFile $UpgradeUnitsFile }
    if (Test-Path $UpgradeBookingsFile) { Invoke-SqlCmdFile $UpgradeBookingsFile }
    if (Test-Path $UpgradeBookingWorkflowFile) { Invoke-SqlCmdFile $UpgradeBookingWorkflowFile }
}

if (-not (Test-Path $SeedFile)) { throw "Seed file not found: $SeedFile" }
Write-Host "Applying seed data..."
Invoke-SqlCmdFile $SeedFile

Write-Host ""
Write-Host "Done. Run the web app in Visual Studio or IIS Express."
Write-Host "Connection: Data Source=$ServerInstance; Initial Catalog=$DatabaseName; Integrated Security=True"
Write-Host 'Sample password for all demo users: password123'
