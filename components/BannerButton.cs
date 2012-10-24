using System;
using Gtk;

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

		public BannerButton () : base ()
		{

			image = new Gtk.Image ();

			image.Xalign = 0.5F;
			image.Yalign = 0.5F;

			this.Add(image);
			this.ShowAll();
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
		
		/*public Label InnerLabel {
			get { return label; }
		}*/
	}
}


