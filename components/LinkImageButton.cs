using System;
using Gtk;

namespace Moscrif.IDE.Components
{

	[System.ComponentModel.Category("Moscfift.Ide.Components")]
	[System.ComponentModel.ToolboxItem(true)]
	public class LinkImageButton:  Gtk.Button
	{
		string hoverMessage = null;
		//Label label;
		//Gtk.Image image;
		string text;
		string desc;
		string icon;
		//bool onlyImage = false;

		protected override void OnClicked ()
		{
			if (!String.IsNullOrEmpty(linkUrl)){
				System.Diagnostics.Process.Start(linkUrl);
			}

			base.OnClicked ();
		}


		public LinkImageButton () : base ()
		{
			/*this.onlyImage = onlyImage;
			image = new Gtk.Image ();

			image.Xalign = 0.5F;
			image.Yalign = 0.5F;
			//image.Xpad = 0;
			//image.Ypad = 0;

			//Add(image);
			HBox box = new HBox (false, 0);

			box.PackEnd (image, true, true, 0);
			Add (box);*/
			Relief = ReliefStyle.None;
			Xalign = 0.5F;
			Yalign = 0.5F;
			//this.
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
		
		public string Description {
			get { return desc; }
			set { desc = value; UpdateLabel (); }
		}
		
		public string Icon {
			get { return icon; }
			set { icon = value; UpdateLabel (); }
		}

		public Gdk.Pixbuf ImageIcon {
			get{
				return null;
			}
			set { 	
				Image = new Image(value);
				Image.Visible = true; 
			}
		}
		
		void UpdateLabel ()
		{
			if (icon != null) {
				Image = new Image(new Gdk.Pixbuf(icon));
				Image.Visible = true;

			} else {
				Image.Visible = false;
			}
			/*string markup = string.Format ("<span underline=\"single\" foreground=\"#5a7ac7\">{0}</span>", text);
			if (!string.IsNullOrEmpty (desc))
				markup += string.Format("\n<span size=\"small\">{0}</span>",desc);
			label.Wrap= true;
			label.Markup = markup;*/
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


