using System;
using System.Collections.Generic;
using Gtk;
using System.Reflection;


namespace Moscrif.IDE.Actions
{
	public class OpenOutputAction: Gtk.Action
	{
		public OpenOutputAction(): base("openoutput", MainClass.Languages.Translate("menu_open_output"), MainClass.Languages.Translate("menu_title_open_output"),null)
		{
		}


		protected override void OnActivated ()
		{
			base.OnActivated();

			MainClass.MainWindow.OpenOutput(false);

		}
	}
}

