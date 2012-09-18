using System;
using System.Collections.Generic;
using Gtk;
using System.Reflection;
using Moscrif.IDE.Components;
using Moscrif.IDE.Iface.Entities;

namespace Moscrif.IDE.Actions
{
	public class OpenWorkspace : Gtk.Action
	{
		public OpenWorkspace() : base("openworkspace", MainClass.Languages.Translate("menu_open_workspace"), MainClass.Languages.Translate("menu_title_open_workspace"), "workspace.png")
		{
		}

		protected override void OnActivated()
		{
			base.OnActivated();

			Gtk.FileChooserDialog fc = new Gtk.FileChooserDialog(MainClass.Languages.Translate("chose_workspace_open"), MainClass.MainWindow, FileChooserAction.Open, "Cancel", ResponseType.Cancel, "Open", ResponseType.Accept);


			FileFilter filter = new FileFilter();
			filter.Name = "Workspace files";
			filter.AddMimeType("Workspace file");
			filter.AddPattern("*.msw");
			fc.AddFilter(filter);

			if (!String.IsNullOrEmpty(MainClass.Settings.LastOpenedWorkspaceDir))
				fc.SetCurrentFolder(MainClass.Settings.LastOpenedWorkspaceDir);


			if (fc.Run() == (int)ResponseType.Accept) {

				MainClass.Settings.LastOpenedWorkspaceDir = System.IO.Path.GetDirectoryName(fc.Filename);

				//CloseActualWorkspace();
				Workspace.Workspace workspace = Workspace.Workspace.OpenWorkspace(fc.Filename);

				if (workspace != null){
					//MainClass.Settings.RecentFiles.AddWorkspace(fc.Filename,fc.Filename);
					MainClass.MainWindow.ReloadWorkspace(workspace,true,true);
				}

			}

			fc.Destroy();
		}
	}
}

