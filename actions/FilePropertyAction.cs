using System;
using System.Collections.Generic;
using Gtk;
using System.Reflection;
using Moscrif.IDE.Components;
using Moscrif.IDE.Iface.Entities;
using Moscrif.IDE.Controls;
using Moscrif.IDE.Option;
using Moscrif.IDE.Workspace;
namespace Moscrif.IDE.Actions
{
	public class FilePropertyAction: Gtk.Action
	{
		public FilePropertyAction(): base("filesproperty", MainClass.Languages.Translate("menu_file_properties"), MainClass.Languages.Translate("menu_title_file_properties"),"project-preferences.png")
		{

		}

		protected override void OnActivated ()
		{
			base.OnActivated();

			MainClass.MainWindow.PropertisALL();

		}
	}
}

