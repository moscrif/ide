using System;
using System.Xml.Serialization;
//using Newtonsoft.Json;
//using Newtonsoft.Json.Serialization;

namespace Moscrif.IDE.Devices
{
	//[JsonObject(MemberSerialization.OptIn)]
	public class PublishProperty
	{
		public PublishProperty()
		{
		}

		public PublishProperty(string name){
			PublishName = name;
			PublishValue ="";
		}

		public PublishProperty(string name, string val){
			PublishName = name;
			PublishValue =val;
		}

		[XmlAttribute("name")]
		//[JsonProperty(PropertyName = "name")]
		public string PublishName;

		[XmlAttribute("value")]
		//[JsonProperty(PropertyName = "value")]
		public string PublishValue;
	}

}

