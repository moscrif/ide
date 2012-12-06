using System;
using Gtk;

namespace Moscrif.IDE.Components
{

	[System.ComponentModel.ToolboxItem(true)]
	public class MenusToolButton:  ToolButton //Gtk.MenuToolButton //Gtk.ToolButton
	{
		Gtk.Menu menu;
		
		public MenusToolButton (Gtk.Menu menu, string icon):base(null,null) // base (icon)
		{
                        HBox box = new HBox(false, 0);
                        box.BorderWidth =  2;
 
                        /* Now on to the image stuff */

                        Gtk.Image image = new Gtk.Image(MainClass.Tools.GetIconFromStock(icon, Gtk.IconSize.SmallToolbar));
			Gtk.Arrow arrow = new Gtk.Arrow (Gtk.ArrowType.Down, Gtk.ShadowType.None);
                        /* Create a label for the button */
                        Label label = new Label ("label_text");
 
                        /* Pack the image and label into the box */
                        box.PackStart (image, false, false, 3);
                        box.PackStart(arrow , false, false, 3);
 
                        image.Show();
                        label.Show();

			this.IconWidget = box;

			this.menu = menu;
			//this.Menu = menu;
			this.Events = Gdk.EventMask.AllEventsMask;
			Child.Events = Gdk.EventMask.AllEventsMask;

			this.Clicked+= delegate(object sender, EventArgs e) {
				this.menu.Popup (null, null, new Gtk.MenuPositionFunc (OnPosition), 3, Gtk.Global.CurrentEventTime);
			};

			if (string.IsNullOrEmpty (icon)) {
				this.Expand = false;
				this.Homogeneous = false;
				this.IconWidget = new Gtk.Arrow (Gtk.ArrowType.Down, Gtk.ShadowType.None);
			}
		}


		void OnPosition (Gtk.Menu menu, out int x, out int y, out bool pushIn)
		{
			this.ParentWindow.GetOrigin (out x, out y);
			x += this.Allocation.X;
			y += this.Allocation.Y + this.Allocation.Height;
			pushIn = true;
		}
	}
}
