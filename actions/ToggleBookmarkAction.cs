using System;
using Gtk;
using System.Reflection;

namespace  Moscrif.IDE.Actions
{
	public class ToggleBookmarkAction : Gtk.Action
	{
		public ToggleBookmarkAction():base("toggleBookmark",MainClass.Languages.Translate("menu_toggle_bookmark"),MainClass.Languages.Translate("menu_title_toggle_bookmark"),"bookmark.png"){



		}

		protected override void OnActivated ()
		{
			base.OnActivated ();

			Gtk.Action act = MainClass.MainWindow.EditorNotebook.EditorAction.GetAction("sourceeditor_togglebookmark");
			if (act!=null){
				act.Activate();
			}
		}
	}
}

