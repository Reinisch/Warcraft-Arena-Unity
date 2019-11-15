using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Reflection;
using System.Text.RegularExpressions;
using ElleFramework.Utils.CustomAttributes;
using ElleFramework.Utils.Serialization;

namespace ElleFramework.Database.MVC
{
    [Serializable]
	public class View
	{
		private static readonly Type	nullIfAttr = typeof( NullIfAttribute );
		private static readonly Type	ignoreAttr = typeof( HashtableIgnoreAttribute );

		protected static object ConvertType( object obj, Type type )
		{
			if(( obj != null ) && ( type != obj.GetType() )) {

				if( type.IsEnum ) {

					obj = ConvertEnum( obj, type );

				} else if( type.IsGenericType && 
// ReSharper disable PossibleNullReferenceException
// ReSharper disable CheckForReferenceEqualityInstead.1
						   type.GetGenericTypeDefinition().Equals( typeof( Nullable<> ))) {
// ReSharper restore CheckForReferenceEqualityInstead.1
// ReSharper restore PossibleNullReferenceException
					NullableConverter nullableConverter = new NullableConverter( type );

					if( nullableConverter.UnderlyingType.IsEnum )
						obj = Enum.ToObject( nullableConverter.UnderlyingType, 
											 ConvertEnum( obj, nullableConverter.UnderlyingType ));
					else
						obj = Convert.ChangeType( obj, nullableConverter.UnderlyingType );

				} else if(( type == typeof( bool )) && ( obj is string )) {
					int num;

					// Convert.ChangeType() non sa convertire "0"/"1" in bool
					if( int.TryParse( (string)obj, out num ))
						obj = num;

					obj = Convert.ChangeType( obj, type );

				} else if(( type == typeof( SerializableTimeSpan )) && ( obj is TimeSpan ))
					obj = new SerializableTimeSpan( (TimeSpan)obj );
				else
					obj = Convert.ChangeType( obj, type );
			}
			
			return obj;
		}

		private static object ConvertEnum( object obj, Type type )
		{
			if( obj is string )
				obj = Enum.Parse( type, (string) obj );
			else
				obj = Convert.ChangeType( obj, Enum.GetUnderlyingType( type ));

			return obj;
		}

		protected static PropertyInfo GetProperty( Type type, string name )
		{
			PropertyInfo	ret = type.GetProperty( name );

			if( ret == null ) {
				PropertyInfo[] props = type.GetProperties();
				
				name = name.ToLowerInvariant();

				for( int i = props.Length - 1; ( ret == null ) && ( i >= 0 ); i-- ) {
					PropertyInfo item = props[ i ];
					
					if( item.Name.ToLowerInvariant().Equals( name ))
						ret = item;
				}
			}

			return ret;
		}

		protected static PropertyInfo GetWritableProperty( Type type, string name )
		{
			PropertyInfo	ret = GetProperty( type, name );

			if(( ret != null ) && !ret.CanWrite )
				ret = null;

			return ret;
		}

        /// <summary>
        /// Load the View values from a DbDataReader, which must already be positioned on the desired record
        /// </summary>
        /// <param name="dr">data reader</param>
        public void Load( DbDataReader dr )
		{
			DataTable table = dr.GetSchemaTable();

			if( table != null ) {
				Type type = GetType();

				for( int i = table.Rows.Count - 1; i >= 0; i-- ) {
					string			name = dr.GetName( i );
					PropertyInfo	prop = GetWritableProperty( type, name );

					if( prop != null ) {
						object obj = dr.GetValue( i );

						if( obj != DBNull.Value )
							prop.SetValue( this, ConvertType( obj, prop.PropertyType ), null );
					}
				}
			}
		}

		/// <summary>
		/// Load View's vlaues from a NameValueCollection, like a Request.Form of a web application.
		/// </summary>
		/// <param name="values">Values to fill view properties.</param>
		public void Load( NameValueCollection values )
		{
			Type type = GetType();

			foreach( string name in values.AllKeys ) {
				PropertyInfo	prop = GetWritableProperty( type, name );

				if( prop != null ) {

					if( prop.PropertyType.IsArray ) {
                        string[]	items = values.GetValues( name );

						if( items != null ) {
							Type	elType = prop.PropertyType.GetElementType();

							if( elType != null ) {
								Array	array = Array.CreateInstance( elType, items.Length );
								int		i = 0;

								foreach( string item in items )
									array.SetValue( ConvertType( item, elType ), i++ );

								prop.SetValue( this, array, null );
							}
						}

					} else
						prop.SetValue( this, ConvertType( values[ name ], prop.PropertyType ), null );
				}
			}
		}

		/// <summary>
		/// Load View values from an hashtable, like ToHashtable method.
		/// 
		/// It can be useful to quickly map an object on another (at least for fields with the same name).
		/// 
		/// @ at start will be removed.
		/// </summary>
		/// <param name="values">Values to use to fill view properties.</param>
		public void Load( Hashtable values )
		{
			Dictionary<string, object>	dict = new Dictionary<string, object>();

			foreach( DictionaryEntry entry in values )
				dict[ entry.Key.ToString() ] = entry.Value;

			Load( dict );
		}

		public void Load( IDictionary<string, object> values )
		{
			Load( this, values );
		}

		public static void Load( object obj, IDictionary<string, object> values )
		{
			Type	type = obj.GetType();

			foreach( KeyValuePair<string, object> entry in values ) {
				string			name = entry.Key.TrimStart( '@' );
				PropertyInfo	prop = GetWritableProperty( type, name );

				if( prop != null ) {
					object val = entry.Value;

					if( val != DBNull.Value )
						prop.SetValue( obj, ConvertType( val, prop.PropertyType ), null );
				}
			}
		}

		private object ToDBValue( object obj )
		{
			if( obj is SerializableTimeSpan )
				obj = (TimeSpan)(SerializableTimeSpan)obj;

			return obj;
		}

		/// <summary>
		/// Creates an hashtable to give to BaseDAO methods (ExecuteDR etc..)
		/// 
		/// Usage can be handled by attributes NullIf and HashtableIgnore
		/// </summary>
		/// <returns>hashtable with properties of the object associated with keys like @PropertyName</returns>
		public Hashtable ToHashtable() => ToHashtable( false, true );
		public Hashtable ToHashtable( bool includeArrays ) => ToHashtable( includeArrays, true );

		/// <summary>
		/// Creates an hashtable to give to BaseDAO methods (ExecuteDR etc..)
		/// 
		/// Usage can be handled by attributes NullIf and HashtableIgnore
		/// </summary>
		/// <param name="includeArrays">Set to true if you want to copy arrays</param>
		/// <param name="includeReadOnly">If false, readonly properties are not included.</param>
		/// <returns>hashtable with properties of the object associated with keys like @PropertyName</returns>
		public Hashtable ToHashtable( bool includeArrays, bool includeReadOnly )
		{
			Hashtable	ret = new Hashtable();

			foreach( PropertyInfo prop in GetProperties( includeArrays, includeReadOnly ))
				ret[ "@" + prop.Name ] = GetPropertyValue( prop );

			return ret;
		}

		private object GetPropertyValue( PropertyInfo prop )
		{
			object	val = prop.GetValue( this, null );
			bool	isDate = val is DateTime;

			if( val == null )
				val = DBNull.Value;
			else {
				NullIfAttribute[]	attrs = (NullIfAttribute[])prop.GetCustomAttributes( nullIfAttr, true );

				foreach( NullIfAttribute attr in attrs ) {
					object nullVal = attr.NullValue;

					if(( isDate && nullVal.Equals( 0 ) && val.Equals( DateTime.MinValue )) ||
					   nullVal.Equals( val )) {
						val = DBNull.Value;
						break;
					}
				}
			}

			return ToDBValue( val );
		}

		private IEnumerable<PropertyInfo> GetProperties( bool includeArrays, bool includeReadOnly )
		{
			List<PropertyInfo>	ret = new List<PropertyInfo>();
			Type				type = GetType();

			foreach( PropertyInfo prop in type.GetProperties() )
				if(( includeArrays || !prop.PropertyType.IsArray || ( prop.PropertyType.GetElementType() == typeof( byte ))) && 
				   ( includeReadOnly || prop.CanWrite ) && prop.CanRead &&
				   ( prop.GetCustomAttributes( ignoreAttr, true ).Length == 0 ))
					ret.Add( prop );

			return ret;
		}
        
		public void CopyFrom( View otherView ) => CopyFrom( otherView, false );
        
		public void CopyFrom( View otherView, bool includeArrays )
		{
			Load( otherView.ToHashtable(includeArrays, false));
		}
        
		public T MakeCopyAs<T>() where T : View, new() => MakeCopyAs<T>( false );
        
		public T MakeCopyAs<T>( bool includeArrays ) where T : View, new()
		{
			T	ret = new T();

			ret.CopyFrom( this, includeArrays );

			return ret;
		}

		/// <summary>
		/// Adds to hashtable the values correspondent to the placeholder used by the query
		/// </summary>
		/// <param name="query"></param>
		/// <param name="prms"></param>
		public void SetQueryParams( string query, Hashtable prms )
		{
			foreach( PropertyInfo prop in GetProperties( false, true )) {
				string key = "@" + prop.Name;

				if( Regex.IsMatch( query, @"(^|[\(\s,=\+\-\*/])" + key + @"($|[\)\s,=\+\-\*/])" ))
					prms[ key ] = GetPropertyValue( prop );
			}
		}
	}
}
