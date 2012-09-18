using System;
using Gtk;
using System.Reflection;
using Moscrif.IDE.Components;

namespace Moscrif.IDE.Actions
{
	public class SavePageAction : Gtk.Action
	{
		public SavePageAction() : base("save",MainClass.Languages.Translate("menu_save"),MainClass.Languages.Translate("menu_title_save"), "save.png")
		{
		}

		protected override void OnActivated()
		{
			base.OnActivated();
			MainClass.MainWindow.EditorNotebook.SaveCurentPage();
		}
	}
}

