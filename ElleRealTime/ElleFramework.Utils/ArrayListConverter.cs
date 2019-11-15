using System.Collections;
using System.Data;
using System.Reflection;

namespace ElleFramework.Utils
{
    public class ArrayListConverter
    {
        /// <summary>
		/// Function for converting an ArrayList into a DataTable object.
		/// In order to perform the conversion the ArrayList must contain objects of the same type.
		/// In particular, it is necessary that all objects are of the same type as the first ArrayList object or at most of a derived type.
		/// The columns will be sorted in the order specified in <c>columnNames</c>.
		/// </summary>
		/// <param name="al">ArrayList to convert into DataTable</param>
		/// <param name="columnNames">Array of strings containing the names of the columns arranged in the order in which they should appear in the table</param>
		/// <param name="appendRemaining">Indicates whether any properties whose name is not indicated in <c>columnNames</c> should be appended to the columns of the table</param>
		/// <returns>DataTable containing the data of the ArrayList in table form</returns>
		public static DataTable convertToDataTable( ArrayList al, string[] columnNames, bool appendRemaining )
		{
			DataTable rval = null;

            // Check that the ArrayList is not null
            if ( al != null ) {
				rval = new DataTable( "ArrayList" );

                // Get the property of the first object
                PropertyInfo[] objProps = al[ 0 ].GetType().GetProperties( BindingFlags.Public | BindingFlags.Instance );

                // Order the properties so that they respect the columns of the table
                if ( columnNames != null && columnNames.Length > 0 ) {
					Hashtable htProp = new Hashtable();
					foreach( PropertyInfo pi in objProps ) {
						htProp.Add( pi.Name, pi );
					}

					PropertyInfo[] tmpObjProps = new PropertyInfo[objProps.Length];
					bool mappingOk = true;
					for( int i = 0; i < columnNames.Length && mappingOk; i++ ) {
						if( htProp.ContainsKey( columnNames[ i ] ) ) {
							tmpObjProps[ i ] = (PropertyInfo) htProp[ columnNames[ i ] ];
							htProp.Remove( columnNames[ i ] );
						} else
							mappingOk = false;
					}

					if( mappingOk ) {
						if( htProp.Count > 0 && appendRemaining ) {
                            // Copy the remaining objects in the Hashtable to the temporary array
                            htProp.Values.CopyTo( tmpObjProps, columnNames.Length );
						}
                        // Replaces the original property array with the reordered one
                        objProps = tmpObjProps;
					}
				}

                // Create table columns
                foreach ( PropertyInfo pi in objProps )
					rval.Columns.Add( new DataColumn
									  {
										  ColumnName = pi.Name,
										  DataType = pi.GetType()
									  } );

                // Inserts the rows of the table
                // Obviously all the objects contained in the ArrayList must be of the same type
                foreach ( object obj in al ) {
					DataRow tmpRow = rval.NewRow();
					for( int i = 0; i < objProps.Length; i++ ) {
						tmpRow[ i ] = objProps[ i ].GetValue( obj, null );
					}
					rval.Rows.Add( tmpRow );
				}
			}

			return rval;
		}

        /// <summary>
        /// Function for converting an ArrayList into a DataTable object.
        /// In order to perform the conversion the ArrayList must contain objects of the same type.
        /// In particular, it is necessary that all objects are of the same type as the first ArrayList object or at most of a derived type.
        /// The columns will be sorted in the order specified in <c>columnNames</c>.
        /// Le proprietà i cui nomi non sono presenti in <c>columnNames</c> non verranno inserite
        /// nella DataTable.
        /// </summary>
        /// <param name="al">ArrayList to convert into DataTable</param>
        /// <param name="columnNames">Array of strings containing the names of the columns arranged in the order in which they should appear in the table</param>
        /// <param name="appendRemaining">Indicates whether any properties whose name is not indicated in <c>columnNames</c> should be appended to the columns of the table</param>
        /// <returns>DataTable containing the data of the ArrayList in table form</returns>
        public static DataTable convertToDataTable( ArrayList al, string[] columnNames )
		{
			return convertToDataTable( al, columnNames, false );
		}

        /// <summary>
        /// Function for converting an ArrayList into a DataTable object.
        /// In order to perform the conversion the ArrayList must contain objects of the same type.
        /// In particular, it is necessary that all objects are of the same type as the first ArrayList object or at most of a derived type.
        /// </summary>
        /// <param name="al">ArrayList to convert into DataTable</param>
        /// <returns>DataTable containing the data of the ArrayList in table form</returns>
        public static DataTable convertToDataTable( ArrayList al )
		{
			return convertToDataTable( al, null, false );
		}
    }
}
