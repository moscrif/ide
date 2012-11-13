using System;
using System.Collections.Generic;
using Gtk;
using System.Reflection;


namespace Moscrif.IDE.Actions
{
	public class OpenOnlineSamples: Gtk.Action
	{
		public OpenOnlineSamples(): base("onlinesamples", MainClass.Languages.Translate("menu_open_online_samples"), MainClass.Languages.Translate("menu_open_online_samples"),null)
		{
		}


		protected override void OnActivated ()
		{
			base.OnActivated();

			string linkUrl = MainClass.Settings.SamplesBaseUrl;//String.Format("http://moscrif.com/samples");
			if (!String.IsNullOrEmpty(linkUrl)){
				System.Diagnostics.Process.Start(linkUrl);
			}

		}
	}
}

