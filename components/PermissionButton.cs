using System;
using Gtk;
using System.Collections.Generic;
using Moscrif.IDE.Controls;

namespace Moscrif.IDE.Components
{
	public class PermissionButton : Button
	{
		private string permission;
		Gtk.Window parentWindow;

		public PermissionButton(string permission,Gtk.Window parent)
		{
			parentWindow = parent;
			this.permission = permission;
		}

		protected override void OnClicked ()
		{
			base.OnClicked ();
			PermissionEditor pe = new PermissionEditor(permission,parentWindow);
			int result = pe.Run();
			if (result == (int)ResponseType.Ok){
				if (!String.IsNullOrEmpty(pe.Permission) ){
					this.permission = pe.Permission;
				}
			}
			pe.Destroy();

		}

		public string Permission {
			get{
				return permission;
			}
			set{
				permission = value;
			}
		}
	}
}

