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
	
	npm i
	
	npm run webpack:build
	
	Copy-Item "$InputDir\dist" -Destination "$OutputDir\dist" -Recurse
	
	dotnet publish -c Release -o "$OutputDir" -f netcoreapp2.2 --self-contained false
}

Export-ModuleMember -Function Compress-Module