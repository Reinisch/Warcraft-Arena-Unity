using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace ElleFramework.Utils.Serialization
{
    [Serializable]
	public class SerializableTimeSpan : IXmlSerializable 
	{
		public TimeSpan Value { get; set; }

		public SerializableTimeSpan()
		{
		}

		public SerializableTimeSpan( TimeSpan ts )
		{
			Value = ts;
		}

		public SerializableTimeSpan( string val )
		{
			Value = TimeSpan.Parse( val );
		}

		public static bool operator==( SerializableTimeSpan ts1, SerializableTimeSpan ts2 )
		{
			bool	ret = ReferenceEquals( ts1, ts2 );

			if( !ret )
				ret = !ReferenceEquals( null, ts1 ) && !ReferenceEquals( null, ts2 ) && ( ts1.Value.Ticks == ts2.Value.Ticks );

			return( ret );
		}

		public static bool operator !=( SerializableTimeSpan ts1, SerializableTimeSpan ts2 )
		{
			return !( ts1 == ts2 );
		}

		public static SerializableTimeSpan operator +( SerializableTimeSpan ts1, SerializableTimeSpan ts2 )
		{
			return new SerializableTimeSpan( ts1.Value + ts2.Value );
		}

		public static SerializableTimeSpan operator -( SerializableTimeSpan ts1, SerializableTimeSpan ts2 )
		{
			return new SerializableTimeSpan( ts1.Value - ts2.Value );
		}

		public override string ToString()
		{
			return( Value.ToString() );
		}

		public string ToString( string format )
		{
			return( Value.ToString( format ));
		}

		public string ToString( string format, IFormatProvider provider )
		{
			return( Value.ToString( format, provider ));
		}

		public static implicit operator TimeSpan( SerializableTimeSpan ts )
		{
			return( ts.Value );
		}

		public static implicit operator SerializableTimeSpan( TimeSpan ts )
		{
			return( new SerializableTimeSpan( ts ));
		}

		public XmlSchema GetSchema()
		{
			 return( null );
		}

		public void ReadXml( XmlReader reader )
		{
			Value = TimeSpan.Parse( reader.ReadElementContentAsString() );
		}

		public void WriteXml( XmlWriter writer )
		{
			 writer.WriteValue( Value.ToString() );
		}

		public bool Equals( SerializableTimeSpan other )
		{
			if( ReferenceEquals( null, other ))
				return false;

			if( ReferenceEquals( this, other ))
				return true;

			return other.Value.Equals( Value );
		}

		public override bool Equals( object obj )
		{
			if( ReferenceEquals( null, obj ))
				return false;

			if( ReferenceEquals( this, obj ))
				return true;

			if( obj.GetType() != typeof( SerializableTimeSpan ))
				return false;

			return Equals( (SerializableTimeSpan) obj );
		}

		public override int GetHashCode()
		{
			return Value.GetHashCode();
		}
	}
}
