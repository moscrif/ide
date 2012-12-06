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
	public class LoggingInfo
	{
		public LoggingInfo() {
			
		}
		string loggUrl = MainClass.Settings.loggUrl ;
		string feedbackUrl = MainClass.Settings.feedbackUrl ;

		private  string token;
		private ActionId action;
		private string data;

		
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

		public void LoggWebThread(ActionId action){
			LoggWebThread(action,"");
		}

		public void LoggWebThread(ActionId action,string data){
			if((MainClass.Settings.Account == null) || (String.IsNullOrEmpty(MainClass.Settings.Account.Token))){
				return;
			}

			this.token = MainClass.Settings.Account.Token;
			this.action = action;
			this.data = data;
			
			Thread runLog = new Thread(new ThreadStart(LoggWeb));
			//filllStartPageThread.Priority = ThreadPriority.Normal;
			runLog.Name = "FilllStartPage";
			runLog.IsBackground = true;
			runLog.Start();
		}
		
		public void LoggWeb(){
			
			LoggWeb(this.token,this.action,this.data,null,null);
		}
		
		public bool LoggWeb(string token,ActionId action,string data,LoginYesTaskHandler loggYesTask,LoginNoTaskHandler loggNoTask){
			string URL =loggUrl;
			
			SystemWebClient client = new SystemWebClient();
			
			if( !string.IsNullOrEmpty(token))
				URL = String.Format(URL+"?token={0}&action={1}&data={2}",token,(int)action,data);
			else {
				return false;
			}	
			//Console.WriteLine(URL);
			
			client.DownloadStringCompleted+= delegate(object sender, DownloadStringCompletedEventArgs e) {
				
				if (e.Cancelled){
					if(loggNoTask!= null) loggNoTask(null,"Wrong username or password.");
					return;
				}
				if (e.Error != null){
					if(loggNoTask!= null) loggNoTask(null,"Wrong username or password.");
					return;
				}
				string result = e.Result;
			};
			
			try{
				client.DownloadStringAsync(new Uri(URL));
				
			}catch(Exception ex){
				string statusDesc = "";
				GetStatusCode(client,out statusDesc);
				Console.WriteLine(ex.Message);

				return false;
			}
			return true;
		}
		
		public bool LoggWebFile(ActionId action,string file,LoginYesTaskHandler loggYesTask,LoginNoTaskHandler loggNoTask){

			if((MainClass.Settings.Account == null) || (String.IsNullOrEmpty(MainClass.Settings.Account.Token))){
				return false;
			}
			string token =MainClass.Settings.Account.Token; 

			string URL =loggUrl;
			
			SystemWebClient client = new SystemWebClient();
			
			if( !string.IsNullOrEmpty(token))
				URL = String.Format(URL+"?token={0}&action={1}",token,(int)action);
			else {
				return false;
			}
			
			//Console.WriteLine(URL);
			
			/*client.UploadStringCompleted+= delegate(object sender, UploadStringCompletedEventArgs e) {
				
				if (e.Cancelled){
					Console.WriteLine("e.Cancelled->"+e.Cancelled);
					if(loggNoTask!= null) loggNoTask(null,"Wrong username or password.");
					return;
				}
				if (e.Error != null){
					Console.WriteLine("e.Error->"+e.Error);
					if(loggNoTask!= null) loggNoTask(null,"Wrong username or password.");
					return;
				}
				string result = e.Result;
				Console.WriteLine("e.Result->"+e.Result);
			};*/

			string data = "";
			try{
				using (StreamReader fileSR = new StreamReader(file)) {
					data = fileSR.ReadToEnd();
					fileSR.Close();
					fileSR.Dispose();
				}
				
				string resp = client.UploadString(new Uri(URL),data);
				Console.WriteLine(resp);
				if(loggYesTask!= null) loggYesTask(null,null);
				
			}catch(Exception ex){
				string statusDesc = "";
				GetStatusCode(client,out statusDesc);
				Console.WriteLine(ex.Message);
				if(loggNoTask!= null) loggNoTask(null,ex.Message);
				
				return false;
			}
			return true;
		}

		public bool LoggWebString(ActionId action,string str,LoginYesTaskHandler loggYesTask,LoginNoTaskHandler loggNoTask){
			
			if((MainClass.Settings.Account == null) || (String.IsNullOrEmpty(MainClass.Settings.Account.Token))){
				return false;
			}
			string token =MainClass.Settings.Account.Token; 
			
			string URL =loggUrl;
			
			SystemWebClient client = new SystemWebClient();
			
			if( !string.IsNullOrEmpty(token))
				URL = String.Format(URL+"?token={0}&action={1}",token,(int)action);
			else {
				return false;
			}
			
			//Console.WriteLine(URL);
			
			/*client.UploadStringCompleted+= delegate(object sender, UploadStringCompletedEventArgs e) {
				
				if (e.Cancelled){
					Console.WriteLine("e.Cancelled->"+e.Cancelled);
					if(loggNoTask!= null) loggNoTask(null,"Wrong username or password.");
					return;
				}
				if (e.Error != null){
					Console.WriteLine("e.Error->"+e.Error);
					if(loggNoTask!= null) loggNoTask(null,"Wrong username or password.");
					return;
				}
				string result = e.Result;
				Console.WriteLine("e.Result->"+e.Result);
			};*/
			
			string data = str;
			try{
				string resp = client.UploadString(new Uri(URL),data);
				Console.WriteLine(resp);
				if(loggYesTask!= null) loggYesTask(null,null);
				
			}catch(Exception ex){
				string statusDesc = "";
				GetStatusCode(client,out statusDesc);
				Console.WriteLine(ex.Message);
				if(loggNoTask!= null) loggNoTask(null,ex.Message);
				
				return false;
			}
			return true;
		}

		public delegate void LoginYesTaskHandler(object sender, Account account);
		public delegate void LoginNoTaskHandler(object sender, string message);

		public bool SendFeedback(string xmlData,LoginYesTaskHandler loggYesTask,LoginNoTaskHandler loggNoTask){
			
			if((MainClass.Settings.Account == null) || (String.IsNullOrEmpty(MainClass.Settings.Account.Token))){
				return false;
			}
			string token =MainClass.Settings.Account.Token; 
			
			string URL =feedbackUrl;
			
			SystemWebClient client = new SystemWebClient();
			
			if( !string.IsNullOrEmpty(token))
				URL = String.Format(URL+"?token={0}&action={1}",token,(int)action);
			else {
				return false;
			}
			
			//string data = "";
			try{

				string resp = client.UploadString(new Uri(URL),xmlData);
				Console.WriteLine(resp);
				if(loggYesTask!= null) loggYesTask(null,null);
				
			}catch(Exception ex){
				string statusDesc = "";
				GetStatusCode(client,out statusDesc);
				Console.WriteLine(ex.Message);
				if(loggNoTask!= null) loggNoTask(null,ex.Message);
				
				return false;
			}
			return true;
		}


		public enum  ActionId {
			IDELogin = 100,
			IDEStart = 101,
			IDEEnd = 102,
			IDEPublish = 103,
			IDENewProject = 104,
			IDECrush = 105,
			IDERequestHelp = 106,
			IDEImportProject = 107
			
		}
	}
}


