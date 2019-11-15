using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using ElleRealTime.Shared.DBEntities.Accounts;
using MagicOnion;

namespace ElleRealTimeBaseDAO.Base
{
    public static class Login
    {
        public static int CheckLogin(ElleRealTimeDbDAO dao, string username, string hashedPassword, DbTransaction trans)
        {
            Hashtable prms = new Hashtable
            {
                { $"@{nameof(Account.Username)}", username },
                { $"@{nameof(Account.Password)}", hashedPassword }
            };

            return dao.ExecuteScalar<int>("SELECT ID " +
                                          "FROM accounts " +
                                          $"WHERE Username = @{nameof(Account.Username)} AND Password = @{nameof(Account.Password)}", prms, trans);
        }

        public static void ModifyPassword(ElleRealTimeDbDAO dao, string username, string hashedPassword, DbTransaction trans)
        {
            Hashtable prms = new Hashtable
            {
                { $"@{nameof(Account.Username)}", username },
                { $"@{nameof(Account.Password)}", hashedPassword }
            };
            dao.ExecuteNonQuery("UPDATE accounts " +
                                $"SET Password = @{nameof(Account.Password)} " +
                                $"WHERE LOWER(Username) = LOWER(@{nameof(Account.Username)})", prms, trans);
        }
    }
}
