# Script to close SAM.Picker.exe and rebuild the solution

Write-Host "Stopping SAM.Picker.exe if running..." -ForegroundColor Yellow
Get-Process SAM.Picker -ErrorAction SilentlyContinue | Stop-Process -Force

Write-Host "Waiting 2 seconds..." -ForegroundColor Yellow
Start-Sleep -Seconds 2

Write-Host "Building solution..." -ForegroundColor Green
dotnet build SAM.sln -c Debug /p:Platform=x86

if ($LASTEXITCODE -eq 0) {
    Write-Host "`nBuild succeeded! You can now run SAM.Picker.exe from the bin folder." -ForegroundColor Green
} else {
    Write-Host "`nBuild failed. Check the errors above." -ForegroundColor Red
}
