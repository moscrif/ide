using System;
using System.Collections.Generic;
using Gtk;
using System.Reflection;
using Moscrif.IDE.Components;
using Moscrif.IDE.Iface.Entities;
using Moscrif.IDE.Controls;
using Moscrif.IDE.Settings;
using Moscrif.IDE.Workspace;

namespace Moscrif.IDE.Actions
{
	public class ProjectPreferencesPopUpAction: Gtk.Action
	{
		public ProjectPreferencesPopUpAction(): base("projectpreferencespopup", MainClass.Languages.Translate("menu_project_preferences"), MainClass.Languages.Translate("menu_title_project_preferences"),"project-preferences.png")
		{
		}

		protected override void OnActivated ()
		{
			base.OnActivated();

			MainClass.MainWindow.PropertisProject();

		}
	}
}
