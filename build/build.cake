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

Task("Clean")
	.Does(() =>
{
	var sourceFolder = parameters.Paths.Directories.SrcRootDir;
	var configuration = parameters.Configuration;
	// Clean solution directories.
	Information("Cleaning {0}", sourceFolder);
	CleanDirectories($"{sourceFolder}/**/bin/{configuration}");
	CleanDirectories($"{sourceFolder}/**/obj/{configuration}");

	// var outputDir = parameters.Paths.Directories.Artifacts;
	// // Clean previous artifacts
	// Information("Cleaning {0}", outputDir);
	// if (!DirectoryExists(outputDir)) CreateDirectory(outputDir);
	// if (DirectoryExists(publishDir)) CleanDirectories(MakeAbsolute(Directory(publishDir)).FullPath);
});

Task("Restore-NuGet-Packages")
	.IsDependentOn("Clean")
	.Does(() =>
{
	string sourceFolder = parameters.Paths.Files.Solution.ToString();
	string nugetServerUrl = parameters.NuGet.ApiUrl;
	Information("Restoring directory: {0}", sourceFolder);
	Information("Restoring NuGet url: {0}", nugetServerUrl);
	DotNetCoreRestore(sourceFolder, new DotNetCoreRestoreSettings
	{
		Sources = new [] {nugetServerUrl}
	});
});


Task("Default")
	.IsDependentOn("Clean")
	.IsDependentOn("Restore-NuGet-Packages");

RunTarget(parameters.Target);