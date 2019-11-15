using System.Data;
using System.Data.Common;

namespace ElleRealTimeBaseDAO.Interfaces
{
    public interface ITransactions
    {
        DbTransaction BeginTransaction();
        DbTransaction BeginTransaction(IsolationLevel level);
        void CommitTransaction(ref DbTransaction trans);
        void RollbackTransaction(ref DbTransaction trans);
    }

}
