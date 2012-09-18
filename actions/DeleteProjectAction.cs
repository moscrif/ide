using System;
using System.Collections.Generic;
using Gtk;
using MessageDialogs = Moscrif.IDE.Controls.MessageDialog;

namespace  Moscrif.IDE.Actions
{
	public class DeleteProjectAction  : Gtk.Action
	{
		public DeleteProjectAction() :base("deleteproject",MainClass.Languages.Translate("menu_delete_project"),MainClass.Languages.Translate("menu_title_delete_project"),null) {
		}

		protected override void OnActivated ()
		{
			base.OnActivated ();

			MessageDialogs md =
				new MessageDialogs(new string[]{MainClass.Languages.Translate("delete_project_delete"),MainClass.Languages.Translate("cancel")}, MainClass.Languages.Translate("delete_project"), "", Gtk.MessageType.Question,MainClass.MainWindow);
				int resultAction = md.ShowDialog();
			if(resultAction == -1){

				MainClass.MainWindow.DeleteProject();
				MainClass.MainWindow.SaveWorkspace();
			}
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

