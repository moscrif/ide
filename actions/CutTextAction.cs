using System;
using Gtk;
using System.Reflection;

namespace Moscrif.IDE.Actions
{
	public class CutTextAction: Gtk.Action
	{
		public CutTextAction():base("cutText",MainClass.Languages.Translate("menu_cut_text"),MainClass.Languages.Translate("menu_title_cut_text"),"edit-cut.png")
		{
		}

		protected override void OnActivated ()
		{
			base.OnActivated ();

			Gtk.Action act = MainClass.MainWindow.EditorNotebook.EditorAction.GetAction("sourceeditor_cutClipboard");
			if (act!=null){
				act.Activate();
			}
		}
	}
}

