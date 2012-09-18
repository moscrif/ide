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
	public class PropertyAllAction: Gtk.Action
	{
		public PropertyAllAction(): base("propertyall", MainClass.Languages.Translate("menu_properties"), MainClass.Languages.Translate("menu_title_properties"),"project-preferences.png")
		{

		}

		protected override void OnActivated ()
		{
			base.OnActivated();

			MainClass.MainWindow.PropertisALL();

		}
	}
}

