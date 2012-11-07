using System;
using Gtk;
using Gdk;
using Cairo;


namespace Moscrif.IDE.Components
{

	[System.ComponentModel.Category("Moscfift.Ide.Components")]
	[System.ComponentModel.ToolboxItem(true)]
	public class BannerButton:  EventBox
	{
		private Gtk.Image image ;

		protected override bool OnButtonPressEvent (Gdk.EventButton evt)
		{
			if (!String.IsNullOrEmpty(linkUrl)){
				System.Diagnostics.Process.Start(linkUrl);
			}
			return base.OnButtonPressEvent (evt);
		}

		protected override bool OnMotionNotifyEvent(Gdk.EventMotion evnt)
		{
			/*int x, y;
			Gdk.ModifierType state;
			evnt.Window.GetPointer (out x, out y, out state);
			if ((state & Gdk.ModifierType.Button1Mask) != 0)
				GdkWindow.Cursor = new Gdk.Cursor(Gdk.CursorType.Hand2);
			else 
				GdkWindow.Cursor = new Gdk.Cursor(Gdk.CursorType.Arrow);
			*/
			return base.OnMotionNotifyEvent(evnt);
		} 

		/*bannerImage.MotionNotifyEvent+= delegate(object o, MotionNotifyEventArgs args) {

		};*/

		public BannerButton () : base ()
		{
			this.Events= Gdk.EventMask.AllEventsMask;
			//this.VisibleWindow = false;

			image = new Gtk.Image ();

			image.Xalign = 0.5F;
			image.Yalign = 0.5F;

			this.WidthRequest = 200;
			this.HeightRequest = 40;
			this.Add(image);
			this.ShowAll();
			image.Realized += delegate {
				image.GdkWindow.Cursor = new Gdk.Cursor(Gdk.CursorType.Hand2);
			};
		}


		/*protected override bool OnExposeEvent(Gdk.EventExpose evnt)
		{

			if ((ImageIcon != null)) {
				var gc = Style.BackgroundGC ( State);
				var lRect = new Rectangle (Allocation.X, Allocation.Y, imageIcon.Width, imageIcon.Height);
				//if (evnt.Region.RectIn (lRect) != OverlapType.Out)
					GdkWindow.DrawPixbuf (gc, imageIcon, 0, 0, lRect.X, lRect.Y, lRect.Width, lRect.Height, RgbDither.None, 0, 0);	
			}
			
			foreach (Widget widget in Children)
				PropagateExpose (widget, evnt);
			
			return base.OnExposeEvent (evnt);
		}*/
			
		public Gdk.Pixbuf ImageIcon {
			get{
				//return imageIcon;
				return this.image.Pixbuf;
			}
			set { 	
				//if(GdkWindow!=null)
				//	GdkWindow.InvalidateRect(new Gdk.Rectangle(0,0, imageIcon.Width, imageIcon.Height), false);
				this.image.Pixbuf = value;
			}
		}
				
		string linkUrl;
		public string LinkUrl {
			get { return linkUrl; }
			set { linkUrl = value; }
		}
		
		/*public Label InnerLabel {
			get { return label; }
		}*/
	}
}


