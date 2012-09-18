using System;
using Gtk;
namespace Moscrif.IDE.Actions
{
	public class FindNextAction : Gtk.Action
	{
		public FindNextAction(): base("findnext", MainClass.Languages.Translate("menu_find_next"),MainClass.Languages.Translate("menu_title_find_next"), null)
		{
		}

		protected override void OnActivated ()
		{
			base.OnActivated ();

			MainClass.MainWindow.EditorNotebook.SearchNext();

		}
	}
}

