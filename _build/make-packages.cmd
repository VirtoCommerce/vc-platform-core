setlocal enabledelayedexpansion

SET PROJECT_PATH=%~dp0%
SET ARTIFACTS_PATH=%1

IF NOT DEFINED ARTIFACTS_PATH (
  SET ARTIFACTS_PATH=%~dp0%\packages\Platform
)

cd ..
cd /d VirtoCommerce.Platform.Assets.AzureBlobStorage

call %PROJECT_PATH%/.nuget/nuget.exe pack VirtoCommerce.Platform.Assets.AzureBlobStorage.v3.nuspec -properties Configuration=Release -OutputDirectory %ARTIFACTS_PATH%
IF !ERRORLEVEL! NEQ 0 goto error

cd ..
cd /d VirtoCommerce.Platform.Assets.FileSystem

call %PROJECT_PATH%/.nuget/nuget.exe pack VirtoCommerce.Platform.Assets.FileSystem.v3.nuspec -properties Configuration=Release -OutputDirectory %ARTIFACTS_PATH%
IF !ERRORLEVEL! NEQ 0 goto error

cd ..
cd /d VirtoCommerce.Platform.Core

call %PROJECT_PATH%/.nuget/nuget.exe pack VirtoCommerce.Platform.Core.v3.nuspec -properties Configuration=Release -OutputDirectory %ARTIFACTS_PATH%
IF !ERRORLEVEL! NEQ 0 goto error

cd ..
cd /d VirtoCommerce.Platform.Data

call %PROJECT_PATH%/.nuget/nuget.exe pack VirtoCommerce.Platform.Data.v3.nuspec -properties Configuration=Release -OutputDirectory %ARTIFACTS_PATH%
IF !ERRORLEVEL! NEQ 0 goto error

cd ..
cd /d VirtoCommerce.Platform.Modules

call %PROJECT_PATH%/.nuget/nuget.exe pack VirtoCommerce.Platform.Modules.v3.nuspec -properties Configuration=Release -OutputDirectory %ARTIFACTS_PATH%
IF !ERRORLEVEL! NEQ 0 goto error

cd ..
cd /d VirtoCommerce.Platform.Security

call %PROJECT_PATH%/.nuget/nuget.exe pack VirtoCommerce.Platform.Security.v3.nuspec -properties Configuration=Release -OutputDirectory %ARTIFACTS_PATH%
IF !ERRORLEVEL! NEQ 0 goto error

goto end

:error
echo An error has occurred during platform nuget packages making.
goto exit

:end
echo Finished successfully.

:exit
