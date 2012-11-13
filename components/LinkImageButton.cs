using System;
using Gtk;

namespace Moscrif.IDE.Components
{

	[System.ComponentModel.Category("Moscfift.Ide.Components")]
	[System.ComponentModel.ToolboxItem(true)]
	public class LinkImageButton:  Gtk.Button
	{
		string hoverMessage = null;

		Label label;
		Gtk.Image image;

		string text;
		//string desc;
		string icon;


		protected override void OnClicked ()
		{
			if (!String.IsNullOrEmpty(linkUrl)){
				System.Diagnostics.Process.Start(linkUrl);
			}

			base.OnClicked ();
		}


		public LinkImageButton () : base ()
		{
			label = new Label ();
			label.Xalign = 0;
			label.Xpad = 0;
			label.Ypad = 0;
			image = new Gtk.Image ();
			
			VBox box = new VBox (false, 6);
			box.PackStart (label, true, true, 0);
			box.PackStart (image, false, false, 0);
			Add (box);
			Relief = ReliefStyle.None;

			/*Relief = ReliefStyle.None;
			Xalign = 0.5F;
			Yalign = 0.5F;*/
		}
			
		public string HoverMessage {
			get { return hoverMessage; }
			set {
				hoverMessage = value;
				this.TooltipText = hoverMessage;
			}
		}

		public new string Label {
			get { return text; }
			set { text = value; UpdateLabel (); }
		}
		
		/*public string Description {
			get { return desc; }
			set { desc = value; UpdateLabel (); }
		}*/
		
		public string Icon {
			get { return icon; }
			set { icon = value; UpdateLabel (); }
		}

		/*public Gdk.Pixbuf ImageIcon {
			get{
				return null;
			}
			set { 	
				Image = new Image(value);
				Image.Visible = true; 
			}
		}*/
		
		void UpdateLabel ()
		{
			if (icon != null) {
				image.Pixbuf = new Gdk.Pixbuf(icon);
				image.Visible = true;

			} else {
				image.Visible = false;
			}
			string markup = string.Format ("<span foreground=\"#697077\">{0}</span>", text);

			label.Wrap= true;
			label.Markup = markup;
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


