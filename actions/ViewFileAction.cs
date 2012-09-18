using System;

namespace  Moscrif.IDE.Actions
{
	public class ViewFileAction : Gtk.Action
	{
		public ViewFileAction():base("view",MainClass.Languages.Translate("menu_view"),MainClass.Languages.Translate("menu_view"),null)
		{
			//MainClass.MainWindow.OpenSelectFile();
		}

		protected override void OnActivated ()
		{
			base.OnActivated ();
			MainClass.MainWindow.OpenSelectFile();
		}
	}
}

