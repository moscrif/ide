using System;
using Gtk;
namespace Moscrif.IDE.Actions
{
	public class FindPreviousAction : Gtk.Action
	{
		public FindPreviousAction(): base("findprevious", MainClass.Languages.Translate("menu_find_previous"),MainClass.Languages.Translate("menu_title_find_previous"), null)
		{
		}

		protected override void OnActivated ()
		{
			base.OnActivated ();

			MainClass.MainWindow.EditorNotebook.SearchPreviu();

		}
	}
}

