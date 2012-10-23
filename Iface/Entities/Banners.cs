using System;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.IO;

namespace Moscrif.IDE.Iface.Entities
{
	[XmlRoot("banners")]
	public class Banners
	{
		public Banners() {Items = new List<Banner>();}

		[XmlElement("banner")]
		public List<Banner> Items {get;set;}

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

		public string Save(){

			XmlSerializer x_serial = new XmlSerializer(typeof(Banners));
			StringWriter textWriter = new StringWriter();
			
			x_serial.Serialize(textWriter, this);
			return textWriter.ToString();
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
		public Gdk.Pixbuf BannerPixbuf{
			get{
				return bannerPixbuf;
			}
			set {
				if(value.Width>200 || value.Height>40){
					bannerPixbuf = value.ScaleSimple(200,40, Gdk.InterpType.Bilinear);
				} else {
					bannerPixbuf = value;
				}
			}
		}
	}
}

