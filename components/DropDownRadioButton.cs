using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Gtk;

namespace Moscrif.IDE.Components
{
	class DropDownRadioButton: DropDownButton
	{
		public DropDownRadioButton()
		{
		}
		
		protected override void OnPressed ()
		{
			//base.OnPressed ();
			Gtk.Menu menu = new Gtk.Menu ();

			if (menu.Children.Length > 0) {
				Gtk.SeparatorMenuItem sep = new Gtk.SeparatorMenuItem ();
				sep.Show ();
				menu.Insert (sep, -1);
			}
			
			Gtk.RadioMenuItem grp = new Gtk.RadioMenuItem ("");
			
			foreach (ComboItem ci in items) {
				Gtk.RadioMenuItem mi = new Gtk.RadioMenuItem (grp, ci.Label.Replace ("_","__"));
				if (ci.Item == items.CurrentItem || ci.Item.Equals (items.CurrentItem))
					mi.Active = true;
				
				ComboItemSet isetLocal = items;
				ComboItem ciLocal = ci;
				mi.Activated += delegate {
					SelectItem (isetLocal, ciLocal);
				};
				mi.ShowAll ();
				menu.Insert (mi, -1);
			}
			menu.Popup (null, null, PositionFunc, 0, Gtk.Global.CurrentEventTime);
		}


	}
}

