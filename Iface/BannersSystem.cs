using System;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Specialized;
using Moscrif.IDE.Iface.Entities;
using System.Security.Cryptography;
using System.Reflection;
using Moscrif.IDE.Tool;
using System.IO;
using System.Timers;
using System.Threading;

namespace Moscrif.IDE.Iface
{
	public class BannersSystem
	{

		private string bannerUrl = MainClass.Settings.bannerUrl ;
		private string bannerFile ;
		private Banners banners;

		public BannersSystem() {
			if(GetBanner() && !string.IsNullOrEmpty(bannerFile)){
				//Console.WriteLine(bannerFile);
				banners = Banners.LoadBanners(bannerFile);
			}
		}

		private int cnt =0;
		//private int tmp =1;
		//private int ErrorCount = 0;
		public Banner NextBanner(){
			if((banners.Items == null) || (banners.Items.Count== 0)){
				return null;
			}

			if(banners.Items.Count>cnt){
				Banner banner =  banners.Items[cnt];
				cnt++;
				if(banner.BannerPixbuf == null){
					try{
						using (WebClient client = new WebClient())
						{
							/*string bannerParth  = System.IO.Path.Combine(MainClass.Paths.ResDir,"banner");
							bannerParth = System.IO.Path.Combine(bannerParth,"test"+tmp.ToString()+".png");
							tmp++;
							if(tmp>3)
								tmp =1;
							*/
							byte[] imageBuffer = client.DownloadData(banner.Image);

							banner.BannerPixbuf =new Gdk.Pixbuf(imageBuffer);//imageBuffer);
						}
					}catch (Exception ex){
						//ErrorCount ++;
						//if(ErrorCount>=5)
						return null;
					}
				}
				return banner;
			} else {
				cnt = 0;
				return NextBanner ();
			}
			return null;
		}

		public bool GetBanner(){
			
			string URL =bannerUrl;		
			WebClient client = new WebClient();
		
			try{
				bannerFile = client.DownloadString(new Uri(URL));

			}catch(Exception ex){
				string statusDesc = "";
				GetStatusCode(client,out statusDesc);
				Console.WriteLine(ex.Message);
				
				return false;
			}
			return true;		
		}

		private static int GetStatusCode(WebClient client, out string statusDescription)
		{
			FieldInfo responseField = client.GetType().GetField("m_WebRequest", BindingFlags.Instance | BindingFlags.NonPublic);
			
			if (responseField != null)
			{
				HttpWebResponse response = responseField.GetValue(client) as HttpWebResponse;
				
				if (response != null)
				{
					statusDescription = response.StatusDescription;
					return (int)response.StatusCode;
				}
			}		
			statusDescription = null;
			return 0;
		}
	}
}

