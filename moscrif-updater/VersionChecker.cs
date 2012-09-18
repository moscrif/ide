using System;
using System.Net;
using System.IO;
using System.Timers;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Specialized;
using System.Security.Cryptography;
	
namespace Moscrif.Updater
{
	public class VersionChecker
	{
		public VersionChecker() {
		}
		
		private static Paths paths = new Paths();

		const string checkVersion = "http://moscrif.com/ide/checkVersion.ashx?v={0}";//"http://localhost:1667/ide/checkVersion.ashx?v={0}" ; 	
		const string getVersion = "http://moscrif.com/ide/getVersion.ashx?v={0}"; //"http://localhost:1667/ide/getVersion.ashx?v={0}";
		
		public void CheckVersion(string version,string token,CheckVersionEndTaskHandler checkVersionEnd){
			string URL = checkVersion;
			WebClient client = new WebClient();
			
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
						
			string webVersion = VersionConverter(version); //String.Format("{0}q{1}{2}", versions[0], versions[1],sFix);
			//Console.WriteLine("webVersion " + webVersion);
			
			if(String.IsNullOrEmpty(token)){
				URL = String.Format(URL,webVersion);
			} else {
				URL = String.Format(URL+"&t={1}",webVersion,token);
			}
			
			
			URL = String.Format(URL,version,token);
			
			client.OpenReadCompleted += delegate(object sender, OpenReadCompletedEventArgs e) {//UploadStringCompleted+= delegate(object sender, UploadStringCompletedEventArgs e) {
							
				if (e.Cancelled){
					if(checkVersionEnd!= null) checkVersionEnd(null,"Check version failed.","","");
					return;
				}

				if (e.Error != null){
					if(checkVersionEnd!= null) checkVersionEnd(null,"Check version failed.","",e.Error.Message);
					return;
				}
				
				
				StreamReader reader = new StreamReader( e.Result );
				string result = reader.ReadToEnd(); 
				
				if(!String.IsNullOrEmpty(result) ){
					if(checkVersionEnd!= null) checkVersionEnd(null,"New version is found.",result,"");
					
				} else {
					if(checkVersionEnd!= null) checkVersionEnd(null,"New version is not found.","","");
					return;
				}

			};
			client.OpenReadAsync(new Uri(URL));
						
		}
		
		public delegate void CheckVersionEndTaskHandler(object sender, string mesage, string newVersion,string systemError);
		
		public string VersionConverter(string version){
			
			string[] versions = version.Split('.');
			/*if (versions.Length != 4){
				return "Invalid Version.";
			}*/
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
			
			/*int iFix = 0;
			
			if(Int32.TryParse(versions[3],out iFix)){
				iFix = 0;
			}
			
			if(iFix>0){
				sFix = ((char)(iFix-1+'a')).ToString();
			}*/
						
			string webVersion = String.Format("{0}q{1}{2}", versions[0], versions[1],sFix);
			return webVersion;			
		}
		
		public string CheckVersionSynch(string version,string token, out string newVersion, bool isConverVersion,bool test){
			newVersion = String.Empty;
			string URL = checkVersion;//"http://moscrif.com/ide/checkVersion.ashx?v={0}";			
			WebClient client = new WebClient();
			
			/*
			int iFix = 0;
			
			if(Int32.TryParse(versions[3],out iFix)){
				iFix = 0;
			}
			string sFix = "";
			if(iFix>0){
				sFix = ((char)(0-1+'a')).ToString();
			}*/
			string webVersion = version;
			
			if(!isConverVersion){
				string[] versions = version.Split('.');
				if (versions.Length != 4){
					return "Invalid Version.";
				}
								
				webVersion = VersionConverter(version);//  String.Format("{0}q{1}{2}", versions[0], versions[1],sFix);
			}
			
			if(String.IsNullOrEmpty(token)){
				URL = String.Format(URL,webVersion);
			} else {
				URL = String.Format(URL+"&t={1}",webVersion,token);
			}
			
			if(test){
				URL = URL+"&test=1";
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
		/*
		public string GetVersionSynch(string version,string token, out Stream file){
			file = null;
			string URL = getVersion;//"http://moscrif.com/ide/getVersion.ashx?v={0}";			
			WebClient client = new WebClient();
			
			
			if(String.IsNullOrEmpty(token)){
				URL = String.Format(URL,version);
			} else {
				URL = String.Format(URL+"&t={1}",version,token);
			}					
			
			URL = String.Format(URL,version,token);
			Console.WriteLine("URL ->{0}",URL);			
			
			Stream strResult = client.OpenRead(new Uri(URL));
			
			file = strResult ;
			return "Ok.";	
		}		
	
		WebClient client = new WebClient();
		
		public bool IsAsynchonlyBusy{
			get{
				if(client !=null)
					return client.IsBusy;
				else return false;
			}
		}
		public delegate void GetVersionEndTaskHandler(object sender, string mesage, Stream newVersion,string systemError);		
		
		public void GetVersionAsynch(string version,string token, GetVersionEndTaskHandler getVersionEnd,bool test){
			//string file = null;
			string URL = getVersion;//"http://moscrif.com/ide/getVersion.ashx?v={0}";			
			client = new WebClient();
			
			
			if(String.IsNullOrEmpty(token)){
				URL = String.Format(URL,version);
			} else {
				URL = String.Format(URL+"&t={1}",version,token);
			}					
			
			if(test){
				URL = URL+"&test=1";
			}
			
			URL = String.Format(URL,version,token);
			Console.WriteLine("URL ->{0}",URL);			
			
			client.OpenReadCompleted += delegate(object sender, OpenReadCompletedEventArgs e) {//UploadStringCompleted+= delegate(object sender, UploadStringCompletedEventArgs e) {
							
				if (e.Cancelled){
					if(getVersionEnd!= null) getVersionEnd(null,"Get version failed.",null,"");
					return;
				}

				if (e.Error != null){
					if(getVersionEnd!= null) getVersionEnd(null,"Get version failed.",null,e.Error.Message);
					return;
				}
				if(getVersionEnd!= null) getVersionEnd(null,"New version is found.",e.Result,"");


			};	
			client.OpenReadAsync(new Uri(URL));
			
			//Stream strResult = client.OpenRead(new Uri(URL));
			
		//	client.O //(new Uri(URL));
			
			//file = strResult ;
			//return "Ok.";		
		}		
		*/
	}
}

