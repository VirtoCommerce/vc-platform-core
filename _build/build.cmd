setlocal enabledelayedexpansion

SET PROJECT_PATH=%~dp0%
SET ARTIFACTS_PATH=%1

IF NOT DEFINED ARTIFACTS_PATH (
  SET ARTIFACTS_PATH=%~dp0%
)

cd ..
cd /d VirtoCommerce.Platform.Web

call npm i
IF !ERRORLEVEL! NEQ 0 goto error

call npm run webpack:build
IF !ERRORLEVEL! NEQ 0 goto error

call dotnet clean -c Release
IF !ERRORLEVEL! NEQ 0 goto error

del /q %ARTIFACTS_PATH%\*
for /d %%x in (%ARTIFACTS_PATH%\*) do @rd /s /q ^"%%x^"

call dotnet publish -c Release -o %ARTIFACTS_PATH% --self-contained false
IF !ERRORLEVEL! NEQ 0 goto error

goto end

:error
echo An error has occurred during platform build.
goto exit

:end
echo Finished successfully.

:exit
