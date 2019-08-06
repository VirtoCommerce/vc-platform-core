setlocal enabledelayedexpansion

SET PROJECT_PATH=%~dp0%
SET ARTIFACTS_PATH=%1

IF NOT DEFINED ARTIFACTS_PATH (
  SET ARTIFACTS_PATH=%~dp0%packages\Modules
)

cd ..
cd ..
cd /d Modules

del /q %ARTIFACTS_PATH%\*

Powershell.exe -nologo -ExecutionPolicy Bypass -File %PROJECT_PATH%\make-packages-all-modules.ps1 %cd% %ARTIFACTS_PATH%

echo Finished.
