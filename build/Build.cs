using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Nuke.Common;
using Nuke.Common.Execution;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.Git;
using Nuke.Common.Tools.GitReleaseManager;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Tools.Npm;
using Nuke.Common.Utilities;
using Nuke.Common.Utilities.Collections;
using VirtoCommerce.Platform.Core.Modularity;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using static Nuke.Common.Tools.Git.GitTasks;

[CheckBuildProjectConfigurations]
[UnsetVisualStudioEnvironmentVariables]
class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode

    public static int Main() => Execute<Build>(x => x.Compile);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    private static string[] ModuleContentFolders = new[] { "dist", "Localizations", "Scripts" };

    [Solution] readonly Solution Solution;
    [GitRepository] readonly GitRepository GitRepository;
    [GitVersion] readonly GitVersion GitVersion;

    [PackageExecutable(packageId: "Swashbuckle.AspNetCore.Cli", packageExecutable: "swagger-cli.exe|dotnet-swagger.dll")]
    Tool SwaggerCli;

    readonly Tool Git;

    readonly string MasterBranch = "master";
    readonly string DevelopBranch = "develop";
    readonly string ReleaseBranchPrefix = "release";
    readonly string HotfixBranchPrefix = "hotfix";

    [Parameter("ApiKey for the specified source")] readonly string ApiKey;
    [Parameter] readonly string Source = @"https://api.nuget.org/v3/index.json";

    [Parameter] static string GlobalModuleIgnoreFileUrl = @"https://raw.githubusercontent.com/VirtoCommerce/vc-platform-core/release/3.0.0/module.ignore";

    AbsolutePath SourceDirectory => RootDirectory / "src";
    AbsolutePath TestsDirectory => RootDirectory / "tests";
    AbsolutePath ArtifactsDirectory => RootDirectory / "artifacts";
    Project WebProject => Solution.AllProjects.FirstOrDefault(x => x.SolutionFolder?.Name == "src" && x.Name.EndsWith("Web"));
    AbsolutePath ModuleManifest => WebProject.Directory / "module.manifest";
    AbsolutePath ModuleIgnoreFile => RootDirectory / "module.ignore";

    string ModuleId => XmlTasks.XmlPeek(ModuleManifest, "module/id").FirstOrDefault();
    string ModuleVersion => XmlTasks.XmlPeek(ModuleManifest, "module/version").FirstOrDefault();
    string ModuleVersionTag => XmlTasks.XmlPeek(ModuleManifest, "module/version-tag").FirstOrDefault();
    AbsolutePath ModuleOutputDirectory => ArtifactsDirectory / (ModuleId + ModuleVersion);

    string ModulePackageUrl => $"https://virtocommerce.blob.core.windows.net/modules3/{ModuleId + "_" + string.Join("-", ModuleVersion, ModuleVersionTag) + ".zip"}";
    GitRepository ModulesRepository => GitRepository.FromUrl("https://github.com/VirtoCommerce/vc-modules.git");

    bool IsModule => FileExists(ModuleManifest);

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            SourceDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
            if (DirectoryExists(TestsDirectory))
            {
                TestsDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
            }
            if (DirectoryExists(TestsDirectory))
            {
                WebProject.Directory.GlobDirectories("**/node_modules").ForEach(DeleteDirectory);
            }
            EnsureCleanDirectory(ArtifactsDirectory);
        });

    Target Restore => _ => _
        .Executes(() =>
        {
            DotNetRestore(s => s
                .SetProjectFile(Solution));
        });

    Target Pack => _ => _
      .DependsOn(Test)
      .Executes(() =>
      {
          DotNetPack(s => s
              .SetProject(Solution)
              .EnableNoBuild()
              .SetConfiguration(Configuration)
              .EnableIncludeSymbols()
              .SetSymbolPackageFormat(DotNetSymbolPackageFormat.snupkg)
              .SetOutputDirectory(ArtifactsDirectory)
              .SetVersion(IsModule ? string.Join("-", ModuleVersion, ModuleVersionTag) : GitVersion.NuGetVersionV2));
      });

    Target Test => _ => _
       .DependsOn(Compile)
       .Executes(() =>
       {
           DotNetTest(s => s
               .SetConfiguration(Configuration)
               .EnableNoBuild()
               .SetLogger("trx")
               .SetResultsDirectory(ArtifactsDirectory)
               .CombineWith(
                   Solution.GetProjects("*.Tests"), (cs, v) => cs
                       .SetProjectFile(v)));
       });

    Target PublishPackages => _ => _
        .DependsOn(Clean, Compile, Test, Pack)
        .Requires(() => ApiKey)
        .Executes(() =>
        {
            var packages = ArtifactsDirectory.GlobFiles("*.nupkg");

            DotNetNuGetPush(s => s
                    .SetSource(Source)
                    .SetApiKey(ApiKey)
                    .CombineWith(
                        packages, (cs, v) => cs
                            .SetTargetPath(v)),
                degreeOfParallelism: 5,
                completeOnFailure: true);
        });

    Target Publish => _ => _
       .DependsOn(Compile)
       .Executes(() =>
       {
           DotNetPublish(s => s
               .SetWorkingDirectory(WebProject.Directory)
               .EnableNoRestore()
               .SetOutput(IsModule ? ModuleOutputDirectory / "bin" : ArtifactsDirectory / "publish")
               .SetConfiguration(Configuration)
               .SetAssemblyVersion(IsModule ? ModuleVersion : GitVersion.GetNormalizedAssemblyVersion())
               .SetFileVersion(IsModule ? ModuleVersion : GitVersion.GetNormalizedFileVersion())
               .SetInformationalVersion(IsModule ? ModuleVersion : GitVersion.InformationalVersion));

       });

    Target WebPackBuild => _ => _
     .Executes(() =>
     {
         if (FileExists(WebProject.Directory / "package.json"))
         {
             NpmTasks.Npm("ci", WebProject.Directory);
             NpmTasks.NpmRun(s => s.SetWorkingDirectory(WebProject.Directory).SetCommand("webpack:build"));
         }
         else
         {
             Logger.Info("Nothing to build.");
         }
     });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetBuild(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .SetAssemblyVersion(IsModule ? ModuleVersion : GitVersion.GetNormalizedAssemblyVersion())
                .SetFileVersion(IsModule ? ModuleVersion : GitVersion.GetNormalizedFileVersion())
                .SetInformationalVersion(IsModule ? ModuleVersion : GitVersion.InformationalVersion)
                .EnableNoRestore());

        });

    Target Compress => _ => _
     .DependsOn(Clean, WebPackBuild, Test, Publish)
     .Executes(() =>
     {
       
         if (IsModule)
         {
             //Copy module.manifest and all content directories into a module output folder
             CopyFileToDirectory(ModuleManifest, ModuleOutputDirectory, FileExistsPolicy.Overwrite);
             foreach (var moduleFolder in ModuleContentFolders)
             {
                 var srcModuleFolder = WebProject.Directory / moduleFolder;
                 if (DirectoryExists(srcModuleFolder))
                 {
                     CopyDirectoryRecursively(srcModuleFolder, ModuleOutputDirectory / moduleFolder, DirectoryExistsPolicy.Merge, FileExistsPolicy.Overwrite);
                 }
             }

             var ignoredFiles = HttpTasks.HttpDownloadString(GlobalModuleIgnoreFileUrl).SplitLineBreaks();
             if (FileExists(ModuleIgnoreFile))
             {
                 ignoredFiles = ignoredFiles.Concat(TextTasks.ReadAllLines(ModuleIgnoreFile)).ToArray();
             }
             ignoredFiles = ignoredFiles.Select(x => x.Trim()).Distinct().ToArray();

             var zipFileName = ArtifactsDirectory / ModuleId + "_" + ModuleVersion + ".zip";
             DeleteFile(zipFileName);
             CompressionTasks.CompressZip(ModuleOutputDirectory, zipFileName, (x) => !ignoredFiles.Contains(x.Name, StringComparer.OrdinalIgnoreCase));
         }
         else
         {
             var zipFileName = ArtifactsDirectory / "VirtoCommerce.Platform." + GitVersion.SemVer + ".zip";
             DeleteFile(zipFileName);
             CompressionTasks.CompressZip(ArtifactsDirectory / "publish", zipFileName);
         }
     });

    Target PublishModuleManifest => _ => _
        .Executes(() =>
        {
            var modulesLocalDirectory = ArtifactsDirectory / "vc-modules";
            var modulesJsonFile = modulesLocalDirectory / "modules_v3.json";
            if (!DirectoryExists(modulesLocalDirectory))
            {
                GitTasks.Git($"clone {ModulesRepository.HttpsUrl} {modulesLocalDirectory}");
            }
            else
            {
                GitTasks.Git($"pull", modulesLocalDirectory);
            }
            var modulesExternalManifests = JsonConvert.DeserializeObject<List<ExternalModuleManifest>>(TextTasks.ReadAllText(modulesJsonFile));
            var manifest = ManifestReader.Read(ModuleManifest);
            manifest.PackageUrl = ModulePackageUrl;
            var existExternalManifest = modulesExternalManifests.FirstOrDefault(x => x.Id == manifest.Id);
            if (existExternalManifest != null)
            {
                existExternalManifest.PublishNewVersion(manifest);
            }
            else
            {
                modulesExternalManifests.Add(ExternalModuleManifest.FromManifest(manifest));
            }
            TextTasks.WriteAllText(modulesJsonFile, JsonConvert.SerializeObject(modulesExternalManifests, Formatting.Indented));
            GitTasks.Git($"commit -am \"{manifest.Id} {manifest.Version}-{manifest.VersionTag}\"", modulesLocalDirectory);

            GitTasks.Git($"push origin HEAD:master -f", modulesLocalDirectory);
        });


    Target SwaggerValidation => _ => _
     .DependsOn(Publish)
     .Requires(() => !IsModule)
     .Executes(() =>
     {
         //dotnet %userprofile%\.nuget\packages\swashbuckle.aspnetcore.cli\4.0.1\lib\netcoreapp2.0\dotnet-swagger.dll tofile --output swagger.json bin/Debug/netcoreapp2.2/VirtoCommerce.Platform.Web.dll VirtoCommerce.Platform
         DotNet($"{ArtifactsDirectory}/bin/dotnet-swagger.dll _tofile --output {ArtifactsDirectory}/swagger.json  {ArtifactsDirectory}/bin/{WebProject.Name}.dll VirtoCommerce.Platform");

         NpmTasks.NpmRun(s => s.SetWorkingDirectory(ArtifactsDirectory).SetCommand("swagger-cli").SetArguments($"validate { (IsLocalBuild ? "-d" : "")} { ArtifactsDirectory}\\swagger.json")); 
     });

}

