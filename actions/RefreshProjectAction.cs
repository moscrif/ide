using System;
namespace Moscrif.IDE.Actions
{
	public class RefreshProjectAction : Gtk.Action
	{
		public RefreshProjectAction(): base("refreshproject", MainClass.Languages.Translate("menu_refresh_project"), MainClass.Languages.Translate("menu_title_refresh_project"), "refresh.png")
		{
		}

		protected override void OnActivated()
		{
			base.OnActivated();

			MainClass.MainWindow.RefreshProject();
		}
	}
}

