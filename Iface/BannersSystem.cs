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
			} else {
				banners = Banners.OpenBannerCache();
			}
			if(!banners.IsCache){
				banners.SaveBannerCache();
			}
		}

		private int cnt =0;

		private int ErrorCount = 0;
		public Banner NextBanner(){
			if((banners.Items == null) || (banners.Items.Count== 0)){
				return null;
			}

			if(banners.Items.Count>cnt){
				Banner banner =  banners.Items[cnt];
				cnt++;
				if(banner.BannerPixbuf == null){
					try{
						if(!banners.IsCache){
							banner.Load();							
							/*banner.LoadFromUrl();
							banner.SaveCache();
							ErrorCount = 0;*/
						} else {
							banner.LoadFromCache();
						}
					}catch (Exception ex){
						ErrorCount ++;
						if(ErrorCount>=3){
							cnt++;
							ErrorCount = 0;
						}
						return null;
					}
				}
				return banner;
			} else {
				cnt = 0;
				return NextBanner ();
			}
			//return null;
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

