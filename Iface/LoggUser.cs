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
using Moscrif.IDE.Option;

namespace Moscrif.IDE.Iface
{
	public class LoggUser
	{
		public LoggUser() {

		}

		string redgisterUrl = MainClass.Settings.redgisterUrl ;
		string pingUrl = MainClass.Settings.pingUrl ;
		string loginUrl = MainClass.Settings.loginUrl ;
		string SALT = Security.SALT;

		public void Register(string email,string login,string password,LoginYesTaskHandler loggYesTask,LoginNoTaskHandler loggNoTask){
			string URL = redgisterUrl;
			SystemWebClient client = new SystemWebClient();

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

				string token = "";
				string licence = "";
				ParseResult(result, ref token,ref licence);

				if(String.IsNullOrEmpty(token)){
					if(loggNoTask!= null) loggNoTask(null,"Register failed.");
					return;
				}

				Account ac = CreateAccount(token);
				
				if( ac!= null ){
					ac.Login = login ;
					Licenses lsl = Licenses.LoadLicenses(licence);

					if(lsl!= null && lsl.Items.Count>0){
						ac.LicenseId = lsl.Items[0].Typ;
					}else {
						ac.LicenseId = "-100";
					}
					
					if(loggYesTask!= null) loggYesTask(null,ac);
				} else {
					if(loggNoTask!= null) loggNoTask(null,"Register failed.");
					return;
				}

				/*Account ac = CreateAccount(result);
				if(ac!= null ){
					ac.Login = login;
					if(loggYesTask!= null) loggYesTask(null,ac);
					
				} else {
					if(loggNoTask!= null) loggNoTask(null,"Login failed.");
					return;
				}*/

			};
			client.UploadStringAsync(new Uri(URL),data);
		}

		public bool Ping(string token){
      			string URL =pingUrl;
			string result;

			SystemWebClient client = new SystemWebClient();

			if( !string.IsNullOrEmpty(token))
				URL = String.Format(URL+"?t={0}",token);
			else {
				return false;
			}			
			try{
				result = client.DownloadString(new Uri(URL));

				if(!string.IsNullOrEmpty(result)){
					Licenses lc = Licenses.LoadLicenses(result);
					//MainClass.User.Licenses = lc;
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
			
		public void CheckLogin(string name, string password,LoginYesTaskHandler loggYesTask,LoginNoTaskHandler loggNoTask){
      			string URL = loginUrl;
			SystemWebClient client = new SystemWebClient();

			string data = String.Format("{0}\n{1}",name,GetMd5Sum(password+SALT)); //\n{2}\n{3}",name,GetMd5Sum(password+SALT),Environment.MachineName,Environment.UserName);
			try{
				string result =  client.UploadString(new Uri(URL),data);
				string token = "";
				string licence = "";
				ParseResult(result, ref token,ref licence);
					
				if(String.IsNullOrEmpty(token)){
					if(loggNoTask!= null) loggNoTask(null,"Wrong username or password.");
					return;
				}

				Account ac = CreateAccount(token);

				if( ac!= null ){
					ac.Login = name ;
					Licenses lsl = Licenses.LoadLicenses(licence);
					//ac.Licenses = lsl;
					if(lsl!= null && lsl.Items.Count>0){
						ac.LicenseId = lsl.Items[0].Typ;
					} else {
						ac.LicenseId = "-100";
					}

					if(loggYesTask!= null) loggYesTask(null,ac);
				} else {
					if(loggNoTask!= null) loggNoTask(null,"Wrong username or password.");
					return;
				}
				
			} catch (Exception ex){
				Logger.Error(ex.Message);

				//if(loggNoTask!= null) loggNoTask(null,"Wrong username or password.");
				if(loggNoTask!= null) loggNoTask(null,ex.Message);
				return;
			}

		}

		private void ParseResult (string input, ref string token, ref string licence){

			int indx = input.IndexOf("<token>");
			int indxLast = input.LastIndexOf("</token>");
			//Console.WriteLine(result);

			if((indx>-1)&&(indxLast>-1)){
				//result = result.Substring(
				Regex regx = new Regex("<token>.*?</token>");
				MatchCollection mc = regx.Matches(input);
				if(mc.Count>0){
					token = mc[0].Value;
					token= token.Replace("<token>",string.Empty);
					token= token.Replace("</token>",string.Empty);
				}
				licence = regx.Replace(input,String.Empty);
			}
		}

		/*	
		public void CheckLoginII(string name, string password,LoginYesTaskHandler loggYesTask,LoginNoTaskHandler loggNoTask){
      			string URL = loginUrl;

			SystemWebClient client = new SystemWebClient();

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

