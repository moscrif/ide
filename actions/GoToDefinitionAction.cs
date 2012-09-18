using System;
using Gtk;
using System.Reflection;
using Moscrif.IDE.Controls;

namespace  Moscrif.IDE.Actions
{
	public class GoToDefinitionAction : Gtk.Action
	{
		public GoToDefinitionAction():base("gotodefinition",MainClass.Languages.Translate("menu_goto_definition"),MainClass.Languages.Translate("menu_title_goto_definition"),null){

		}

		protected override void OnActivated ()
		{
			base.OnActivated ();

			Gtk.Action act = MainClass.MainWindow.EditorNotebook.EditorAction.GetAction("sourceeditor_gotoDefinition");
			if (act!=null){
				act.Activate();
			}


		}

	}
}

