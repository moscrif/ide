using System;
using Moscrif.IDE.Controls;

namespace  Moscrif.IDE.Actions
{
	public class AddThemeAction : Gtk.Action
	{
		public AddThemeAction():base("addtheme",MainClass.Languages.Translate("menu_add_theme"),MainClass.Languages.Translate("menu_title_add_theme"),"themes.png")
		{

		}

		protected override void OnActivated ()
		{
			base.OnActivated ();
			MainClass.MainWindow.AddTheme();
		}
	}
}

