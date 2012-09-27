
// This file has been generated by the GUI designer. Do not modify.
namespace Moscrif.IDE.Settings
{
	internal partial class GlobalOptionsWidget2
	{
		private global::Gtk.VBox vbox3;
		private global::Gtk.Table table1;
		private global::Gtk.ColorButton cbBackground;
		private global::Gtk.CheckButton chbAutoselectProject;
		private global::Gtk.CheckButton chbOpenLastOpenedW;
		private global::Gtk.CheckButton chbShowDebugDevic;
		private global::Gtk.CheckButton chbShowUnsupportDevic;
		private global::Gtk.FontButton fontbutton1;
		private global::Gtk.Label label1;
		private global::Gtk.Label label2;
		private global::Gtk.Label label3;
		private global::Gtk.Label label4;
		private global::Gtk.Label label6;
		private global::Gtk.Frame frame1;
		private global::Gtk.Alignment GtkAlignment2;
		private global::Gtk.HBox hbox1;
		private global::Gtk.Label label5;
		private global::Gtk.ScrolledWindow GtkScrolledWindow1;
		private global::Gtk.TreeView tvIgnoreFile;
		private global::Gtk.VButtonBox vbuttonbox1;
		private global::Gtk.Button btnAddIF;
		private global::Gtk.Button btnEditIF;
		private global::Gtk.Button btnDeleteIF;
		private global::Gtk.Button button8;
		private global::Gtk.Label GtkLabel10;
		
		protected virtual void Build ()
		{
			global::Stetic.Gui.Initialize (this);
			// Widget Moscrif.IDE.Settings.GlobalOptionsWidget2
			global::Stetic.BinContainer.Attach (this);
			this.Name = "Moscrif.IDE.Settings.GlobalOptionsWidget2";
			// Container child Moscrif.IDE.Settings.GlobalOptionsWidget2.Gtk.Container+ContainerChild
			this.vbox3 = new global::Gtk.VBox ();
			this.vbox3.Name = "vbox3";
			this.vbox3.Spacing = 6;
			// Container child vbox3.Gtk.Box+BoxChild
			this.table1 = new global::Gtk.Table (((uint)(10)), ((uint)(3)), false);
			this.table1.Name = "table1";
			this.table1.RowSpacing = ((uint)(6));
			this.table1.ColumnSpacing = ((uint)(6));
			// Container child table1.Gtk.Table+TableChild
			this.cbBackground = new global::Gtk.ColorButton ();
			this.cbBackground.CanFocus = true;
			this.cbBackground.Events = ((global::Gdk.EventMask)(784));
			this.cbBackground.Name = "cbBackground";
			this.table1.Add (this.cbBackground);
			global::Gtk.Table.TableChild w1 = ((global::Gtk.Table.TableChild)(this.table1 [this.cbBackground]));
			w1.TopAttach = ((uint)(3));
			w1.BottomAttach = ((uint)(4));
			w1.LeftAttach = ((uint)(1));
			w1.RightAttach = ((uint)(2));
			w1.XOptions = ((global::Gtk.AttachOptions)(4));
			w1.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.chbAutoselectProject = new global::Gtk.CheckButton ();
			this.chbAutoselectProject.CanFocus = true;
			this.chbAutoselectProject.Name = "chbAutoselectProject";
			this.chbAutoselectProject.Label = global::Mono.Unix.Catalog.GetString ("Auto select project");
			this.chbAutoselectProject.DrawIndicator = true;
			this.chbAutoselectProject.UseUnderline = true;
			this.table1.Add (this.chbAutoselectProject);
			global::Gtk.Table.TableChild w2 = ((global::Gtk.Table.TableChild)(this.table1 [this.chbAutoselectProject]));
			w2.TopAttach = ((uint)(4));
			w2.BottomAttach = ((uint)(5));
			w2.LeftAttach = ((uint)(1));
			w2.RightAttach = ((uint)(2));
			w2.XOptions = ((global::Gtk.AttachOptions)(4));
			w2.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.chbOpenLastOpenedW = new global::Gtk.CheckButton ();
			this.chbOpenLastOpenedW.CanFocus = true;
			this.chbOpenLastOpenedW.Name = "chbOpenLastOpenedW";
			this.chbOpenLastOpenedW.Label = global::Mono.Unix.Catalog.GetString ("Open last opened workspace.");
			this.chbOpenLastOpenedW.DrawIndicator = true;
			this.chbOpenLastOpenedW.UseUnderline = true;
			this.table1.Add (this.chbOpenLastOpenedW);
			global::Gtk.Table.TableChild w3 = ((global::Gtk.Table.TableChild)(this.table1 [this.chbOpenLastOpenedW]));
			w3.TopAttach = ((uint)(5));
			w3.BottomAttach = ((uint)(6));
			w3.LeftAttach = ((uint)(1));
			w3.RightAttach = ((uint)(2));
			w3.XOptions = ((global::Gtk.AttachOptions)(4));
			w3.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.chbShowDebugDevic = new global::Gtk.CheckButton ();
			this.chbShowDebugDevic.CanFocus = true;
			this.chbShowDebugDevic.Name = "chbShowDebugDevic";
			this.chbShowDebugDevic.Label = global::Mono.Unix.Catalog.GetString ("Show beta platforms.");
			this.chbShowDebugDevic.DrawIndicator = true;
			this.chbShowDebugDevic.UseUnderline = true;
			this.table1.Add (this.chbShowDebugDevic);
			global::Gtk.Table.TableChild w4 = ((global::Gtk.Table.TableChild)(this.table1 [this.chbShowDebugDevic]));
			w4.TopAttach = ((uint)(7));
			w4.BottomAttach = ((uint)(8));
			w4.LeftAttach = ((uint)(1));
			w4.RightAttach = ((uint)(2));
			w4.XOptions = ((global::Gtk.AttachOptions)(4));
			w4.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.chbShowUnsupportDevic = new global::Gtk.CheckButton ();
			this.chbShowUnsupportDevic.CanFocus = true;
			this.chbShowUnsupportDevic.Name = "chbShowUnsupportDevic";
			this.chbShowUnsupportDevic.Label = global::Mono.Unix.Catalog.GetString ("Show obsolete platforms.");
			this.chbShowUnsupportDevic.DrawIndicator = true;
			this.chbShowUnsupportDevic.UseUnderline = true;
			this.table1.Add (this.chbShowUnsupportDevic);
			global::Gtk.Table.TableChild w5 = ((global::Gtk.Table.TableChild)(this.table1 [this.chbShowUnsupportDevic]));
			w5.TopAttach = ((uint)(6));
			w5.BottomAttach = ((uint)(7));
			w5.LeftAttach = ((uint)(1));
			w5.RightAttach = ((uint)(2));
			w5.XOptions = ((global::Gtk.AttachOptions)(4));
			w5.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.fontbutton1 = new global::Gtk.FontButton ();
			this.fontbutton1.CanFocus = true;
			this.fontbutton1.Name = "fontbutton1";
			this.fontbutton1.FontName = "Monospace 10";
			this.fontbutton1.ShowStyle = false;
			this.fontbutton1.UseFont = true;
			this.fontbutton1.UseSize = true;
			this.table1.Add (this.fontbutton1);
			global::Gtk.Table.TableChild w6 = ((global::Gtk.Table.TableChild)(this.table1 [this.fontbutton1]));
			w6.TopAttach = ((uint)(8));
			w6.BottomAttach = ((uint)(9));
			w6.LeftAttach = ((uint)(1));
			w6.RightAttach = ((uint)(2));
			w6.XOptions = ((global::Gtk.AttachOptions)(4));
			w6.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.label1 = new global::Gtk.Label ();
			this.label1.Name = "label1";
			this.label1.Xalign = 1F;
			this.label1.LabelProp = global::Mono.Unix.Catalog.GetString ("Color :");
			this.table1.Add (this.label1);
			global::Gtk.Table.TableChild w7 = ((global::Gtk.Table.TableChild)(this.table1 [this.label1]));
			w7.TopAttach = ((uint)(3));
			w7.BottomAttach = ((uint)(4));
			w7.XOptions = ((global::Gtk.AttachOptions)(4));
			w7.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.label2 = new global::Gtk.Label ();
			this.label2.TooltipMarkup = "Path to Publish Tools";
			this.label2.Name = "label2";
			this.label2.Xalign = 1F;
			this.label2.LabelProp = global::Mono.Unix.Catalog.GetString ("Publish Tools :");
			this.table1.Add (this.label2);
			global::Gtk.Table.TableChild w8 = ((global::Gtk.Table.TableChild)(this.table1 [this.label2]));
			w8.XOptions = ((global::Gtk.AttachOptions)(4));
			w8.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.label3 = new global::Gtk.Label ();
			this.label3.TooltipMarkup = "Path to Framework";
			this.label3.Name = "label3";
			this.label3.Xalign = 1F;
			this.label3.LabelProp = global::Mono.Unix.Catalog.GetString ("Libs Directory :");
			this.table1.Add (this.label3);
			global::Gtk.Table.TableChild w9 = ((global::Gtk.Table.TableChild)(this.table1 [this.label3]));
			w9.TopAttach = ((uint)(1));
			w9.BottomAttach = ((uint)(2));
			w9.XOptions = ((global::Gtk.AttachOptions)(4));
			w9.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.label4 = new global::Gtk.Label ();
			this.label4.TooltipMarkup = "Path to Emulator and Compiler";
			this.label4.Name = "label4";
			this.label4.Xalign = 1F;
			this.label4.LabelProp = global::Mono.Unix.Catalog.GetString ("Emulator :");
			this.table1.Add (this.label4);
			global::Gtk.Table.TableChild w10 = ((global::Gtk.Table.TableChild)(this.table1 [this.label4]));
			w10.TopAttach = ((uint)(2));
			w10.BottomAttach = ((uint)(3));
			w10.XOptions = ((global::Gtk.AttachOptions)(4));
			w10.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.label6 = new global::Gtk.Label ();
			this.label6.Name = "label6";
			this.label6.Xalign = 1F;
			this.label6.LabelProp = global::Mono.Unix.Catalog.GetString ("Console and task font :");
			this.table1.Add (this.label6);
			global::Gtk.Table.TableChild w11 = ((global::Gtk.Table.TableChild)(this.table1 [this.label6]));
			w11.TopAttach = ((uint)(8));
			w11.BottomAttach = ((uint)(9));
			w11.XOptions = ((global::Gtk.AttachOptions)(4));
			w11.YOptions = ((global::Gtk.AttachOptions)(4));
			this.vbox3.Add (this.table1);
			global::Gtk.Box.BoxChild w12 = ((global::Gtk.Box.BoxChild)(this.vbox3 [this.table1]));
			w12.Position = 0;
			// Container child vbox3.Gtk.Box+BoxChild
			this.frame1 = new global::Gtk.Frame ();
			this.frame1.Name = "frame1";
			// Container child frame1.Gtk.Container+ContainerChild
			this.GtkAlignment2 = new global::Gtk.Alignment (0F, 0F, 1F, 1F);
			this.GtkAlignment2.Name = "GtkAlignment2";
			this.GtkAlignment2.LeftPadding = ((uint)(12));
			// Container child GtkAlignment2.Gtk.Container+ContainerChild
			this.hbox1 = new global::Gtk.HBox ();
			this.hbox1.HeightRequest = 102;
			this.hbox1.Name = "hbox1";
			this.hbox1.Spacing = 6;
			// Container child hbox1.Gtk.Box+BoxChild
			this.label5 = new global::Gtk.Label ();
			this.label5.Name = "label5";
			this.label5.Xalign = 1F;
			this.label5.LabelProp = global::Mono.Unix.Catalog.GetString ("Ignored :");
			this.hbox1.Add (this.label5);
			global::Gtk.Box.BoxChild w13 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.label5]));
			w13.Position = 0;
			w13.Expand = false;
			w13.Fill = false;
			// Container child hbox1.Gtk.Box+BoxChild
			this.GtkScrolledWindow1 = new global::Gtk.ScrolledWindow ();
			this.GtkScrolledWindow1.Name = "GtkScrolledWindow1";
			this.GtkScrolledWindow1.ShadowType = ((global::Gtk.ShadowType)(1));
			// Container child GtkScrolledWindow1.Gtk.Container+ContainerChild
			this.tvIgnoreFile = new global::Gtk.TreeView ();
			this.tvIgnoreFile.CanFocus = true;
			this.tvIgnoreFile.Name = "tvIgnoreFile";
			this.GtkScrolledWindow1.Add (this.tvIgnoreFile);
			this.hbox1.Add (this.GtkScrolledWindow1);
			global::Gtk.Box.BoxChild w15 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.GtkScrolledWindow1]));
			w15.Position = 1;
			// Container child hbox1.Gtk.Box+BoxChild
			this.vbuttonbox1 = new global::Gtk.VButtonBox ();
			this.vbuttonbox1.Name = "vbuttonbox1";
			this.vbuttonbox1.LayoutStyle = ((global::Gtk.ButtonBoxStyle)(3));
			// Container child vbuttonbox1.Gtk.ButtonBox+ButtonBoxChild
			this.btnAddIF = new global::Gtk.Button ();
			this.btnAddIF.CanFocus = true;
			this.btnAddIF.Name = "btnAddIF";
			this.btnAddIF.UseUnderline = true;
			this.btnAddIF.Label = global::Mono.Unix.Catalog.GetString ("Add");
			this.vbuttonbox1.Add (this.btnAddIF);
			global::Gtk.ButtonBox.ButtonBoxChild w16 = ((global::Gtk.ButtonBox.ButtonBoxChild)(this.vbuttonbox1 [this.btnAddIF]));
			w16.Expand = false;
			w16.Fill = false;
			// Container child vbuttonbox1.Gtk.ButtonBox+ButtonBoxChild
			this.btnEditIF = new global::Gtk.Button ();
			this.btnEditIF.CanFocus = true;
			this.btnEditIF.Name = "btnEditIF";
			this.btnEditIF.UseUnderline = true;
			this.btnEditIF.Label = global::Mono.Unix.Catalog.GetString ("Edit");
			this.vbuttonbox1.Add (this.btnEditIF);
			global::Gtk.ButtonBox.ButtonBoxChild w17 = ((global::Gtk.ButtonBox.ButtonBoxChild)(this.vbuttonbox1 [this.btnEditIF]));
			w17.Position = 1;
			w17.Expand = false;
			w17.Fill = false;
			// Container child vbuttonbox1.Gtk.ButtonBox+ButtonBoxChild
			this.btnDeleteIF = new global::Gtk.Button ();
			this.btnDeleteIF.CanFocus = true;
			this.btnDeleteIF.Name = "btnDeleteIF";
			this.btnDeleteIF.UseUnderline = true;
			this.btnDeleteIF.Label = global::Mono.Unix.Catalog.GetString ("Delete");
			this.vbuttonbox1.Add (this.btnDeleteIF);
			global::Gtk.ButtonBox.ButtonBoxChild w18 = ((global::Gtk.ButtonBox.ButtonBoxChild)(this.vbuttonbox1 [this.btnDeleteIF]));
			w18.Position = 2;
			w18.Expand = false;
			w18.Fill = false;
			// Container child vbuttonbox1.Gtk.ButtonBox+ButtonBoxChild
			this.button8 = new global::Gtk.Button ();
			this.button8.CanFocus = true;
			this.button8.Name = "button8";
			this.button8.UseUnderline = true;
			this.button8.Label = global::Mono.Unix.Catalog.GetString ("Reset");
			this.vbuttonbox1.Add (this.button8);
			global::Gtk.ButtonBox.ButtonBoxChild w19 = ((global::Gtk.ButtonBox.ButtonBoxChild)(this.vbuttonbox1 [this.button8]));
			w19.Position = 3;
			w19.Expand = false;
			w19.Fill = false;
			this.hbox1.Add (this.vbuttonbox1);
			global::Gtk.Box.BoxChild w20 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.vbuttonbox1]));
			w20.Position = 2;
			w20.Expand = false;
			w20.Fill = false;
			this.GtkAlignment2.Add (this.hbox1);
			this.frame1.Add (this.GtkAlignment2);
			this.GtkLabel10 = new global::Gtk.Label ();
			this.GtkLabel10.Name = "GtkLabel10";
			this.GtkLabel10.LabelProp = global::Mono.Unix.Catalog.GetString ("<b>Ignore Directories</b>");
			this.GtkLabel10.UseMarkup = true;
			this.frame1.LabelWidget = this.GtkLabel10;
			this.vbox3.Add (this.frame1);
			global::Gtk.Box.BoxChild w23 = ((global::Gtk.Box.BoxChild)(this.vbox3 [this.frame1]));
			w23.Position = 1;
			w23.Expand = false;
			w23.Fill = false;
			this.Add (this.vbox3);
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			this.Hide ();
			this.btnAddIF.Clicked += new global::System.EventHandler (this.OnBtnAddIFClicked);
			this.btnEditIF.Clicked += new global::System.EventHandler (this.OnBtnEditIFClicked);
			this.btnDeleteIF.Clicked += new global::System.EventHandler (this.OnBtnDeleteIFClicked);
			this.button8.Clicked += new global::System.EventHandler (this.OnButton8Clicked);
		}
	}
}
