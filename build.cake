#tool "nuget:?package=xunit.runner.console"

using System;
using System.Data.SqlClient;

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
        //c:\Program Files\Microsoft SQL Server\120\Tools\Binn\
        //c:\Program Files\Microsoft SQL Server\130\Tools\Binn\
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
        // // var masterConnectionString = "Server=(localdb)\v12.0;";
        // var masterConnectionString = @"data source=.\SQLEXPRESS;integrated security=SSPI;Initial Catalog=master;MultipleActiveResultSets=True";
        // using (var masterConnection = new SqlConnection(masterConnectionString))
        // {
        //     Information("Before opening connection");
        //     masterConnection.Open();

        //     var command = new SqlCommand("Create database [NSaga-Testing]", masterConnection);

        //     command.ExecuteNonQuery();
        // }
        // var pathToSqlCmd = @"c:\Program Files (x86)\Microsoft SQL Server\Client SDK\ODBC\130\Tools\Binn\sqlcmd.exe";
        // var pathToSqlCmd = @"c:\Program Files\Microsoft SQL Server\Client SDK\ODBC\110\Tools\Binn\sqlcmd.exe";
        // StartProcess(pathToSqlCmd, new ProcessSettings() { Arguments = @"-S ""Server=(localdb)\v12.0;"" -i ./src/Tests/SqlServer/CreateDatabase.sql" });
        // ExecuteSqlQuery("create database [NSaga-Testing]", new SqlQuerySettings()
        // {
        //     Provider = "MsSql",
        //     ConnectionString = "Server=(localdb)\v12.0;"
        // });

        // ExecuteSqlFile("./src/Tests/SqlServer/CreateDatabase.sql", new SqlQuerySettings()
        // {
        //     Provider = "MsSql",
        //     ConnectionString = "Server=(localdb)\v12.0;"
        // });
        // Information("Created NSaga-Testing database");

        // ExecuteSqlFile("./src/NSaga.SqlServer/Install.sql", new SqlQuerySettings()
        // {
        //     Provider = "MsSql",
        //     ConnectionString = "Server=(localdb)\v12.0;Database=NSaga-Testing"
        // });
        // Information("Created SQL Schema for NSaga");
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
