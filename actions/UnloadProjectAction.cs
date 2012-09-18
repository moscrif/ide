using System;
using System.Collections.Generic;
using Gtk;
using MessageDialogs = Moscrif.IDE.Controls.MessageDialog;

namespace  Moscrif.IDE.Actions
{
	public class UnloadProjectAction  : Gtk.Action
	{
		public UnloadProjectAction() :base("unloadproject",MainClass.Languages.Translate("menu_unload_project"),MainClass.Languages.Translate("menu_title_unload_project"),"project-unload.png") {
		}

		protected override void OnActivated ()
		{
			base.OnActivated ();

			/*MessageDialogs md =
				new MessageDialogs(new string[]{MainClass.Languages.Translate("delete_project_unload"),MainClass.Languages.Translate("cancel")}, MainClass.Languages.Translate("delete_project"), MainClass.Languages.Translate("delete_project_f1"), Gtk.MessageType.Question,MainClass.MainWindow);
				int resultAction = md.ShowDialog();
			if(resultAction = -1){*/
				MainClass.MainWindow.UnloadProject();
				MainClass.MainWindow.SaveWorkspace();
			//}

		/*	MessageDialogs md =
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

