using System;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Moscrif.IDE.Iface.Entities
{
	public class FileSetting
	{
		public FileSetting(string filename)
		{
			FileName =filename;
			Folding = new List<SettingValue>();
			//Bookmarks = new List<int>();
			Bookmarks2 = new List<MyBookmark>();
		}

		public FileSetting(){

		}

		[XmlAttribute("filePath")]
		public string FileName;

		[XmlArray("foldings")]
		[XmlArrayItem("folding")]
		public List<SettingValue> Folding ;


		/*[XmlArray("bookmarks")]
		[XmlArrayItem("bookmark")]
		public List<int> Bookmarks;*/


		//[XmlIgnore]
		[XmlArray("bookmarks2")]
		[XmlArrayItem("bookmark2")]
		public List<MyBookmark> Bookmarks2;

	}

	public class MyBookmark {

		public MyBookmark()
		{
		}

		public MyBookmark(int line ,string name)
		{
			Line =line;
			Name = name;
		}

		[XmlAttribute("line")]
		public int Line{get;set;}

		[XmlAttribute("name")]
		public string Name{get;set;}

	}

}

