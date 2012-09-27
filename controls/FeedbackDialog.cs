using System;
using Moscrif.IDE.Iface.Entities;
using Moscrif.IDE.Settings;
using Moscrif.IDE.Iface;
using System.IO;
using MessageDialogs = Moscrif.IDE.Controls.MessageDialog;

namespace Moscrif.IDE.Controls
{
	public partial class FeedbackDialog : Gtk.Dialog
	{
		public FeedbackDialog()
		{
			this.Build();
			Pango.FontDescription customFont = lblFeedback.Style.FontDescription.Copy();//  Pango.FontDescription.FromString(MainClass.Settings.ConsoleTaskFont);
			customFont.Size = 24;
			customFont.Weight = Pango.Weight.Bold;
			lblFeedback.ModifyFont(customFont);
			lblStatus.ModifyFont(customFont);
			lblStatus.LabelProp = "";
			//imgError.SetFromStock ("gtk-dialog-error", Gtk.IconSize.Dialog);
		}

		public void LoginYesWrite(object sender, Account  account)
		{
			lblStatus.LabelProp = MainClass.Languages.Translate("feedback-send");
			buttonOk.Sensitive = true;
		}
		
		public void LoginNoWrite(object sender, string  message)
		{
			lblStatus.LabelProp = MainClass.Languages.Translate("error_feedback");
			buttonOk.Sensitive = true;
			MessageDialogs md =
				new MessageDialogs(MessageDialogs.DialogButtonType.Ok, MainClass.Languages.Translate("error_feedback"),message, Gtk.MessageType.Error,null);
			md.ShowDialog();

		}

		protected void OnButtonOkClicked (object sender, EventArgs e)
		{
			if((MainClass.Settings.Account == null) || (String.IsNullOrEmpty(MainClass.Settings.Account.Token))){
				MessageDialogs md =
					new MessageDialogs(MessageDialogs.DialogButtonType.Ok, MainClass.Languages.Translate("invalid_login_f1"),"", Gtk.MessageType.Error,null);
				md.ShowDialog();

				return ;
			}

			string feedText =tvFeedback.Buffer.Text;
			buttonOk.Sensitive = false;

			if(string.IsNullOrEmpty(feedText)){
				return;
			}

			lblStatus.LabelProp = "Sending...";
			while (Gtk.Application.EventsPending ())
				Gtk.Application.RunIteration ();		

			LoggingInfo log = new LoggingInfo();
			log.LoggWebString (LoggingInfo.ActionId.IDERequestHelp,feedText,LoginYesWrite,LoginNoWrite);
			//this.Respond(Gtk.ResponseType.Ok);
		}
	}
}

