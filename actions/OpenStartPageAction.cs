using System.IO;
using System;
using Gtk;


namespace Moscrif.IDE.Actions
{
	public class OpenStartPageAction  : Gtk.Action
	{
		public OpenStartPageAction() : base("showstartpage", MainClass.Languages.Translate("menu_show_start_page"), MainClass.Languages.Translate("menu_title_show_start_page"), "home.png")
		{
		}

		protected override void OnActivated()
		{
			base.OnActivated();

			MainClass.MainWindow.OpenFile("StartPage",false);

		}

	}
}

