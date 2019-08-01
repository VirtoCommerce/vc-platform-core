Param(
	[string] $NugetDir,
	[string] $ProjectDir,
	[string] $ArtifactDir
)

Import-Module (Join-Path $PSScriptRoot VirtoCommerce.Module.Packages.psm1)

$modulePaths = @(Get-ChildItem $ProjectDir -Recurse -Depth 1)

foreach ($_ in $modulePaths) {                                                                                                             
	Pack-Module $NugetDir $_.DirectoryName $ArtifactDir
}