using System;
using System.Xml;
using System.Xml.Serialization;


namespace Moscrif.IDE.Iface.Entities
{
	public class KeyBinding
	{

		public KeyBinding()
		{
		}

		public KeyBinding(string name,string description,string action,string key)
		{
			Name =name;
			Description=description;
			Action=action;
			Key=key;
		}

		[XmlAttribute("name")]
		public string Name{
			get;
			set;
		}

		[XmlAttribute("description")]
		public string Description{
			get;
			set;
		}

		[XmlAttribute("action")]
		public string Action{
			get;
			set;
		}

		[XmlAttribute("key")]
		public string Key{
			get;
			set;
		}

	}
}

