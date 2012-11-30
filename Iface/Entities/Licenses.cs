using System;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Moscrif.IDE.Iface.Entities
{
	[XmlRoot("licenses")]
	public class Licenses
	{
		public Licenses() {
			Items = new List<License>();
			IsCache = false;
		}
		
		[XmlElement("license")]
		public List<License> Items {get;set;}

		[XmlIgnore]
		public bool IsCache {get;set;}
		
		[XmlIgnore]
		public string CacheFilePath {
			get{
				return System.IO.Path.Combine(MainClass.Paths.BannerCache,"licenses.xml");
			}
		}

		public static Licenses LoadLicenses(string data){
			try {
				XmlSerializer serializer = new XmlSerializer(typeof(Licenses));//,xRoot);
				
				Licenses banners= (Licenses)serializer.Deserialize((new StringReader(data) ));
				return banners;
			}catch(Exception ex){
				Moscrif.IDE.Tool.Logger.Error("Load License :" +ex.Message);
				Console.WriteLine("Load License ->" + ex.Message);
				return new Licenses();
			}
		}
		
		public void SaveLicensesCache(){
			try{
				XmlSerializer x_serial = new XmlSerializer(typeof(Licenses));
				
				using (XmlWriter wr = XmlWriter.Create( this.CacheFilePath )) {
					x_serial.Serialize( wr, this );
				}

				/*StringWriter textWriter = new StringWriter();
				
				x_serial.Serialize(textWriter, this);
				return textWriter.ToString();*/
				
			}catch(Exception ex){
				Tool.Logger.Error(ex.Message);
			}
		}

		public static Licenses OpenLicensesCache(){
			string bannerCachePath = System.IO.Path.Combine(MainClass.Paths.BannerCache,"licenses.xml");
			if (File.Exists(bannerCachePath)){
				using (FileStream fs = File.OpenRead(bannerCachePath)) {
					XmlSerializer serializer = new XmlSerializer(typeof(Licenses));
					
					Licenses banners= (Licenses)serializer.Deserialize(fs);
					banners.IsCache = true;
					return banners;
				}
			}
			return new Licenses();
		}
		
	}
	
	public class License
	{
		public License()
		{
		}

		public static License LoadLicense(string data){
			try {
				//Console.WriteLine(data);
				XmlSerializer serializer = new XmlSerializer(typeof(License));//,xRoot);
				
				License banners= (License)serializer.Deserialize((new StringReader(data) ));
				return banners;
			}catch(Exception ex){
				Moscrif.IDE.Tool.Logger.Error(ex.Message);
				Console.WriteLine(ex.Message);
				return new License();
			}
		}

		[XmlElement("token")]
		public string Token;

		[XmlElement("name")]
		public string Name ;
		
		[XmlElement("code")]
		public string Code;
		
		[XmlElement("type")]
		public string Typ;
		
		[XmlArray("featutes")]
		[XmlArrayItem("feature")]
		public List<Feature> Featutes {get;set;}
	}


	
	public class FeatureComparer : IEqualityComparer<Feature>
	{
		public bool Equals(Feature x, Feature y)	
		{	
			return x.Code == y.Code;	
		}
		

		public int GetHashCode(Feature obj)
		{
			return obj.Name.GetHashCode();
		}
	}
	

	
	public class Feature
	{
		public Feature()
		{
		}

		[XmlElement("name")]
		public string Name ;
		
		[XmlElement("code")]
		public string Code;
		
		[XmlElement("description")]
		public string Description;
		
		[XmlElement("sup")]
		public string Sup;

		[XmlElement("type")]
		public string Type;

		[XmlElement("mobile")]
		public string Mobile;

		[XmlElement("value",IsNullable=true)]
		public string Value;
	}

}

