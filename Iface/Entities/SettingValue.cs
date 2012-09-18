using System;
using System.Xml;
using System.Xml.Serialization;

namespace Moscrif.IDE.Iface.Entities
{
	public class SettingValue
	{
		public SettingValue()
		{
		}

		public SettingValue(string val, string display)
		{
			this.Value= val;
			this.Display=display;
		}

		[XmlAttribute("value")]
		public string Value{
			get;
			set;
		}

		[XmlAttribute("display")]
		public string Display{
			get;
			set;
		}

	}
}

