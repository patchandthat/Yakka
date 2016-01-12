using System;
using System.Data;
using System.Data.Entity;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using Dapper;
using Yakka.DataModels;

namespace Yakka.DataLayer
{
    class SqliteDb : IYakkaDb
    {
        private const string ConnectionStringFormat = "Data Source={0};Version=3;";
        private static string DatabaseFile => Path.Combine(Environment.CurrentDirectory, "YakkaData.sqlite");

        private static IDbConnection GetConnection()
        {
            return new SQLiteConnection(string.Format(ConnectionStringFormat, DatabaseFile));
        }
        
        public void SaveSettings(YakkaSettings settings)
        {
            if (!File.Exists(DatabaseFile))
            {
                CreateDatabase();
            }

            using (IDbConnection connection = GetConnection())
            {
                connection.Open();
                connection.Execute(
                    @"INSERT INTO Settings
                    (FIELDS)
                    VALUES (@params)",
                    settings); //where param names match property names
            }
        }

        private void CreateDatabase()
        {
            SQLiteConnection.CreateFile(DatabaseFile);

            using (IDbConnection connection = GetConnection())
            {
                connection.Open();
                connection.Execute(
                    @"create table Settings
                    (
                        ID  integer identity primary key AUTOINCREMENT,
                        ServerAddress ,
                        ServerPort ,
                        Username ,
                        RememberSettings ,
                        ConnectAutomatically ,
                        LaunchOnStartup 
                    )"); //etc
            }
        }

        public YakkaSettings LoadSettings()
        {
            using (IDbConnection connection = GetConnection())
            {
                connection.Open();
                return connection.Query<YakkaSettings>("SELECT --etc").FirstOrDefault();
            }
        }
    }
}
