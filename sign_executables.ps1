# Code Signing Script for HxB SAM Enhanced
# This script signs all executables with your code signing certificate

# Configuration
$CertificateThumbprint = "YOUR_CERTIFICATE_THUMBPRINT_HERE" # Get from certificate details
$TimestampServer = "http://timestamp.digicert.com" # DigiCert timestamp server
# Alternative timestamp servers:
# http://timestamp.sectigo.com
# http://timestamp.globalsign.com/tsa/r6advanced1

# Path to signtool.exe (adjust based on your Windows SDK installation)
$SignToolPath = "C:\Program Files (x86)\Windows Kits\10\bin\10.0.22621.0\x64\signtool.exe"

# Files to sign
$FilesToSign = @(
    ".\upload\SAM.Picker.exe",
    ".\upload\SAM.Game.exe",
    ".\upload\SAM.API.dll"
)

Write-Host "╔════════════════════════════════════════════════════════╗" -ForegroundColor Cyan
Write-Host "║        Code Signing - HxB SAM Enhanced V1.3           ║" -ForegroundColor Cyan
Write-Host "╚════════════════════════════════════════════════════════╝`n" -ForegroundColor Cyan

# Check if signtool exists
if (-not (Test-Path $SignToolPath)) {
    Write-Host "❌ ERROR: signtool.exe not found at: $SignToolPath" -ForegroundColor Red
    Write-Host "`nPlease install Windows SDK or update the path in this script." -ForegroundColor Yellow
    Write-Host "Download: https://developer.microsoft.com/en-us/windows/downloads/windows-sdk/" -ForegroundColor Yellow
    exit 1
}

# Sign each file
$successCount = 0
$failCount = 0

foreach ($file in $FilesToSign) {
    if (Test-Path $file) {
        Write-Host "📝 Signing: $file" -ForegroundColor Yellow
        
        # Sign the file
        & $SignToolPath sign /sha1 $CertificateThumbprint /fd SHA256 /tr $TimestampServer /td SHA256 /v $file
        
        if ($LASTEXITCODE -eq 0) {
            Write-Host "✅ Successfully signed: $file`n" -ForegroundColor Green
            $successCount++
        } else {
            Write-Host "❌ Failed to sign: $file`n" -ForegroundColor Red
            $failCount++
        }
    } else {
        Write-Host "⚠️  File not found: $file" -ForegroundColor Yellow
        $failCount++
    }
}

Write-Host "`n╔════════════════════════════════════════════════════════╗" -ForegroundColor Cyan
Write-Host "║                   SIGNING COMPLETE                     ║" -ForegroundColor Cyan
Write-Host "╚════════════════════════════════════════════════════════╝" -ForegroundColor Cyan
Write-Host "✅ Successful: $successCount" -ForegroundColor Green
Write-Host "❌ Failed: $failCount" -ForegroundColor Red

if ($successCount -gt 0) {
    Write-Host "`n🎉 Your executables are now signed!" -ForegroundColor Green
    Write-Host "Publisher will show as: Hegxib (or your certificate name)" -ForegroundColor Cyan
}
