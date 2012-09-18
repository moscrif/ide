using System;
using Gtk;

namespace Moscrif.IDE.Components
{

	[System.ComponentModel.Category("Moscfift.Ide.Components")]
	[System.ComponentModel.ToolboxItem(true)]
	public class ToggleButtonCustom:  Gtk.ToggleButton
	{
		string hoverMessage = null;
		Label label;
		Gtk.Image image;
		string text;
		string desc;
		string icon;
		//bool onlyImage = false;
		public ToggleButtonCustom(): base ()
		{
			label = new Label ();
			label.Xalign = 0;
			label.Xpad = 0;
			label.Ypad = 0;
			image = new Gtk.Image ();
			
			HBox box = new HBox (false, 6);
			box.PackStart (image, false, false, 0);
			box.PackStart (label, true, true, 0);
			Add (box);
			//Relief = ReliefStyle.None;

			//this.GdkWindow.Cursor = new Gdk.Cursor(Gdk.CursorType.Hand1);
		}

		protected override void OnClicked ()
		{
			if (!String.IsNullOrEmpty(linkUrl)){
				System.Diagnostics.Process.Start(linkUrl);
			}

			base.OnClicked ();
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
		
		void UpdateLabel ()
		{
			if (icon != null) {
				image.Pixbuf = MainClass.Tools.GetIconFromStock (icon, Gtk.IconSize.Menu);//new Gdk.Pixbuf(icon);
				image.Visible = true;
			} else {
				image.Visible = false;
			}
			string markup = string.Format ("<span underline=\"single\" foreground=\"#5a7ac7\">{0}</span>", text);
			if (!string.IsNullOrEmpty (desc))
				markup += string.Format("\n<span size=\"small\">{0}</span>",desc);
			label.Wrap= true;
			label.Markup = markup;
		}
		
		string linkUrl;
		
		public string LinkUrl {
			get { return linkUrl; }
			set { linkUrl = value; }
		}
		
		public Label InnerLabel {
			get { return label; }
		}
	}
}

