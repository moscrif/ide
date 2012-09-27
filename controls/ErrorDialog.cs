using System;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Specialized;
using MessageDialogs = Moscrif.IDE.Controls.MessageDialog;
using Moscrif.IDE.Iface.Entities;
using Moscrif.IDE.Settings;
using Moscrif.IDE.Iface;
using System.IO;

namespace Moscrif.IDE.Controls
{
	public partial class ErrorDialog : Gtk.Dialog
	{
		public ErrorDialog()
		{
			this.Build();
			this.Title="Error";
			Pango.FontDescription customFont = lblLabel.Style.FontDescription.Copy();//  Pango.FontDescription.FromString(MainClass.Settings.ConsoleTaskFont);
			customFont.Size = 24;
			customFont.Weight = Pango.Weight.Bold;
			lblLabel.ModifyFont(customFont);
			//imgError.SetFromStock ("gtk-dialog-error", Gtk.IconSize.Dialog);
			lblStatus.ModifyFont(customFont);
			lblStatus.LabelProp = "";
			this.ShowAll();
		}

		public string LabelText {
			set{}
		}

		public string ErrorMessage {
			set{
				tvError.Buffer.Text = value;
			}
		}

		public void LoginYesWrite(object sender, Account  account)
		{
			this.Respond(Gtk.ResponseType.Ok);
		}
		
		public void LoginNoWrite(object sender, string  message)
		{
			Console.WriteLine(message);
			lblStatus.LabelProp = MainClass.Languages.Translate("error_error");

			MessageDialogs md =
				new MessageDialogs(MessageDialogs.DialogButtonType.Ok, MainClass.Languages.Translate("error_error"),message, Gtk.MessageType.Error,null);
			md.ShowDialog();
			this.Respond(Gtk.ResponseType.Ok);
		}

		protected void OnButtonOkClicked (object sender, EventArgs e)
		{
			if((MainClass.Settings.Account == null) || (String.IsNullOrEmpty(MainClass.Settings.Account.Token))){
				this.Respond(Gtk.ResponseType.Ok);
			}

			lblStatus.LabelProp = "Sending...";
			buttonOk.Sensitive = false;
			while (Gtk.Application.EventsPending ())
				Gtk.Application.RunIteration ();

			string logFile =System.IO.Path.Combine(MainClass.Paths.SettingDir,"moscrif-ide.log");
			Tool.Logger.Close();
			LoggingInfo log = new LoggingInfo();
			log.LoggWebFile (LoggingInfo.ActionId.IDECrush,logFile,LoginYesWrite,LoginNoWrite);
			//this.Respond(Gtk.ResponseType.Ok);
			//string log2 ="C:/moscrif-ide.log";
			//string text = "";

			/*try{
				File.Copy(log,log2);
			} catch{

			}
			this.Respond(Gtk.ResponseType.Ok);*/
			/*using (StreamReader sr = new StreamReader(log)){
				text = sr.ReadToEnd();
			}*/

			//throw new System.NotImplementedException ();
		}
	}

}

