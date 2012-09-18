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
	public class NewAction : Gtk.Action
	{
		public NewAction() : base("newfile", MainClass.Languages.Translate("menu_new_file"), MainClass.Languages.Translate("menu_title_new_file"), "file-new.png")
		{
			
		}
		protected override void OnActivated()
		{
			Console.WriteLine("newfile - activate");
			base.OnActivated();
			NewFileDialog nfd = new NewFileDialog();

			int result = nfd.Run();
			if (result == (int)ResponseType.Ok) {

				string fileName = nfd.FileName;
				string content = nfd.Content;
				MainClass.MainWindow.CreateFile(fileName,content);
			}
			nfd.Destroy();
			MainClass.MainWindow.SaveWorkspace();
		}
	}
}

