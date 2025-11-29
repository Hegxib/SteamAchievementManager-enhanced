# Steam Achievement Manager - Build Script
# This script builds the project without requiring Visual Studio

Write-Host "=== Steam Achievement Manager - Build Script ===" -ForegroundColor Cyan
Write-Host ""

# Check if we're in the right directory
if (-not (Test-Path "SAM.sln")) {
    Write-Host "ERROR: SAM.sln not found. Please run this script from the solution directory." -ForegroundColor Red
    exit 1
}

# Look for MSBuild in common locations
$msbuildPaths = @(
    "C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe",
    "C:\Program Files\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe",
    "C:\Program Files\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\MSBuild.exe",
    "C:\Program Files\Microsoft Visual Studio\2022\BuildTools\MSBuild\Current\Bin\MSBuild.exe",
    "C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe",
    "C:\Program Files (x86)\Microsoft Visual Studio\2019\Professional\MSBuild\Current\Bin\MSBuild.exe",
    "C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\MSBuild\Current\Bin\MSBuild.exe",
    "C:\Program Files (x86)\Microsoft Visual Studio\2019\BuildTools\MSBuild\Current\Bin\MSBuild.exe",
    "C:\Program Files (x86)\MSBuild\14.0\Bin\MSBuild.exe"
)

$msbuild = $null
foreach ($path in $msbuildPaths) {
    if (Test-Path $path) {
        $msbuild = $path
        Write-Host "Found MSBuild: $msbuild" -ForegroundColor Green
        break
    }
}

if (-not $msbuild) {
    Write-Host ""
    Write-Host "MSBuild not found!" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "You need to install Visual Studio to build this project:" -ForegroundColor Yellow
    Write-Host "1. Download Visual Studio Community (free): https://visualstudio.microsoft.com/downloads/" -ForegroundColor White
    Write-Host "2. During installation, select '.NET desktop development' workload" -ForegroundColor White
    Write-Host "3. Make sure '.NET Framework 4.8 targeting pack' is selected" -ForegroundColor White
    Write-Host ""
    Write-Host "Alternative: Download pre-built release from GitHub:" -ForegroundColor Yellow
    Write-Host "https://github.com/gibbed/SteamAchievementManager/releases" -ForegroundColor White
    Write-Host ""
    exit 1
}

# Build the solution
Write-Host ""
Write-Host "Building solution..." -ForegroundColor Cyan
Write-Host "Configuration: Debug" -ForegroundColor Gray
Write-Host "Platform: x86" -ForegroundColor Gray
Write-Host ""

& $msbuild "SAM.sln" /p:Configuration=Debug /p:Platform=x86 /v:minimal

if ($LASTEXITCODE -eq 0) {
    Write-Host ""
    Write-Host "=== BUILD SUCCESSFUL ===" -ForegroundColor Green
    Write-Host ""
    Write-Host "Output files are in: .\bin\" -ForegroundColor White
    Write-Host ""
    Write-Host "To run the application:" -ForegroundColor Cyan
    Write-Host "  .\bin\SAM.Picker.exe" -ForegroundColor White
    Write-Host ""
    Write-Host "New Features Added:" -ForegroundColor Cyan
    Write-Host "  ✓ Bulk run multiple games" -ForegroundColor Green
    Write-Host "  ✓ Set achievement unlock times" -ForegroundColor Green
    Write-Host ""
} else {
    Write-Host ""
    Write-Host "=== BUILD FAILED ===" -ForegroundColor Red
    Write-Host "Check the error messages above for details." -ForegroundColor Yellow
    Write-Host ""
    exit 1
}
