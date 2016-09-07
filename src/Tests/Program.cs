using System;
using Tests.SqlServer;

namespace Tests
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            Console.WriteLine("Wiping and restoring database");

            var databaseRestorer = new DatabaseSetup();
            databaseRestorer.SetUpDatabase();

            Console.WriteLine("Restoring database complete");

            return 0;
        }
    }
}
