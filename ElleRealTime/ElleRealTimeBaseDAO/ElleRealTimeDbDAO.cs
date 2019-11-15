using System;
using System.Collections;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using ElleFramework.Database;
using ElleFramework.Utils;
using ElleRealTime.Shared.BO;

namespace ElleRealTimeBaseDAO
{
    public class ElleRealTimeDbDAO : ElleBaseDAO
    {
        public bool ForceTimeout { get; set; }
        public int CommandTimeout { get; set; }
        private static string ConnString = Utils.ElleRealTimeDB.ConnectionString;
        private static string ProviderName = ApplicationUtils.Configuration.Database.ProviderName;

        [NotNull]
        public virtual string ConcatOperator => "+";

        public ElleRealTimeDbDAO() : base(ProviderName, ConnString)
        {
        }

        public ElleRealTimeDbDAO(string connStringName) : base(connStringName)
        {
        }

        public ElleRealTimeDbDAO(string providerName, string connString) : base(ProviderName, ConnString)
        {
        }

        protected void SetCommandTimeout(DbCommand cmd)
        {
            if (ForceTimeout)
                cmd.CommandTimeout = CommandTimeout;
        }

        protected override DbCommand GetCommand(string query, Array prms)
        {
            DbCommand cmd = base.GetCommand(query, prms);
            SetCommandTimeout(cmd);
            return cmd;
        }

        protected override DbCommand GetCommand(string query)
        {
            DbCommand cmd = base.GetCommand(query);
            SetCommandTimeout(cmd);
            return cmd;
        }

        protected override DbCommand GetCommand(string query, Hashtable prms)
        {
            DbCommand cmd = base.GetCommand(query, prms);
            SetCommandTimeout(cmd);
            return cmd;
        }

    }

}
