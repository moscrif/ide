using System;
using Gtk;
using System.Reflection;

namespace  Moscrif.IDE.Actions
{
	public class ClearBookmarksAction : Gtk.Action
	{
		public ClearBookmarksAction():base("clearBookmarks",MainClass.Languages.Translate("menu_clear_bookmarks"),MainClass.Languages.Translate("menu_title_clear_bookmarks"),null){//"bookmark-clear.png"
		}

		protected override void OnActivated ()
		{
			base.OnActivated ();

			Gtk.Action act = MainClass.MainWindow.EditorNotebook.EditorAction.GetAction("sourceeditor_clearbookmark");
			if (act!=null){
				act.Activate();
			}
		}
	}
}

