using System;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Specialized;
using System.IO;
using System.Timers;
using System.Threading;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Reflection;
using Moscrif.IDE.Tool;
using Moscrif.IDE.Controls;
using Moscrif.IDE.Iface.Entities;
using System.Linq;

namespace Moscrif.IDE.Iface
{
	public class LicensesSystem
	{
		private string licenceUrl = MainClass.Settings.licenceUrl ;
		private string licenceFile ;
		public Licenses Licenses;

		public License GetLicence(string typ){
			if(Licenses.Items==null || Licenses.Items.Count<=0)
				return null;

			return	Licenses.Items.Find(x=>x.Typ==typ);
		}

		public License GetNextLicence(string typ){
			string newTyp = typ;
			int iTyp =0;
			if(Int32.TryParse(typ,out iTyp)){
				iTyp = iTyp-100;
			}
			return GetLicence(iTyp.ToString());
		}

		public LicensesSystem()
		{
			Licenses = Licenses.OpenLicensesCache();
			GenerateList();
		}

		private void GenerateList(){
			ListFrameworkClass = new List<FrameworkClass>();
			ListFrameworkClass.Add(new FrameworkClass("facebook","libraries_facebook","net - Facebook",LicenceTyp.BASIC));
			ListFrameworkClass.Add(new FrameworkClass("sqlite","libraries_sqlitelib","sqlite",LicenceTyp.BASIC));
			ListFrameworkClass.Add(new FrameworkClass("zip","libraries_ziplib","zip",LicenceTyp.BASIC));
			ListFrameworkClass.Add(new FrameworkClass("ads","libraries_ad","ads",LicenceTyp.BASIC));
			ListFrameworkClass.Add(new FrameworkClass("crypto","libraries_cryptolib","crypto",LicenceTyp.PROFESIONAL));
			ListFrameworkClass.Add(new FrameworkClass("store","libraries_storelib","store",LicenceTyp.PROFESIONAL));

			ListFrameworkClass.Add(new FrameworkClass("signapp","basic_compiledsourcecode","compile & sign",LicenceTyp.PROFESIONAL));
			ListFrameworkClass.Add(new FrameworkClass("marketdistribution","---","Market - Distribution",LicenceTyp.BASIC));

			ListFrameworkClass.Add(new FrameworkClass("conditions","---","Conditions",LicenceTyp.BASIC));
			ListFrameworkClass.Add(new FrameworkClass("androidsupporteddevices","---","Android supported devices",LicenceTyp.PROFESIONAL));
			ListFrameworkClass.Add(new FrameworkClass("windowsandmac","publishtools_desktop_1","Windows and Mac OS",LicenceTyp.BASIC));
		}

		public License GetUserLicense(){
			if(MainClass.User!=null){
				if(MainClass.User.License!= null)
					return MainClass.User.License;
			}
			return GetLicence("-100");
		} 

		public List<FrameworkClass> ListFrameworkClass;

		public void LoadFromWeb(){
			if(GetLicenses() && !string.IsNullOrEmpty(licenceFile)){
				Licenses = Licenses.LoadLicenses(licenceFile);
			} 
			if(!Licenses.IsCache){
				Licenses.SaveLicensesCache();
			}
		} 

		public List<Feature> GetUserDifferent(License lcs2 ){
			if(lcs2== null) return new List<Feature>();

			License userLicence = MainClass.LicencesSystem.GetUserLicense();
			if(userLicence == null){
				return lcs2.Featutes;
			}
			List<Feature> difList = new List<Feature>(lcs2.Featutes.Except(userLicence.Featutes,new FeatureComparer()).ToArray());
			return difList;
		}


		public bool CheckFunction(string functionName,Gtk.Window parentError){
			string userLicenceId ="-100";
			if(MainClass.User != null){
				userLicenceId = MainClass.User.LicenseId;
			}

			int iTyp =0;
			if(!Int32.TryParse(userLicenceId,out iTyp)){
				iTyp = -100;
			}

			FrameworkClass fc = ListFrameworkClass.Find(x=>x.Name == functionName);
			if(fc!= null){
				if(iTyp <= (int)fc.Typ ){
					return true;
				} else
				{
					BuyDialog ld = new BuyDialog((int)fc.Typ,fc.Title,parentError);
					if(ld.Run() == (int)Gtk.ResponseType.Ok){
						
					}
					ld.Destroy();
					return true;
					//return false;
				}
			}

			return true;
		}

		public bool GetLicenses(){
			
			string URL =licenceUrl;		
			SystemWebClient client = new SystemWebClient();
			
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

		private static int GetStatusCode(SystemWebClient client, out string statusDescription)
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

		public class FrameworkClass{
			public FrameworkClass(string name, string code,string title,LicenceTyp typ){
				Name = name;
				Code = code;
				Typ = typ;
				Title = title;
			}
			public string Name;
			public string Code;
			public string Title;
			public LicenceTyp Typ;
							
		} 

		public enum LicenceTyp{
			BASIC = -200,
			PROFESIONAL = -300
		}
	}
}

