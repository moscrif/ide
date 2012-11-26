using System;
using System.Net;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Specialized;
using MessageDialogs = Moscrif.IDE.Controls.MessageDialog;
using Moscrif.IDE.Iface.Entities;
using Moscrif.IDE.Settings;
using Moscrif.IDE.Iface;
using Moscrif.IDE.Components;
using System.Threading;
using Gtk;
using Moscrif.IDE.Tool;
using Moscrif.IDE.Components;

namespace Moscrif.IDE.Controls
{
	public class LoginRegisterDialog: Dialog
	{
		public event EventHandler LogginSucces;
		
		bool exitTrue = false;
		BannerButton bannerImage = new BannerButton ();
		Notebook notebook1;
		Entry entrLoginR;
		Entry entrPasswordR1;
		Entry entrPasswordR2;
		Entry entrEmailR;

		Entry entrLogin;
		Entry entrPassword;
		CheckButton chbRemember;
		Components.LinkButton linkbutton1;

		Button btnInfo;
		Button btnClose;

		public LoginRegisterDialog(Gtk.Window parentWindows)
		{
			if(parentWindows!=null)
				this.TransientFor = parentWindows;
			else 
				this.TransientFor = MainClass.MainWindow;

			this.BorderWidth = 10;

			notebook1 = new Notebook();
			btnInfo = new Button();
			btnInfo.Label = "Login";
			btnInfo.UseUnderline = true;
			btnInfo.CanFocus = true;

			btnClose = new Button();
			btnClose.CanDefault = true;
			btnClose.CanFocus = true;
			btnClose.Name = "buttonCancel";
			btnClose.UseStock = true;
			btnClose.UseUnderline = true;
			btnClose.Label = "gtk-cancel";
			btnClose.Clicked+= delegate(object sender, EventArgs e)
			{
				this.Respond(Gtk.ResponseType.Close);
			};

			entrLoginR = new Entry();
			entrPasswordR1 = new Entry();
			entrPasswordR1.Visibility = false;
			entrPasswordR2 = new Entry();
			entrPasswordR2.Visibility = false;
			entrEmailR = new Entry();
			
			entrLogin = new Entry();
			entrPassword = new Entry();
			entrPassword.Visibility = false;
			chbRemember = new CheckButton();
			chbRemember.Label = MainClass.Languages.Translate("loginDialog_remember");

			linkbutton1 = new Components.LinkButton();
			linkbutton1.HeightRequest=25;
			linkbutton1.UseWebStile = true;
			linkbutton1.LinkUrl = "http://moscrif.com/request-new-password";
			linkbutton1.Label = MainClass.Languages.Translate("loginDialog_forgot_password");

			Table mainTbl = new Table(2,1,false);
			mainTbl.RowSpacing = 6;
			mainTbl.ColumnSpacing = 6;
			//mainTbl.BorderWidth = 6;

			this.Title = MainClass.Languages.Translate("moscrif_ide_title_f1");

			
			if(MainClass.Settings.Account != null){
				if(!String.IsNullOrEmpty(MainClass.Settings.Account.Login))
					entrLogin.Text = MainClass.Settings.Account.Login;
				chbRemember.Active =MainClass.Settings.Account.Remember;
			}
			bannerImage.WidthRequest = 400;
			bannerImage.HeightRequest = 120;
			
			mainTbl.Attach(bannerImage,0,1,0,1,AttachOptions.Fill,AttachOptions.Shrink,0,0);

			Table tblPage1 = new Table(5,2,false);
			tblPage1.BorderWidth = 6;
			tblPage1.ColumnSpacing = 6;
			tblPage1.RowSpacing = 6;
			AddControl(ref tblPage1, 0,entrLogin,MainClass.Languages.Translate("loginDialog_login"));
			AddControl(ref tblPage1, 1,entrPassword,MainClass.Languages.Translate("loginDialog_password"));
			AddControl(ref tblPage1, 2,chbRemember,"");
			AddControl(ref tblPage1, 3,linkbutton1,"");

			notebook1.AppendPage(tblPage1,new Label ("Login"));

			Table tblPage2 = new Table(5,2,false);
			tblPage2.ColumnSpacing = 6;
			tblPage2.RowSpacing = 6;
			tblPage2.BorderWidth = 6;
			AddControl(ref tblPage2, 0,entrLoginR,MainClass.Languages.Translate("loginDialog_login"));
			AddControl(ref tblPage2, 1,entrPasswordR1,MainClass.Languages.Translate("loginDialog_reg_password1"));
			AddControl(ref tblPage2, 2,entrPasswordR2,MainClass.Languages.Translate("loginDialog_reg_password2"));
			AddControl(ref tblPage2, 3,entrEmailR,MainClass.Languages.Translate("loginDialog_reg_email"));

			notebook1.AppendPage(tblPage2,new Label ("Register"));

			Gtk.Notebook.NotebookChild nchT1 = (Gtk.Notebook.NotebookChild)this.notebook1[tblPage1];
			nchT1.TabExpand = true;
			nchT1.TabFill = false;

			Gtk.Notebook.NotebookChild nchT2 = (Gtk.Notebook.NotebookChild)this.notebook1[tblPage2];
			nchT2.TabExpand = true;
			nchT2.TabFill = false;

			mainTbl.Attach(notebook1,0,1,1,2,AttachOptions.Fill|AttachOptions.Expand,AttachOptions.Fill|AttachOptions.Expand,0,0);

			LoadDefaultBanner();
			
			Thread BannerThread = new Thread(new ThreadStart(BannerThreadLoop));
			
			BannerThread.Name = "BannerThread";
			BannerThread.IsBackground = true;
			BannerThread.Start();

			entrPassword.GrabFocus();
			//entrLogin.GrabFocus();

			this.VBox.Add(mainTbl);

			this.ActionArea.Add(btnClose);
			this.ActionArea.Add(btnInfo);

			btnInfo.Clicked += new System.EventHandler (this.OnBtnInfoClicked);
			this.notebook1.SwitchPage += new Gtk.SwitchPageHandler (this.OnNotebook1SwitchPage);
			this.entrPassword.KeyReleaseEvent += new Gtk.KeyReleaseEventHandler (this.OnEntrPasswordKeyReleaseEvent);
			this.entrLogin.KeyReleaseEvent += new Gtk.KeyReleaseEventHandler (this.OnEntrLoginKeyReleaseEvent);
			//this.BorderWidth = 6;
			this.HeightRequest = 370;
			this.ShowAll();
		}

		private void AddControl(ref Table tbl, int top, Widget ctrl, string label){
			Label lbl = new Label(label);
			lbl.Xalign = 1;
			lbl.Yalign = 0.5F;
			lbl.WidthRequest = 100;
			
			tbl.Attach(lbl,0,1,(uint)top,(uint)(top+1),AttachOptions.Shrink,AttachOptions.Shrink,2,2);
			tbl.Attach(ctrl,1,2,(uint)top,(uint)(top+1),AttachOptions.Fill|AttachOptions.Expand,AttachOptions.Fill,2,2);
		}

		private void Login()
		{
			if (String.IsNullOrEmpty(entrLogin.Text)){
				MessageDialogs md =
					new MessageDialogs(MessageDialogs.DialogButtonType.Ok, MainClass.Languages.Translate("login_is_required"), "", Gtk.MessageType.Error,this);
				md.ShowDialog();
				return;
			}
			if (String.IsNullOrEmpty(entrPassword.Text)){
				MessageDialogs md =
					new MessageDialogs(MessageDialogs.DialogButtonType.Ok, MainClass.Languages.Translate("password_is_required"), "", Gtk.MessageType.Error,this);
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
			if(exitTrue){
				if(LogginSucces!=null){
					LogginSucces(this,null);
				}
				this.Respond(Gtk.ResponseType.Ok);
			}
		}
		
		private void LoadDefaultBanner(){
			//hbMenuRight
			string bannerParth  = System.IO.Path.Combine(MainClass.Paths.ResDir,"banner");
			bannerParth = System.IO.Path.Combine(bannerParth,"banner1.png");
			if(File.Exists(bannerParth)){
				bannerImage.ImageIcon = new Gdk.Pixbuf(bannerParth);
				bannerImage.LinkUrl = "http://moscrif.com/download";
			}
		}
		
		private BannersSystem bannersSystem; 
		private void BannerThreadLoop()
		{
			bannersSystem = MainClass.BannersSystem;
			
			bool play = true;
			bool isBussy = false;
			int bnrIndex = 1;
			try {
				while (play) {
					if (!isBussy) {
						isBussy = true;
						//Banner bnr = bannersSystem.NextBanner();
						Banner bnr = bannersSystem.GetBanner(bnrIndex);
						if((bnr != null) && (bnr.BannerPixbuf != null)){
							Gtk.Application.Invoke(delegate{
								bannerImage.ImageIcon = bnr.BannerPixbufResized400;
								bannerImage.LinkUrl = bnr.Url;
							});
							
						} else {
							//Console.WriteLine("Banner is NULL");
						}
						if(bnrIndex< bannersSystem.GetCount-1)
							bnrIndex++;
						else 
							bnrIndex=0;
						isBussy = false;
					}
					Thread.Sleep (15003);
				}
			}catch(ThreadAbortException tae){
				Thread.ResetAbort ();
				Logger.Error("ERROR - Cannot run banner thread.");
				Logger.Error(tae.Message);
				LoadDefaultBanner();
			}finally{
				
			}
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
				//account.Login = entrLogin.Text;
				account.Remember = chbRemember.Active;
				MainClass.User = account;
				MainClass.MainWindow.SetLogin();
				exitTrue = true;
				
			} else {
				MainClass.MainWindow.SetLogin();
				ShowModalError(MainClass.Languages.Translate("login_failed"));
				exitTrue = false;
				return;
			}
		}
		
		private void Register()
		{
			if (String.IsNullOrEmpty(entrLoginR.Text)){
				MessageDialogs md =
					new MessageDialogs(MessageDialogs.DialogButtonType.Ok, MainClass.Languages.Translate("login_is_required"), "", Gtk.MessageType.Error,this);
				md.ShowDialog();
				return;
			}
			if (String.IsNullOrEmpty(entrPasswordR1.Text)){
				MessageDialogs md =
					new MessageDialogs(MessageDialogs.DialogButtonType.Ok, MainClass.Languages.Translate("password_is_required"), "", Gtk.MessageType.Error,this);
				md.ShowDialog();
				return;
			}
			if(entrPasswordR1.Text != entrPasswordR2.Text){
				MessageDialogs md =
					new MessageDialogs(MessageDialogs.DialogButtonType.Ok, MainClass.Languages.Translate("password_dont_match"), "", Gtk.MessageType.Error,this);
				md.ShowDialog();
				return;
			}
			
			if(!CheckEmail(entrEmailR.Text)){
				MessageDialogs md =
					new MessageDialogs(MessageDialogs.DialogButtonType.Ok, MainClass.Languages.Translate("email_address_invalid"), "", Gtk.MessageType.Error,this);
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
}

