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
		
		if (-not $OutputDir) {
			$OutputDir = $InputDir
		}
	} 
	else {
		if ($ProjectName) {
			if(Test-Path $ProjectName -PathType Container) {
				$InputDir = $ProjectName
			}
		} else {
			$InputDir = (Get-Item -Path ".\").FullName
		}		
	}    
	
	Set-Location ($InputDir)
	
	$tmp = [Guid]::NewGuid()
	
	[xml]$xml = Get-Content $PSScriptRoot\module.manifest
	$VCModuleId = (Select-Xml -Xml $xml -XPath "//module/id" | Select-Object -ExpandProperty Node).InnerText
	$VCModuleVersion = (Select-Xml -Xml $xml -XPath "//module/version" | Select-Object -ExpandProperty Node).InnerText
	$VCModuleZip = "$VCModuleId" + "_" + "$VCModuleVersion.zip"
	
	npm i
	
	npm run webpack:build
	
	Copy-Item "$InputDir\dist" -Destination "$OutputDir\$tmp\dist" -Recurse
	
	Copy-Item "$InputDir\Localizations" -Destination "$OutputDir\$tmp\Localizations" -Recurse
	
	Copy-Item "$InputDir\module.manifest" -Destination "$OutputDir\$tmp"
	
	dotnet publish -c Release -o "$OutputDir\$tmp\bin" -f netcoreapp2.2 --self-contained false
	
	Compress-Archive -Path "$OutputDir\$tmp\*" -DestinationPath "$OutputDir\$VCModuleZip"
	
	Remove-Item "$OutputDir\$tmp" -Recurse
}

Export-ModuleMember -Function Compress-Module