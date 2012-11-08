using System;
using Gtk;

namespace Moscrif.IDE.Components
{

	[System.ComponentModel.Category("Moscfift.Ide.Components")]
	[System.ComponentModel.ToolboxItem(true)]
	public class WebButton:  Gtk.Button
	{
		string hoverMessage = null;
		Label label;
		string text;

		protected override void OnClicked ()
		{
			if (!String.IsNullOrEmpty(linkUrl)){
				System.Diagnostics.Process.Start(linkUrl);
			}

			base.OnClicked ();
		}

		public WebButton () : base ()
		{
			label = new Label ();
			label.Xalign = 0;
			label.Xpad = 0;
			label.Ypad = 0;

			HBox box = new HBox (false, 6);
			box.PackStart (label, true, true, 0);
			Add (box);
			Relief = ReliefStyle.None;

			box.HeightRequest = 25;
			//this.GdkWindow.Cursor = new Gdk.Cursor(Gdk.CursorType.Hand1);
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
		
		void UpdateLabel ()
		{

			string markup = string.Format ("<span  foreground=\"#697077\"><b>{0}</b></span>", text);
			//label.Wrap= true;
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

