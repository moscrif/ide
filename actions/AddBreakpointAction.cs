using System;
using Gtk;
using System.Reflection;

namespace Moscrif.IDE.Actions
{
	public class AddBreakpointAction : Gtk.Action
	{
		public AddBreakpointAction():base("addBreakpoint",MainClass.Languages.Translate("menu_add_breakpoint"),MainClass.Languages.Translate("menu_title_add_breakpoint"),"breakpoint.png"){



		}

		protected override void OnActivated ()
		{
			base.OnActivated ();

			Gtk.Action act = MainClass.MainWindow.EditorNotebook.EditorAction.GetAction("sourceeditor_addbreakpoint");
			if (act!=null){
				act.Activate();
			}
		}
	}
}

