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
using Moscrif.IDE.Iface.Entities;

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
			string result;

			WebClient client = new WebClient();

			if( !string.IsNullOrEmpty(token))
				URL = String.Format(URL+"?t={0}",token);
			else {
				return false;
			}			
			try{
				result = client.DownloadString(new Uri(URL));

				if(!string.IsNullOrEmpty(result)){
					Licenses lc = Licenses.LoadLicenses(result);
					MainClass.User.Licenses = lc;
					if(lc!= null && lc.Items.Count>0){
						MainClass.User.LicenseId = lc.Items[0].Typ;
					}
				}

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
			//Console.WriteLine(URL);
			WebClient client = new WebClient();

			string data = String.Format("{0}\n{1}",name,GetMd5Sum(password+SALT)); //\n{2}\n{3}",name,GetMd5Sum(password+SALT),Environment.MachineName,Environment.UserName);
			try{
				string result =  client.UploadString(new Uri(URL),data);
				int indx = result.IndexOf("<token>");
				int indxLast = result.LastIndexOf("</token>");
				//Console.WriteLine(result);
				string token = "";
				if((indx>-1)&&(indxLast>-1)){
					//result = result.Substring(
					Regex regx = new Regex("<token>.*?</token>");
					MatchCollection mc = regx.Matches(result);
					if(mc.Count>0){
						token = mc[0].Value;
						token= token.Replace("<token>",string.Empty);
						token= token.Replace("</token>",string.Empty);
					}
					result = regx.Replace(result,String.Empty);
				}
				Account ac = CreateAccount(token);

				if( ac!= null ){
					ac.Login = name ;
					Licenses lsl = Licenses.LoadLicenses(result);
					ac.Licenses = lsl;
					if(lsl!= null && lsl.Items.Count>0){
						ac.LicenseId = lsl.Items[0].Typ;
					}

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
		/*	
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
		*/
		private Account CreateAccount(string response){

			if(!String.IsNullOrEmpty(response)){
				//Console.WriteLine(response);
				/*License lcs = License.LoadLicense(response);
				Account a = new Account();
				a.Token = lcs.Token;
				return a;*/
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

