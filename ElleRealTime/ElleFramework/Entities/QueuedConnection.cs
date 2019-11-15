using System;
using System.Data.Common;

namespace ElleFramework.Database.Entities
{
    class QueuedConnection
    {
        public DbConnection Connection { get; set; }
        public DateTime			LastUsedTime { get; set; }
    }
}
