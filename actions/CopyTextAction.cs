using System;
using Gtk;
using System.Reflection;

namespace Moscrif.IDE.Actions
{
	public class CopyTextAction: Gtk.Action
	{
		public CopyTextAction():base("copyText",MainClass.Languages.Translate("menu_copy_text"),MainClass.Languages.Translate("menu_title_copy_text"),"edit-copy.png")
		{
		}

		protected override void OnActivated ()
		{
			base.OnActivated ();

			Gtk.Action act = MainClass.MainWindow.EditorNotebook.EditorAction.GetAction("sourceeditor_copyClipboard");
			if (act!=null){
				act.Activate();
			}
		}

	}
}

