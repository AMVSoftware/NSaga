using System;


internal static class AzureTablesHelper
{
    public static string GetConnectionString()
    {
        return Environment.GetEnvironmentVariable("NSagaAzureTableStorage"); // "UseDevelopmentStorage=true";
    }
}