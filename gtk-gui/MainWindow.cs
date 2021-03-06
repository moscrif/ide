
// This file has been generated by the GUI designer. Do not modify.

public partial class MainWindow
{
	private global::Gtk.UIManager UIManager;
	private global::Gtk.VBox vbMain;
	private global::Gtk.HBox hbMenu;
	private global::Gtk.VPaned vpMenuLeft;
	private global::Gtk.VBox vbMenuMidle;
	private global::Gtk.Table tblMenuRight;
	private global::Gtk.VPaned vpBody;
	private global::Gtk.HPaned hpBodyMidle;
	private global::Gtk.HPaned hpOutput;
	private global::Gtk.Statusbar statusbar1;
	private global::Gtk.HBox hbox1;
	private global::Gtk.Label lblMessage1;
	private global::Gtk.Table table1;
	private global::Gtk.Label lblMessage2;
	private global::Gtk.ProgressBar pbProgress;
	
	protected virtual void Build ()
	{
		global::Stetic.Gui.Initialize (this);
		// Widget MainWindow
		this.UIManager = new global::Gtk.UIManager ();
		global::Gtk.ActionGroup w1 = new global::Gtk.ActionGroup ("Default");
		this.UIManager.InsertActionGroup (w1, 0);
		this.AddAccelGroup (this.UIManager.AccelGroup);
		this.Name = "MainWindow";
		this.Title = global::Mono.Unix.Catalog.GetString ("Moscrif Ide");
		this.WindowPosition = ((global::Gtk.WindowPosition)(4));
		// Container child MainWindow.Gtk.Container+ContainerChild
		this.vbMain = new global::Gtk.VBox ();
		this.vbMain.Name = "vbMain";
		this.vbMain.Spacing = 6;
		// Container child vbMain.Gtk.Box+BoxChild
		this.hbMenu = new global::Gtk.HBox ();
		this.hbMenu.Name = "hbMenu";
		// Container child hbMenu.Gtk.Box+BoxChild
		this.vpMenuLeft = new global::Gtk.VPaned ();
		this.vpMenuLeft.WidthRequest = 290;
		this.vpMenuLeft.CanFocus = true;
		this.vpMenuLeft.Name = "vpMenuLeft";
		this.vpMenuLeft.Position = 1;
		this.hbMenu.Add (this.vpMenuLeft);
		global::Gtk.Box.BoxChild w2 = ((global::Gtk.Box.BoxChild)(this.hbMenu [this.vpMenuLeft]));
		w2.Position = 0;
		w2.Expand = false;
		// Container child hbMenu.Gtk.Box+BoxChild
		this.vbMenuMidle = new global::Gtk.VBox ();
		this.vbMenuMidle.Name = "vbMenuMidle";
		this.hbMenu.Add (this.vbMenuMidle);
		global::Gtk.Box.BoxChild w3 = ((global::Gtk.Box.BoxChild)(this.hbMenu [this.vbMenuMidle]));
		w3.Position = 1;
		// Container child hbMenu.Gtk.Box+BoxChild
		this.tblMenuRight = new global::Gtk.Table (((uint)(2)), ((uint)(2)), false);
		this.tblMenuRight.WidthRequest = 370;
		this.tblMenuRight.Name = "tblMenuRight";
		this.tblMenuRight.RowSpacing = ((uint)(6));
		this.tblMenuRight.ColumnSpacing = ((uint)(6));
		this.hbMenu.Add (this.tblMenuRight);
		global::Gtk.Box.BoxChild w4 = ((global::Gtk.Box.BoxChild)(this.hbMenu [this.tblMenuRight]));
		w4.Position = 2;
		w4.Expand = false;
		this.vbMain.Add (this.hbMenu);
		global::Gtk.Box.BoxChild w5 = ((global::Gtk.Box.BoxChild)(this.vbMain [this.hbMenu]));
		w5.Position = 0;
		w5.Expand = false;
		// Container child vbMain.Gtk.Box+BoxChild
		this.vpBody = new global::Gtk.VPaned ();
		this.vpBody.CanFocus = true;
		this.vpBody.Name = "vpBody";
		this.vpBody.Position = 442;
		// Container child vpBody.Gtk.Paned+PanedChild
		this.hpBodyMidle = new global::Gtk.HPaned ();
		this.hpBodyMidle.CanFocus = true;
		this.hpBodyMidle.Name = "hpBodyMidle";
		this.hpBodyMidle.Position = 200;
		this.vpBody.Add (this.hpBodyMidle);
		global::Gtk.Paned.PanedChild w6 = ((global::Gtk.Paned.PanedChild)(this.vpBody [this.hpBodyMidle]));
		w6.Resize = false;
		// Container child vpBody.Gtk.Paned+PanedChild
		this.hpOutput = new global::Gtk.HPaned ();
		this.hpOutput.CanFocus = true;
		this.hpOutput.Name = "hpOutput";
		this.hpOutput.Position = 1;
		this.vpBody.Add (this.hpOutput);
		this.vbMain.Add (this.vpBody);
		global::Gtk.Box.BoxChild w8 = ((global::Gtk.Box.BoxChild)(this.vbMain [this.vpBody]));
		w8.Position = 1;
		// Container child vbMain.Gtk.Box+BoxChild
		this.statusbar1 = new global::Gtk.Statusbar ();
		this.statusbar1.Name = "statusbar1";
		this.statusbar1.Spacing = 6;
		this.statusbar1.BorderWidth = ((uint)(1));
		// Container child statusbar1.Gtk.Box+BoxChild
		this.hbox1 = new global::Gtk.HBox ();
		this.hbox1.Name = "hbox1";
		this.hbox1.Spacing = 7;
		// Container child hbox1.Gtk.Box+BoxChild
		this.lblMessage1 = new global::Gtk.Label ();
		this.lblMessage1.WidthRequest = 175;
		this.lblMessage1.Name = "lblMessage1";
		this.lblMessage1.LabelProp = global::Mono.Unix.Catalog.GetString ("lblMessage1");
		this.lblMessage1.Selectable = true;
		this.hbox1.Add (this.lblMessage1);
		global::Gtk.Box.BoxChild w9 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.lblMessage1]));
		w9.Position = 0;
		w9.Expand = false;
		w9.Fill = false;
		// Container child hbox1.Gtk.Box+BoxChild
		this.table1 = new global::Gtk.Table (((uint)(1)), ((uint)(4)), false);
		this.table1.Name = "table1";
		this.table1.RowSpacing = ((uint)(6));
		this.table1.ColumnSpacing = ((uint)(6));
		// Container child table1.Gtk.Table+TableChild
		this.lblMessage2 = new global::Gtk.Label ();
		this.lblMessage2.WidthRequest = 175;
		this.lblMessage2.Name = "lblMessage2";
		this.lblMessage2.LabelProp = global::Mono.Unix.Catalog.GetString ("lblMessage2");
		this.table1.Add (this.lblMessage2);
		global::Gtk.Table.TableChild w10 = ((global::Gtk.Table.TableChild)(this.table1 [this.lblMessage2]));
		w10.XOptions = ((global::Gtk.AttachOptions)(4));
		w10.YOptions = ((global::Gtk.AttachOptions)(4));
		this.hbox1.Add (this.table1);
		global::Gtk.Box.BoxChild w11 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.table1]));
		w11.Position = 1;
		// Container child hbox1.Gtk.Box+BoxChild
		this.pbProgress = new global::Gtk.ProgressBar ();
		this.pbProgress.Name = "pbProgress";
		this.hbox1.Add (this.pbProgress);
		global::Gtk.Box.BoxChild w12 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.pbProgress]));
		w12.Position = 2;
		this.statusbar1.Add (this.hbox1);
		global::Gtk.Box.BoxChild w13 = ((global::Gtk.Box.BoxChild)(this.statusbar1 [this.hbox1]));
		w13.Position = 1;
		this.vbMain.Add (this.statusbar1);
		global::Gtk.Box.BoxChild w14 = ((global::Gtk.Box.BoxChild)(this.vbMain [this.statusbar1]));
		w14.Position = 2;
		w14.Expand = false;
		w14.Fill = false;
		this.Add (this.vbMain);
		if ((this.Child != null)) {
			this.Child.ShowAll ();
		}
		this.DefaultWidth = 1315;
		this.DefaultHeight = 667;
		this.Hide ();
		this.DeleteEvent += new global::Gtk.DeleteEventHandler (this.OnDeleteEvent);
		this.Realized += new global::System.EventHandler (this.OnRealized);
		this.Shown += new global::System.EventHandler (this.OnShown);
	}
}
