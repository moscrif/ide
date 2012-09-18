using System;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.IO;
using Moscrif.IDE.Iface.Entities;

namespace Moscrif.IDE.Iface
{
	public class ProjectTemplate
	{

		[XmlIgnore]
		public string FilePath
		{
			get;
			set;
		}

		public ProjectTemplate(){

		}

		public ProjectTemplate(string filePath)
		{
			FilePath = filePath;
			if (Libs == null) Libs = new List<SettingValue>();
		}


		public void SaveProjectTemplate()
		{
			using (FileStream fs = new FileStream(FilePath, FileMode.Create)) {
				XmlSerializer serializer = new XmlSerializer(typeof(ProjectTemplate));
				serializer.Serialize(fs, this);
			}
		}


		static public ProjectTemplate OpenProjectTemplate(string filePath)
		{
			if (System.IO.File.Exists(filePath)) {

				try {
					using (FileStream fs = File.OpenRead(filePath)) {
						XmlSerializer serializer = new XmlSerializer(typeof(ProjectTemplate));
						ProjectTemplate s = (ProjectTemplate)serializer.Deserialize(fs);

						return s;
					}
				} catch {//(Exception ex) {
					return new ProjectTemplate();
				}
			} else {
				return new ProjectTemplate();
			}
		}

		[XmlAttribute("description")]
		public string Description;
		
		[XmlArrayAttribute("libs")]
		[XmlArrayItem("lib")]
		public List<SettingValue> Libs;

	}

	/*public class TemplateDescription
	{
		public TemplateDescription()
		{
		}
		
		public TemplateDescription(string description)
		{
			this.Description = description;
		}
		
		[XmlAttribute("description")]
		public string Description;
		
		[XmlArrayAttribute("libs")]
		[XmlArrayItem("libs")]
		public List<String> Libs;	
	}*/
}

