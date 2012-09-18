using System;

using MessageDialogs = Moscrif.IDE.Controls.MessageDialog;

namespace  Moscrif.IDE.Actions
{
	public class RenameProjectAction  : Gtk.Action
	{
		public RenameProjectAction() :base("renameproject",MainClass.Languages.Translate("menu_rename_project"),MainClass.Languages.Translate("menu_title_rename_project"),"project-unload.png") {
		}

		protected override void OnActivated ()
		{
			base.OnActivated ();

			MainClass.MainWindow.RenameProject();
			/*EntryDialog ed = new EntryDialog("","New Project");
			int result = ed.Run();
			if (result == (int)ResponseType.Ok){
				if (!String.IsNullOrEmpty(ed.TextEntry) ){
					string directory = MainClass.Tools.RemoveDiacritics(ed.TextEntry);
					MainClass.MainWindow.CreateDirectory(directory);
				}
			}
			ed.Destroy();*/


			/*MessageDialogs md =
				new MessageDialogs(new string[]{MainClass.Languages.Translate("delete_project_unload"),MainClass.Languages.Translate("delete_project_delete"),MainClass.Languages.Translate("cancel")}, MainClass.Languages.Translate("delete_project"), MainClass.Languages.Translate("delete_project_f1"), Gtk.MessageType.Question,MainClass.MainWindow);
				int resultAction = md.ShowDialog();

				switch (resultAction)
				{
				    case -1: //"Unload"
						MainClass.MainWindow.UnloadProject();
						MainClass.MainWindow.SaveWorkspace();
					break;
				    case -2://"Delete"
						MainClass.MainWindow.DeleteProject();
						MainClass.MainWindow.SaveWorkspace();
				        break;
				    default: //"Cancel"
				  break;
				}*/
		}
	}
}

