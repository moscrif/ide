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
//		public NewProjectWizzardAction(): base("newprojectwizzard", MainClass.Languages.Translate("menu_new_project_wizzard"), MainClass.Languages.Translate("menu_title_new_project_wizzard"),"project-new.png")
		public NewProjectWizzardAction() : base("newproject", MainClass.Languages.Translate("menu_new_project"), MainClass.Languages.Translate("menu_title_new_project"),"project-new.png")
		{

		}

		protected override void OnActivated()
		{
			base.OnActivated();

			NewProjectWizzard_New npw = new NewProjectWizzard_New(null);
			npw.Run();
			npw.Destroy();
		}
	}
}

