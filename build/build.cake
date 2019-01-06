#tool "nuget:?package=GitVersion.CommandLine"
#load "./parameters.cake"
#tool "nuget:?package=OpenCover"
#tool "nuget:?package=ReportGenerator"



using System.Text.RegularExpressions;

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
    EnsureDirectoryExists(parameters.Paths.Directories.TestResultsDir);
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
		Verbosity = DotNetCoreVerbosity.Minimal,     
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

Task("Tests-With_Coverage")
    .Does(() =>
{
	Action<ICakeContext> testAction = context =>
  	{
		foreach(var testDir in parameters.Paths.Directories.TestDirs)
    	{
	 		var projects = GetFiles($"{testDir}/**/*.Test.csproj");
        	foreach(var project in projects)
        	{
				var vSTestReportPath  = $"{parameters.Paths.Directories.TestResultsDir.CombineWithFilePath(project.GetFilenameWithoutExtension()).FullPath}.xml";
				var settings = new DotNetCoreTestSettings
     				{
	    				Configuration = parameters.Configuration,
						OutputDirectory = parameters.Paths.Directories.TestResultsDir,
						ResultsDirectory = parameters.Paths.Directories.TestResultsDir,
						Verbosity = DotNetCoreVerbosity.Minimal,
						VSTestReportPath  = vSTestReportPath,
     				};
 					context.DotNetCoreTest(project.FullPath, settings);
        	}
		}
	 };
 
		
 	    OpenCover(tool => testAction(tool),
                        parameters.Paths.Files.TestCoverageOutput,
                        new OpenCoverSettings()
                        {
                            ReturnTargetCodeOffset = 0,
                            OldStyle = true,
                            MergeOutput = true
                        }
                        .WithFilter($"+[{parameters.AppInfo.AppName}]*")
                        .WithFilter($"-[*.Test]*")
						.ExcludeByAttribute("*.ExcludeFromCodeCoverage*")
                        .ExcludeByFile("*.Designer.cs;*.g.cs;*.g.i.cs"));
      ReportGenerator(parameters.Paths.Files.TestCoverageOutput, parameters.Paths.Directories.TestResultsCoverReportDir);
});
Task("Create-NuGet-Packages")
    .Does(() =>
    {                
		var basePath = parameters.Paths.Directories.SrcRootDir.Combine(parameters.AppInfo.AppName)
							.Combine("bin").
							Combine(parameters.Configuration).
							Combine(parameters.AppInfo.TargetFrameworkFull);
		var fileSource = basePath.Combine($"{parameters.AppInfo.AppName}.dll");
		var outputDirectory = parameters.Paths.Directories.NugetRootDir;
		var nuspecDirectory = parameters.Paths.Directories.NuspecRootDir;
		Information(fileSource.ToString());
		Information(basePath.ToString());
		Information(outputDirectory.ToString());

  		var nuGetPackSettings   = new NuGetPackSettings {
                                     Id                      = "AChehre.FluentlyHttpClient",
                                     Version                 = parameters.Version.SemVersion,
                                     Title                   = "Fluently HttpClient",
                                     Authors                 = new[] {"Ahmad Chehreghani, Stephen Lautier"},
                                     Owners                  = new[] {"Ahmad Chehreghani, Stephen Lautier"},
                                     Description             = "Fluent Http Client with a fluent APIs which are intuitive, easy to use and also highly extensible.",
                                     Summary                 = "Fluent Http Client with a fluent APIs which are intuitive, easy to use and also highly extensible.",
                                     ProjectUrl              = new Uri("https://github.com/AChehre/FluentlyHttpClient"),
                                     LicenseUrl              = new Uri("https://github.com/AChehre/FluentlyHttpClient/blob/master/LICENSE.md"),
                                     Tags                    = new [] {"httpclient","fluentapi","fluenthttp","graphql","graphqlclient"},
                                     RequireLicenseAcceptance= false,
                                     Symbols                 = false,
                                     NoPackageAnalysis       = true,
                                     Files                   = new [] {
                                                                          new NuSpecContent 
																		  {
																			  Source = fileSource.ToString(), 
																			  Target = $@"lib\{parameters.AppInfo.TargetFrameworkFull}"
																		  },
                                                                       },
                                     BasePath                = basePath.ToString(),
                                     OutputDirectory         = outputDirectory.ToString()
                                 };

    //  var nuspecFiles = GetFiles($"{nuspecDirectory.ToString()}/*.nuspec");
     NuGetPack(nuGetPackSettings);

    });
 Task("Push-Nuget-Packages")
.Does(() =>
{
	var nugetFiles = System.IO.Directory.GetFiles(parameters.Paths.Directories.NugetRootDir.ToString(), "*.nupkg")
										.Select(z => new FilePath(z)).ToList();
	var settings = new NuGetPushSettings()
	{
		Source = parameters.NuGet.ApiUrl,
		ApiKey = parameters.NuGet.ApiKey,
	};

	NuGetPush(nugetFiles, settings);
});
Task("Default")
	//.IsDependentOn("Clean")
	//.IsDependentOn("Restore")
	//.IsDependentOn("Build")
	//.IsDependentOn("Tests-With_Coverage")
	.IsDependentOn("Create-NuGet-Packages");
	//.IsDependentOn("Push-Nuget-Packages");

RunTarget(parameters.Target);

