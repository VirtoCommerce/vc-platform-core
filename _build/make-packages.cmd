setlocal enabledelayedexpansion

SET PROJECT_PATH=%~dp0%
SET ARTIFACTS_PATH=%1

IF NOT DEFINED ARTIFACTS_PATH (
  SET ARTIFACTS_PATH=%~dp0%\packages\Platform
)

cd ..
cd /d VirtoCommerce.Platform.Assets.AzureBlobStorage

call nuget pack -IncludeReferencedProjects -Symbols -Properties Configuration=Release -OutputDirectory %ARTIFACTS_PATH%
IF !ERRORLEVEL! NEQ 0 goto error

cd ..
cd /d VirtoCommerce.Platform.Assets.FileSystem

call nuget pack -IncludeReferencedProjects -Symbols -Properties Configuration=Release -OutputDirectory %ARTIFACTS_PATH%
IF !ERRORLEVEL! NEQ 0 goto error

cd ..
cd /d VirtoCommerce.Platform.Core

call nuget pack -IncludeReferencedProjects -Symbols -Properties Configuration=Release -OutputDirectory %ARTIFACTS_PATH%
IF !ERRORLEVEL! NEQ 0 goto error

cd ..
cd /d VirtoCommerce.Platform.Data

call nuget pack -IncludeReferencedProjects -Symbols -Properties Configuration=Release -OutputDirectory %ARTIFACTS_PATH%
IF !ERRORLEVEL! NEQ 0 goto error

cd ..
cd /d VirtoCommerce.Platform.Modules

call nuget pack -IncludeReferencedProjects -Symbols -Properties Configuration=Release -OutputDirectory %ARTIFACTS_PATH%
IF !ERRORLEVEL! NEQ 0 goto error

cd ..
cd /d VirtoCommerce.Platform.Security

call nuget pack -IncludeReferencedProjects -Symbols -Properties Configuration=Release -OutputDirectory %ARTIFACTS_PATH%
IF !ERRORLEVEL! NEQ 0 goto error

goto end

:error
echo An error has occurred during platform nuget packages making.
goto exit

:end
echo Finished successfully.

:exit
