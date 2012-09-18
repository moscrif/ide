using System;
using Gtk;
using System.Reflection;

namespace Moscrif.IDE.Actions
{
	public class PasteTextAction: Gtk.Action
	{
		public PasteTextAction():base("pasteText",MainClass.Languages.Translate("menu_paste_text"),MainClass.Languages.Translate("menu_title_paste_text"),"edit-paste.png")
		{
		}

		protected override void OnActivated ()
		{
			base.OnActivated ();

			Gtk.Action act = MainClass.MainWindow.EditorNotebook.EditorAction.GetAction("sourceeditor_pasteClipboard");
			if (act!=null){
				act.Activate();
			}
		}
	}
}

