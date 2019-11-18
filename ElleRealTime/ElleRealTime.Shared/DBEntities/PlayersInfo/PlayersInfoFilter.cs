using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace ElleRealTime.Shared.DBEntities.PlayersInfo
{
    public class PlayersInfoFilter : Filter
    {
        private static readonly Hashtable htDecodeColNames = new Hashtable();
        private const string DEFAULT_ORDERBY = "AccountID";

        public int AccountID { get; set; }

        static PlayersInfoFilter()
        {
            htDecodeColNames.Add("colAccountID", "AccountID");
        }

        public override string WhereCondition(Hashtable prms)
        {
            List<string> conds = new List<string>();

            if (AccountID > 0)
            {
                conds.Add("AccountID = @AccountID");
                prms["@AccountID"] = AccountID;
            }

            return MakeWhereCondition(conds);
        }

        public override string OrderByCondition()
        {
            return OrderByCondition(htDecodeColNames, DEFAULT_ORDERBY);
        }
    }
}
