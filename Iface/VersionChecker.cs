using System;
using System.Net;
using System.IO;
using System.Timers;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Specialized;
using Moscrif.IDE.Iface.Entities;
using System.Security.Cryptography;
using Moscrif.IDE.Tool;
	
namespace Moscrif.IDE.Iface
{
	public class VersionChecker
	{
		public VersionChecker() {
		}

		//private static Paths paths = new Paths();
		string checkVersion =  MainClass.Settings.checkVersion ;//"http://moscrif.com/ide/checkVersion.ashx?v={0}";	//localhost:1667		;
		string getVersion =MainClass.Settings.getVersion ;// "http://moscrif.com/ide/getVersion.ashx?v={0}";


		public void CheckVersion(string version,string token,CheckVersionEndTaskHandler checkVersionEnd,bool test){
			string URL = checkVersion;//"http://moscrif.com/ide/checkVersion.ashx?v={0}";
			SystemWebClient client = new SystemWebClient();
			
			string[] versions = version.Split('.');
			if (versions.Length != 4){
				if(checkVersionEnd!= null) checkVersionEnd(null,"Invalid Version.","","");
				return;
			}
			/*int iFix = 0;
			
			if(Int32.TryParse(versions[3],out iFix)){
				iFix = 0;
			}
			string sFix = "";
			if(iFix>0){
				sFix = ((char)(0-1+'a')).ToString();
			}*/
						
			string webVersion = MainClass.Tools.VersionConverter(version);//String.Format("{0}q{1}{2}", versions[0], versions[1],sFix);
			//Console.WriteLine("webVersion " + webVersion);
			
			if(String.IsNullOrEmpty(token)){
				URL = String.Format(URL,webVersion);
			} else {
				URL = MainClass.Settings.checkVersionLog;
				URL = String.Format(URL+"&t={1}",webVersion,token);
			}
			if(test){
				URL = URL+"&test=1";
			}

			//client.Do

			URL = String.Format(URL,version,token);
			
			client.OpenReadCompleted += delegate(object sender, OpenReadCompletedEventArgs e) {//UploadStringCompleted+= delegate(object sender, UploadStringCompletedEventArgs e) {
							
				if (e.Cancelled){
					Logger.Error("Error check version Cancelled: {0}",e.Cancelled);
					if(checkVersionEnd!= null){
						checkVersionEnd(null,MainClass.Languages.Translate("check_version_failed"),"","");
					}
					return;
				}

				if (e.Error != null){
					Logger.Error("Error check version : {0}",e.Error.Message);
					if(checkVersionEnd!= null){
						checkVersionEnd(null,MainClass.Languages.Translate("check_version_failed"),"",e.Error.Message);
					}
					return;
				}


				StreamReader reader = new StreamReader( e.Result );
				string result = reader.ReadToEnd();

				if(!String.IsNullOrEmpty(result) ){
					if(checkVersionEnd!= null) checkVersionEnd(null,"New version is found.",result,"");

				} else {
					if(checkVersionEnd!= null) checkVersionEnd(null,MainClass.Languages.Translate("new_version_not_found"),"","");
					return;
				}

			};
			client.OpenReadAsync(new Uri(URL));
		}
		
		public delegate void CheckVersionEndTaskHandler(object sender, string mesage, string newVersion,string systemError);
		/*
		public string VersionConverter(string version){
			
			string[] versions = version.Split('.');
			string versionFile = System.IO.Path.Combine(paths.AppPath,"version.txt");
			string sFix = "";
			
			if(System.IO.File.Exists(versionFile)){
				try {
					using (StreamReader file = new StreamReader(versionFile)) {
						string text = file.ReadToEnd();
						sFix = text.Trim();
					}
				} catch {
				}						
			}
			

			string webVersion = String.Format("{0}q{1}{2}", versions[0], versions[1],sFix);
			return webVersion;			
		}
		*/
		
		public string CheckVersionSynch(string version,string token, out string newVersion){
			newVersion = String.Empty;
			string URL = getVersion;//"http://moscrif.com/ide/checkVersion.ashx?v={0}";
			SystemWebClient client = new SystemWebClient();
			
			string[] versions = version.Split('.');
			if (versions.Length != 4){
				return "Invalid Version.";
			}
			/*
			int iFix = 0;
			
			if(Int32.TryParse(versions[3],out iFix)){
				iFix = 0;
			}
			string sFix = "";
			if(iFix>0){
				sFix = ((char)(0-1+'a')).ToString();
			}*/
						
			string webVersion = MainClass.Tools.VersionConverter(version);//  String.Format("{0}q{1}{2}", versions[0], versions[1],sFix);
			
			if(String.IsNullOrEmpty(token)){
				URL = String.Format(URL,webVersion);
			} else {
				URL =MainClass.Settings.getVersionLog;
				URL = String.Format(URL+"&t={1}",webVersion,token);
			}
			
			URL = String.Format(URL,version,token);
			
			Stream strResult = client.OpenRead(new Uri(URL));

			StreamReader reader = new StreamReader( strResult );
			string result = reader.ReadToEnd();

			if(!String.IsNullOrEmpty(result) ){
				newVersion = result;
				return "New version is found.";
				
			} else {
				return "New version is not found.";
			}
					
		}		
		
	/*	public string GetVersionSynch(string version,string token, out Stream file){
			file = null;
			string URL = "http://moscrif.com/ide/getVersion.ashx?v={0}";			
			SystemWebClient client = new SystemWebClient();
			
			
			if(String.IsNullOrEmpty(token)){
				URL = String.Format(URL,version);
			} else {
				URL = String.Format(URL+"&t={1}",version,token);
			}
			
			URL = String.Format(URL,version,token);
			//Console.WriteLine("URL ->{0}",URL);	
				
			Stream strResult = client.OpenRead(new Uri(URL));
			
			file = strResult ;
			return "Ok.";		
		}		
	*/	
	}
}

