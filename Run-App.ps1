<#
  Starts the Property Rental web app with IIS Express (no Visual Studio needed).

  First time on this PC (database missing / errors on open):
    .\Run-App.ps1 -SetupDatabase

  Normal use:
    .\Run-App.ps1
#>
param(
    [switch]$SetupDatabase,
    [int]$Port = 56182
)

$ErrorActionPreference = 'Stop'
$ScriptRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$WebRoot = Join-Path $ScriptRoot 'prjRentalManagement'

if (-not (Test-Path (Join-Path $WebRoot 'Web.config'))) {
    throw "Web.config not found under: $WebRoot"
}

$iisExpress = @(
    "${env:ProgramFiles}\IIS Express\iisexpress.exe",
    "${env:ProgramFiles(x86)}\IIS Express\iisexpress.exe"
) | Where-Object { Test-Path $_ } | Select-Object -First 1

if (-not $iisExpress) {
    throw 'IIS Express not found. Install Visual Studio ASP.NET workload or IIS Express.'
}

if ($SetupDatabase) {
    $setup = Join-Path $ScriptRoot 'Setup-LocalEnvironment.ps1'
    if (-not (Test-Path $setup)) { throw "Setup script missing: $setup" }
    & $setup
}

Write-Host ""
Write-Host "Starting site: http://localhost:$Port/"
Write-Host "Press Q then Enter in this window to stop the server."
Write-Host ""

& $iisExpress "/path:$WebRoot" "/port:$Port"
