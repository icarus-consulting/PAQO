#addin "Cake.Figlet"

var target = Argument("target", "Default");
var configuration   = "Release";

///////////////////////////////////////////////////////////////////////////////
// GLOBAL VARIABLES
///////////////////////////////////////////////////////////////////////////////
// We define where the build artifacts should be places
// this is relative to the project root folder
var buildArtifacts                  = Directory("./artifacts");
var deployment                      = Directory("./artifacts/deployment");
var version                         = "1.0.0"; // e.g. 0.1.0

///////////////////////////////////////////////////////////////////////////////
// MODULES
///////////////////////////////////////////////////////////////////////////////
var modules = Directory("./src");
// To skip building a project in the source folder add the project folder name
// as string to the list e.g. "Yaapii.SimEngine.Tmx.Setup".
var blacklistModules = new List<string>() { };

// Unit tests
var unitTests = Directory("./tests");
// To skip executing a test in the tests folder add the test project folder name
// as string to the list e.g.
var blacklistUnitTests = new List<string>() { };

///////////////////////////////////////////////////////////////////////////////
// CONFIGURATION VARIABLES
///////////////////////////////////////////////////////////////////////////////
var isAppVeyor                      = AppVeyor.IsRunningOnAppVeyor;
var isWindows                       = IsRunningOnWindows();

// For GitHub release
var owner                           = "icarus-consulting";
var repository                      = "PAQO";

// For AppVeyor NuGetFeed
var nuGetSource             = "https://api.nuget.org/v3/index.json";
var appVeyorNuGetFeed       = "https://ci.appveyor.com/nuget/icarus/api/v2/package";

///////////////////////////////////////////////////////////////////////////////
// Clean
///////////////////////////////////////////////////////////////////////////////
Task("Clean")
.Does(() =>
{
    Information(Figlet("Clean"));

	// Clean the artifacts folder to prevent old builds be present
	// https://cakebuild.net/dsl/directory-operations/
    CleanDirectories(new DirectoryPath[] { buildArtifacts });
});

///////////////////////////////////////////////////////////////////////////////
// Restore
///////////////////////////////////////////////////////////////////////////////
Task("Restore")
.Does(() =>
{
    Information(Figlet("Restore"));

	// Collect all csproj files recusive from the root directory
	// and run a nuget restore
    var projects = GetFiles("./**/*.csproj");
    foreach(var project in projects)
    {
        DotNetCoreRestore(project.GetDirectory().FullPath);
    }
});

///////////////////////////////////////////////////////////////////////////////
// Version
///////////////////////////////////////////////////////////////////////////////
Task("Version")
.WithCriteria(() => isAppVeyor && BuildSystem.AppVeyor.Environment.Repository.Tag.IsTag)
.Does(() =>
{
    Information(Figlet("Version"));

    version = BuildSystem.AppVeyor.Environment.Repository.Tag.Name;
    Information($"Set version to '{version}'");
});

///////////////////////////////////////////////////////////////////////////////
// Build
///////////////////////////////////////////////////////////////////////////////
Task("Build")
.IsDependentOn("Clean")
.IsDependentOn("Restore")
.IsDependentOn("Version")
.Does(() =>
{
    Information(Figlet("Build"));

	var settings =
		new DotNetCoreBuildSettings()
		{
			Configuration = configuration,
			NoRestore = true,
			MSBuildSettings = new DotNetCoreMSBuildSettings().SetVersionPrefix(version)
		};
	foreach(var module in GetSubDirectories(modules))
	{
		if(!blacklistModules.Contains(module.GetDirectoryName()))
		{
			Information($"Building {module.GetDirectoryName()}");

			DotNetCoreBuild(
				module.FullPath,
				settings
			);
		}
		else
		{
			Warning($"Skipping build {module.GetDirectoryName()}");
		}
	}
});

///////////////////////////////////////////////////////////////////////////////
// Test
///////////////////////////////////////////////////////////////////////////////
Task("Test")
.IsDependentOn("Build")
.Does(() =>
{
    Information(Figlet("Unit Test"));

	var settings =
		new DotNetCoreTestSettings()
		{
			Configuration = configuration,
			NoRestore = true
		};
	foreach(var test in GetSubDirectories(unitTests))
	{
		if(!blacklistUnitTests.Contains(test.GetDirectoryName()))
		{
			Information($"Testing {test.GetDirectoryName()}");
			DotNetCoreTest(
				test.FullPath,
				settings
			);
		}
		else
		{
			Warning($"Skipping test {test.GetDirectoryName()}");
		}
	}
});

///////////////////////////////////////////////////////////////////////////////
// Default
///////////////////////////////////////////////////////////////////////////////
Task("Default")
.IsDependentOn("Clean")
.IsDependentOn("Restore")
.IsDependentOn("Version")
.IsDependentOn("Build")
.IsDependentOn("Test")
;

RunTarget(target);