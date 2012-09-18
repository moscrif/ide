using System;
using System.Collections.Generic;
using Gtk;
using System.Reflection;
using Moscrif.IDE.Components;
using Moscrif.IDE.Iface.Entities;
using Moscrif.IDE.Controls;
using Moscrif.IDE.Settings;
using MessageDialogs = Moscrif.IDE.Controls.MessageDialog;

namespace Moscrif.IDE.Actions
{
	public class ProjectPreferencesAction: Gtk.Action
	{
		public ProjectPreferencesAction(): base("projectpreferences", MainClass.Languages.Translate("menu_project_preferences"), MainClass.Languages.Translate("menu_title_project_preferences"),"project-preferences.png")
		{
		}

		protected override void OnActivated ()
		{
			base.OnActivated();

			if (MainClass.Workspace.ActualProject == null){
					MessageDialogs md =
					new MessageDialogs(MessageDialogs.DialogButtonType.Ok, "Please, select project.", "", Gtk.MessageType.Error);
					md.ShowDialog();
				return;
			}

			PreferencesDialog pd = new PreferencesDialog(TypPreferences.ProjectSetting,MainClass.Workspace.ActualProject,MainClass.Workspace.ActualProject.ProjectName);
			int result = pd.Run();
			if (result == (int)ResponseType.Ok) {
				MainClass.Workspace.SaveProject(MainClass.Workspace.ActualProject);
			}
			pd.Destroy();
		}
	}
}
//
