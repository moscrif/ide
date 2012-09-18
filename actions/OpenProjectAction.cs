using System;
using System.Collections.Generic;
using Gtk;
using System.Reflection;
using Moscrif.IDE.Controls;
using Moscrif.IDE.Iface.Entities;
using MessageDialogs = Moscrif.IDE.Controls.MessageDialog;


namespace  Moscrif.IDE.Actions
{
	public class OpenProjectAction : Gtk.Action
	{
		public OpenProjectAction() :base("loadproject",MainClass.Languages.Translate("menu_load_project"),MainClass.Languages.Translate("menu_title_load_project"),"project.png") {
		}
		
		protected override void OnActivated ()
		{
			base.OnActivated ();

			if (String.IsNullOrEmpty(MainClass.Workspace.FilePath)){
				MessageDialogs md = new MessageDialogs(MessageDialogs.DialogButtonType.Ok, MainClass.Languages.Translate("please_create_workspace"), "", Gtk.MessageType.Error);
				md.ShowDialog();
				return;

			}

			OpenProjectDialog nfd = new OpenProjectDialog();

			int result = nfd.Run();
			if (result == (int)ResponseType.Ok) {

				//string fileName = nfd.FileName;
				//MainClass.MainWindow.CreateFile(fileName);
			}
			nfd.Destroy();
			MainClass.MainWindow.SaveWorkspace();
			return;

			/*
			Gtk.FileChooserDialog fc=
			new Gtk.FileChooserDialog("Choose the project to open",
		                            MainClass.MainWindow,
		                            FileChooserAction.Open,
		                            "Cancel",ResponseType.Cancel,
		                            "Open",ResponseType.Accept);
			
			FileFilter filter = new FileFilter();
			filter.Name = "Project files";
			filter.AddMimeType("Project file");
			filter.AddPattern("*.msp");
			fc.AddFilter(filter);
			fc.SetCurrentFolder(MainClass.Workspace.RootDirectory);
			
			if (fc.Run() == (int)ResponseType.Accept){ 
				MainClass.MainWindow.OpenProject(fc.Filename,true);
			}
		
			fc.Destroy();
			MainClass.MainWindow.SaveWorkspace();*/
		}
	}
}

