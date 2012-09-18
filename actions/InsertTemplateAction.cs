using System;
using Gtk;
namespace  Moscrif.IDE.Actions
{
	public class InsertTemplateAction : Gtk.Action
	{
		public InsertTemplateAction():base("inserttemplate",MainClass.Languages.Translate("menu_insert_template"),MainClass.Languages.Translate("menu_title_insert_template"),null){
		}

		protected override void OnActivated ()
		{
			base.OnActivated ();
			Gtk.Action act = MainClass.MainWindow.EditorNotebook.EditorAction.GetAction("sourceeditor_inserttemplate");
			if (act!=null){
				act.Activate();
			}
		}

	}
}