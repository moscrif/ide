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
	public class ExportZipProjectAction: Gtk.Action
	{
		public ExportZipProjectAction(): base("exportZipProject", MainClass.Languages.Translate("menu_export_project_zip"), MainClass.Languages.Translate("menu_title_export_project_zip"),"")
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

			Gtk.FileChooserDialog fc = new Gtk.FileChooserDialog(MainClass.Languages.Translate("save_project_export"), MainClass.MainWindow, FileChooserAction.Save, "Cancel", ResponseType.Cancel, "Save", ResponseType.Accept);

			if (!String.IsNullOrEmpty(MainClass.Settings.LastOpenedExportDir))
				fc.SetCurrentFolder(MainClass.Settings.LastOpenedExportDir);

			FileFilter filter = new FileFilter();
			filter.Name = "zip files";
			filter.AddMimeType("zip file");
			filter.AddPattern("*.zip");
			fc.AddFilter(filter);

			string exportFile = fc.Filename;

			string appname = "";
			int typ = -1;
			Gtk.TreeIter ti = new Gtk.TreeIter();
			MainClass.MainWindow.WorkspaceTree.GetSelectedFile(out appname, out typ, out ti);

			if (String.IsNullOrEmpty(appname)){
				return;
			}

			Project p = MainClass.Workspace.FindProject_byApp(appname, true);

			if(p== null){
				return;
			}

			fc.CurrentName =p.ProjectName;
			//fc.SetFilename(p.ProjectName);

			if (fc.Run() != (int)ResponseType.Accept) {
				fc.Destroy();
				return;
			}

			string name =fc.Filename;


			string ext = System.IO.Path.GetExtension(name);

			if(ext.ToLower() != ".zip")
				name = name+".zip";

			if(p!= null){
				p.Export(name,false);
				MainClass.Settings.LastOpenedExportDir = System.IO.Path.GetDirectoryName(fc.Filename);
			}
			fc.Destroy();
		}
	}
}

