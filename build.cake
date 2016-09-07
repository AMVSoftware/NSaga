#tool "nuget:?package=xunit.runner.console"
#r "tools/Cake.SqlServer.dll"

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

// Define directories.
var buildDir = Directory("./src/NSaga/bin") + Directory(configuration);
var solution = "./src/NSaga.sln";

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

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


Task("Run-Unit-Tests")
    .IsDependentOn("Build")
    .IsDependentOn("Create-DB-And-Schema")
    .Does(() =>
    {
        XUnit2("./src/**/bin/" + configuration + "/Tests.dll");
    });



Task("Start-LocalDB")
    .Description(@"Starts LocalDB - executes the following: C:\Program Files\Microsoft SQL Server\120\Tools\Binn\SqlLocalDB.exe create v12.0 12.0 -s")
    .Does(() => 
    {
        // var sqlLocalDbPath = @"c:\Program Files\Microsoft SQL Server\130\Tools\Binn\SqlLocalDB.exe";
        var sqlLocalDbPath = @"C:\Program Files\Microsoft SQL Server\120\Tools\Binn\SqlLocalDB.exe";
        if(!FileExists(sqlLocalDbPath))
        {
            Information("Unable to start LocalDB");
            throw new Exception("LocalDB v12 is not installed. Can't complete tests");
        }

        StartProcess(sqlLocalDbPath, new ProcessSettings(){ Arguments="create \"v12.0\" 12.0 -s" });
    });


Task("Create-DB-And-Schema")
    .IsDependentOn("Start-LocalDB")
    .Description("Creates database and installs schema")
    .Does(() => 
    {
        var masterConnectionString = @"data source=(LocalDb)\v12.0;";
        var dbConnectionString = @"data source=(LocalDb)\v12.0;Database=NSaga-Testing";

        DropAndCreateDatabase(masterConnectionString, "NSaga-Testing");
        ExecuteSqlFile(dbConnectionString, "./src/NSaga.SqlServer/Install.sql");
    });


Task("SqlExpress")
    .Description("Create NSaga database on SqlExpress for local execution")
    .Does(() => 
    {
        var masterConnectionString = @"data source=.\SQLEXPRESS;integrated security=SSPI;";
        var dbConnectionString = @"data source=.\SQLEXPRESS;integrated security=SSPI;Initial Catalog=NSaga;";

        CreateDatabaseIfNotExist(masterConnectionString, "NSaga");
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
