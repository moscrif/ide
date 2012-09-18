using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using Gtk;
using Moscrif.IDE.Controls;

namespace Moscrif.IDE.Actions
{
	public class NewDirectoryAction : Gtk.Action
	{
		public NewDirectoryAction() : base("newdirectory", MainClass.Languages.Translate("menu_new_directory"), MainClass.Languages.Translate("menu_title_new_directory"), "folder-new.png")
		{
		}

		protected override void OnActivated()
		{
			base.OnActivated();

			EntryDialog ed = new EntryDialog("","New Directory");
			int result = ed.Run();
			if (result == (int)ResponseType.Ok){
				if (!String.IsNullOrEmpty(ed.TextEntry) ){
					string directory = MainClass.Tools.RemoveDiacritics(ed.TextEntry);
					MainClass.MainWindow.CreateDirectory(directory);
				}
			}
			ed.Destroy();
			/*Gtk.FileChooserDialog fc = new Gtk.FileChooserDialog("Choose the Directory to Copy", null, FileChooserAction.SelectFolder, "Cancel", ResponseType.Cancel, "Open", ResponseType.Accept);

			if (fc.Run() == (int)ResponseType.Accept) {
				MainClass.Workspace.NewDirectory(fc.Filename);
			}
			fc.Destroy();*/
		}
	}
}

