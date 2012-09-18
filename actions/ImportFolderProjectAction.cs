using System;
using System.Collections.Generic;
using Gtk;
using System.Reflection;
using Moscrif.IDE.Components;
using Moscrif.IDE.Controls;
using MessageDialogs = Moscrif.IDE.Controls.MessageDialog;
using Moscrif.IDE.Iface.Entities;
using System.IO;
using Moscrif.IDE.Workspace;
using System.Threading;
using System.Text;

namespace Moscrif.IDE.Actions
{
	public class ImportFolderProjectAction: Gtk.Action
	{
		public ImportFolderProjectAction(): base("importFolderProject", MainClass.Languages.Translate("menu_import_project_folder"), MainClass.Languages.Translate("menu_title_import_project_folder"),"")
		{

		}

		protected override void OnActivated()
		{
			base.OnActivated();

			if(String.IsNullOrEmpty(MainClass.Workspace.FilePath)){
				MessageDialogs md = new MessageDialogs(MessageDialogs.DialogButtonType.Ok, MainClass.Languages.Translate("workspace_not_exist"), MainClass.Languages.Translate("please_create_workspace"), Gtk.MessageType.Error);
				md.ShowDialog();
				return;
			}

			Gtk.FileChooserDialog fc = new Gtk.FileChooserDialog(MainClass.Languages.Translate("chose_project_import"), MainClass.MainWindow, FileChooserAction.Open, "Cancel", ResponseType.Cancel, "Open", ResponseType.Accept);

			if (!String.IsNullOrEmpty(MainClass.Settings.LastOpenedImportDir))
				fc.SetCurrentFolder(MainClass.Settings.LastOpenedImportDir);

			FileFilter filter = new FileFilter();
			filter.Name = "app files";
			filter.AddMimeType("app file");
			filter.AddPattern("*.app");
			fc.AddFilter(filter);


			FileFilter filter2 = new FileFilter();
			filter2.Name = "msp files";
			filter2.AddMimeType("msp file");
			filter2.AddPattern("*.msp");
			fc.AddFilter(filter2);

			if (fc.Run() == (int)ResponseType.Accept) {

				MainClass.Settings.LastOpenedImportDir = System.IO.Path.GetDirectoryName(fc.Filename);

				MainClass.MainWindow.ImportProject(fc.Filename,false);
			}

			fc.Destroy();
		}

	}
}

