@echo off
echo Starting PsyNet AI Medicine Recommendation API...
echo.

cd /d "d:\Sasha\PsyNet - Copy\PsyNet.Web\AIModels"

echo Checking if virtual environment exists...
if not exist "venv" (
    echo Creating virtual environment...
    python -m venv venv
)

echo Activating virtual environment...
call venv\Scripts\activate.bat

echo Installing required packages...
pip install -r requirements.txt

echo Starting Flask API server...
echo The API will be available at: http://localhost:5001
echo.
echo Press Ctrl+C to stop the server
echo.

python app.py

pause
