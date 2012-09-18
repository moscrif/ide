using System;
using System.Xml;
using System.Xml.Serialization;

namespace Moscrif.IDE.Iface.Entities
{
	public class WebObject
	{
		public WebObject()
		{
		}
		
		public WebObject(string title,string url,string description,string hovermessage)
		{
			this.Title = title;
			this.Url = url;
			this.Description = description;
			this.HoverMessage = hovermessage;
		}
		
		[XmlAttribute("title")]
		public string Title;
		
		[XmlAttribute("url")]
		public string Url;
		
		[XmlAttribute("description")]
		public string Description;
		
		[XmlAttribute("hovermessage")]
		public string HoverMessage;		
	}
}

