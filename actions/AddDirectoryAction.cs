using System;
using Gtk;
using Moscrif.IDE.Controls;

namespace Moscrif.IDE.Actions
{
	public class AddDirectoryAction : Gtk.Action
	{
		public AddDirectoryAction() : base("adddirectory", MainClass.Languages.Translate("menu_add_dir"), MainClass.Languages.Translate("menu_title_add_dir"), null)
		{
		}

		protected override void OnActivated ()
		{
			base.OnActivated ();

			Gtk.FileChooserDialog fc = new Gtk.FileChooserDialog(MainClass.Languages.Translate("chose_dir_to_copy"), 
			                                                     MainClass.MainWindow,
			                                                     FileChooserAction.SelectFolder, 
			                                                     "Cancel", ResponseType.Cancel, 
			                                                     "Open", ResponseType.Accept);

			if (fc.Run() == (int)ResponseType.Accept) {
				MainClass.MainWindow.AddDirectory(fc.Filename);
			}
			fc.Destroy();

		}
	}
}

