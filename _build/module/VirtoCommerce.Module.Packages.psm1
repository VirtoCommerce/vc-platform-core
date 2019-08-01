function Pack-Module
{
    Param(
        [string] $NugetDir,
        [string] $ProjectDir,
        [string] $OutputDir
    )
	
	$InputDir = ""

	if ($ProjectDir) {
	    if(Test-Path $ProjectDir -PathType Container) {
		    $InputDir = $ProjectDir
		}
	} else {
		$InputDir = $PSScriptRoot
	}	
	
	if (-not $OutputDir) {
		$OutputDir = $InputDir
	}
	
	Set-Location ($InputDir)

    $workingFile = ""
    
    $modulePaths = @(Get-ChildItem $InputDir -Directory)
    foreach ($_ in $modulePaths) {      
        if(!($_.Name.Contains(".Web")) -And !($_.Name.Contains(".Test")) -And !($_.Name.Contains(".Tests"))) {			
            if(Test-Path "$_\*" -Include *.nuspec -PathType Leaf) {
                $workingFile = @(Get-ChildItem -Path "$_\*.nuspec")
                & "$NugetDir\nuget.exe" pack $workingFile.FullName -properties Configuration=Release -Build -OutputDirectory $OutputDir
            }
            else {
                if(Test-Path "$_\*" -Include *.csproj -PathType Leaf) {
                    $workingFile = @(Get-ChildItem -Path "$_\*.csproj")
                    & "$NugetDir\nuget.exe" pack $workingFile.FullName -properties Configuration=Release -Build -OutputDirectory $OutputDir
                }
            }
	    }	     
    }	
}

Export-ModuleMember -Function Pack-Module