using System;
using System.Collections.Generic;
using Gtk;
using System.Reflection;


namespace Moscrif.IDE.Actions
{
	public class OpenOnlineDemos: Gtk.Action
	{
		public OpenOnlineDemos(): base("onlinedemos", MainClass.Languages.Translate("menu_open_online_demos"), MainClass.Languages.Translate("menu_open_online_demos"),null)
		{
		}


		protected override void OnActivated ()
		{
			base.OnActivated();

			string linkUrl = MainClass.Settings.DemosBaseUrl;//String.Format("http://moscrif.com/demos");
			if (!String.IsNullOrEmpty(linkUrl)){
				System.Diagnostics.Process.Start(linkUrl);
			}

		}
	}
}

