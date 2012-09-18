using System;
using Gtk;
using System.Reflection;

namespace  Moscrif.IDE.Actions
{
	public class PreviousBookmarkAction : Gtk.Action
	{
		public PreviousBookmarkAction():base("previouBookmark",MainClass.Languages.Translate("menu_previous_bookmark"),MainClass.Languages.Translate("menu_title_previous_bookmark"),"bookmark-previous.png"){
		}

		protected override void OnActivated ()
		{
			base.OnActivated ();

			//Console.WriteLine("Action ToggleBookmarkAction activated");
			Gtk.Action act = MainClass.MainWindow.EditorNotebook.EditorAction.GetAction("sourceeditor_prevbookmark");
			//Console.WriteLine("-----------");
			if (act!=null){
				//Console.WriteLine(act.Name);
				act.Activate();
				//Console.WriteLine("-----------");
			}
		}
	}
}

