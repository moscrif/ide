using System;
using System.Collections.Generic;
using Gtk;
using System.Reflection;


namespace Moscrif.IDE.Actions
{
	public class OpenSubmitSupport: Gtk.Action
	{
		public OpenSubmitSupport(): base("submitsupport", MainClass.Languages.Translate("menu_submit_support"), MainClass.Languages.Translate("menu_submit_support"),null)
		{
		}


		protected override void OnActivated ()
		{
			base.OnActivated();

			string linkUrl = String.Format("http://moscrif.com/support");
			if (!String.IsNullOrEmpty(linkUrl)){
				System.Diagnostics.Process.Start(linkUrl);
			}

		}
	}
}

