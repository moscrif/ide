using System;
using Gtk;

namespace Moscrif.IDE.Actions
{
	public class DeleteFileAction : Gtk.Action
	{
		public DeleteFileAction() : base("deletefile", MainClass.Languages.Translate("menu_delete"), MainClass.Languages.Translate("menu_title_delete"), "delete.png")
		{
		}

		protected override void OnActivated ()
		{
			base.OnActivated ();
			MainClass.MainWindow.DeleteFile();
			//MainClass.Workspace.DeleteFile();
		}
	}
}

