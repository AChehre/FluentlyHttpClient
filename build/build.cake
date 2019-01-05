#tool "nuget:?package=GitVersion.CommandLine"
#load "./parameters.cake"
// #tool "nuget:?package=xunit.runner.console"
// #addin "NuGet.Core"
// #addin nuget:?package=System.Threading.Tasks.Dataflow&version=4.5.24
// #r "References/CSProjectHelpers.dll"
// #r "References/Microsoft.Build.dll"

///////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////
BuildParameters parameters = BuildParameters.GetParameters(Context);

///////////////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
///////////////////////////////////////////////////////////////////////////////

Setup(ctx =>
{
   	parameters = BuildParameters.GetParameters(Context);
   	Information("AppInfo========================================");
	Information(parameters.AppInfo.ToString());
	Information("Nuget========================================");
	Information(parameters.NuGet.ToString());
	Information("GitHub========================================");
	Information(parameters.GitHub.ToString());		 	


	Information("Parameters========================================");
   	Information($"SemVersion: {parameters.Version.SemVersion}");
   	Information($"IsLocalBuild: {parameters.IsLocalBuild}");    
   	Information($"IsTagged: {parameters.IsTagged}");
   	Information($"IsPullRequest: {parameters.IsPullRequest}");
   	Information($"Target: {parameters.Target}");   
	
   Information("Running tasks...");
});

Teardown(ctx =>
{
   // Executed AFTER the last task.
   Information("Finished running tasks.");
});


///////////////////////////////////////////////////////////////////////////////
// TASK DEFINITIONS
///////////////////////////////////////////////////////////////////////////////
private void CleanProjects(string projectKind, IEnumerable<string> projectNames)
{
    foreach(var project in projectNames)
    {
        CleanDirectories($"./{projectKind}/{project}/bin/**");
        CleanDirectories($"./{projectKind}/{project}/obj/**");
    }
}

Task("Clean")
	.Does(() =>
{
	var srcRootDir = parameters.Paths.Directories.SrcRootDir;
	var testsRootDir = parameters.Paths.Directories.TestsRootDir;
	var specsRootDir = parameters.Paths.Directories.SpecsRootDir;
	var configuration = parameters.Configuration;
	// Clean solution directories.
	Information("Cleaning source folder {0}", srcRootDir);
	CleanDirectories($"{srcRootDir}/**/bin/{configuration}");
	CleanDirectories($"{srcRootDir}/**/obj/{configuration}");

	
	Information("Cleaning test folders {0}", testsRootDir);
	CleanDirectories($"{testsRootDir}/**/bin/{configuration}");
	CleanDirectories($"{testsRootDir}/**/obj/{configuration}");

	Information("Cleaning specs folders {0}", specsRootDir);
	CleanDirectories($"{specsRootDir}/**/bin/{configuration}");
	CleanDirectories($"{specsRootDir}/**/obj/{configuration}");


	CleanDirectories(parameters.Paths.Directories.ToClean);        

    EnsureDirectoryExists(parameters.Paths.Directories.ArtifactsDir);
    EnsureDirectoryExists(parameters.Paths.Directories.TestResults);
    EnsureDirectoryExists(parameters.Paths.Directories.NugetRootDir);


	// var outputDir = parameters.Paths.Directories.Artifacts;
	// // Clean previous artifacts
	// Information("Cleaning {0}", outputDir);
	// if (!DirectoryExists(outputDir)) CreateDirectory(outputDir);
	// if (DirectoryExists(publishDir)) CleanDirectories(MakeAbsolute(Directory(publishDir)).FullPath);
});

Task("Restore")
	.Does(() =>
{
	string solution = parameters.Paths.Files.Solution.ToString();
	string nugetServerUrl = parameters.NuGet.ApiUrl;
	Information("Restoring solution: {0}", solution);
	Information("Restoring NuGet url: {0}", nugetServerUrl);
	DotNetCoreRestore(solution, new DotNetCoreRestoreSettings
	{
		Sources = new [] {nugetServerUrl}
	});
});

Task("Build")
	.Does(()=>
{
	string solution = parameters.Paths.Files.Solution.ToString();
	DotNetBuild(solution, settings => new DotNetCoreBuildSettings
	{
		Configuration = parameters.Configuration,
		NoRestore = true,
		ArgumentCustomization = args => args.Append($"/p:SemVer={parameters.Version.SemVersion}")
	});
});


Task("Default")
	.IsDependentOn("Clean")
	.IsDependentOn("Restore")
	.IsDependentOn("Build");

RunTarget(parameters.Target);

