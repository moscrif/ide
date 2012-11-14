using System;
using Gtk;
using Gdk;
using Cairo;

namespace Moscrif.IDE.Components
{

	[System.ComponentModel.Category("Moscfift.Ide.Components")]
	[System.ComponentModel.ToolboxItem(true)]
	public class BannerButton: EventBox
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
			//GdkWindow.Cursor = new Gdk.Cursor(Gdk.CursorType.Hand2);
			return base.OnMotionNotifyEvent(evnt);
		} 

		public BannerButton () : base ()
		{
			this.Events= Gdk.EventMask.AllEventsMask;
			//this.VisibleWindow = false;

			image = new Gtk.Image ();

			image.Xalign = 0.5F;
			image.Yalign = 0.5F;

			HBox hb = new HBox();
			hb.PackStart(image);

			this.Add(hb);
			this.ShowAll();
			this.Realized += delegate {
				this.GdkWindow.Cursor = new Gdk.Cursor(Gdk.CursorType.Hand2);
			};
		}
			
		public Gdk.Pixbuf ImageIcon {
			get{
				return this.image.Pixbuf;
			}
			set { 	
				this.image.Pixbuf = value;
			}
		}
				
		string linkUrl;
		public string LinkUrl {
			get { return linkUrl; }
			set { linkUrl = value; }
		}
	}
}


