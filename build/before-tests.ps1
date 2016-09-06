# replace_appsettings.ps1
# http://kevsoft.net/2015/09/18/using-powershell-to-replace-appsettings.html

$configPath = "$($env:appveyor_build_folder)\Tests\bin\Release\Tests.dll.config"

Write-Output "Loading config file from $configPath"
$xml = [xml](Get-Content $configPath)

ForEach($add in $xml.configuration.connectionStrings.add)
{
    Write-Output "Processing ConnectionStrings key $($add.name)"

    $matchingEnvVar = [Environment]::GetEnvironmentVariable($add.name)

    if($matchingEnvVar)
    {
        Write-Output "Found matching environment variable for key: $($add.name)"
        Write-Output "Replacing value $($add.connectionString) with $matchingEnvVar"

        $add.connectionString = $matchingEnvVar
    }
}

$xml.Save($configPath)



$sqlInstance = "(local)\SQL2016"
$dbName = "NSaga-Testing"

# Execute SQL to create database
sqlcmd -S "$sqlInstance" -U "sa" -P "Password12!" -d "master" -i $env:appveyor_build_folder\Tests\bin\Release\SqlServer\CreateDatabase.sql

# execute SQL to create shema
sqlcmd -S "$sqlInstance" -U "sa" -P "Password12!" -d "$dbName" -i $env:appveyor_build_folder\Tests\bin\Release\SqlServer\Install.sql



#$startPath = "$($env:appveyor_build_folder)\path-to-your-bin"
#$sqlInstance = "(local)\SQL2012SP1"

## replace the db connection with the local instance
#$config = join-path $startPath "Tests.dll.config"
#$doc = (gc $config) -as [xml]
#$doc.SelectSingleNode('//connectionStrings/add[@name="store"]').connectionString = "Server=$sqlInstance; Database=$dbName; Trusted_connection=true"
#$doc.Save($config)
#
## attach mdf to local instance
#$mdfFile = join-path $startPath "store.mdf"
#$ldfFile = join-path $startPath "store_log.ldf"
#sqlcmd -S "$sqlInstance" -Q "Use [master]; CREATE DATABASE [$dbName] ON (FILENAME = '$mdfFile'),(FILENAME = '$ldfFile') for ATTACH"