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
	public class FilePreferencesPopUpAction: Gtk.Action
	{
		public FilePreferencesPopUpAction(): base("filepreferences",MainClass.Languages.Translate("menu_file_preferencies"), MainClass.Languages.Translate("menu_title_file_preferencies"),"file-properties.png")
		{
		}

		protected override void OnActivated ()
		{
			base.OnActivated();
			MainClass.MainWindow.PropertisFile();
		}
	}
}