using System;
using Gtk;
using System.Reflection;
using Moscrif.IDE.Components;

namespace  Moscrif.IDE.Actions
{
	public class SaveAllPageAction : Gtk.Action
	{
		public SaveAllPageAction() : base("saveall", MainClass.Languages.Translate("menu_save_all"), MainClass.Languages.Translate("menu_title_save_all"), "save-all.png")
		{
		}

		protected override void OnActivated()
		{
			base.OnActivated();
			MainClass.MainWindow.EditorNotebook.SaveAllPage();
			MainClass.MainWindow.SaveWorkspace();
		}
	}
}

