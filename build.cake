#tool "nuget:?package=xunit.runner.console"
#addin "nuget:?package=Cake.SqlServer"


var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var buildDir = Directory("./src/NSaga/bin") + Directory(configuration);
var solution = "./src/NSaga.sln";


Task("Clean")
    .Does(() =>
    {
        // if built locally, VS locks Samples.vshost.exe file - prevents this tak from failing
        Func<IFileSystemInfo, bool> exclude_vshost = fileSystemInfo => fileSystemInfo.Path.FullPath.EndsWith(".vshost.exe", StringComparison.OrdinalIgnoreCase);

        CleanDirectories("./src/**/bin/"+configuration, exclude_vshost);
    });


Task("Restore-NuGet-Packages")
    .IsDependentOn("Clean")
    .Does(() =>
    {
        Information("Starting Nuget Restore");

        NuGetRestore(solution);
    });


Task("Build")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() =>
    {
        MSBuild(solution, settings =>
            settings.SetConfiguration(configuration));
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
        XUnit2("./src/**/bin/" + configuration + "/Tests.dll");
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

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Run-Unit-Tests");
    

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
