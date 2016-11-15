#tool "nuget:?package=xunit.runner.console"
#tool "nuget:?package=GitVersion.CommandLine"
#addin "nuget:?package=Cake.SqlServer"
#load "./parameters.cake"

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
bool publishingError = false;

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
        CleanDirectories(parameters.Artefacts);
        CleanDirectories(parameters.ArtefactsBin);

        CleanDirectories(new DirectoryPath[]{
            Directory("./src/Tests/bin/"),
            Directory("./src/Tests/obj/"),
            Directory(parameters.Artefacts),
            Directory(parameters.NSagaBinDir),
            Directory(parameters.NSagaBinDir + "../../obj/"),
            Directory(parameters.AutofacBinDir),
            Directory(parameters.AutofacBinDir + "../../obj/"),
            Directory(parameters.SimpleInjectorBinDir),
            Directory(parameters.SimpleInjectorBinDir + "../../obj/"),
        });

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
		EnsureDirectoryExists(parameters.Artefacts);
		EnsureDirectoryExists(parameters.ArtefactsBin);

		CopyFiles(new FilePath[] { "LICENSE", "README.md", "ReleaseNotes.md" }, parameters.ArtefactsBin);

        CopyFileToDirectory(parameters.NSagaBinDir + "NSaga.dll", parameters.ArtefactsBin);
        CopyFileToDirectory(parameters.NSagaBinDir + "NSaga.pdb", parameters.ArtefactsBin);
        CopyFileToDirectory(parameters.NSagaBinDir + "NSaga.xml", parameters.ArtefactsBin);

        CopyFileToDirectory(parameters.AutofacBinDir + "NSaga.Autofac.dll", parameters.ArtefactsBin);
        CopyFileToDirectory(parameters.AutofacBinDir + "NSaga.Autofac.pdb", parameters.ArtefactsBin);
        CopyFileToDirectory(parameters.AutofacBinDir + "NSaga.Autofac.xml", parameters.ArtefactsBin);

        CopyFileToDirectory(parameters.SimpleInjectorBinDir + "NSaga.SimpleInjector.dll", parameters.ArtefactsBin);
        CopyFileToDirectory(parameters.SimpleInjectorBinDir + "NSaga.SimpleInjector.pdb", parameters.ArtefactsBin);
        CopyFileToDirectory(parameters.SimpleInjectorBinDir + "NSaga.SimpleInjector.xml", parameters.ArtefactsBin);
	});


Task("Package")
    .IsDependentOn("Copy-Files")
    .Does(() =>
	{
		var releaseNotes = ParseReleaseNotes("./ReleaseNotes.md");

        var settings = new NuGetPackSettings
		{
			Version = parameters.Version,
			ReleaseNotes = releaseNotes.Notes.ToArray(),
			BasePath = parameters.ArtefactsBin,
			OutputDirectory = parameters.Artefacts,
			Symbols = false,
			NoPackageAnalysis = true
		};

		NuGetPack("./src/NSaga/NSaga.nuspec", settings);
		NuGetPack("./src/NSaga.Autofac/NSaga.Autofac.nuspec", settings);
		NuGetPack("./src/NSaga.SimpleInjector/NSaga.SimpleInjector.nuspec", settings);
	});


Task("Publish-MyGet")
    .IsDependentOn("Package")
    .WithCriteria(() => parameters.ShouldPublishToMyGet)
    .Does(() =>
	{
		// Resolve the API key.
		var apiKey = EnvironmentVariable("MYGET_API_KEY");
		if(string.IsNullOrEmpty(apiKey)) {
			throw new InvalidOperationException("Could not resolve MyGet API key.");
		}

		// Resolve the API url.
		var apiUrl = EnvironmentVariable("MYGET_API_URL");
		if(string.IsNullOrEmpty(apiUrl)) {
			throw new InvalidOperationException("Could not resolve MyGet API url.");
		}

		// Push the Packages
        var files = GetFiles(parameters.Artefacts + "*.nupkg");
        foreach(var file in files)
        {
            Information("Found nupkg file: {0}", file);
            
            NuGetPush(file, new NuGetPushSettings {
                Source = apiUrl,
                ApiKey = apiKey
            });
        }

	})
	.OnError(exception =>
	{
		Information("Publish-MyGet Task failed, but continuing with next Task...");
		publishingError = true;
	});    


Task("Upload-AppVeyor-Artifacts")
    .IsDependentOn("Package")
    .WithCriteria(() => parameters.IsRunningOnAppVeyor)
    .Does(() =>
	{
        var files = GetFiles(parameters.Artefacts + "*.nupkg");
        foreach(var file in files)
        {
    		AppVeyor.UploadArtifact(file);
        }
        
	});

Task("AppVeyor")
    .IsDependentOn("Publish-MyGet")
    .IsDependentOn("Upload-AppVeyor-Artifacts")
    .Finally(() =>
    {
        if(publishingError)
        {
            throw new Exception("An error occurred during the publishing of Cake.  All publishing tasks have been attempted.");
        }
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
    .IsDependentOn("Package");
    
RunTarget(target);
