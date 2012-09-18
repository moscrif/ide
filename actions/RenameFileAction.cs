using System;
using Gtk;
using Moscrif.IDE.Controls;

namespace Moscrif.IDE.Actions
{
	public class RenameFileAction : Gtk.Action
	{
		public RenameFileAction() : base("renamefile", MainClass.Languages.Translate("menu_rename_file"), MainClass.Languages.Translate("menu_title_rename_file"), "rename.png")
		{
		}

		protected override void OnActivated()
		{
			base.OnActivated();

			MainClass.MainWindow.RenameFile();
			//MainClass.Workspace.RenameFile("");

			/*EntryDialog ed = new EntryDialog("","New Name");
			int result = ed.Run();
			if (result == (int)ResponseType.Ok){
				if (!String.IsNullOrEmpty(ed.TextEntry) ){
					string directory = MainClass.Tools.RemoveDiacritics(ed.TextEntry);
					MainClass.Workspace.RenameFile(directory);
				}
			}
			ed.Destroy();*/
		}

	}
}

