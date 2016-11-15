#tool "nuget:?package=xunit.runner.console"
#tool "nuget:?package=GitVersion.CommandLine"
#addin "nuget:?package=Cake.SqlServer"
#load "./parameters.cake"

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
// var buildDir = Directory("./src/NSaga/bin") + Directory(configuration);
// var solution = "./src/NSaga.sln";


BuildParameters parameters = BuildParameters.GetParameters(Context);

Setup(context =>
{
    parameters.Initialize(context);

    Information("SemVersion: {0}", parameters.SemVersion);
    Information("Version: {0}", parameters.Version);
    Information("Building from branch: " + AppVeyor.Environment.Repository.Branch);
});


Task("debug")
    .Does(() => {
        Information("debug");
    });


Task("Clean")
    .Does(() =>
    {
        // if built locally, VS locks Samples.vshost.exe file - prevents this tak from failing
        Func<IFileSystemInfo, bool> exclude_vshost = fileSystemInfo => fileSystemInfo.Path.FullPath.EndsWith(".vshost.exe", StringComparison.OrdinalIgnoreCase);

        CleanDirectories("./src/**/bin/"+configuration, exclude_vshost);
        CleanDirectories("./src/**/obj/"+configuration, exclude_vshost);

        // CleanDirectories(new DirectoryPath[]{
        //     parameters.BuildDir,
        //     parameters.BuildResultDir,
        // });

    });


Task("Restore-NuGet-Packages")
    .IsDependentOn("Clean")
    .Does(() =>
    {
        Information("Starting Nuget Restore");

        NuGetRestore(parameters.Solution);
    });


Task("Build")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() =>
    {
        MSBuild(parameters.Solution, settings =>
            settings.SetPlatformTarget(PlatformTarget.MSIL)
                    .WithTarget("Build")
                    .SetConfiguration(configuration));
    });

Task("Create-DB-And-Schema")
    .Description("Creates database and installs schema")
    .Does(() => 
    {
        LocalDbCreateInstance("v12.0", LocalDbVersion.V12);

        var masterConnectionString = @"data source=(LocalDb)\v12.0;";
        var dbConnectionString = @"data source=(LocalDb)\v12.0;Database=NSaga-Testing";

        DropAndCreateDatabase(masterConnectionString, "NSaga-Testing");
        ExecuteSqlFile(dbConnectionString, "./src/NSaga/SqlServer/Install.sql");
    });



Task("Run-Unit-Tests")
    .IsDependentOn("Build")
    .IsDependentOn("Create-DB-And-Schema")
    .Does(() =>
    {
        XUnit2("./src/Tests/bin/" + configuration + "/Tests.dll");
    });



Task("Copy-Files")
    .IsDependentOn("Run-Unit-Tests")
    .Does(() =>
	{
		// EnsureDirectoryExists(parameters.ResultBinDir);
		//EnsureDirectoryExists(BuildParameters.IntegrationTestsFolder);

		// CopyFileToDirectory(parameters.BuildDir + "/NSaga.dll", parameters.ResultBinDir);
		// CopyFileToDirectory(parameters.BuildDir + "/NSaga.pdb", parameters.ResultBinDir);
		// CopyFileToDirectory(parameters.BuildDir + "/NSaga.xml", parameters.ResultBinDir);
		// CopyFiles(new FilePath[] { "LICENSE", "README.md", "ReleaseNotes.md" }, parameters.ResultBinDir);


		// CopyFileToDirectory(parameters.BuildDir + "/NSaga.dll", BuildParameters.IntegrationTestsFolder);
	});



Task("SqlExpress")
    .Description("Create NSaga database on SqlExpress for local execution")
    .Does(() => 
    {
        var masterConnectionString = @"data source=.\SQLEXPRESS;integrated security=SSPI;";
        var dbConnectionString = @"data source=.\SQLEXPRESS;integrated security=SSPI;Initial Catalog=NSaga;";

        CreateDatabaseIfNotExists(masterConnectionString, "NSaga");
        ExecuteSqlFile(dbConnectionString, "./src/NSaga.SqlServer/Install.sql");
        
        Information("Created SQL Schema for NSaga database");
    });

Task("Default")
    .IsDependentOn("Run-Unit-Tests");
    
RunTarget(target);
