using System;
using Gtk;
using System.Reflection;

namespace  Moscrif.IDE.Actions
{
	public class NextBookmarkAction : Gtk.Action
	{
		public NextBookmarkAction():base("nextBookmark",MainClass.Languages.Translate("menu_title_next_bookmark"),MainClass.Languages.Translate("menu_title_next_bookmark"),"bookmark-next.png"){
		}

		protected override void OnActivated ()
		{
			base.OnActivated ();
			Gtk.Action act = MainClass.MainWindow.EditorNotebook.EditorAction.GetAction("sourceeditor_nextbookmark");
			if (act!=null){
				act.Activate();
			}
		}
	}
}

