using System;
using System.Collections.Generic;
using Gtk;
using System.Reflection;


namespace Moscrif.IDE.Actions
{
	public class OpenOnlineApiReferenceAction: Gtk.Action
	{
		public OpenOnlineApiReferenceAction(): base("onlineapireference", MainClass.Languages.Translate("menu_open_online_api_ref"), MainClass.Languages.Translate("menu_open_online_api_ref"),null)
		{
		}


		protected override void OnActivated ()
		{
			base.OnActivated();

			string linkUrl = String.Format("http://moscrif.com/api");
			if (!String.IsNullOrEmpty(linkUrl)){
				System.Diagnostics.Process.Start(linkUrl);
			}

		}
	}
}

