@echo off
setlocal

set SCRIPT_DIR=%~dp0
set INPUT_FILE=%SCRIPT_DIR%categories.example.txt

python "%SCRIPT_DIR%generate_prompts.py" --input "%INPUT_FILE%" --output "C:\Projects\prompt\prompts.txt" --parts 10

echo.
echo Finished. Press any key to close.
pause > nul
