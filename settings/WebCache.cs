using System;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.IO;
using Moscrif.IDE.Iface.Entities;

namespace Moscrif.IDE.Settings
{
	public class WebCache
	{

		[XmlIgnore]
		public string FilePath
		{
			get;
			set;
		}

		public WebCache()
		{
			FilePath = System.IO.Path.Combine(MainClass.Paths.SettingDir, ".webcache");
			if (ListRss == null) ListRss = new List<WebObject>();
			if (ListTweet == null) ListTweet = new List<WebObject>();
		}

		public WebCache(string filePath)
		{
			FilePath = filePath;
			if (ListRss == null) ListRss = new List<WebObject>();
			if (ListTweet == null) ListTweet = new List<WebObject>();
		}


		public void SaveWebCache()
		{
			using (FileStream fs = new FileStream(FilePath, FileMode.Create)) {
				XmlSerializer serializer = new XmlSerializer(typeof(WebCache));
				serializer.Serialize(fs, this);
			}
		}


		static public WebCache OpenWebCache(string filePath)
		{
			if (System.IO.File.Exists(filePath)) {

				try {
					using (FileStream fs = File.OpenRead(filePath)) {
						XmlSerializer serializer = new XmlSerializer(typeof(WebCache));
						WebCache s = (WebCache)serializer.Deserialize(fs);

						return s;
					}
				} catch {//(Exception ex) {
					return new WebCache();
				}
			} else {
				return new WebCache();
			}
		}

		[XmlArrayAttribute("rss")]
		[XmlArrayItem("item")]
		public List<WebObject> ListRss;

		[XmlArrayAttribute("twitter")]
		[XmlArrayItem("tweet")]
		public List<WebObject> ListTweet;
	}
}

