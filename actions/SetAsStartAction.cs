using System;
using Gtk;

namespace Moscrif.IDE.Actions
{
	public class SetAsStartAction: Gtk.Action
	{
		public SetAsStartAction():base("setasstart",MainClass.Languages.Translate("menu_start_file"),MainClass.Languages.Translate("menu_title_start_file"),null){

		}

		protected override void OnActivated ()
		{
			base.OnActivated ();
			MainClass.MainWindow.SetFileAsStartup();
		}
	}
}

