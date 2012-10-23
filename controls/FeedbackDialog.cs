using System;
using Moscrif.IDE.Iface.Entities;
using Moscrif.IDE.Settings;
using Moscrif.IDE.Iface;
using Moscrif.IDE.Components;
using System.IO;
using MessageDialogs = Moscrif.IDE.Controls.MessageDialog;
using Gtk;

namespace Moscrif.IDE.Controls
{
	public partial class FeedbackDialog : Gtk.Dialog
	{
		Notebook notb = new Notebook();

		FeedbackControl feedbackIssue = new  FeedbackControl(FeedbackControl.TypFeedback.Issue);
		FeedbackControl feedbackQuestion = new  FeedbackControl(FeedbackControl.TypFeedback.Question);
		FeedbackControl feedbackSuggestion = new  FeedbackControl(FeedbackControl.TypFeedback.Suggestion);
		public FeedbackDialog()
		{
			this.Build();
			this.TransientFor = MainClass.MainWindow;

			Pango.FontDescription customFont = lblFeedback.Style.FontDescription.Copy();//  Pango.FontDescription.FromString(MainClass.Settings.ConsoleTaskFont);
			customFont.Size = 24;
			customFont.Weight = Pango.Weight.Bold;
			lblFeedback.ModifyFont(customFont);
			lblStatus.ModifyFont(customFont);
			lblStatus.LabelProp = "";

			notb.AppendPage(feedbackIssue,new Label("Report an Issue"));
			notb.AppendPage(feedbackQuestion,new Label("Ask a Question"));
			notb.AppendPage(feedbackSuggestion,new Label("Make a Suggestion"));

			tblMain.Attach(notb,0,2,1,2,AttachOptions.Fill|AttachOptions.Expand,AttachOptions.Fill|AttachOptions.Expand,0,0);
			//imgError.SetFromStock ("gtk-dialog-error", Gtk.IconSize.Dialog);
			tblMain.ShowAll();
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
				new MessageDialogs(MessageDialogs.DialogButtonType.Ok, MainClass.Languages.Translate("error_feedback"),message, Gtk.MessageType.Error,this);
			md.ShowDialog();

		}

		protected void OnButtonOkClicked (object sender, EventArgs e)
		{

			string feedText = "";
			switch (notb.Page){
				case 0:
				feedText = feedbackIssue.GetData();
					break;
				case 1:
				feedText = feedbackQuestion.GetData();
					break;
				case 2:
				feedText = feedbackSuggestion.GetData();
					break;
			}
			Console.WriteLine(feedText);

			if(MainClass.Settings.Account == null) 
				Console.WriteLine("Account IS NULL");

			if(String.IsNullOrEmpty(MainClass.Settings.Account.Token)) 
				Console.WriteLine("Token IS NULL");

			if((MainClass.Settings.Account == null) || (String.IsNullOrEmpty(MainClass.Settings.Account.Token))){
				MessageDialogs md =
					new MessageDialogs(MessageDialogs.DialogButtonType.Ok, MainClass.Languages.Translate("invalid_login_f1"),"", Gtk.MessageType.Error,this);
				md.ShowDialog();
				Console.WriteLine("IS NULL");
				return ;
			}

			buttonOk.Sensitive = false;

			if(string.IsNullOrEmpty(feedText)){
				return;
			}

			lblStatus.LabelProp = "Sending...";
			while (Gtk.Application.EventsPending ())
				Gtk.Application.RunIteration ();		

			LoggingInfo log = new LoggingInfo();
			log.SendFeedback (feedText,LoginYesWrite,LoginNoWrite);

			//this.Respond(Gtk.ResponseType.Ok);
		}
	}
}

