using System;
using Gtk;
namespace Moscrif.IDE.Actions
{
	public class AddFileAction : Gtk.Action
	{
		public AddFileAction() : base("addfile", MainClass.Languages.Translate("menu_add_file"), MainClass.Languages.Translate("menu_title_add_file"), null)
		{
		}

		protected override void OnActivated ()
		{
			base.OnActivated ();

			//Console.WriteLine("Action addfile (type={1}) activated");


			Gtk.FileChooserDialog fc = new Gtk.FileChooserDialog( MainClass.Languages.Translate("chose_file_to_copy"), null, FileChooserAction.Open, "Cancel", ResponseType.Cancel, "Open", ResponseType.Accept);

			if (fc.Run() == (int)ResponseType.Accept) {
				MainClass.MainWindow.AddFile(fc.Filename);
			}
			fc.Destroy();

		}
	}
}

