using System;
using Gtk;
using Gdk;
using Moscrif.IDE.Iface.Entities;

namespace Moscrif.IDE.Controls
{
	public partial class LicenceDialog : Gtk.Dialog
	{
		TextView view1;
		//TextView view2;
		const int gray50_width = 2;
		const int gray50_height = 2;
		const string gray50_bits = "\x02\x01";

		bool hoveringOverLink = false;
		Gdk.Cursor handCursor, regularCursor;
		License lcs = null;
		public LicenceDialog()
		{
			this.Build();
			btnBuyNow.ModifyBg(StateType.Normal,new Color(109,158,24));

			this.ModifyBg (Gtk.StateType.Normal, Style.White);
			string typ ="-100";
			if(MainClass.User!=null)
				typ =MainClass.User.LicenseId;
			
			lcs = MainClass.LicencesSystem.GetNextLicence(typ);

			handCursor = new Gdk.Cursor (Gdk.CursorType.Hand2);
			regularCursor = new Gdk.Cursor (Gdk.CursorType.Xterm);
					
			view1 = new TextView ();
			TextBuffer buffer = view1.Buffer;
			view1.KeyPressEvent += new KeyPressEventHandler (KeyPress);
			view1.WidgetEventAfter += new WidgetEventAfterHandler (EventAfter);
			view1.MotionNotifyEvent += new MotionNotifyEventHandler (MotionNotify);
			
			ScrolledWindow sw = new ScrolledWindow ();
			sw.SetPolicy (PolicyType.Automatic, PolicyType.Automatic);
			table1.Attach(sw,0,1,0,1,AttachOptions.Expand|AttachOptions.Fill,AttachOptions.Expand|AttachOptions.Fill,0,0);
			sw.Add (view1);
			
			/*sw = new ScrolledWindow ();
			sw.SetPolicy (PolicyType.Automatic, PolicyType.Automatic);
			table1.Attach(sw,0,1,1,2,AttachOptions.Expand|AttachOptions.Fill,AttachOptions.Expand|AttachOptions.Fill,0,0);
			sw.Add (view2);*/

			CreateTags (buffer);
			InsertText (buffer);
		//	AttachWidgets(view1);

			this.ShowAll();
			view1.ModifyBase(StateType.Normal,this.Style.Background(StateType.Normal));
		}
		private TextChildAnchor tableAnchor;
		private TextChildAnchor animationAnchor;
		//private TextChildAnchor buttonAnchor;
		/*private TextChildAnchor menuAnchor;
		private TextChildAnchor scaleAnchor;
		private TextChildAnchor animationAnchor;
		private TextChildAnchor entryAnchor;*/
		/*
		private void AttachWidgets (TextView textView)
		{
			// This is really different from the C version, but the
			// C versions seems a little pointless.

			int x = 0;
			Table tbl = new Table( (uint)MainClass.Settings.LibsDefine.Count,1,false);

			if(lcs!= null){
				foreach(Feature ftv in lcs.Featutes){
					CheckButton chb = new CheckButton(ftv.Name);
					chb.Name = ftv.Name;
					chb.Active = true;
					chb.Sensitive = false;
					tbl.Attach(chb,0,1,(uint)x,(uint)(x+1),AttachOptions.Fill,AttachOptions.Fill,5,0);
					x++;
				}
			}
			//foreach(string sv in MainClass.Settings.LibsDefine){
			//	CheckButton chb = new CheckButton(sv);
			//	chb.Name = sv;
			//	chb.Active = true;
			//	chb.Sensitive = false;
			//	tbl.Attach(chb,0,1,(uint)x,(uint)(x+1),AttachOptions.Fill,AttachOptions.Fill,5,0);
			//	x++;
			//}
			textView.AddChildAtAnchor (tbl, tableAnchor);

			//Gtk.Image image = Gtk.Image(MainClass.Tools.GetIconFromStock("file-ms.png")); //LoadFromResource ("floppybuddy.gif");
			//textView.AddChildAtAnchor (image, animationAnchor);
			//image.ShowAll ();
		
		}*/

		private void CreateTags (TextBuffer buffer)
		{
			// Create a bunch of tags. Note that it's also possible to
			// create tags with gtk_text_tag_new() then add them to the
			// tag table for the buffer, gtk_text_buffer_create_tag() is
			// just a convenience function. Also note that you don't have
			// to give tags a name; pass NULL for the name to create an
			// anonymous tag.
			//
			// In any real app, another useful optimization would be to create
			// a GtkTextTagTable in advance, and reuse the same tag table for
			// all the buffers with the same tag set, instead of creating
			// new copies of the same tags for every buffer.
			//
			// Tags are assigned default priorities in order of addition to the
			// tag table.	 That is, tags created later that affect the same text
			// property affected by an earlier tag will override the earlier
			// tag.  You can modify tag priorities with
			// gtk_text_tag_set_priority().
			
			TextTag tag  = new TextTag ("heading");
			tag.Weight = Pango.Weight.Bold;
			tag.Size = (int) Pango.Scale.PangoScale * 15;
			//tag.Justification = Justification.Center;
			tag.Editable = false;
			tag.Foreground= "#5A646E";
			buffer.TagTable.Add (tag);
			
			// The C gtk-demo passes NULL for the drawable param, which isn't
			// multi-head safe, so it seems bad to allow it in the C# API.
			// But the Window isn't realized at this point, so we can't get
			// an actual Drawable from it. So we kludge for now.
			Pixmap stipple = Pixmap.CreateBitmapFromData (Gdk.Screen.Default.RootWindow, gray50_bits, gray50_width, gray50_height);
			
			tag  = new TextTag ("background_stipple");
			tag.BackgroundStipple = stipple;
			tag.Editable = false;
			buffer.TagTable.Add (tag);
			
			tag  = new TextTag ("foreground_stipple");
			tag.ForegroundStipple = stipple;
			tag.Editable = false;
			buffer.TagTable.Add (tag);

			tag  = new TextTag ("word_wrap");
			tag.WrapMode = WrapMode.Word;
			tag.Editable = false;
			buffer.TagTable.Add (tag);

		}

		private void InsertText (TextBuffer buffer)
		{
			Pixbuf pixbuf = MainClass.Tools.GetIconFromStock("logo74.png",IconSize.Dialog);
			pixbuf = pixbuf.ScaleSimple (32, 32, InterpType.Bilinear);

			Pixbuf pixbuf2 = MainClass.Tools.GetIconFromStock("file-ms.png",IconSize.Menu);
			// get start of buffer; each insertion will revalidate the
			// iterator to point to just after the inserted text.
			
			TextIter insertIter = buffer.StartIter;

			buffer.InsertWithTagsByName (ref insertIter, String.Format("Moscrif {0} ",lcs.Name), "heading");
			buffer.Insert (ref insertIter,"\n\n");

			buffer.Insert (ref insertIter,
			               String.Format("The <FEATURE NAME> is available for Moscrif {0} license. Please purchase an upgrade to unlock this Buying Moscrif {0} License you also unlock:\n\n",lcs.Name));

			//tableAnchor = buffer.CreateChildAnchor (ref insertIter);
			if(lcs!= null){
				foreach(Feature ftv in lcs.Featutes){
					buffer.Insert (ref insertIter," ");
					buffer.InsertPixbuf (ref insertIter, pixbuf2);
					buffer.Insert (ref insertIter, String.Format(" {0} \n", ftv.Name));
				}
			}
			//buttonAnchor = buffer.CreateChildAnchor (ref insertIter);

			buffer.Insert (ref insertIter,"\n");
			buffer.Insert (ref insertIter,"More features area avalable in Pro License.");
			InsertLink (buffer, ref insertIter, "Buy Pro now!", 1);

			/*buttonAnchor = buffer.CreateChildAnchor (ref insertIter);
			buffer.Insert (ref insertIter, " and a menu: ");
			menuAnchor = buffer.CreateChildAnchor (ref insertIter);
			buffer.Insert (ref insertIter, " and a scale: ");
			scaleAnchor = buffer.CreateChildAnchor (ref insertIter);
			buffer.Insert (ref insertIter, " and an animation: ");
			animationAnchor	= buffer.CreateChildAnchor (ref insertIter);
			buffer.Insert (ref insertIter, " finally a text entry: ");
			entryAnchor = buffer.CreateChildAnchor (ref insertIter);
			buffer.Insert (ref insertIter, ".\n");
			*/

			buffer.ApplyTag ("word_wrap", buffer.StartIter, buffer.EndIter);
		}

		void InsertLink (TextBuffer buffer, ref TextIter iter, string text, int page)
		{
			TextTag tag = new TextTag (null);
			tag.Foreground = "blue";
			tag.Underline = Pango.Underline.Single;
			buffer.TagTable.Add (tag);
			buffer.InsertWithTags (ref iter, text, tag);
		}

		void KeyPress (object sender, KeyPressEventArgs args)
		{
			TextView view = sender as TextView;
			
			switch ((Gdk.Key) args.Event.KeyValue) {
			case Gdk.Key.Return:
			case Gdk.Key.KP_Enter:
				TextIter iter = view.Buffer.GetIterAtMark (view.Buffer.InsertMark);
				FollowIfLink (view, iter);
				break;
			default:
				break;
			}
		}
		
		// Links can also be activated by clicking.
		void EventAfter (object sender, WidgetEventAfterArgs args)
		{
			if (args.Event.Type != Gdk.EventType.ButtonRelease)
				return;
			
			Gdk.EventButton evt = (Gdk.EventButton)args.Event;
			
			if (evt.Button != 1)
				return;
			
			TextView view = sender as TextView;
			TextIter start, end, iter;
			int x, y;
			
			// we shouldn't follow a link if the user has selected something
			view.Buffer.GetSelectionBounds (out start, out end);
			if (start.Offset != end.Offset)
				return;
			
			view.WindowToBufferCoords (TextWindowType.Widget, (int) evt.X, (int) evt.Y, out x, out y);
			iter = view.GetIterAtLocation (x, y);
			
			FollowIfLink (view, iter);
		}
		
		// Update the cursor image if the pointer moved.
		void MotionNotify (object sender, MotionNotifyEventArgs args)
		{
			TextView view = sender as TextView;
			int x, y;
			Gdk.ModifierType state;
			
			view.WindowToBufferCoords (TextWindowType.Widget, (int) args.Event.X, (int) args.Event.Y, out x, out y);
			SetCursorIfAppropriate (view, x, y);
			
			view.GdkWindow.GetPointer (out x, out y, out state);
		}

		void SetCursorIfAppropriate (TextView view, int x, int y)
		{
			bool hovering = false;
			TextIter iter = view.GetIterAtLocation (x, y);
			
			foreach (TextTag tag in iter.Tags) {
				if (tag.Underline ==  Pango.Underline.Single) {
					hovering = true;
					break;
				}
			}
			
			if (hovering != hoveringOverLink) {
				Gdk.Window window = view.GetWindow (Gtk.TextWindowType.Text);
				
				hoveringOverLink = hovering;
				if (hoveringOverLink)
					window.Cursor = handCursor;
				else
					window.Cursor = regularCursor;
			}
		}
		void FollowIfLink (TextView view, TextIter iter)
		{
			foreach (TextTag tag in iter.Tags) {
				if (tag.Underline ==  Pango.Underline.Single){
					string url = "http://moscrif.com/download?t={0}";
					if (MainClass.User!=null && (!String.IsNullOrEmpty(MainClass.User.Token))) {
						url = string.Format(url,MainClass.User.Token);
						
					}
					
					System.Diagnostics.Process.Start(url);	
				}
			}
		}

		protected virtual void OnBtnBuyNowClicked (object sender, System.EventArgs e)
		{
			string url = "http://moscrif.com/download?t={0}";
			if (MainClass.User!=null && (!String.IsNullOrEmpty(MainClass.User.Token))) {
				url = string.Format(url,MainClass.User.Token);
				
			}
			
			System.Diagnostics.Process.Start(url);			
			this.Respond( Gtk.ResponseType.Ok );
		}
	}
}

