using System;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Specialized;
using MessageDialogs = Moscrif.IDE.Controls.MessageDialog;
using Moscrif.IDE.Iface.Entities;
using Moscrif.IDE.Settings;
using Moscrif.IDE.Iface;

namespace Moscrif.IDE.Controls
{
	public partial class LoginDialog : Gtk.Dialog
	{
		//protected virtual void OnButtonOkClicked (object sender, System.EventArgs e)
		bool exitTrue = false;
		private void Login()
		{
			if (String.IsNullOrEmpty(entrLogin.Text)){
				MessageDialogs md =
					new MessageDialogs(MessageDialogs.DialogButtonType.Ok, MainClass.Languages.Translate("login_is_required"), "", Gtk.MessageType.Error);
				md.ShowDialog();
				return;
			}
			if (String.IsNullOrEmpty(entrPassword.Text)){
				MessageDialogs md =
					new MessageDialogs(MessageDialogs.DialogButtonType.Ok, MainClass.Languages.Translate("password_is_required"), "", Gtk.MessageType.Error);
				md.ShowDialog();
				return;
			}
			MainClass.User = null;

			//CheckLogin();
			try{
				LoggUser log = new LoggUser();
				log.CheckLogin(entrLogin.Text,entrPassword.Text,LoginYesWrite,LoginNoWrite);
			}catch(Exception ex){
				ShowModalError(ex.Message);
				return;
			}
			if(exitTrue)
				this.Respond(Gtk.ResponseType.Ok);
		}

		public void LoginNoWrite(object sender, string  message)
		{
			ShowModalError(message);
			exitTrue = false;
		}

		public void LoginYesWrite(object sender, Account  account)
		{
			//Console.WriteLine("LoginYesWrite");
			if( account!= null ){
				account.Login = entrLogin.Text;
				account.Remember = chbRemember.Active;
				MainClass.User = account;
				MainClass.MainWindow.SetLogin();
				exitTrue = true;
			} else {
				ShowModalError(MainClass.Languages.Translate("login_failed"));
				exitTrue = false;
				return;
			}
		}

		//protected virtual void OnButtonRegisterClicked (object sender, System.EventArgs e)
		private void Register()
		{

			if (String.IsNullOrEmpty(entrLoginR.Text)){
				MessageDialogs md =
					new MessageDialogs(MessageDialogs.DialogButtonType.Ok, MainClass.Languages.Translate("login_is_required"), "", Gtk.MessageType.Error);
				md.ShowDialog();
				return;
			}
			if (String.IsNullOrEmpty(entrPasswordR1.Text)){
				MessageDialogs md =
					new MessageDialogs(MessageDialogs.DialogButtonType.Ok, MainClass.Languages.Translate("password_is_required"), "", Gtk.MessageType.Error);
				md.ShowDialog();
				return;
			}
			if(entrPasswordR1.Text != entrPasswordR2.Text){
				MessageDialogs md =
					new MessageDialogs(MessageDialogs.DialogButtonType.Ok, MainClass.Languages.Translate("password_dont_match"), "", Gtk.MessageType.Error);
				md.ShowDialog();
				return;
			}

			if(!CheckEmail(entrEmailR.Text)){
				MessageDialogs md =
					new MessageDialogs(MessageDialogs.DialogButtonType.Ok, MainClass.Languages.Translate("email_address_invalid"), "", Gtk.MessageType.Error);
				md.ShowDialog();
				return;
			}
			try{
				LoggUser log = new LoggUser();
				log.Register(entrEmailR.Text,entrLoginR.Text,entrPasswordR1.Text,LoginYesWrite,LoginNoWrite);
			}catch(Exception ex){
				ShowModalError(ex.Message);
				return;
			}
			this.Respond(Gtk.ResponseType.Ok);
		}


		public LoginDialog(Gtk.Window parentWindows)
		{
			this.Build();
			this.HeightRequest = 205;
			if(parentWindows!=null)
				this.TransientFor = parentWindows;
			else
				this.TransientFor = MainClass.MainWindow;

			this.Title = MainClass.Languages.Translate("moscrif_ide_title_f1");
			btnInfo.Label = "Login";

			if(MainClass.Settings.Account != null){
				if(!String.IsNullOrEmpty(MainClass.Settings.Account.Login))
				entrLogin.Text = MainClass.Settings.Account.Login;
				chbRemember.Active =MainClass.Settings.Account.Remember;
			}

			entrLogin.GrabFocus();
		}

		/*
		public void CheckLogin(){
      			string URL = "http://moscrif.com/ide/checkLogin.ashx";

			//MoscrifWebClient client = new MoscrifWebClient();
			WebClient client = new WebClient();
			//client.Headers.Add("Content-Type","application/x-www-form-urlencoded");

			string data = String.Format("{0}\n{1}",entrLogin.Text,MainClass.Tools.GetMd5Sum(entrPassword.Text+SALT));
			//byte[] byteArray = Encoding.ASCII.GetBytes(data);
			client.UploadStringCompleted+= delegate(object sender, UploadStringCompletedEventArgs e) {

				if (e.Cancelled){
					ShowModalError("Wrong username or password.");
					return;
				}
				if (e.Error != null){
					ShowModalError("Wrong username or password.");
					return;
				}
				Console.WriteLine("Yes");
				string result = e.Result;
				Console.WriteLine(result);
				Account ac = CreateAccount(result);
				if( ac!= null ){
					ac.Login = entrLogin.Text;
					ac.Remember = chbRemember.Active;
					MainClass.User = ac;
					MainClass.MainWindow.SetLogin();

				} else {
					ShowModalError("Login to failed.");
					return;
				}
			};

			//string responseArray =
			//client.UploadStringAsync(new Uri(URL),data);
			client.UploadStringAsync(new Uri(URL),data);

        		//ASCIIEncoding enc = new ASCIIEncoding();
        		//string source = enc.GetString(responseArray);
			//Console.WriteLine(responseArray);

		}

		public void Register(){
			string URL = "http://moscrif.com/ide/registerLogin.ashx";
			WebClient client = new WebClient();

			string data = String.Format("{0}\n{1}\n{2}",entrEmailR.Text,entrLoginR.Text,MainClass.Tools.GetMd5Sum(entrPasswordR1.Text+SALT));
			client.UploadStringCompleted+= delegate(object sender, UploadStringCompletedEventArgs e) {
				Console.WriteLine(1);

				if (e.Cancelled){
					ShowModalError("Register to failed.");
					return;
				}

				if (e.Error != null){
					Console.WriteLine(e.Error.Message);
					Console.WriteLine(e.Error.Source);
					Console.WriteLine(e.Error.StackTrace);

					if(e.Error.InnerException != null){
						Console.WriteLine(e.Error.InnerException);
					}
					ShowModalError("Register to failed. 2");
					return;
				}

				string result = e.Result;
				Console.WriteLine(result);
				Account ac = CreateAccount(result);
				if(ac!= null ){
					ac.Login = entrLoginR.Text;
					MainClass.User = ac;
					MainClass.MainWindow.SetLogin();
				} else {
					ShowModalError("Login to failed.");
					return;
				}

			};
			client.UploadStringAsync(new Uri(URL),data);
		}
*/
		private void ShowModalError(string message){

			Gtk.Application.Invoke(delegate {
				MessageDialogs md =
					new MessageDialogs(MessageDialogs.DialogButtonType.Ok, message, "", Gtk.MessageType.Error,this);
				md.ShowDialog();
			});

		}

		public bool CheckEmail(string emailAddress){
                 	string patternStrict = @"^(([^<>()[\]\\.,;:\s@\""]+"
                        + @"(\.[^<>()[\]\\.,;:\s@\""]+)*)|(\"".+\""))@"
                        + @"((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}"
                        + @"\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+"
                        + @"[a-zA-Z]{2,}))$";

			Regex reStrict = new Regex(patternStrict);
                 	bool isStrictMatch = reStrict.IsMatch(emailAddress);
                  	return isStrictMatch;
		}
		
		protected virtual void OnBtnInfoClicked (object sender, System.EventArgs e)
		{
			if(notebook1.CurrentPage == 1){
				Register();
			} else if(notebook1.CurrentPage == 0){
				Login();
			}
		}
		
		protected virtual void OnNotebook1SwitchPage (object o, Gtk.SwitchPageArgs args)
		{
			if(notebook1.CurrentPage == 1){
				btnInfo.Label = "Register";
			} else if(notebook1.CurrentPage == 0){
				btnInfo.Label = "Login";
			}
		}
		
		protected virtual void OnEntrLoginKeyReleaseEvent (object o, Gtk.KeyReleaseEventArgs args)
		{
			if (args.Event.Key == Gdk.Key.Return) {
				Login();
			}
		}

		protected virtual void OnEntrPasswordKeyReleaseEvent (object o, Gtk.KeyReleaseEventArgs args)
		{
			if (args.Event.Key == Gdk.Key.Return) {
				Login();
			}
		}
		
		



	}

/*	public class MoscrifWebClient : WebClient
	{
    		private CookieContainer m_container = new CookieContainer();
    		protected override WebRequest GetWebRequest(Uri address){
        		WebRequest request = base.GetWebRequest(address);
        		if (request is HttpWebRequest){
            			(request as HttpWebRequest).CookieContainer = m_container;
        		}
        	return request;
    		}
	}*/
}

