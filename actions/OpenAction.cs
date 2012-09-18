using System.IO;
using System;
using Gtk;

namespace Moscrif.IDE.Actions
{
	public class OpenAction : Gtk.Action
	{
		public OpenAction() : base("open", MainClass.Languages.Translate("menu_open"), MainClass.Languages.Translate("menu_title_open"), "file-open.png")
		{
			
		}

		protected override void OnActivated()
		{
			base.OnActivated();
			
			Gtk.FileChooserDialog fc = new Gtk.FileChooserDialog(MainClass.Languages.Translate("chose_file_open"), MainClass.MainWindow, FileChooserAction.Open, "Cancel", ResponseType.Cancel, "Open", ResponseType.Accept);
			fc.SetCurrentFolder(MainClass.Workspace.RootDirectory);

			FileFilter filter = new FileFilter();
			filter.Name = "Moscrif file (*.ms,*.mso,*.tab, *.msw)";
			filter.AddMimeType("text/moscrif ");
			filter.AddPattern("*.ms");
			filter.AddPattern("*.msw");
			filter.AddPattern("*.mso");
			filter.AddPattern("*.tab");
			fc.AddFilter(filter);

			filter = new FileFilter();
			filter.Name = "Moscrif Workspace (*.msw)";
			filter.AddMimeType("text/moscrif ");
			filter.AddPattern("*.msw");
			fc.AddFilter(filter);

			filter = new FileFilter();
			filter.Name = "PNG and JPEG images (*.png,*.jpg)";
			filter.AddMimeType("image/png");
			filter.AddPattern("*.png");
			filter.AddMimeType("image/jpeg");
			filter.AddPattern("*.jpg");
			fc.AddFilter(filter);

			filter = new FileFilter();
			filter.Name = "Text file (*.txt)";
			filter.AddMimeType("text/plain");
			filter.AddPattern("*.txt");
			fc.AddFilter(filter);

			filter = new FileFilter();
			filter.Name = "Xml file (*.xml)";
			filter.AddMimeType("text/xml");
			filter.AddPattern("*.txt");
			fc.AddFilter(filter);

			filter = new FileFilter();
			filter.Name = "All file ";
			filter.AddPattern("*.*");
			fc.AddFilter(filter);

			if (!String.IsNullOrEmpty(MainClass.Settings.LastOpenedFileDir))
				fc.SetCurrentFolder(MainClass.Settings.LastOpenedFileDir);


			if (fc.Run() == (int)ResponseType.Accept) {

				MainClass.Settings.LastOpenedFileDir = System.IO.Path.GetDirectoryName(fc.Filename);

				if (Path.GetExtension(fc.Filename) == ".msp") {
					MainClass.MainWindow.OpenProject(fc.Filename,true);
				} else if (Path.GetExtension(fc.Filename) == ".msw") {
					Workspace.Workspace workspace = Workspace.Workspace.OpenWorkspace(fc.Filename);
					if (workspace != null){
						//CloseActualWorkspace();
						MainClass.MainWindow.ReloadWorkspace(workspace,true,true);
					}
				} else {
					MainClass.MainWindow.OpenFile(fc.Filename,true);
				}
				MainClass.MainWindow.SaveWorkspace();
			}
			
			fc.Destroy();
		}
		
	}
}

