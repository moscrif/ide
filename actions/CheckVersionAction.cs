using System;
using System.Diagnostics;
using MessageDialogs = Moscrif.IDE.Controls.MessageDialog;
using Moscrif.IDE.Iface.Entities;
using Moscrif.IDE.Option;
using Moscrif.IDE.Iface;

namespace Moscrif.IDE.Actions
{
	public class CheckVersionAction: Gtk.Action
	{
		public CheckVersionAction():base("checkversion",MainClass.Languages.Translate("menu_check_version"),MainClass.Languages.Translate("menu_title_check_version"),null){
		}

		private bool test = false;

		protected override void OnActivated ()
		{
			base.OnActivated ();

			string version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

			VersionChecker vers = new VersionChecker();

			if(MainClass.User != null)
				vers.CheckVersion(version,MainClass.User.Token,CheckVersionWriteLogin,test);
			else vers.CheckVersion(version,"",CheckVersionWriteNoLogin,test);
		}

		public void CheckVersionWriteLogin(object sender, string  message, string  version,string  systemError)
		{
			if( !String.IsNullOrEmpty(version) ){

				string fileName = System.IO.Path.GetFileNameWithoutExtension(version);

				Gtk.Application.Invoke(delegate {
					MessageDialogs md =
						new MessageDialogs(MessageDialogs.DialogButtonType.YesNo, MainClass.Languages.Translate("new_version_is",fileName),MainClass.Languages.Translate("download_version") , Gtk.MessageType.Info,null);
					int res= md.ShowDialog();

					if(res==(int)Gtk.ResponseType.Yes){

						RunUpdater(MainClass.User.Token);
					}
				});

			} else  if (String.IsNullOrEmpty(version) ) {
				int i = systemError.LastIndexOf(":");
				string msg = systemError;
				if(i>-1)
					msg = systemError.Substring(i+1);

				if(msg.Trim().ToLower() == "(410) gone."){
					ShowModalError(MainClass.Languages.Translate("your_account_expired"),"");

				} else ShowModalError(message,systemError);
			}
		}


		public void CheckVersionWriteNoLogin(object sender, string  message, string  version,string  systemError)
		{
			if( !String.IsNullOrEmpty(version) ){

				string fileName = System.IO.Path.GetFileNameWithoutExtension(version);

				Gtk.Application.Invoke(delegate {
					MessageDialogs md =
						new MessageDialogs(MessageDialogs.DialogButtonType.YesNo, MainClass.Languages.Translate("new_version_is",fileName),MainClass.Languages.Translate("download_version"), Gtk.MessageType.Question,null);
					int res= md.ShowDialog();

					if(res==(int)Gtk.ResponseType.Yes){
						RunUpdater("");
					}
				});

			} else {
				ShowModalError(message,systemError);
			}
		}

		private void RunUpdater(string token){

			MainClass.MainWindow.InicializeQuit();

			if (MainClass.Platform.IsWindows){
				Process proc=new Process();

				proc.StartInfo.FileName= System.IO.Path.Combine(MainClass.Paths.AppPath,"moscrif-updater.exe");
				proc.StartInfo.Arguments = " -t:autom";
				if( !String.IsNullOrEmpty(token))
					proc.StartInfo.Arguments += " -u:"+token;

				proc.Start();
				MainClass.MainWindow.QuitApplication();
			} else {
				Process proc=new Process();

				proc.StartInfo.FileName= "mono";//System.IO.Path.Combine(MainClass.Paths.AppPath,"moscrif-updater.exe");
				proc.StartInfo.Arguments = System.IO.Path.Combine(MainClass.Paths.AppPath,"moscrif-updater.exe") +" -t:autom";

				if( !String.IsNullOrEmpty(token))
					proc.StartInfo.Arguments += " -u:"+token;

				proc.Start();
				MainClass.MainWindow.QuitApplication();

			}
		}

		private void ShowModalError(string message,string message2){

			Gtk.Application.Invoke(delegate {
				MessageDialogs md =
					new MessageDialogs(MessageDialogs.DialogButtonType.Ok, message, message2, Gtk.MessageType.Info,null);
				md.ShowDialog();
			});

		}
	}
}

