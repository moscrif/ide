using System;
using System.Collections.Generic;
using Gtk;
using System.Reflection;
using Moscrif.IDE.Components;
using Moscrif.IDE.Iface.Entities;
using Moscrif.IDE.Controls;
using Moscrif.IDE.Settings;
using Moscrif.IDE.Workspace;

namespace  Moscrif.IDE.Actions
{
	public class DirectoryPreferencesPopUpAction : Gtk.Action
	{
		public DirectoryPreferencesPopUpAction(): base("directorypreferences", MainClass.Languages.Translate("menu_dir_preferencies"), MainClass.Languages.Translate("menu_title_dir_preferencies"),"folder-properties-16.png")
		{
		}

		protected override void OnActivated ()
		{
			base.OnActivated();
			MainClass.MainWindow.PropertisDirectory();
		}
	}
}

