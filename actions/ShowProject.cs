using System;
namespace Moscrif.IDE.Actions
{
	public class ShowProject:Gtk.Action
	{
		public ShowProject(): base("showprojectpopup", MainClass.Languages.Translate("menu_show"), MainClass.Languages.Translate("menu_title_show"),null)
		{
		}

		protected override void OnActivated ()
		{
			base.OnActivated();

			MainClass.MainWindow.ShowProjectInExplorer(true);

		}
	}
}

