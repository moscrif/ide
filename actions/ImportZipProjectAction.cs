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
	public class ImportZipProjectAction: Gtk.Action
	{
		public ImportZipProjectAction(): base("importZipProject", MainClass.Languages.Translate("menu_import_project_zip"), MainClass.Languages.Translate("menu_title_import_project_zip"),"")
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
			filter.Name = "zip files";
			filter.AddMimeType("zip file");
			filter.AddPattern("*.zip");
			fc.AddFilter(filter);

			if (fc.Run() == (int)ResponseType.Accept) {

				MainClass.Settings.LastOpenedImportDir = System.IO.Path.GetDirectoryName(fc.Filename);

				MainClass.MainWindow.ImportProject(fc.Filename,true);
			}

			fc.Destroy();
		}

	}
}

