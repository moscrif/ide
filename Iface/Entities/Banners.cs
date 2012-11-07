using System;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Moscrif.IDE.Iface.Entities
{
	[XmlRoot("banners")]
	public class Banners
	{
		public Banners() {
			Items = new List<Banner>();
			IsCache = false;
		}

		[XmlElement("banner")]
		public List<Banner> Items {get;set;}

		[XmlIgnore]
		public bool IsCache {get;set;}

		[XmlIgnore]
		public string CacheFilePath {
			get{
				return System.IO.Path.Combine(MainClass.Paths.BannerCache,"banners.xml");
			}
		}

		public static Banners LoadBanners(string data){
			try {
				XmlSerializer serializer = new XmlSerializer(typeof(Banners));//,xRoot);
			
				Banners banners= (Banners)serializer.Deserialize((new StringReader(data) ));
				return banners;
			}catch(Exception ex){
				Moscrif.IDE.Tool.Logger.Error(ex.Message);
				Console.WriteLine(ex.Message);
				return new Banners();
			}
		}

		public static Banners OpenBannerCache(){
			string bannerCachePath = System.IO.Path.Combine(MainClass.Paths.BannerCache,"banners.xml");
			if (File.Exists(bannerCachePath)){
				using (FileStream fs = File.OpenRead(bannerCachePath)) {
					XmlSerializer serializer = new XmlSerializer(typeof(Banners));
					
					Banners banners= (Banners)serializer.Deserialize(fs);
					banners.IsCache = true;
					return banners;
				}
			}
			return new Banners();

		}

		public void SaveBannerCache(){
			try{
				XmlSerializer x_serial = new XmlSerializer(typeof(Banners));

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

	}

	public class Banner
	{
		public Banner()
		{
		}
		private Gdk.Pixbuf bannerPixbuf;

		[XmlElement("name")]
		public string Name ;

		[XmlElement("description")]
		public string Description;

		[XmlElement("url")]
		public string Url;

		[XmlElement("image")]
		public string Image;

		[XmlIgnore]
		public string CacheFilePath{
			get{
				return System.IO.Path.Combine(MainClass.Paths.BannerCache,CacheFileName);
			}
		}

		[XmlIgnore]
		public string CacheFileName{
			get{
				//MainClass.Tools.GetMd5Sum
				return  Moscrif.IDE.Tool.Cryptographer.MD5Hash(Image)+".png";
			}
		}

		public void Load(){
			try{
				LoadFromUrl();
				SaveCache();
			} catch (Exception ex){
				Console.WriteLine(ex.Message);
				LoadFromCache();
			}			
		}

		public void LoadFromCache(){
			if(File.Exists(CacheFilePath))
				this.BannerPixbuf =new Gdk.Pixbuf(CacheFilePath);
			else 
				this.BannerPixbuf = null;
		}


		public void LoadFromUrl(){
			using (WebClientTimeout client = new WebClientTimeout(5000))
			{
				byte[] imageBuffer = client.DownloadData(this.Image);
				this.BannerPixbuf =new Gdk.Pixbuf(imageBuffer);//imageBuffer);
			}
		}

		public void SaveCache(){
			try{
				if(File.Exists(CacheFilePath)){
					File.Delete(CacheFilePath);
				}
				if(BannerPixbuf!= null){
					BannerPixbuf.Save(CacheFilePath,"png");
				}
			}catch(Exception ex){
				Tool.Logger.Error(ex.Message);
			}

		}

		[XmlIgnore]
		public Gdk.Pixbuf BannerPixbuf{
			get{
				return bannerPixbuf;
			}
			set {
				//bannerPixbuf.AddAlpha(
				if(value.Width>200 || value.Height>40){
					bannerPixbuf = value.ScaleSimple(200,40, Gdk.InterpType.Bilinear);
				} else {
					bannerPixbuf = value;
				}
			}
		}
	}
}

