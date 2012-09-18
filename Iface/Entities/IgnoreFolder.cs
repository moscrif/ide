using System;
using System.Xml;
using System.Xml.Serialization;

namespace Moscrif.IDE.Iface.Entities
{
	public class IgnoreFolder
	{
		public IgnoreFolder()
		{
		}

		public IgnoreFolder(string folder, bool isIde, bool  isPub)
		{
			Folder = folder;
			IsForIde = isIde;
			IsForPublish = isPub;
		}

		[XmlAttribute("folder")]
		public string Folder{
			get;
			set;
		}

		[XmlAttribute("isforide")]
		public bool IsForIde{
			get;
			set;
		}

		[XmlAttribute("isforpublish")]
		public bool IsForPublish{
			get;
			set;
		}

	}
}

