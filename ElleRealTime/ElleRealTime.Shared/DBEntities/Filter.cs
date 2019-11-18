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
        /// Concat operator for string concatenation
        /// </summary>
        public string ConcatOperator { get; set; }
        /// <summary>
        /// FROM clause required for SELECTs that doesn't use a true table.
        /// (ex. "FROM DUAL" for Oracle)
        /// </summary>
        public string DummyFrom { get; set; }
        /// <summary>
        /// Schema in which you can find user function. Required because SqlServer always want the schema name for user functions.
        /// Required because SQL Server always wants schema name for scalar functions. 
        /// </summary>
        public string FunctionSchema { get; set; }

        protected Filter()
        {
            FirstRow = 1;
            RowNumber = int.MaxValue;
            ConcatOperator = "+"; // SQL Server - default.
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
