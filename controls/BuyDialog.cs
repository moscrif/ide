using System;
using System.Collections;
using System.Collections.Generic;
using Gtk;
using Gdk;
using Moscrif.IDE.Iface.Entities;
using Moscrif.IDE.Components;


namespace Moscrif.IDE.Controls
{
	public class BuyDialog: Dialog
	{

		TextView viewHeader;
		TextView viewFooter;
		TextView viewTable;

		bool hoveringOverLink = false;
		Gdk.Cursor handCursor, regularCursor;
		License featureLicence = null;
		
		string featureTitle;
		int featureLicenceId =-100;

		public BuyDialog(int featureLicenceId,string featureTitle,Gtk.Window parent) : base ( )
		{
			this.featureTitle = featureTitle;
			this.featureLicenceId = featureLicenceId;
			this.HasSeparator = false;
			if(parent!=null)
				this.TransientFor = parent;
			this.WidthRequest = 570;
			this.HeightRequest = 450;
			//btnBuyNow.ModifyBg(StateType.Normal,new Color(109,158,24));
			
			this.ModifyBg (Gtk.StateType.Normal, Style.White);
			string typ ="-100";
			if(MainClass.User!=null)
				typ =MainClass.User.LicenseId;
			
			string userLicenceId ="-100";
			if(MainClass.User != null){
				userLicenceId = MainClass.User.LicenseId;
			}
			
			int iTyp =0;
			if(!Int32.TryParse(userLicenceId,out iTyp)){
				iTyp = -100;
			}

			featureLicence = MainClass.LicencesSystem.GetLicence(featureLicenceId.ToString());
			
			handCursor = new Gdk.Cursor (Gdk.CursorType.Hand2);
			regularCursor = new Gdk.Cursor (Gdk.CursorType.Xterm);
			
			viewHeader = new TextView ();
			TextBuffer buffer = viewHeader.Buffer;

			viewFooter = new TextView ();
			TextBuffer buffer2 = viewFooter.Buffer;
			viewFooter.KeyPressEvent += new KeyPressEventHandler (KeyPress);
			viewFooter.WidgetEventAfter += new WidgetEventAfterHandler (EventAfter);
			viewFooter.MotionNotifyEvent += new MotionNotifyEventHandler (MotionNotify);
			viewFooter.HeightRequest = 15;
			//viewFooter.ModifyBg(StateType.Normal,new Color(255,0,0));//242,247,252

			viewTable = new TextView ();
			TextBuffer buffer3 = viewTable.Buffer;


			ScrolledWindow sw = new ScrolledWindow ();
			sw.SetPolicy (PolicyType.Automatic, PolicyType.Automatic);
			sw.HeightRequest = 115;
			sw.Add (viewHeader);

			ScrolledWindow sw2 = new ScrolledWindow ();
			sw2.SetPolicy (PolicyType.Automatic, PolicyType.Automatic);
			sw2.HeightRequest = 15;
			sw2.Add (viewFooter);

			ScrolledWindow sw3 = new ScrolledWindow ();
			sw3.SetPolicy (PolicyType.Automatic, PolicyType.Automatic);
			sw3.Add (viewTable);

			CreateTags (buffer);
			CreateTags (buffer2);
			CreateTags (buffer3);
			InsertTextHeader (buffer);
			InsertTextFooter (buffer2);
			InsertTextTable (buffer3);

			this.ShowAll();
			//viewHeader.ModifyBase(StateType.Normal,this.Style.Background(StateType.Normal));

			Table tbl = new Table(4,1,false);

			//Button btnBuy = new Button();
			BannerButton btnBuy = new BannerButton();
			btnBuy.ModifyBase(StateType.Normal,new Gdk.Color(109,158,24));
			btnBuy.ModifyBg(StateType.Normal,new Color(109,158,24));
			btnBuy.HeightRequest = 38;
			btnBuy.WidthRequest = 170;
			string buyPath = System.IO.Path.Combine(MainClass.Paths.ResDir,"btnBuy.png");
			btnBuy.ButtonPressEvent+= delegate(object o, ButtonPressEventArgs args) {
				string url = "http://moscrif.com/download?t={0}";
				if (MainClass.User!=null && (!String.IsNullOrEmpty(MainClass.User.Token))) {
					url = string.Format(url,MainClass.User.Token);
					
				}
				
				System.Diagnostics.Process.Start(url);			
				this.Respond( Gtk.ResponseType.Ok );
			};

			btnBuy.ImageIcon = new Pixbuf(buyPath);
			//btnBuy.Xalign = 0.5F;
			//btnBuy.Yalign =0.5F;
			/*btnBuy.Clicked+= delegate(object sender, EventArgs e)
			{
				string url = "http://moscrif.com/download?t={0}";
				if (MainClass.User!=null && (!String.IsNullOrEmpty(MainClass.User.Token))) {
					url = string.Format(url,MainClass.User.Token);
					
				}
				
				System.Diagnostics.Process.Start(url);			
				this.Respond( Gtk.ResponseType.Ok );
			};*/
			//btnBuy.Label="Buy";


			//Button btnCancel = new Button();
			BannerButton btnCancel = new BannerButton();
			btnCancel.HeightRequest = 38;
			btnCancel.WidthRequest = 170;
			string cancelPath = System.IO.Path.Combine(MainClass.Paths.ResDir,"btnCancel.png");

			btnCancel.ImageIcon = new Pixbuf(cancelPath);
			btnCancel.ButtonPressEvent+= delegate(object o, ButtonPressEventArgs args) {
				this.Respond( Gtk.ResponseType.Cancel );
			};
			/*btnCancel.Clicked+= delegate(object sender, EventArgs e) {
				this.Respond( Gtk.ResponseType.Cancel );
			};*/
			//btnCancel.Label="Cancel";
			//btnCancel.ModifyBg(StateType.Normal,new Color(109,158,24));
			//btnCancel.Xalign = 0.5F;
			//btnCancel.Yalign =0.5F;
			Table tblButton = new Table (1,4,false);
			tblButton.ColumnSpacing = 12;
			tblButton.BorderWidth = 6;
			tblButton.Attach(new Label(""),0,1,0,1,AttachOptions.Expand,AttachOptions.Expand,0,0);
			tblButton.Attach(btnCancel,1,2,0,1,AttachOptions.Shrink,AttachOptions.Shrink,0,0);
			tblButton.Attach(btnBuy,2,3,0,1,AttachOptions.Shrink,AttachOptions.Shrink,0,0);
			tblButton.Attach(new Label(""),3,4,0,1,AttachOptions.Expand,AttachOptions.Expand,0,0);

			/*HBox h = new HBox ( );
			h.BorderWidth = 6;
			h.Spacing = 12;
			h.PackStart (btnCancel,false,false,0);
			h.PackStart (btnBuy,false,false,0);
			h.HeightRequest = 50;*/

			tbl.Attach(sw,0,1,0,1,AttachOptions.Fill,AttachOptions.Fill,0,0);

			tbl.Attach(sw3,0,1,1,2,AttachOptions.Fill|AttachOptions.Expand,AttachOptions.Fill|AttachOptions.Expand,0,0);
			tbl.Attach(tblButton,0,1,2,3,AttachOptions.Fill,AttachOptions.Fill,0,0);
			tbl.Attach(sw2,0,1,3,4,AttachOptions.Fill,AttachOptions.Shrink,0,0);

			tbl.ShowAll();
			this.VBox.Add (tbl);
			/*HBox h = new HBox ( );
			h.BorderWidth = 6;
			h.Spacing = 12;
			
			VBox v = new VBox ( );


			v.PackStart (sw);

			Button btnBuy = new Button();
			btnBuy.ModifyBase(StateType.Normal,new Gdk.Color(109,158,24));
			btnBuy.ModifyBg(StateType.Normal,new Color(109,158,24));
			btnBuy.Label="Buy";

			v.PackEnd (btnBuy);
			
			h.PackEnd (v);
			h.ShowAll ( );
			this.VBox.Add (h);*/
			
			//this.AddButton (Stock.Cancel, ResponseType.Cancel);  
			//this.AddButton (Stock.Ok, ResponseType.Ok);
		}

		private void CreateTags (TextBuffer buffer)
		{
			TextTag tag  = new TextTag ("heading");
			tag.Weight = Pango.Weight.Bold;
			tag.Size = (int) Pango.Scale.PangoScale * 15;
			//tag.Justification = Justification.Center;
			tag.Editable = false;
			tag.Foreground= "#5A646E";
			buffer.TagTable.Add (tag);
			
			tag  = new TextTag ("word_wrap");
			tag.WrapMode = WrapMode.Word;
			tag.Editable = false;
			buffer.TagTable.Add (tag);

			tag  = new TextTag ("word_justification");
			tag.Justification = Justification.Center;
			tag.Editable = false;
			buffer.TagTable.Add (tag);
			
		}
		
		private void InsertTextHeader (TextBuffer buffer)
		{
			Pixbuf pixbuf = MainClass.Tools.GetIconFromStock("logo74.png",IconSize.Dialog);
			pixbuf = pixbuf.ScaleSimple (32, 32, InterpType.Bilinear);
			
			Pixbuf pixbuf2 = MainClass.Tools.GetIconFromStock("file-ms.png",IconSize.Menu);

			TextIter insertIter = buffer.StartIter;

			string upgradeLicence = "";
			if(featureLicence != null){
				upgradeLicence = featureLicence.Name;
			}

			buffer.InsertWithTagsByName (ref insertIter, String.Format("Moscrif {0} ",upgradeLicence), "heading");
			buffer.Insert (ref insertIter,"\n\n");
			
			buffer.Insert (ref insertIter,
			               String.Format("The \"{0}\" is available for Moscrif {1} license. Please purchase an upgrade to unlock this Buying Moscrif {1} License you also unlock:\n\n",featureTitle,upgradeLicence));
			
			buffer.ApplyTag ("word_wrap", buffer.StartIter, buffer.EndIter);
		}

		private void InsertTextFooter (TextBuffer buffer)
		{

			TextIter insertIter = buffer.StartIter;

			buffer.Insert (ref insertIter,"More features area avalable in Pro License. ");
			InsertLink (buffer, ref insertIter, "Buy Pro now!", 1);
			
			buffer.ApplyTag ("word_justification", buffer.StartIter, buffer.EndIter);
		}

		private void InsertTextTable (TextBuffer buffer)
		{
			Pixbuf pixbuf = MainClass.Tools.GetIconFromStock("logo74.png",IconSize.Dialog);
			pixbuf = pixbuf.ScaleSimple (32, 32, InterpType.Bilinear);
			Pixbuf pixbuf2 = MainClass.Tools.GetIconFromStock("file-ms.png",IconSize.Menu);

			TextIter insertIter = buffer.StartIter;

			List<Feature> listDif = MainClass.LicencesSystem.GetUserDifferent(featureLicence);

			if(listDif!= null){
				foreach(Feature ftv in listDif){
					buffer.Insert (ref insertIter," ");
					buffer.InsertPixbuf (ref insertIter, pixbuf2);
					buffer.Insert (ref insertIter, String.Format(" {0} \n", ftv.Name));
				}
			}
			
			//buffer.Insert (ref insertIter,"\n");
			//buffer.Insert (ref insertIter,"More features area avalable in Pro License.");
			//InsertLink (buffer, ref insertIter, "Buy Pro now!", 1);
			
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
					string url = "http://moscrif.com/download";
					if (MainClass.User!=null && (!String.IsNullOrEmpty(MainClass.User.Token))) {
						url = string.Format(url,MainClass.User.Token);
						
					}
					
					System.Diagnostics.Process.Start(url);	
				}
			}
		}

	}
}
