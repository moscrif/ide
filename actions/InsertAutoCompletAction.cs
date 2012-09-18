using System;
using Gtk;
namespace  Moscrif.IDE.Actions
{
	public class InsertAutoCompletAction : Gtk.Action
	{
		public InsertAutoCompletAction():base("insertcomplete",MainClass.Languages.Translate("menu_insert_complete"),MainClass.Languages.Translate("menu_title_insert_complete"),null){

		}

		protected override void OnActivated ()
		{
			base.OnActivated ();
			Gtk.Action act = MainClass.MainWindow.EditorNotebook.EditorAction.GetAction("sourceeditor_insertautocomplete");
			if (act!=null){
				act.Activate();
			}
		}
	}
}

