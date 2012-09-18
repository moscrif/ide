using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace  Moscrif.IDE.Iface.Entities
{
	public class LogicalSystem : ICloneable
	{
		public LogicalSystem()
		{
		}

		#region ICloneable implementation
		object ICloneable.Clone()
		{
			LogicalSystem ls = (LogicalSystem)MemberwiseClone();
			ls.Mask =  new List<string>(ls.Mask.ToArray());

			return ls;

		}

		#endregion

		public LogicalSystem Clone()
		{
			return (LogicalSystem)this.MemberwiseClone();
		}


		[XmlAttribute("display")]
		public string Display;

		[XmlAttribute("description")]
		public string Description;

		[XmlArray("masks")]
		[XmlArrayItem("mask")]
		public List<string> Mask;


		static public List<LogicalSystem> GetDefaultLogicalSystem (){

			List<LogicalSystem> list = new List<LogicalSystem>();

			LogicalSystem images = new LogicalSystem();
			images.Display = "Images";
			images.Description = "Images";
			images.Mask = new List<string>();
			images.Mask.Add(".jpg");
			images.Mask.Add(".png");
			list.Add(images);

			LogicalSystem source = new LogicalSystem();
			source.Display = "Source codes";
			source.Description = "Source codes";
			source.Mask = new List<string>();
			source.Mask.Add(".ms");
			source.Mask.Add(".mso");
			list.Add(source);

			LogicalSystem font = new LogicalSystem();
			font.Display = "Fonts";
			font.Description = "Fonts";
			font.Mask = new List<string>();
			font.Mask.Add(".ttf");
			list.Add(font);

			LogicalSystem database = new LogicalSystem();
			database.Display = "Databases";
			database.Description = "Databases";
			database.Mask = new List<string>();
			database.Mask.Add(".db");
			database.Mask.Add(".tab");
			database.Mask.Add(".cvs");
			list.Add(database);

			LogicalSystem text = new LogicalSystem();
			text.Display = "Texts";
			text.Description = "Texts";
			text.Mask = new List<string>();
			text.Mask.Add(".txt");
			text.Mask.Add(".cvs");
			list.Add(text);

			LogicalSystem hypertext = new LogicalSystem();
			hypertext.Display = "Hypertexts";
			hypertext.Description = "Hypertexts";
			hypertext.Mask = new List<string>();
			hypertext.Mask.Add(".xml");
			hypertext.Mask.Add(".htm");
			hypertext.Mask.Add(".html");
			list.Add(hypertext);

			return list;

		}

	}
}

