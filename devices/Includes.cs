using System;
using System.Xml.Serialization;
//using Newtonsoft.Json;
//using Newtonsoft.Json.Serialization;

namespace Moscrif.IDE.Devices
{
//	[JsonObject(MemberSerialization.OptIn)]
	public class Includes
	{
		public Includes(){
			this.Skin = new Skin();
			this.Fonts = new string[]{};
		}
/*
		[JsonProperty(PropertyName = "binaries")]
		public bool Binaries;

		[JsonProperty(PropertyName = "core")]
		public bool Core;

		[JsonProperty(PropertyName = "ui")]
		public bool Ui;
*/

		//[JsonProperty(PropertyName = "fonts")]
		[XmlArray("fonts")]
		public string[] Fonts;

		//[JsonProperty(PropertyName = "skin")]
		[XmlElement("skin")]
		public Skin Skin;

		[XmlIgnore]
		//[JsonProperty(PropertyName = "files")]
		public string[] Files;

	}
}

