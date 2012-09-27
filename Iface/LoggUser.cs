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
using Moscrif.IDE.Settings;

namespace Moscrif.IDE.Iface
{
	public class LoggUser
	{
		public LoggUser() {

		}

		string redgisterUrl = MainClass.Settings.checkVersion ;
		string pingUrl = MainClass.Settings.pingUrl ;
		string loggUrl = MainClass.Settings.loggUrl ;
		string loginUrl = MainClass.Settings.loginUrl ;
		string SALT = Security.SALT;

		public void Register(string email,string login,string password,LoginYesTaskHandler loggYesTask,LoginNoTaskHandler loggNoTask){
			string URL = redgisterUrl;
			WebClient client = new WebClient();

			string data = String.Format("{0}\n{1}\n{2}",email,login,GetMd5Sum(password+SALT));
			client.UploadStringCompleted+= delegate(object sender, UploadStringCompletedEventArgs e) {


				if (e.Cancelled){
					if(loggNoTask!= null) loggNoTask(null,"Register failed.");
					return;
				}

				if (e.Error != null){
					if(loggNoTask!= null) loggNoTask(null,"Register failed.");
					return;
				}
				string result = e.Result;

				Account ac = CreateAccount(result);
				if(ac!= null ){
					ac.Login = login;
					if(loggYesTask!= null) loggYesTask(null,ac);
					
				} else {
					if(loggNoTask!= null) loggNoTask(null,"Login failed.");
					return;
				}

			};
			client.UploadStringAsync(new Uri(URL),data);
		}

		public bool Ping(string token){
      			string URL =pingUrl;

			WebClient client = new WebClient();

			if( !string.IsNullOrEmpty(token))
				URL = String.Format(URL+"?t={0}",token);
			else {
				return false;
			}			
			try{
				string aaa = client.DownloadString(new Uri(URL));
			}catch(Exception ex){
				string statusDesc = "";
				GetStatusCode(client,out statusDesc);
				Console.WriteLine(ex.Message);
				Logger.Error(ex.Message);

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
			
		public void CheckLogin(string name, string password,LoginYesTaskHandler loggYesTask,LoginNoTaskHandler loggNoTask){
      			string URL = loginUrl;
			Console.WriteLine(URL);
			WebClient client = new WebClient();

			string data = String.Format("{0}\n{1}",name,GetMd5Sum(password+SALT)); //\n{2}\n{3}",name,GetMd5Sum(password+SALT),Environment.MachineName,Environment.UserName);
			try{
				string result =  client.UploadString(new Uri(URL),data);
				Account ac = CreateAccount(result);
				if( ac!= null ){
					ac.Login = name ;
					if(loggYesTask!= null) loggYesTask(null,ac);
				} else {
					if(loggNoTask!= null) loggNoTask(null,"Wrong username or password.");
					return;
				}
				
			} catch (Exception ex){
				Logger.Error(ex.Message);

				if(loggNoTask!= null) loggNoTask(null,"Wrong username or password.");
				return;
			}

		}	
		public void CheckLoginII(string name, string password,LoginYesTaskHandler loggYesTask,LoginNoTaskHandler loggNoTask){
      			string URL = loginUrl;

			WebClient client = new WebClient();

			string data = String.Format("{0}\n{1}\n{2}\n{3}",name,GetMd5Sum(password+SALT),Environment.MachineName,Environment.UserName);
			client.UploadStringCompleted+= delegate(object sender, UploadStringCompletedEventArgs e) {

				if (e.Cancelled){
					if(loggNoTask!= null) loggNoTask(null,"Wrong username or password.");
					return;
				}
				if (e.Error != null){
					if(loggNoTask!= null) loggNoTask(null,"Wrong username or password.");
					return;
				}
				string result = e.Result;
				Account ac = CreateAccount(result);
				if( ac!= null ){
					ac.Login = name ;
					if(loggYesTask!= null) loggYesTask(null,ac);
				} else {
					if(loggNoTask!= null) loggNoTask(null,"Wrong username or password.");
					return;
				}
			};

			client.UploadStringAsync(new Uri(URL),data);
		}
		
		private Account CreateAccount(string response){

			if(!String.IsNullOrEmpty(response)){
				string[] responses = response.Replace('\r', '\0').Split('\n');
				Account a = new Account();
				a.Token = responses[0];
				return a;
			} else return null;
		}
		
		public string GetMd5Sum(string str) {

			byte[] input = ASCIIEncoding.ASCII.GetBytes(str);
        		byte[] output = MD5.Create().ComputeHash(input);
        		StringBuilder sb = new StringBuilder();

			for(int i=0;i<output.Length;i++) {
            			sb.Append(output[i].ToString("X2"));
        		}
        		return sb.ToString();
    		}
		
		public delegate void LoginYesTaskHandler(object sender, Account account);
		public delegate void LoginNoTaskHandler(object sender, string message);
	}
}

