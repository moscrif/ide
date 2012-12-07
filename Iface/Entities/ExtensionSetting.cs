using System;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Moscrif.IDE.Iface.Entities
{
	public class ExtensionSetting
	{
		public ExtensionSetting(){
		}
		
		public ExtensionSetting(string extension, OpenTyp typ){
			Extension = extension;
			OpenType = typ;
			ExternalProgram="";
		}
		
		public enum OpenTyp{
			[XmlEnum("0")]
			TEXT = 0,
			[XmlEnum("1")]
			IMAGE = 1,
			[XmlEnum("2")]
			DATABASE = 2,
			[XmlEnum("-1")]
			SYSTEM = -1,
			[XmlEnum("-2")]
			EXTERNAL = -2
		}
		
		[XmlAttribute("openType")]
		public OpenTyp OpenType;
		
		[XmlAttribute("Extension")]
		public string Extension;
		
		[XmlAttribute("externalProgram")]
		public string ExternalProgram;
		
		[XmlAttribute("parameter")]
		public string Parameter;
		
		[XmlIgnore]
		public List<string> Extensions{
			get{
				string[] exts = Extension.Split(' ',',',';');
				
				List<string> extensions = new List<string>(exts);
				
				return extensions;
			}
		}
	}
}

