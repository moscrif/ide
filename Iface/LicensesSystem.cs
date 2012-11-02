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
	public class LicensesSystem
	{
		private string licenceUrl = MainClass.Settings.licenceUrl ;
		private string licenceFile ;
		public Licenses Licenses;

		public LicensesSystem()
		{
			if(GetLicenses() && !string.IsNullOrEmpty(licenceFile)){
				//Console.WriteLine(bannerFile);
				Licenses = Licenses.LoadLicenses(licenceFile);
			} else {
				Licenses = Licenses.OpenLicensesCache();
			}
			if(!Licenses.IsCache){
				Licenses.SaveLicensesCache();
			}
		}

		public bool GetLicenses(){
			
			string URL =licenceUrl;		
			WebClient client = new WebClient();
			
			try{
				licenceFile = client.DownloadString(new Uri(URL));
				
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

