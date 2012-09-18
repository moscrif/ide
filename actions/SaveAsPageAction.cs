using System;
using Gtk;
using System.Reflection;
using Moscrif.IDE.Components;

namespace Moscrif.IDE.Actions
{
	public class SaveAsPageAction : Gtk.Action
	{
		public SaveAsPageAction() : base("saveas", MainClass.Languages.Translate("menu_save_as"), MainClass.Languages.Translate("menu_title_save_as"),null ){//"save-as.png"
		}

		protected override void OnActivated()
		{
			base.OnActivated();
			MainClass.MainWindow.EditorNotebook.SaveAs();
		}
	}
}