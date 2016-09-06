using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Xunit;

namespace Tests.SqlServer
{
    public class DatabaseSetup
    {
        [Fact(Skip = "Not a test")]
        //[Fact]
        public void SetUpDatabase()
        {
            var masterConnectionString = ConfigurationManager.ConnectionStrings["MasterConnectionString"].ConnectionString;
            var connectionString = ConfigurationManager.ConnectionStrings["TestingConnectionString"].ConnectionString;

            ExecuteSqlFile(masterConnectionString, "CreateDatabase.sql");
            ExecuteSqlFile(connectionString, "Install.sql");
        }


        private static void ExecuteSqlFile(String connectionString, string filename)
        {
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var sqlFile = Directory.GetFiles(baseDirectory, filename, SearchOption.AllDirectories).FirstOrDefault();
            if (sqlFile == null)
            {
                Console.WriteLine("File {0} is not found in path {1}", filename, baseDirectory);
                return;
            }

            Console.WriteLine($"Executing sql file {sqlFile}");

            var allSqlCommands = File.ReadAllText(sqlFile);

            var commandStrings = Regex.Split(allSqlCommands, @"^\s*GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase);


            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                foreach (var sqlCommand in commandStrings)
                {
                    if (sqlCommand.Trim() != "")
                    {
                        var command = new SqlCommand(sqlCommand, connection);
                        command.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}
