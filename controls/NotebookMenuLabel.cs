using System;
using Gdk;
using Gtk;
using Moscrif.IDE.Components;
using Moscrif.IDE.Editors;

namespace Moscrif.IDE.Controls
{
	public class NotebookMenuLabel : HBox
	{
		public Label lblDisplay= new Label();
		public string caption = "page";
		private Gtk.Menu popupMenu;
		//Gtk.Image image;

		public NotebookMenuLabel(string stockIcon, string caption, Gtk.Menu popupMenu)
		{
			this.CanFocus=false;
			this.BorderWidth = 1;
			this.popupMenu = popupMenu;

			Gtk.Image image = new Gtk.Image(MainClass.Tools.GetIconFromStock(stockIcon, Gtk.IconSize.Menu));
			image.SetPadding(2,2);
			this.PackStart(image, false, false, 0);


			lblDisplay.Text = caption;
			this.caption = caption;

			this.PackStart(lblDisplay, false, false, 0);

			Pixbuf default_pixbuf = null;
			string file = System.IO.Path.Combine(MainClass.Paths.ResDir, "stock-menu.png");
			if (System.IO.File.Exists(file)) {
				default_pixbuf = new Pixbuf(file);

				Button btnClose = new Button(new Gtk.Image(default_pixbuf));
				btnClose.TooltipText = "Menu";
				btnClose.Relief = ReliefStyle.None;
				btnClose.CanFocus = false;
				btnClose.WidthRequest = btnClose.HeightRequest = 19;

				this.popupMenu.AttachToWidget(btnClose,new Gtk.MenuDetachFunc(DetachWidget));

				btnClose.Clicked += delegate {
					if (this.popupMenu!= null){
						this.popupMenu.ShowAll();
						//this.popupMenu.Popup();
						this.popupMenu.Popup(null,null, new Gtk.MenuPositionFunc (GetPosition) ,3,Gtk.Global.CurrentEventTime);
					}
				};
				
				this.PackEnd(btnClose, false, false, 0);
			}


			this.ShowAll();
		}

		private void DetachWidget(Gtk.Widget attach_widget, Gtk.Menu menu){}
		static void GetPosition(Gtk.Menu menu, out int x, out int y, out bool push_in){
			
			menu.AttachWidget.GdkWindow.GetOrigin(out x, out y);

			x =menu.AttachWidget.Allocation.X+x;//+menu.AttachWidget.WidthRequest;
			y =menu.AttachWidget.Allocation.Y+y+menu.AttachWidget.HeightRequest;

			//Gtk.Requisition menu_req = menu.SizeRequest();

			push_in = true;
		}

		public void SetLabel(string newName){

			caption = newName;
			lblDisplay.Text = caption;
		}
	}
}

