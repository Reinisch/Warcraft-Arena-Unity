using System;
using System.Collections.Generic;
using System.Text;
using ElleFramework.Database;
using ElleFramework.Utils.Utility;

namespace ElleRealTime.Shared.BO
{
    public class DB
    {
        public readonly ElleBaseDAO EllAppDB;
        public bool Connected { get; set; }

        public string ConnectionString { get; }
        public string ProviderName { get; }

        public DB(DatabaseType dbtype, Config conf)
        {
            switch (dbtype)
            {
                case DatabaseType.MySql:
                    ConnectionString = GetMySqlConnectionString(conf.Database);
                    ProviderName = conf.Database.ProviderName;

                    break;
                case DatabaseType.MSSql:
                    ConnectionString = GetSqlServerConnectionString(conf.Database);
                    ProviderName = conf.Database.ProviderName;
                    break;
            }


        }

        public static string GetMySqlConnectionString(Database conf)
        {
            return $"server={conf.Host};uid={conf.User};pwd={conf.Password};database={conf.Name};SslMode=none;AllowZeroDateTime=True;Convert Zero Datetime=True;Allow User Variables=True;";
        }

        public static string GetSqlServerConnectionString(Database conf)
        {
            return $"server={conf.Host};database={conf.Name};uid={conf.User};pwd={conf.Password}";
        }

    }

}
