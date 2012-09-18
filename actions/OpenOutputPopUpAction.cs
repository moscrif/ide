using System;
namespace Moscrif.IDE.Actions
{
	public class OpenOutputPopUpAction: Gtk.Action
	{
		public OpenOutputPopUpAction(): base("openoutputpopup", MainClass.Languages.Translate("menu_open_output"), MainClass.Languages.Translate("menu_title_open_output"),null)
		{
		}

		protected override void OnActivated ()
		{
			base.OnActivated();

			MainClass.MainWindow.OpenOutput(true);

		}
	}
}

