using System;
using System.Collections.Generic;
using Gtk;
using System.Reflection;


namespace Moscrif.IDE.Actions
{
	public class OpenOnlineDocument: Gtk.Action
	{
		public OpenOnlineDocument(): base("onlinedocumentation", MainClass.Languages.Translate("menu_open_online_documen"), MainClass.Languages.Translate("menu_open_online_documen"),null)
		{
		}


		protected override void OnActivated ()
		{
			base.OnActivated();

			string linkUrl = MainClass.Settings.DocumentsBaseUrl;//String.Format("http://moscrif.com/documents");
			if (!String.IsNullOrEmpty(linkUrl)){
				System.Diagnostics.Process.Start(linkUrl);
			}

		}
	}
}

