using System;
using System.Net;
using System.IO;
using System.Timers;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Specialized;
using Moscrif.IDE.Iface.Entities;
using System.Security.Cryptography;

namespace Moscrif.IDE.Iface
{
	public class SignApp
	{
		//private static Paths paths = new Paths();
		string signUrl = MainClass.Settings.signUrl;// "http://moscrif.com/ide/signApp.ashx?t={0}&a={1}";
		
		public SignApp() {
		}
		
		public bool PostFile(string appFile,string token, out string signFile){//string email,string login,string password,LoginYesTaskHandler loggYesTask,LoginNoTaskHandler loggNoTask){
			
			string URL = signUrl;//"http://moscrif.com/ide/signApp.ashx?t={0}&a={1}";
					
			if( !string.IsNullOrEmpty(token)){
					string app = System.IO.Path.GetFileNameWithoutExtension(appFile);
					if (app.ToLower().EndsWith(".app"))
						app = app.Substring(0, app.Length - 4);
					URL = String.Format(URL,token,app);
				//URL = String.Format(URL+"?t={0}&a=" + "unsigned.app-hash",token);
			}else {
				signFile = "ERROR TOKEN";
				return false;
			}
			//Console.WriteLine("URL -> {0}", URL);
			
			WebClient client = new WebClient();
			
			string data ="";
			
			using (StreamReader file = new StreamReader(appFile)) {
				data = file.ReadToEnd();
				file.Close();
				file.Dispose();
			}
				//Console.WriteLine("data -> {0}", data);
			
			/*client.UploadStringCompleted+= delegate(object sender, UploadStringCompletedEventArgs e) {
				//Console.WriteLine(1);
				
				if (e.Cancelled){
					if(loggNoTask!= null) loggNoTask(null,"Register to failed.");
					return;
				}

				if (e.Error != null){
					if(loggNoTask!= null) loggNoTask(null,"Register to failed.");
					return;
				}

				string result = e.Result;
				//Console.WriteLine(result);
				Account ac = CreateAccount(result);
				if(ac!= null ){
					ac.Login = login;
					if(loggYesTask!= null) loggYesTask(null,ac);
					
				} else {
					if(loggNoTask!= null) loggNoTask(null,"Login to failed.");
					return;
				}

			};*/
			//client.UploadStringAsync(new Uri(URL),data);
			signFile =  client.UploadString(new Uri(URL),data);//(new Uri(URL),data);
			
			if(signFile.StartsWith("Error")){
				return false;
			}
			
			//byte[] respons = client.UploadFile(new Uri(URL),appFile);//UploadString(new Uri(URL),data);
			
			//Console.WriteLine("Respons is -> {0}",respons);
			return true;
		}
		
	}
}

