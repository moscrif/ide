using System;
using System.Collections.Generic;
using Gtk;
using System.Reflection;
using Moscrif.IDE.Components;
using Moscrif.IDE.Controls.Wizard;
using MessageDialogs = Moscrif.IDE.Controls.MessageDialog;
using Moscrif.IDE.Iface.Entities;


namespace Moscrif.IDE.Actions
{
	public class NewProjectWizzardAction: Gtk.Action
	{
		public NewProjectWizzardAction(): base("newprojectwizzard", MainClass.Languages.Translate("menu_new_project_wizzard"), MainClass.Languages.Translate("menu_title_new_project_wizzard"),"project-new.png")
		{

		}

		protected override void OnActivated()
		{
			base.OnActivated();

			NewProjectWizzard_Old npw = new NewProjectWizzard_Old();
			npw.Run();
			Console.WriteLine("Test");
			npw.Destroy();
		}
	}
}

