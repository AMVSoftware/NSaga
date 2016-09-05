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
        //[Fact(Skip = "Not a test")]
        [Fact]
        public void SetUpDatabase()
        {
            var connectionString = ConfigurationManager.AppSettings["TestingConnectionString"];
            var masterConnectionString = ConfigurationManager.AppSettings["MasterConnectionString"];
            
            var dbName = "NSaga-Testing";

            ReallyDropDatabase(dbName, masterConnectionString);
            CreateDatabase(dbName, masterConnectionString);

            CreateSchema(connectionString);

            //SeedData(childConnectionString);
        }


        /// <summary>
        /// Drops the database that is specified in the connection string.
        /// 
        /// Drops the database even if the connection is open. Sql is stolen from here:
        /// http://daniel.wertheim.se/2012/12/02/entity-framework-really-do-drop-create-database-if-model-changes-and-db-is-in-use/
        /// </summary>
        private static void ReallyDropDatabase(String databaseToBeDroppedName, String masterConnectionString)
        {
            const string dropDatabaseSql =
            "if (select DB_ID('{0}')) is not null\r\n"
            + "begin\r\n"
            + "alter database [{0}] set offline with rollback immediate;\r\n"
            + "alter database [{0}] set online;\r\n"
            + "drop database [{0}];\r\n"
            + "end";

            using (var connection = new SqlConnection(masterConnectionString))
            {
                connection.Open();

                var sqlToExecute = String.Format(dropDatabaseSql, databaseToBeDroppedName);

                var command = new SqlCommand(sqlToExecute, connection);

                Console.WriteLine("Dropping database");
                command.ExecuteNonQuery();
                Console.WriteLine("Database is dropped");
            }
        }


        private static void CreateDatabase(string dbName, string masterConnectionString)
        {
            using (var masterConnection = new SqlConnection(masterConnectionString))
            {
                masterConnection.Open();

                var command = new SqlCommand($"Create database [{dbName}]", masterConnection);

                Console.WriteLine("Creating database {0}", dbName);
                command.ExecuteNonQuery();
                Console.WriteLine("Database is created {0}", dbName);
            }
        }



        private void CreateSchema(String connectionString)
        {
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
