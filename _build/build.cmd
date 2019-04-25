SET PROJECT_PATH=%~dp0%
SET ARTIFACTS_PATH=%1

cd ..
cd /d VirtoCommerce.Platform.Web

call npm i

call npm run webpack:build

call dotnet publish -c Release -o %ARTIFACTS_PATH% -f netcoreapp2.2 --self-contained false