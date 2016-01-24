using System;
using System.Data;
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
                    ([ServerAddress]
                    ,[ServerPort]
                    ,[Username]
                    ,[RememberSettings]
                    ,[ConnectAutomatically]
                    ,[LaunchOnStartup])
                    VALUES 
                    (@ServerAddress
                    ,@ServerPort
                    ,@Username
                    ,@RememberSettings
                    ,@ConnectAutomatically
                    ,@LaunchOnStartup)",
                    settings);
            }
        }

        private void CreateDatabase()
        {
            SQLiteConnection.CreateFile(DatabaseFile);

            using (IDbConnection connection = GetConnection())
            {
                connection.Open();
                connection.Execute(
                    @"CREATE TABLE Settings
                    (
                        [ID] INTEGER PRIMARY KEY AUTOINCREMENT,
                        [ServerAddress] NVARCHAR(100) NOT NULL,
                        [ServerPort] INT NOT NULL,
                        [Username] NVARCHAR(100) NOT NULL,
                        [RememberSettings] INT NOT NULL,
                        [ConnectAutomatically] INT NOT NULL,
                        [LaunchOnStartup] INT NOT NULL
                    )");
            }
        }

        public YakkaSettings LoadSettings()
        {
            if (!File.Exists(DatabaseFile))
            {
                return new YakkaSettings();
            }

            using (IDbConnection connection = GetConnection())
            {
                connection.Open();
                return connection.Query<YakkaSettings>(
                    @"SELECT
                        [ID],
                        [ServerAddress],
                        [ServerPort],
                        [Username],
                        [RememberSettings],
                        [ConnectAutomatically],
                        [LaunchOnStartup]
                    FROM [Settings]
                    ORDER BY [ID] DESC")
                    .FirstOrDefault();
            }
        }
    }
}
