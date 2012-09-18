using System;
using Gtk;
using System.Reflection;

namespace  Moscrif.IDE.Actions
{
	public class CommentUncommentAction: Gtk.Action
	{
		public CommentUncommentAction():base("commentUncomment",MainClass.Languages.Translate("menu_commentUncomment"),MainClass.Languages.Translate("menu_title_commentUncomment"),"bookmark.png"){

		}

		protected override void OnActivated ()
		{
			base.OnActivated ();

			Gtk.Action act = MainClass.MainWindow.EditorNotebook.EditorAction.GetAction("sourceeditor_commentUncomment");
			if (act!=null){
				act.Activate();
			}
		}


	}
}

