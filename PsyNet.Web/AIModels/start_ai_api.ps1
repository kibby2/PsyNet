# PsyNet AI Medicine Recommendation API Startup Script
Write-Host "Starting PsyNet AI Medicine Recommendation API..." -ForegroundColor Green
Write-Host ""

# Navigate to AIModels directory
Set-Location "d:\Sasha\PsyNet - Copy\PsyNet.Web\AIModels"

# Check if virtual environment exists
if (-not (Test-Path "venv")) {
    Write-Host "Creating virtual environment..." -ForegroundColor Yellow
    python -m venv venv
}

# Activate virtual environment
Write-Host "Activating virtual environment..." -ForegroundColor Yellow
& "venv\Scripts\Activate.ps1"

# Install required packages
Write-Host "Installing required packages..." -ForegroundColor Yellow
pip install -r requirements.txt

# Start Flask API server
Write-Host "Starting Flask API server..." -ForegroundColor Green
Write-Host "The API will be available at: http://localhost:5001" -ForegroundColor Cyan
Write-Host ""
Write-Host "Press Ctrl+C to stop the server" -ForegroundColor Red
Write-Host ""

python app.py
