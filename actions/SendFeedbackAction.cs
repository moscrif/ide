using System;
using System.Collections.Generic;
using Gtk;
using System.Reflection;
using Moscrif.IDE.Controls;

namespace Moscrif.IDE.Actions
{
	public class SendFeedbackAction: Gtk.Action
	{
		public SendFeedbackAction(): base("sendfeedback", MainClass.Languages.Translate("menu_send_feedback"), MainClass.Languages.Translate("menu_send_feedback"),"about.png")
		{
		}


		protected override void OnActivated ()
		{
			base.OnActivated();

			FeedbackDialog fbd = new FeedbackDialog();
			
			int result = fbd.Run();
			if (result == (int)ResponseType.Ok) {

			}
			fbd.Destroy();
		}
	}
}

