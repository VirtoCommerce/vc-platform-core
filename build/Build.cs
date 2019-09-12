using System;
using System.Linq;
using Nuke.Common;
using Nuke.Common.Execution;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.Git;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Tools.Npm;
using Nuke.Common.Utilities;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

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

    Target Publish => _ => _
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

    Target WebPackBuild => _ => _
     .Executes(() =>
     {
         if (FileExists(WebProject.Directory / "packages.json"))
         {
             NpmTasks.NpmInstall(s => s.SetWorkingDirectory(WebProject.Directory));
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

            if (IsModule)
            {
                DotNetPublish(s => s
                     .SetWorkingDirectory(WebProject.Directory)
                     .EnableNoRestore()
                     .SetOutput(ModuleOutputDirectory / "bin")
                     .SetConfiguration(Configuration)
                     .SetAssemblyVersion(ModuleVersion)
                     .SetFileVersion(ModuleVersion)
                     .SetInformationalVersion(ModuleVersion));

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
            }
        });

    Target Compress => _ => _
     .DependsOn(Clean, Compile, Test, WebPackBuild)
     .Executes(() =>
     {
         var ignoredFiles = HttpTasks.HttpDownloadString(GlobalModuleIgnoreFileUrl).SplitLineBreaks();
         if (FileExists(ModuleIgnoreFile))
         {
             ignoredFiles = ignoredFiles.Concat(TextTasks.ReadAllLines(ModuleIgnoreFile)).ToArray();
         }
         ignoredFiles = ignoredFiles.Select(x => x.Trim()).Distinct().ToArray();

         var zipFileName = ArtifactsDirectory / ModuleId + "_" + ModuleVersion + ".zip";
         DeleteFile(zipFileName);
         CompressionTasks.CompressZip(ModuleOutputDirectory, zipFileName, (x) => !ignoredFiles.Contains(x.Name, StringComparer.OrdinalIgnoreCase));
     });

   
}

