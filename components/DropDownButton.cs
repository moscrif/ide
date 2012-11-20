//
// ConfigurationComboBox.cs
//
// Author:
//   Lluis Sanchez Gual
//
// Copyright (C) 2005 Novell, Inc (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Gtk;

namespace Moscrif.IDE.Components
{
	class DropDownButton : Gtk.Button
	{
		Gtk.Label label;
		//List<ComboItemSet> items = new List<ComboItemSet> ();
		ComboItemSet items = new ComboItemSet ();

		public event EventHandler<ChangedEventArgs> Changed;
		
		public class ComboItem
		{
			public ComboItem (string label, object item) {
				Label = label;
				Item = item;
			}
			public string Label { get; set; }
			public object Item { get; set; }
		}
		
		public class ComboItemSet: List<ComboItem>
		{
			public object CurrentItem { get; set; }
			
			public new void Clear ()
			{
				base.Clear ();
				CurrentItem = null;
			}

			public ComboItem FindItem(string label){
				return this.Find(x=>x.Label == label);
			}
		}
		
		public class ChangedEventArgs: EventArgs
		{
			public ComboItemSet ItemSet { get; set; }
			public object Item { get; set; }
		}
		
		public DropDownButton ()
		{
			Gtk.HBox hbox = new Gtk.HBox ();
			
			label = new Gtk.Label ();
			label.Xalign = 0;
			label.WidthRequest = 125;
			label.Ellipsize = Pango.EllipsizeMode.End;

			hbox.PackStart (label, true, true, 3);
			
			hbox.PackEnd (new Gtk.Arrow (Gtk.ArrowType.Down, Gtk.ShadowType.None), false, false, 1);
			hbox.PackEnd (new Gtk.VSeparator (), false, false, 1);
			Child = hbox;
		}
		
		public void ModifyLabelFont (Pango.FontDescription font)
		{
			label.ModifyFont (font);
		}

		public object CurrentItem {
			get {
				return items.CurrentItem;
			}
			set {
				items.CurrentItem = value;
			}
		}

		public string MarkupFormat {
			get ;
			set ;
		}

		public string ActiveText {
			get {
				return label.Text;
			}
			set {
				if(!String.IsNullOrEmpty(MarkupFormat)){
					label.UseMarkup = true;
					label.Markup = String.Format(MarkupFormat,value);
				} else {
					label.Text = value;
				}
			}
		}
		
		public void Clear ()
		{
			items.Clear ();
			ActiveText = "";
		}
		
		public void SetItemSet (ComboItemSet iset)
		{
			items.Clear();
			items = iset;
		}
		
		public void SelectItem (ComboItemSet iset, ComboItem item)
		{
			if(item != null){
				iset.CurrentItem = item.Item;
				ActiveText = item.Label;
			} else {
				iset.CurrentItem = null;
				ActiveText =  "";
			}

			if (Changed != null) {
				ChangedEventArgs args = new ChangedEventArgs ();
				args.ItemSet = iset;
				if(item!=null)
					args.Item = item.Item;
				else args.Item = null;

				Changed (this, args);
			}
		}

		protected override void OnPressed ()
		{
			base.OnPressed ();
			Gtk.Menu menu = new Gtk.Menu ();
			//foreach (ComboItemSet iset in items) {

			//if (items.Count == 0)
			//	continue;
				
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
			//}
			menu.Popup (null, null, PositionFunc, 0, Gtk.Global.CurrentEventTime);
		}
		
		void PositionFunc (Gtk.Menu mn, out int x, out int y, out bool push_in)
		{
			this.GdkWindow.GetOrigin (out x, out y);
			Gdk.Rectangle rect = this.Allocation;
			x += rect.X;
			y += rect.Y + rect.Height;
			
			//if the menu would be off the bottom of the screen, "drop" it upwards
			if (y + mn.Requisition.Height > this.Screen.Height) {
				y -= mn.Requisition.Height;
				y -= rect.Height;
			}
			if (mn.SizeRequest ().Width < rect.Width)
				mn.WidthRequest = rect.Width;
			
			//let GTK reposition the button if it still doesn't fit on the screen
			push_in = true;
		}
	}
	
}
