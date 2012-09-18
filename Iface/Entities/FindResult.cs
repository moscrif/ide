using System;
using System.Xml;
using System.Xml.Serialization;

namespace Moscrif.IDE.Iface.Entities
{
	public class FindResult
	{
		public FindResult()
		{
		}

		public FindResult(object key, object val)
		{
			this.Key=key;
			this.Value= val;
		}

		public FindResult(object key, object val,object StartOffset,object EndOffset)
		{
			this.Key=key;
			this.Value= val;
			this.StartOffset = StartOffset;
			this.EndOffset = EndOffset;
		}

		public object Key{
			get;
			set;
		}

		public object Value{
			get;
			set;
		}

		public object StartOffset{
			get;
			set;
		}

		public object EndOffset{
			get;
			set;
		}
	}
}

