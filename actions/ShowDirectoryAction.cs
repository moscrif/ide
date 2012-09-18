using System;

namespace Moscrif.IDE.Actions
{
	public class ShowDirectoryAction:Gtk.Action
	{
		public ShowDirectoryAction(): base("showfolderpopup", MainClass.Languages.Translate("menu_show"), MainClass.Languages.Translate("menu_title_show"),null)
		{
		}

		protected override void OnActivated ()
		{
			base.OnActivated();

			MainClass.MainWindow.ShowDirectory();

		}
	}
}

