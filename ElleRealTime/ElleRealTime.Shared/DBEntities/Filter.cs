using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using ElleFramework.Database;

namespace ElleRealTime.Shared.DBEntities
{
    public abstract class Filter
    {
        public enum SortDirections
        {
            Ascending,
            Descending
        };

        public int FirstRow { get; set; }
        public int RowNumber { get; set; }
        public string SortCol { get; set; }
        public SortDirections SortDir { get; set; }
        //public Utente CurrentUser { get; set; }

        /// <summary>
        /// L'operatore per la concatenazione di stringhe voluto dal DB
        /// </summary>
        public string ConcatOperator { get; set; }
        /// <summary>
        /// La clausola FROM richiesta per le SELECT che non vanno su una tabella vera
        /// (es. "FROM DUAL" per Oracle)
        /// </summary>
        public string DummyFrom { get; set; }
        /// <summary>
        /// Lo schema in cui si trovano le funzioni utente. Necessario perché SQL Server vuole sempre
        /// il nome dello schema esplicitato per le funzioni scalari (sigh).
        /// 
        /// Include il punto finale per un piů semplice utilizzo.
        /// </summary>
        public string FunctionSchema { get; set; }

        protected Filter()
        {
            FirstRow = 1;
            RowNumber = int.MaxValue;
            ConcatOperator = "+"; // SQL Server, per default
            FunctionSchema = "dbo.";
            DummyFrom = "";
        }

        [XmlIgnore]
        public int LastRow => FirstRow + RowNumber - 1;

        public abstract string WhereCondition(Hashtable prms);
        public abstract string OrderByCondition();

        protected static string MakeWhereCondition(List<string> conditions)
        {
            return ElleDAOUtils.MakeWhereCondition(conditions);
        }

        protected string OrderByCondition(Hashtable htDecodeColNames, string defaultOrderBy)
        {
            string ret = " ";

            if (!string.IsNullOrWhiteSpace(SortCol) && htDecodeColNames.Contains(SortCol))
            {

                ret += htDecodeColNames[SortCol];

                if (SortDir == SortDirections.Descending)
                {
                    Regex regex = new Regex("(?<!\\([^\\)]*),");
                    string[] fields = regex.Split(ret);

                    ret = string.Join(" DESC, ", fields) + " DESC ";
                }

            }
            else
                ret += defaultOrderBy;

            return $" ORDER BY {ret} ";
        }

        protected string GetThreeStatesFilter(string column, ThreeStatesFilter filter, Hashtable prms)
        {
            string ret;

            switch (filter)
            {

                case ThreeStatesFilter.OnlyFalse:
                    ret = column + " = @False";
                    prms["@False"] = false;
                    break;

                case ThreeStatesFilter.OnlyTrue:
                    ret = column + " = @True";
                    prms["@True"] = true;
                    break;

                default:
                    ret = "";
                    break;
            }

            return ret;
        }

        protected void AddThreeStatesFilter(List<string> conds, string colonna, ThreeStatesFilter filter, Hashtable prms)
        {
            if (filter != ThreeStatesFilter.All)
                conds.Add(GetThreeStatesFilter(colonna, filter, prms));
        }
    }
}
