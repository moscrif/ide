using System;
using Gtk;
using System.Reflection;
using Moscrif.IDE.Controls;

namespace Moscrif.IDE.Actions
{
	public class GoToLineAction : Gtk.Action
	{
		public GoToLineAction():base("gotoline",MainClass.Languages.Translate("menu_goto"),MainClass.Languages.Translate("menu_title_goto"),null){
		}

		protected override void OnActivated ()
		{
			base.OnActivated ();

			EntryDialog ed = new EntryDialog("",MainClass.Languages.Translate("line_number"),true);
			int result = ed.Run();
			if (result == (int)Gtk.ResponseType.Ok){
				if (!String.IsNullOrEmpty(ed.TextEntry) ){
					int line = 0;

					if(Int32.TryParse(ed.TextEntry,out line) ){

						MainClass.MainWindow.GoToFile(null, (object)line );
					}
				}
			}
			ed.Destroy();

			/*
			//Console.WriteLine("Action ToggleBookmarkAction activated");
			Gtk.Action act = MainClass.MainWindow.EditorNotebook.EditorAction.GetAction("sourceeditor_GoToLine");
			//Console.WriteLine("-----------");
			if (act!=null){
				//Console.WriteLine(act.Name);
				act.Activate();
				//Console.WriteLine("-----------");
			}
			*/
		}
	}
}

