Param(
	[string] $ProjectDir,
	[string] $ArtifactDir
)

Import-Module (Join-Path $PSScriptRoot VirtoCommerce.Module.psm1)

$modulePaths = @(Get-ChildItem $ProjectDir -Recurse -Filter module.manifest)

foreach ($_ in $modulePaths) {                                                                                                             
	Compress-Module $_.DirectoryName $ArtifactDir
}