using System;
using System.Xml;
using System.Xml.Serialization;

namespace Moscrif.IDE.Iface.Entities
{
	public class FeedbackData
	{
		public FeedbackData()
		{
		}

		[XmlAttribute("typ")]
		public int Typ;
		
		[XmlAttribute("subject")]
		public string Subject;
		
		[XmlAttribute("product")]
		public string Product;
		
		[XmlAttribute("system")]
		public string System;
		
		[XmlAttribute("version")]
		public string Version;
		
		[XmlAttribute("description")]
		public string Description;

	}
}

