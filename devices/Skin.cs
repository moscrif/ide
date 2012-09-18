using System;
using System.Xml.Serialization;
//using Newtonsoft.Json;
//using Newtonsoft.Json.Serialization;

namespace Moscrif.IDE.Devices
{
	//[JsonObject(MemberSerialization.OptIn)]
	public class Skin
	{
		[XmlAttribute("name")]
	//	[JsonProperty(PropertyName = "name")]
		public string Name;

		[XmlAttribute("resolution")]
	//	[JsonProperty(PropertyName = "resolution")]
		public string Resolution;

		[XmlAttribute("ResolutionJson")]
	//	[JsonProperty(PropertyName = "resolution")]
		public string ResolutionJson;

		[XmlAttribute("theme")]
	//	[JsonProperty(PropertyName = "theme")]
		public string Theme;
	}
}

