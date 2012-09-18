using System;
using Gtk;

namespace Moscrif.IDE.Actions
{
	public class ToggleExcludeAction: Gtk.Action
	{
		public ToggleExcludeAction():base("toggleexclude",MainClass.Languages.Translate("menu_exclude") , MainClass.Languages.Translate("menu_title_exclude"),"file-exclude.png"){

		}

		protected override void OnActivated ()
		{
			base.OnActivated ();

			MainClass.MainWindow.ToggleExcludeFile();
		}
	}
}

