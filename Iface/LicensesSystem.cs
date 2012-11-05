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

		public License GetLicence(string typ){
			return	Licenses.Items.Find(x=>x.Typ==typ);
		}

		public License GetNextLicence(string typ){
			string newTyp = typ;
			int iTyp =0;
			if(Int32.TryParse(typ,out iTyp)){
				iTyp = iTyp-100;
			}

			return	Licenses.Items.Find(x=>x.Typ==iTyp.ToString());
		}

		public LicensesSystem()
		{
			Licenses = Licenses.OpenLicensesCache();
		}

		public void LoadFromWeb(){
			if(GetLicenses() && !string.IsNullOrEmpty(licenceFile)){
				Licenses = Licenses.LoadLicenses(licenceFile);
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

