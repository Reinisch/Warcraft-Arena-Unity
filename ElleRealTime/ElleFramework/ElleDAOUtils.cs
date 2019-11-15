using System;
using System.Collections;
using System.Collections.Generic;

namespace ElleFramework.Database
{
    public static class ElleDAOUtils
    {
        public static string MakeWhereCondition( List<string> conditions )
		{
			string[]	conds = conditions.ToArray();
			string		ret;

			ret = string.Join( " AND ", Array.ConvertAll( conds, c => "(" + c + ")" ));

			if( !string.IsNullOrEmpty( ret ))
				ret = " WHERE " + ret;

			return ret + " ";
		}

		public static string MergeConditions( string a, string b )
		{
			List<string>	l = new List<string>();

			a = a.Trim();
			b = b.Trim();

			if( a.StartsWith( "WHERE " ))
				a = a.Substring( 6 );

			if( b.StartsWith( "WHERE " ))
				b = b.Substring( 6 );

			if( !string.IsNullOrEmpty( a ))
				l.Add( a );

			if( !string.IsNullOrEmpty( b ))
				l.Add( b );
            
			return MakeWhereCondition( l );
		}

		public static string MakeInList( IEnumerable values, string prefix, Hashtable prms )
		{
			List<string>	keys = new List<string>();
			int				idx = 0;
			string			ret;

			foreach( object id in values ) {
				string key = prefix + idx++;

				keys.Add( key );

				prms[ key ] = id;
			}

			if( keys.Count > 0 )
				ret = string.Join( ", ", keys.ToArray() );
			else
				ret = "NULL";

			return ret;
		}


        public static string MakeIn( string field, IEnumerable values, string prefix, Hashtable prms )
		{
			return " " + field + " IN ( " + MakeInList( values, prefix, prms ) + " ) ";
		}
        
        public static string MakeNotIn( string field, IEnumerable values, string prefix, Hashtable prms )
		{
			return " " + field + " NOT IN ( " + MakeInList( values, prefix, prms ) + " ) ";
		}

		/// <summary>
		/// Generate the condition to verify the equality of a field, considering NULL values too.
		/// </summary>
		/// <param name="field">Target Field</param>
		/// <param name="paramName">Generic Parameter Name</param>
		/// <param name="value">Desired value</param>
		/// <param name="nullValue">value that corresponds to NULL</param>
		/// <param name="prms">hashtable object with parameters</param>
		/// <returns>string with condition (without spaces (trimmed))</returns>
		public static string IsEqualCondition( string field, string paramName, object value, object nullValue, Hashtable prms )
		{
			if( ElleBaseDAO.GetNullableValue( value, nullValue ) == DBNull.Value )
				field += " IS NULL";
			else {

				field += " = " + paramName;

				prms[ paramName ] = value;
			}

			return field;
		}
    }
}
