Script for building and making nuget packages for virtocommerce platform modules

Please use with windows powershell only.

1. Run powershell
2. Change current directory to _build\module
3. Run ./init.ps1 for load build module to execution context
4. Run Compress-Module "<path-to-module-with-web>" "<artifact-path>". Artifact path argument is optional

If you want to buld all modules at once please run build-all-modules.cmd with optional parameter <path-to-artifact-directory> (where will be placed builded and packed modules)

If you want to make nuget packages for all modules at once please run make-packages.cmd with optional path to artifact folder
If run without parameters artifact folder will be packages\Modules in current directory

Making nuget package for module uses nuspec file as priority. If nuspec file isn't exists nuget package will be compile from scratch