function Compress-Module
{
    Param(
        [string] $ProjectName,
        [string] $OutputDir
    )
	
	$env:Path += ";C:\Program Files\nodejs\"

	$InputDir = ""

	if (Get-Module -ListAvailable -Name Get-Project) {
		if ($ProjectName) {
			$project = Get-Project -Name $ProjectName
		} else {
			$project = Get-Project
		}
		
		$InputDir = Split-Path $project.FullName -Parent
	} 
	else {
		if ($ProjectName) {
			if(Test-Path $ProjectName -PathType Container) {
				$InputDir = $ProjectName
			}
		} else {
			$InputDir = $PSScriptRoot
		}		
	}    
	
	if (-not $OutputDir) {
		$OutputDir = $InputDir
	}
	
	Set-Location ($InputDir)
		
	if(Test-Path "$InputDir\module.manifest" -PathType Leaf) {
		$tmp = [Guid]::NewGuid()
	
		[xml]$xml = Get-Content $InputDir\module.manifest
		$VCModuleId = (Select-Xml -Xml $xml -XPath "//module/id" | Select-Object -ExpandProperty Node).InnerText
		$VCModuleVersion = (Select-Xml -Xml $xml -XPath "//module/version" | Select-Object -ExpandProperty Node).InnerText
		$VCModuleZip = "$VCModuleId" + "_" + "$VCModuleVersion.zip"
				
		New-Item -ItemType Directory -Path "$OutputDir\$tmp"
	
		if(Test-Path "$InputDir\package.json" -PathType Leaf) {	
			npm i
			
			npm run webpack:build
		}
		
		if(Test-Path "$InputDir\dist") {
			Copy-Item "$InputDir\dist" -Destination "$OutputDir\$tmp\dist" -Recurse
		}
	
		if(Test-Path "$InputDir\Localizations") {
			Copy-Item "$InputDir\Localizations" -Destination "$OutputDir\$tmp\Localizations" -Recurse
		}
		
		if(Test-Path "$InputDir\Scripts") {
			Copy-Item "$InputDir\Scripts" -Destination "$OutputDir\$tmp\Scripts" -Recurse
		}
		
		if(Test-Path "$InputDir\Content") {
			Copy-Item "$InputDir\Content" -Destination "$OutputDir\$tmp\Content" -Recurse
		}
	
		Copy-Item "$InputDir\module.manifest" -Destination "$OutputDir\$tmp"
	
		if(Test-Path "$InputDir\module.ignore" -PathType Leaf) {
			Copy-Item "$InputDir\module.ignore" -Destination "$OutputDir\$tmp"
		}

		dotnet clean -c Release
	
		dotnet publish -c Release -o "$OutputDir\$tmp\bin" --self-contained false
	
		$platformDlls = 
"AspNet.Security.OAuth.Validation.dll",
"CoContra.dll",
"Dapper.dll",
"EntityFrameworkCore.TypedOriginalValues.dll",
"EntityFrameworkCore.Triggers.dll", 
"Hangfire.AspNetCore.dll",
"Hangfire.Core.dll",
"Hangfire.MemoryStorage.dll",
"HangFire.SqlServer.dll",
"Microsoft.AspNetCore.dll", 
"Microsoft.AspNetCore.App.dll", 
"Microsoft.Extensions.Caching.Abstractions.dll", 
"Microsoft.Extensions.DependencyInjection.Abstractions.dll", 
"Microsoft.Extensions.Identity.Stores.dll", 
"Microsoft.Extensions.Primitives.dll", 
"Microsoft.ApplicationInsights.AspNetCore.dll",
"Microsoft.AspNetCore.Diagnostics.dll",
"Microsoft.AspNetCore.Diagnostics.Abstractions.dll",
"Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore.dll",
"Microsoft.AspNetCore.Mvc.ViewFeatures.dll",
"Microsoft.Extensions.Logging.AzureAppServices.dll",
"Microsoft.Extensions.PlatformAbstractions.dll",
"Microsoft.VisualStudio.Web.BrowserLink.dll",
"Microsoft.AspNetCore.Http.Abstractions.dll",
"Microsoft.AspNetCore.SignalR.dll", 
"Microsoft.AspNetCore.SignalR.Client.dll", 
"Microsoft.AspNetCore.SignalR.Core.dll",
"Microsoft.AspNetCore.SignalR.Client.Core.dll",
"Microsoft.EntityFrameworkCore.dll", 
"Microsoft.EntityFrameworkCore.Design.dll", 
"Microsoft.EntityFrameworkCore.Relational.dll",
"Microsoft.EntityFrameworkCore.SqlServer.dll", 
"Microsoft.EntityFrameworkCore.Abstractions.dll",
"Microsoft.Net.Http.Headers.dll", 
"Microsoft.AspNetCore.Mvc.Core.dll", 
"Microsoft.Extensions.Logging.Abstractions.dll",
"Microsoft.Extensions.Options.dll",
"Microsoft.AspNetCore.Identity.dll",
"Microsoft.AspNetCore.Identity.EntityFrameworkCore.dll",
"Microsoft.EntityFrameworkCore.Design.dll",
"Microsoft.AspNetCore.Http.Connections.Client.dll",
"Newtonsoft.Json.dll", 
"OpenIddict.dll",
"OpenIddict.EntityFrameworkCore.dll",
"OpenIddict.Mvc.dll",
"Serialize.Linq.dll", 
"Swashbuckle.AspNetCore.dll",
"System.Security.Cryptography.Algorithms.dll",
"System.Interactive.Async.dll",
"VirtoCommerce.Smidge.dll",
"VirtoCommerce.Smidge.Nuglify.dll",
"WindowsAzure.Storage.dll"

		$dlls = @(Get-ChildItem -Path "$OutputDir\$tmp\bin" -Name)

		foreach ($_ in $dlls) {                                                                                                             
			if($_.StartsWith("VirtoCommerce")) {
				if(($_.Contains("Module") -And !$_.StartsWith($VCModuleId)) -Or $_.StartsWith("VirtoCommerce.Platform")) {			
					Remove-Item ("$OutputDir\$tmp\bin\" + $_)
				}
			}
		}
	
		if(Test-Path "$OutputDir\$tmp\module.ignore" -PathType Leaf) {
			$ignore = Get-Content "$OutputDir\$tmp\module.ignore"	

			Compare-Object -ReferenceObject $ignore -DifferenceObject $dlls -IncludeEqual | ForEach-Object -Process { if($_.SideIndicator -eq "==") { Remove-Item ("$OutputDir\$tmp\bin\" + $_.InputObject) }}
		}	

		Compare-Object -ReferenceObject $platformDlls -DifferenceObject $dlls -IncludeEqual | ForEach-Object -Process { if($_.SideIndicator -eq "==") { Remove-Item ("$OutputDir\$tmp\bin\" + $_.InputObject) }}
	
		Compress-Archive -Path "$OutputDir\$tmp\*" -DestinationPath "$OutputDir\$VCModuleZip" -Force
	
		Remove-Item "$OutputDir\$tmp" -Recurse
		}
		else {
			"Module isn't exist"
		}
}

Export-ModuleMember -Function Compress-Module