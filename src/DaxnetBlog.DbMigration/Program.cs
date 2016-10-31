using DaxnetBlog.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.IO;
using System.Text.RegularExpressions;

namespace DaxnetBlog.DbMigration
{
    public class Program
    {
        public static void Main(string[] args)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                var processedFiles = GetProcessedFiles(connection).ToList();
                var transaction = connection.BeginTransaction();

                try
                {
                    var migrated = false;
                    foreach (var file in Directory.EnumerateFiles("scripts", "*.sql").Where(x => !processedFiles.Contains(x)).OrderBy(x => x))
                    {
                        migrated = true;
                        var script = File.ReadAllText(file);
                        DoMigration(file, script, connection, transaction);
                    }

                    if (!migrated)
                    {
                        Console.WriteLine("INFO: Nothing to be migrated.");
                    }

                    transaction.Commit();
                    Console.WriteLine("Migration Done!");
                    Environment.Exit(0);
                }
                catch(Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine(ex);
                    Environment.Exit(-1);
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        static void DoMigration (string file, string script, SqlConnection connection, SqlTransaction transaction)
        {
            Console.WriteLine($"Migrating {file}...");
            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                IEnumerable<string> commandStrings = Regex.Split(script, @"^\s*GO\s*$",
                           RegexOptions.Multiline | RegexOptions.IgnoreCase);

                foreach (var sql in commandStrings)
                {
                    if (!string.IsNullOrEmpty(sql))
                    {
                        command.CommandText = sql;
                        command.ExecuteNonQuery();
                    }
                }

                command.CommandText = "INSERT INTO [MigrationHistory] ([Name], [DateAndTime]) VALUES (@Name, @DateAndTime)";
                command.Parameters.Clear();
                command.Parameters.Add(new SqlParameter("@Name", file));
                command.Parameters.Add(new SqlParameter("@DateAndTime", DateTime.UtcNow));

                command.ExecuteNonQuery();
            }
        }

        static IEnumerable<string> GetProcessedFiles(SqlConnection connection)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"SELECT [Name] FROM [MigrationHistory]";
                command.CommandType = System.Data.CommandType.Text;
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return reader["Name"].ToString();
                    }
                }
            }
        }

        static string ConnectionString
        {
            get
            {
                const string DefaultConnectionString = @"Server=localhost; Database=DaxnetBlogDB; Integrated Security=SSPI;";
                var crypto = Crypto.Create(CryptoTypes.EncTypeTripleDes);
                var connectionString = Environment.GetEnvironmentVariable("DAXNETBLOG_SQL_STR");
                if (string.IsNullOrEmpty(connectionString))
                {
                    connectionString = DefaultConnectionString;
                }
                else
                {
                    try
                    {
                        var c = Crypto.Create(CryptoTypes.EncTypeTripleDes);
                        connectionString = c.Decrypt(connectionString, Crypto.GlobalKey);
                    }
                    catch
                    {
                        connectionString = DefaultConnectionString;
                    }
                }
                return connectionString;
            }
        }
    }
}
