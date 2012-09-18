using System;
namespace Moscrif.IDE.Actions
{
	public class ViewProjectAppAction: Gtk.Action
	{
		public ViewProjectAppAction():base("viewappfile",MainClass.Languages.Translate("menu_view_app_file"),MainClass.Languages.Translate("menu_view_app_file"),null)
		{
		}


		protected override void OnActivated ()
		{
			base.OnActivated ();
			MainClass.MainWindow.OpenSelectFile();
		}
	}
}

