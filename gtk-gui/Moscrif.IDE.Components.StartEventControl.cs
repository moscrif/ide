
// This file has been generated by the GUI designer. Do not modify.
namespace Moscrif.IDE.Components
{
	public partial class StartEventControl
	{
		private global::Gtk.Table table1;
		private global::Gtk.Table tblAction;
		private global::Gtk.HBox hbox1;
		private global::Gtk.Image imgActions;
		private global::Gtk.Label lblActions;
		private global::Gtk.Label lblAccount;
		private global::Gtk.Label lblProject;
		private global::Gtk.Label lblWorkspace;
		private global::Gtk.Label lbRecent;
		private global::Gtk.Table tblContent;
		private global::Gtk.HBox hbox3;
		private global::Gtk.Image imgContent;
		private global::Gtk.Label lblContent;
		private global::Gtk.Table tblSamples;
		private global::Gtk.HBox hbox2;
		private global::Gtk.Image imgSamples;
		private global::Gtk.Label lblSamples;
		private global::Gtk.Table tblTwitt;
		private global::Gtk.Button btnTwitLoad;
		private global::Gtk.Image imgTwiter;
		private global::Gtk.Label lblTwiter;
		
		protected virtual void Build ()
		{
			global::Stetic.Gui.Initialize (this);
			// Widget Moscrif.IDE.Components.StartEventControl
			global::Stetic.BinContainer.Attach (this);
			this.Name = "Moscrif.IDE.Components.StartEventControl";
			// Container child Moscrif.IDE.Components.StartEventControl.Gtk.Container+ContainerChild
			this.table1 = new global::Gtk.Table (((uint)(5)), ((uint)(3)), false);
			this.table1.Name = "table1";
			this.table1.RowSpacing = ((uint)(6));
			this.table1.ColumnSpacing = ((uint)(6));
			// Container child table1.Gtk.Table+TableChild
			this.tblAction = new global::Gtk.Table (((uint)(6)), ((uint)(5)), false);
			this.tblAction.Name = "tblAction";
			this.tblAction.RowSpacing = ((uint)(6));
			this.tblAction.ColumnSpacing = ((uint)(6));
			// Container child tblAction.Gtk.Table+TableChild
			this.hbox1 = new global::Gtk.HBox ();
			this.hbox1.Name = "hbox1";
			this.hbox1.Spacing = 6;
			// Container child hbox1.Gtk.Box+BoxChild
			this.imgActions = new global::Gtk.Image ();
			this.imgActions.WidthRequest = 24;
			this.imgActions.HeightRequest = 24;
			this.imgActions.Name = "imgActions";
			this.hbox1.Add (this.imgActions);
			global::Gtk.Box.BoxChild w1 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.imgActions]));
			w1.Position = 0;
			w1.Expand = false;
			w1.Fill = false;
			// Container child hbox1.Gtk.Box+BoxChild
			this.lblActions = new global::Gtk.Label ();
			this.lblActions.Name = "lblActions";
			this.lblActions.Xalign = 0F;
			this.lblActions.LabelProp = global::Mono.Unix.Catalog.GetString ("Actions");
			this.hbox1.Add (this.lblActions);
			global::Gtk.Box.BoxChild w2 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.lblActions]));
			w2.Position = 1;
			w2.Expand = false;
			w2.Fill = false;
			this.tblAction.Add (this.hbox1);
			global::Gtk.Table.TableChild w3 = ((global::Gtk.Table.TableChild)(this.tblAction [this.hbox1]));
			w3.RightAttach = ((uint)(5));
			w3.XOptions = ((global::Gtk.AttachOptions)(4));
			w3.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child tblAction.Gtk.Table+TableChild
			this.lblAccount = new global::Gtk.Label ();
			this.lblAccount.Name = "lblAccount";
			this.lblAccount.LabelProp = global::Mono.Unix.Catalog.GetString ("Account");
			this.tblAction.Add (this.lblAccount);
			global::Gtk.Table.TableChild w4 = ((global::Gtk.Table.TableChild)(this.tblAction [this.lblAccount]));
			w4.TopAttach = ((uint)(1));
			w4.BottomAttach = ((uint)(2));
			w4.LeftAttach = ((uint)(3));
			w4.RightAttach = ((uint)(4));
			w4.XOptions = ((global::Gtk.AttachOptions)(4));
			w4.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child tblAction.Gtk.Table+TableChild
			this.lblProject = new global::Gtk.Label ();
			this.lblProject.Name = "lblProject";
			this.lblProject.LabelProp = global::Mono.Unix.Catalog.GetString ("Project");
			this.tblAction.Add (this.lblProject);
			global::Gtk.Table.TableChild w5 = ((global::Gtk.Table.TableChild)(this.tblAction [this.lblProject]));
			w5.TopAttach = ((uint)(1));
			w5.BottomAttach = ((uint)(2));
			w5.LeftAttach = ((uint)(1));
			w5.RightAttach = ((uint)(2));
			w5.XOptions = ((global::Gtk.AttachOptions)(4));
			w5.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child tblAction.Gtk.Table+TableChild
			this.lblWorkspace = new global::Gtk.Label ();
			this.lblWorkspace.Name = "lblWorkspace";
			this.lblWorkspace.LabelProp = global::Mono.Unix.Catalog.GetString ("Workspace");
			this.tblAction.Add (this.lblWorkspace);
			global::Gtk.Table.TableChild w6 = ((global::Gtk.Table.TableChild)(this.tblAction [this.lblWorkspace]));
			w6.TopAttach = ((uint)(1));
			w6.BottomAttach = ((uint)(2));
			w6.XOptions = ((global::Gtk.AttachOptions)(4));
			w6.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child tblAction.Gtk.Table+TableChild
			this.lbRecent = new global::Gtk.Label ();
			this.lbRecent.Name = "lbRecent";
			this.lbRecent.LabelProp = global::Mono.Unix.Catalog.GetString ("Recent");
			this.tblAction.Add (this.lbRecent);
			global::Gtk.Table.TableChild w7 = ((global::Gtk.Table.TableChild)(this.tblAction [this.lbRecent]));
			w7.TopAttach = ((uint)(1));
			w7.BottomAttach = ((uint)(2));
			w7.LeftAttach = ((uint)(2));
			w7.RightAttach = ((uint)(3));
			w7.XOptions = ((global::Gtk.AttachOptions)(4));
			w7.YOptions = ((global::Gtk.AttachOptions)(4));
			this.table1.Add (this.tblAction);
			global::Gtk.Table.TableChild w8 = ((global::Gtk.Table.TableChild)(this.table1 [this.tblAction]));
			w8.TopAttach = ((uint)(1));
			w8.BottomAttach = ((uint)(2));
			w8.RightAttach = ((uint)(2));
			w8.XPadding = ((uint)(10));
			w8.XOptions = ((global::Gtk.AttachOptions)(4));
			w8.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.tblContent = new global::Gtk.Table (((uint)(2)), ((uint)(6)), false);
			this.tblContent.HeightRequest = 60;
			this.tblContent.Name = "tblContent";
			this.tblContent.RowSpacing = ((uint)(6));
			this.tblContent.ColumnSpacing = ((uint)(6));
			// Container child tblContent.Gtk.Table+TableChild
			this.hbox3 = new global::Gtk.HBox ();
			this.hbox3.Name = "hbox3";
			this.hbox3.Spacing = 6;
			// Container child hbox3.Gtk.Box+BoxChild
			this.imgContent = new global::Gtk.Image ();
			this.imgContent.WidthRequest = 24;
			this.imgContent.HeightRequest = 24;
			this.imgContent.Name = "imgContent";
			this.hbox3.Add (this.imgContent);
			global::Gtk.Box.BoxChild w9 = ((global::Gtk.Box.BoxChild)(this.hbox3 [this.imgContent]));
			w9.Position = 0;
			w9.Expand = false;
			w9.Fill = false;
			// Container child hbox3.Gtk.Box+BoxChild
			this.lblContent = new global::Gtk.Label ();
			this.lblContent.Name = "lblContent";
			this.lblContent.Xalign = 0F;
			this.lblContent.LabelProp = global::Mono.Unix.Catalog.GetString ("Content");
			this.hbox3.Add (this.lblContent);
			global::Gtk.Box.BoxChild w10 = ((global::Gtk.Box.BoxChild)(this.hbox3 [this.lblContent]));
			w10.Position = 1;
			w10.Expand = false;
			w10.Fill = false;
			this.tblContent.Add (this.hbox3);
			global::Gtk.Table.TableChild w11 = ((global::Gtk.Table.TableChild)(this.tblContent [this.hbox3]));
			w11.RightAttach = ((uint)(6));
			w11.YOptions = ((global::Gtk.AttachOptions)(4));
			this.table1.Add (this.tblContent);
			global::Gtk.Table.TableChild w12 = ((global::Gtk.Table.TableChild)(this.table1 [this.tblContent]));
			w12.TopAttach = ((uint)(3));
			w12.BottomAttach = ((uint)(4));
			w12.XPadding = ((uint)(10));
			w12.XOptions = ((global::Gtk.AttachOptions)(4));
			w12.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.tblSamples = new global::Gtk.Table (((uint)(2)), ((uint)(6)), false);
			this.tblSamples.Name = "tblSamples";
			this.tblSamples.RowSpacing = ((uint)(6));
			this.tblSamples.ColumnSpacing = ((uint)(6));
			// Container child tblSamples.Gtk.Table+TableChild
			this.hbox2 = new global::Gtk.HBox ();
			this.hbox2.Name = "hbox2";
			this.hbox2.Spacing = 6;
			// Container child hbox2.Gtk.Box+BoxChild
			this.imgSamples = new global::Gtk.Image ();
			this.imgSamples.WidthRequest = 24;
			this.imgSamples.HeightRequest = 24;
			this.imgSamples.Name = "imgSamples";
			this.hbox2.Add (this.imgSamples);
			global::Gtk.Box.BoxChild w13 = ((global::Gtk.Box.BoxChild)(this.hbox2 [this.imgSamples]));
			w13.Position = 0;
			w13.Expand = false;
			w13.Fill = false;
			// Container child hbox2.Gtk.Box+BoxChild
			this.lblSamples = new global::Gtk.Label ();
			this.lblSamples.Name = "lblSamples";
			this.lblSamples.Xalign = 0F;
			this.lblSamples.LabelProp = global::Mono.Unix.Catalog.GetString ("Samples");
			this.hbox2.Add (this.lblSamples);
			global::Gtk.Box.BoxChild w14 = ((global::Gtk.Box.BoxChild)(this.hbox2 [this.lblSamples]));
			w14.Position = 1;
			w14.Expand = false;
			w14.Fill = false;
			this.tblSamples.Add (this.hbox2);
			global::Gtk.Table.TableChild w15 = ((global::Gtk.Table.TableChild)(this.tblSamples [this.hbox2]));
			w15.RightAttach = ((uint)(6));
			w15.YOptions = ((global::Gtk.AttachOptions)(4));
			this.table1.Add (this.tblSamples);
			global::Gtk.Table.TableChild w16 = ((global::Gtk.Table.TableChild)(this.table1 [this.tblSamples]));
			w16.TopAttach = ((uint)(2));
			w16.BottomAttach = ((uint)(3));
			w16.RightAttach = ((uint)(2));
			w16.XPadding = ((uint)(10));
			w16.XOptions = ((global::Gtk.AttachOptions)(4));
			w16.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.tblTwitt = new global::Gtk.Table (((uint)(6)), ((uint)(2)), false);
			this.tblTwitt.WidthRequest = 0;
			this.tblTwitt.Name = "tblTwitt";
			this.tblTwitt.RowSpacing = ((uint)(6));
			this.tblTwitt.ColumnSpacing = ((uint)(6));
			// Container child tblTwitt.Gtk.Table+TableChild
			this.btnTwitLoad = new global::Gtk.Button ();
			this.btnTwitLoad.WidthRequest = 250;
			this.btnTwitLoad.Sensitive = false;
			this.btnTwitLoad.CanFocus = true;
			this.btnTwitLoad.Name = "btnTwitLoad";
			this.btnTwitLoad.UseUnderline = true;
			this.btnTwitLoad.Relief = ((global::Gtk.ReliefStyle)(2));
			this.btnTwitLoad.Label = "";
			this.tblTwitt.Add (this.btnTwitLoad);
			global::Gtk.Table.TableChild w17 = ((global::Gtk.Table.TableChild)(this.tblTwitt [this.btnTwitLoad]));
			w17.TopAttach = ((uint)(1));
			w17.BottomAttach = ((uint)(2));
			w17.RightAttach = ((uint)(2));
			w17.XOptions = ((global::Gtk.AttachOptions)(4));
			w17.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child tblTwitt.Gtk.Table+TableChild
			this.imgTwiter = new global::Gtk.Image ();
			this.imgTwiter.WidthRequest = 24;
			this.imgTwiter.HeightRequest = 24;
			this.imgTwiter.Name = "imgTwiter";
			this.tblTwitt.Add (this.imgTwiter);
			global::Gtk.Table.TableChild w18 = ((global::Gtk.Table.TableChild)(this.tblTwitt [this.imgTwiter]));
			w18.XOptions = ((global::Gtk.AttachOptions)(0));
			w18.YOptions = ((global::Gtk.AttachOptions)(0));
			// Container child tblTwitt.Gtk.Table+TableChild
			this.lblTwiter = new global::Gtk.Label ();
			this.lblTwiter.Name = "lblTwiter";
			this.lblTwiter.Xalign = 0F;
			this.lblTwiter.LabelProp = global::Mono.Unix.Catalog.GetString ("Twitter");
			this.tblTwitt.Add (this.lblTwiter);
			global::Gtk.Table.TableChild w19 = ((global::Gtk.Table.TableChild)(this.tblTwitt [this.lblTwiter]));
			w19.LeftAttach = ((uint)(1));
			w19.RightAttach = ((uint)(2));
			w19.YOptions = ((global::Gtk.AttachOptions)(4));
			this.table1.Add (this.tblTwitt);
			global::Gtk.Table.TableChild w20 = ((global::Gtk.Table.TableChild)(this.table1 [this.tblTwitt]));
			w20.XPadding = ((uint)(10));
			w20.YPadding = ((uint)(10));
			w20.XOptions = ((global::Gtk.AttachOptions)(0));
			w20.YOptions = ((global::Gtk.AttachOptions)(4));
			this.Add (this.table1);
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			this.Hide ();
		}
	}
}
