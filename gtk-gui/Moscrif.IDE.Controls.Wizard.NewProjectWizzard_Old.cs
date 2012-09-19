
// This file has been generated by the GUI designer. Do not modify.
namespace Moscrif.IDE.Controls.Wizard
{
	public partial class NewProjectWizzard_Old
	{
		private global::Gtk.HBox hbPage;
		private global::Gtk.ScrolledWindow GtkScrolledWindow;
		private global::Gtk.TreeView tvSteps;
		private global::Gtk.Notebook ntpPages;
		private global::Gtk.Table table1;
		private global::Gtk.CheckButton cbCopyLibs;
		private global::Gtk.CheckButton cbSubFolder;
		private global::Gtk.Entry entWorkspace;
		private global::Moscrif.IDE.Components.FileEntry feRoot;
		private global::Gtk.Label label3;
		private global::Gtk.Label label4;
		private global::Gtk.Label label5;
		private global::Gtk.Label label1;
		private global::Gtk.VBox vbox2;
		private global::Gtk.Table tblPrjGeneric;
		private global::Gtk.ComboBox cbAppTyp;
		private global::Gtk.Entry entrProjectName;
		private global::Gtk.Label label10;
		private global::Gtk.Label label9;
		private global::Gtk.Table tblPage1;
		private global::Gtk.Entry entrName;
		private global::Gtk.Label label16;
		private global::Gtk.Label label17;
		private global::Gtk.Label label2;
		private global::Gtk.VBox vbox3;
		private global::Moscrif.IDE.Components.SkinThemeControl skinThemeControl;
		private global::Gtk.Table table3;
		private global::Gtk.ScrolledWindow GtkScrolledWindow1;
		private global::Gtk.TreeView nvFonts;
		private global::Gtk.Label label12;
		private global::Gtk.Label label8;
		private global::Gtk.VBox vbox4;
		private global::Gtk.ScrolledWindow GtkScrolledWindow2;
		private global::Gtk.TreeView tvLibs;
		private global::Gtk.Label label11;
		private global::Gtk.VBox vbPage4;
		private global::Gtk.Label label13;
		private global::Gtk.ScrolledWindow GtkScrolledWindow3;
		private global::Gtk.TextView textview1;
		private global::Gtk.Label label14;
		private global::Gtk.Button button31;
		private global::Gtk.Button btnBack;
		private global::Gtk.Button btnNext;
		
		protected virtual void Build ()
		{
			global::Stetic.Gui.Initialize (this);
			// Widget Moscrif.IDE.Controls.Wizard.NewProjectWizzard_Old
			this.Name = "Moscrif.IDE.Controls.Wizard.NewProjectWizzard_Old";
			this.WindowPosition = ((global::Gtk.WindowPosition)(4));
			// Internal child Moscrif.IDE.Controls.Wizard.NewProjectWizzard_Old.VBox
			global::Gtk.VBox w1 = this.VBox;
			w1.Name = "dialog1_VBox";
			w1.BorderWidth = ((uint)(2));
			// Container child dialog1_VBox.Gtk.Box+BoxChild
			this.hbPage = new global::Gtk.HBox ();
			this.hbPage.Name = "hbPage";
			this.hbPage.Spacing = 6;
			// Container child hbPage.Gtk.Box+BoxChild
			this.GtkScrolledWindow = new global::Gtk.ScrolledWindow ();
			this.GtkScrolledWindow.WidthRequest = 159;
			this.GtkScrolledWindow.Name = "GtkScrolledWindow";
			this.GtkScrolledWindow.ShadowType = ((global::Gtk.ShadowType)(2));
			// Container child GtkScrolledWindow.Gtk.Container+ContainerChild
			this.tvSteps = new global::Gtk.TreeView ();
			this.tvSteps.WidthRequest = 50;
			this.tvSteps.Sensitive = false;
			this.tvSteps.CanFocus = true;
			this.tvSteps.Name = "tvSteps";
			this.tvSteps.EnableSearch = false;
			this.GtkScrolledWindow.Add (this.tvSteps);
			this.hbPage.Add (this.GtkScrolledWindow);
			global::Gtk.Box.BoxChild w3 = ((global::Gtk.Box.BoxChild)(this.hbPage [this.GtkScrolledWindow]));
			w3.Position = 0;
			w3.Expand = false;
			w3.Padding = ((uint)(5));
			// Container child hbPage.Gtk.Box+BoxChild
			this.ntpPages = new global::Gtk.Notebook ();
			this.ntpPages.CanFocus = true;
			this.ntpPages.Name = "ntpPages";
			this.ntpPages.CurrentPage = 0;
			this.ntpPages.ShowBorder = false;
			this.ntpPages.BorderWidth = ((uint)(6));
			// Container child ntpPages.Gtk.Notebook+NotebookChild
			this.table1 = new global::Gtk.Table (((uint)(5)), ((uint)(2)), false);
			this.table1.Name = "table1";
			this.table1.RowSpacing = ((uint)(6));
			this.table1.ColumnSpacing = ((uint)(6));
			// Container child table1.Gtk.Table+TableChild
			this.cbCopyLibs = new global::Gtk.CheckButton ();
			this.cbCopyLibs.CanFocus = true;
			this.cbCopyLibs.Name = "cbCopyLibs";
			this.cbCopyLibs.Label = global::Mono.Unix.Catalog.GetString ("Copy All Libs");
			this.cbCopyLibs.DrawIndicator = true;
			this.cbCopyLibs.UseUnderline = true;
			this.table1.Add (this.cbCopyLibs);
			global::Gtk.Table.TableChild w4 = ((global::Gtk.Table.TableChild)(this.table1 [this.cbCopyLibs]));
			w4.TopAttach = ((uint)(3));
			w4.BottomAttach = ((uint)(4));
			w4.LeftAttach = ((uint)(1));
			w4.RightAttach = ((uint)(2));
			w4.XPadding = ((uint)(5));
			w4.XOptions = ((global::Gtk.AttachOptions)(4));
			w4.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.cbSubFolder = new global::Gtk.CheckButton ();
			this.cbSubFolder.CanFocus = true;
			this.cbSubFolder.Name = "cbSubFolder";
			this.cbSubFolder.Label = global::Mono.Unix.Catalog.GetString ("Create directory for workspace");
			this.cbSubFolder.Active = true;
			this.cbSubFolder.DrawIndicator = true;
			this.cbSubFolder.UseUnderline = true;
			this.table1.Add (this.cbSubFolder);
			global::Gtk.Table.TableChild w5 = ((global::Gtk.Table.TableChild)(this.table1 [this.cbSubFolder]));
			w5.TopAttach = ((uint)(4));
			w5.BottomAttach = ((uint)(5));
			w5.LeftAttach = ((uint)(1));
			w5.RightAttach = ((uint)(2));
			w5.XPadding = ((uint)(5));
			w5.XOptions = ((global::Gtk.AttachOptions)(4));
			w5.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.entWorkspace = new global::Gtk.Entry ();
			this.entWorkspace.CanFocus = true;
			this.entWorkspace.Events = ((global::Gdk.EventMask)(2048));
			this.entWorkspace.Name = "entWorkspace";
			this.entWorkspace.IsEditable = true;
			this.entWorkspace.InvisibleChar = '●';
			this.table1.Add (this.entWorkspace);
			global::Gtk.Table.TableChild w6 = ((global::Gtk.Table.TableChild)(this.table1 [this.entWorkspace]));
			w6.LeftAttach = ((uint)(1));
			w6.RightAttach = ((uint)(2));
			w6.XPadding = ((uint)(5));
			w6.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.feRoot = null;
			this.table1.Add (this.feRoot);
			global::Gtk.Table.TableChild w7 = ((global::Gtk.Table.TableChild)(this.table1 [this.feRoot]));
			w7.TopAttach = ((uint)(1));
			w7.BottomAttach = ((uint)(2));
			w7.LeftAttach = ((uint)(1));
			w7.RightAttach = ((uint)(2));
			w7.XPadding = ((uint)(5));
			w7.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.label3 = new global::Gtk.Label ();
			this.label3.Name = "label3";
			this.label3.Xalign = 1F;
			this.label3.LabelProp = global::Mono.Unix.Catalog.GetString ("Location :");
			this.table1.Add (this.label3);
			global::Gtk.Table.TableChild w8 = ((global::Gtk.Table.TableChild)(this.table1 [this.label3]));
			w8.TopAttach = ((uint)(1));
			w8.BottomAttach = ((uint)(2));
			w8.XPadding = ((uint)(5));
			w8.XOptions = ((global::Gtk.AttachOptions)(4));
			w8.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.label4 = new global::Gtk.Label ();
			this.label4.Name = "label4";
			this.label4.Xalign = 1F;
			this.label4.LabelProp = global::Mono.Unix.Catalog.GetString ("Output Directory :");
			this.table1.Add (this.label4);
			global::Gtk.Table.TableChild w9 = ((global::Gtk.Table.TableChild)(this.table1 [this.label4]));
			w9.TopAttach = ((uint)(2));
			w9.BottomAttach = ((uint)(3));
			w9.XPadding = ((uint)(5));
			w9.XOptions = ((global::Gtk.AttachOptions)(4));
			w9.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.label5 = new global::Gtk.Label ();
			this.label5.Name = "label5";
			this.label5.Xalign = 1F;
			this.label5.LabelProp = global::Mono.Unix.Catalog.GetString ("Workspace Name :");
			this.table1.Add (this.label5);
			global::Gtk.Table.TableChild w10 = ((global::Gtk.Table.TableChild)(this.table1 [this.label5]));
			w10.XPadding = ((uint)(5));
			w10.XOptions = ((global::Gtk.AttachOptions)(4));
			w10.YOptions = ((global::Gtk.AttachOptions)(4));
			this.ntpPages.Add (this.table1);
			// Notebook tab
			this.label1 = new global::Gtk.Label ();
			this.label1.Name = "label1";
			this.label1.LabelProp = "page0";
			this.ntpPages.SetTabLabel (this.table1, this.label1);
			this.label1.ShowAll ();
			// Container child ntpPages.Gtk.Notebook+NotebookChild
			this.vbox2 = new global::Gtk.VBox ();
			this.vbox2.Name = "vbox2";
			this.vbox2.Spacing = 6;
			// Container child vbox2.Gtk.Box+BoxChild
			this.tblPrjGeneric = new global::Gtk.Table (((uint)(2)), ((uint)(2)), false);
			this.tblPrjGeneric.Name = "tblPrjGeneric";
			this.tblPrjGeneric.RowSpacing = ((uint)(6));
			this.tblPrjGeneric.ColumnSpacing = ((uint)(8));
			this.tblPrjGeneric.BorderWidth = ((uint)(6));
			// Container child tblPrjGeneric.Gtk.Table+TableChild
			this.cbAppTyp = global::Gtk.ComboBox.NewText ();
			this.cbAppTyp.Name = "cbAppTyp";
			this.tblPrjGeneric.Add (this.cbAppTyp);
			global::Gtk.Table.TableChild w12 = ((global::Gtk.Table.TableChild)(this.tblPrjGeneric [this.cbAppTyp]));
			w12.TopAttach = ((uint)(1));
			w12.BottomAttach = ((uint)(2));
			w12.LeftAttach = ((uint)(1));
			w12.RightAttach = ((uint)(2));
			w12.XOptions = ((global::Gtk.AttachOptions)(4));
			w12.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child tblPrjGeneric.Gtk.Table+TableChild
			this.entrProjectName = new global::Gtk.Entry ();
			this.entrProjectName.CanFocus = true;
			this.entrProjectName.Events = ((global::Gdk.EventMask)(3072));
			this.entrProjectName.Name = "entrProjectName";
			this.entrProjectName.IsEditable = true;
			this.entrProjectName.InvisibleChar = '●';
			this.tblPrjGeneric.Add (this.entrProjectName);
			global::Gtk.Table.TableChild w13 = ((global::Gtk.Table.TableChild)(this.tblPrjGeneric [this.entrProjectName]));
			w13.LeftAttach = ((uint)(1));
			w13.RightAttach = ((uint)(2));
			w13.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child tblPrjGeneric.Gtk.Table+TableChild
			this.label10 = new global::Gtk.Label ();
			this.label10.WidthRequest = 75;
			this.label10.Name = "label10";
			this.label10.Xalign = 1F;
			this.label10.LabelProp = global::Mono.Unix.Catalog.GetString ("Type :");
			this.tblPrjGeneric.Add (this.label10);
			global::Gtk.Table.TableChild w14 = ((global::Gtk.Table.TableChild)(this.tblPrjGeneric [this.label10]));
			w14.TopAttach = ((uint)(1));
			w14.BottomAttach = ((uint)(2));
			w14.XOptions = ((global::Gtk.AttachOptions)(4));
			w14.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child tblPrjGeneric.Gtk.Table+TableChild
			this.label9 = new global::Gtk.Label ();
			this.label9.WidthRequest = 80;
			this.label9.Name = "label9";
			this.label9.Xalign = 1F;
			this.label9.LabelProp = global::Mono.Unix.Catalog.GetString ("Project Name :");
			this.tblPrjGeneric.Add (this.label9);
			global::Gtk.Table.TableChild w15 = ((global::Gtk.Table.TableChild)(this.tblPrjGeneric [this.label9]));
			w15.XOptions = ((global::Gtk.AttachOptions)(4));
			w15.YOptions = ((global::Gtk.AttachOptions)(4));
			this.vbox2.Add (this.tblPrjGeneric);
			global::Gtk.Box.BoxChild w16 = ((global::Gtk.Box.BoxChild)(this.vbox2 [this.tblPrjGeneric]));
			w16.Position = 0;
			w16.Expand = false;
			w16.Fill = false;
			// Container child vbox2.Gtk.Box+BoxChild
			this.tblPage1 = new global::Gtk.Table (((uint)(2)), ((uint)(3)), false);
			this.tblPage1.Name = "tblPage1";
			this.tblPage1.RowSpacing = ((uint)(6));
			this.tblPage1.ColumnSpacing = ((uint)(6));
			// Container child tblPage1.Gtk.Table+TableChild
			this.entrName = new global::Gtk.Entry ();
			this.entrName.CanFocus = true;
			this.entrName.Events = ((global::Gdk.EventMask)(3072));
			this.entrName.Name = "entrName";
			this.entrName.IsEditable = true;
			this.entrName.InvisibleChar = '●';
			this.tblPage1.Add (this.entrName);
			global::Gtk.Table.TableChild w17 = ((global::Gtk.Table.TableChild)(this.tblPage1 [this.entrName]));
			w17.LeftAttach = ((uint)(1));
			w17.RightAttach = ((uint)(2));
			w17.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child tblPage1.Gtk.Table+TableChild
			this.label16 = new global::Gtk.Label ();
			this.label16.TooltipMarkup = "Pattern to be Used for Output Artefacts (Installation Files)";
			this.label16.WidthRequest = 88;
			this.label16.Name = "label16";
			this.label16.Xalign = 1F;
			this.label16.LabelProp = global::Mono.Unix.Catalog.GetString ("File Name :");
			this.tblPage1.Add (this.label16);
			global::Gtk.Table.TableChild w18 = ((global::Gtk.Table.TableChild)(this.tblPage1 [this.label16]));
			w18.XOptions = ((global::Gtk.AttachOptions)(4));
			w18.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child tblPage1.Gtk.Table+TableChild
			this.label17 = new global::Gtk.Label ();
			this.label17.TooltipMarkup = "Output Directory";
			this.label17.WidthRequest = 83;
			this.label17.Name = "label17";
			this.label17.Xalign = 1F;
			this.label17.LabelProp = global::Mono.Unix.Catalog.GetString ("Output :");
			this.tblPage1.Add (this.label17);
			global::Gtk.Table.TableChild w19 = ((global::Gtk.Table.TableChild)(this.tblPage1 [this.label17]));
			w19.TopAttach = ((uint)(1));
			w19.BottomAttach = ((uint)(2));
			w19.XOptions = ((global::Gtk.AttachOptions)(4));
			w19.YOptions = ((global::Gtk.AttachOptions)(4));
			this.vbox2.Add (this.tblPage1);
			global::Gtk.Box.BoxChild w20 = ((global::Gtk.Box.BoxChild)(this.vbox2 [this.tblPage1]));
			w20.Position = 1;
			w20.Expand = false;
			w20.Fill = false;
			this.ntpPages.Add (this.vbox2);
			global::Gtk.Notebook.NotebookChild w21 = ((global::Gtk.Notebook.NotebookChild)(this.ntpPages [this.vbox2]));
			w21.Position = 1;
			// Notebook tab
			this.label2 = new global::Gtk.Label ();
			this.label2.Name = "label2";
			this.label2.LabelProp = "page1";
			this.ntpPages.SetTabLabel (this.vbox2, this.label2);
			this.label2.ShowAll ();
			// Container child ntpPages.Gtk.Notebook+NotebookChild
			this.vbox3 = new global::Gtk.VBox ();
			this.vbox3.Name = "vbox3";
			// Container child vbox3.Gtk.Box+BoxChild
			this.skinThemeControl = null;
			this.vbox3.Add (this.skinThemeControl);
			global::Gtk.Box.BoxChild w22 = ((global::Gtk.Box.BoxChild)(this.vbox3 [this.skinThemeControl]));
			w22.Position = 0;
			w22.Expand = false;
			w22.Fill = false;
			// Container child vbox3.Gtk.Box+BoxChild
			this.table3 = new global::Gtk.Table (((uint)(1)), ((uint)(2)), false);
			this.table3.Name = "table3";
			this.table3.RowSpacing = ((uint)(5));
			this.table3.ColumnSpacing = ((uint)(8));
			this.table3.BorderWidth = ((uint)(8));
			// Container child table3.Gtk.Table+TableChild
			this.GtkScrolledWindow1 = new global::Gtk.ScrolledWindow ();
			this.GtkScrolledWindow1.HeightRequest = 126;
			this.GtkScrolledWindow1.Name = "GtkScrolledWindow1";
			this.GtkScrolledWindow1.ShadowType = ((global::Gtk.ShadowType)(1));
			// Container child GtkScrolledWindow1.Gtk.Container+ContainerChild
			this.nvFonts = new global::Gtk.TreeView ();
			this.nvFonts.CanFocus = true;
			this.nvFonts.Name = "nvFonts";
			this.GtkScrolledWindow1.Add (this.nvFonts);
			this.table3.Add (this.GtkScrolledWindow1);
			global::Gtk.Table.TableChild w24 = ((global::Gtk.Table.TableChild)(this.table3 [this.GtkScrolledWindow1]));
			w24.LeftAttach = ((uint)(1));
			w24.RightAttach = ((uint)(2));
			w24.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table3.Gtk.Table+TableChild
			this.label12 = new global::Gtk.Label ();
			this.label12.TooltipMarkup = "List of Fonts to be Embeded into Output Installation";
			this.label12.Name = "label12";
			this.label12.Xalign = 1F;
			this.label12.LabelProp = global::Mono.Unix.Catalog.GetString ("Fonts :");
			this.table3.Add (this.label12);
			global::Gtk.Table.TableChild w25 = ((global::Gtk.Table.TableChild)(this.table3 [this.label12]));
			w25.XOptions = ((global::Gtk.AttachOptions)(4));
			w25.YOptions = ((global::Gtk.AttachOptions)(4));
			this.vbox3.Add (this.table3);
			global::Gtk.Box.BoxChild w26 = ((global::Gtk.Box.BoxChild)(this.vbox3 [this.table3]));
			w26.Position = 1;
			w26.Expand = false;
			w26.Fill = false;
			this.ntpPages.Add (this.vbox3);
			global::Gtk.Notebook.NotebookChild w27 = ((global::Gtk.Notebook.NotebookChild)(this.ntpPages [this.vbox3]));
			w27.Position = 2;
			// Notebook tab
			this.label8 = new global::Gtk.Label ();
			this.label8.Name = "label8";
			this.label8.LabelProp = global::Mono.Unix.Catalog.GetString ("page2");
			this.ntpPages.SetTabLabel (this.vbox3, this.label8);
			this.label8.ShowAll ();
			// Container child ntpPages.Gtk.Notebook+NotebookChild
			this.vbox4 = new global::Gtk.VBox ();
			this.vbox4.Name = "vbox4";
			this.vbox4.Spacing = 6;
			// Container child vbox4.Gtk.Box+BoxChild
			this.GtkScrolledWindow2 = new global::Gtk.ScrolledWindow ();
			this.GtkScrolledWindow2.Name = "GtkScrolledWindow2";
			this.GtkScrolledWindow2.ShadowType = ((global::Gtk.ShadowType)(1));
			// Container child GtkScrolledWindow2.Gtk.Container+ContainerChild
			this.tvLibs = new global::Gtk.TreeView ();
			this.tvLibs.CanFocus = true;
			this.tvLibs.Name = "tvLibs";
			this.GtkScrolledWindow2.Add (this.tvLibs);
			this.vbox4.Add (this.GtkScrolledWindow2);
			global::Gtk.Box.BoxChild w29 = ((global::Gtk.Box.BoxChild)(this.vbox4 [this.GtkScrolledWindow2]));
			w29.Position = 0;
			this.ntpPages.Add (this.vbox4);
			global::Gtk.Notebook.NotebookChild w30 = ((global::Gtk.Notebook.NotebookChild)(this.ntpPages [this.vbox4]));
			w30.Position = 3;
			// Notebook tab
			this.label11 = new global::Gtk.Label ();
			this.label11.Name = "label11";
			this.label11.LabelProp = global::Mono.Unix.Catalog.GetString ("page3");
			this.ntpPages.SetTabLabel (this.vbox4, this.label11);
			this.label11.ShowAll ();
			// Container child ntpPages.Gtk.Notebook+NotebookChild
			this.vbPage4 = new global::Gtk.VBox ();
			this.vbPage4.Name = "vbPage4";
			this.vbPage4.Spacing = 6;
			this.ntpPages.Add (this.vbPage4);
			global::Gtk.Notebook.NotebookChild w31 = ((global::Gtk.Notebook.NotebookChild)(this.ntpPages [this.vbPage4]));
			w31.Position = 4;
			// Notebook tab
			this.label13 = new global::Gtk.Label ();
			this.label13.Name = "label13";
			this.label13.LabelProp = global::Mono.Unix.Catalog.GetString ("page4");
			this.ntpPages.SetTabLabel (this.vbPage4, this.label13);
			this.label13.ShowAll ();
			// Container child ntpPages.Gtk.Notebook+NotebookChild
			this.GtkScrolledWindow3 = new global::Gtk.ScrolledWindow ();
			this.GtkScrolledWindow3.Name = "GtkScrolledWindow3";
			this.GtkScrolledWindow3.ShadowType = ((global::Gtk.ShadowType)(1));
			// Container child GtkScrolledWindow3.Gtk.Container+ContainerChild
			this.textview1 = new global::Gtk.TextView ();
			this.textview1.CanFocus = true;
			this.textview1.Name = "textview1";
			this.GtkScrolledWindow3.Add (this.textview1);
			this.ntpPages.Add (this.GtkScrolledWindow3);
			global::Gtk.Notebook.NotebookChild w33 = ((global::Gtk.Notebook.NotebookChild)(this.ntpPages [this.GtkScrolledWindow3]));
			w33.Position = 5;
			// Notebook tab
			this.label14 = new global::Gtk.Label ();
			this.label14.Name = "label14";
			this.label14.LabelProp = global::Mono.Unix.Catalog.GetString ("page5");
			this.ntpPages.SetTabLabel (this.GtkScrolledWindow3, this.label14);
			this.label14.ShowAll ();
			this.hbPage.Add (this.ntpPages);
			global::Gtk.Box.BoxChild w34 = ((global::Gtk.Box.BoxChild)(this.hbPage [this.ntpPages]));
			w34.Position = 1;
			w1.Add (this.hbPage);
			global::Gtk.Box.BoxChild w35 = ((global::Gtk.Box.BoxChild)(w1 [this.hbPage]));
			w35.Position = 0;
			// Internal child Moscrif.IDE.Controls.Wizard.NewProjectWizzard_Old.ActionArea
			global::Gtk.HButtonBox w36 = this.ActionArea;
			w36.Name = "dialog1_ActionArea";
			w36.Spacing = 10;
			w36.BorderWidth = ((uint)(5));
			w36.LayoutStyle = ((global::Gtk.ButtonBoxStyle)(4));
			// Container child dialog1_ActionArea.Gtk.ButtonBox+ButtonBoxChild
			this.button31 = new global::Gtk.Button ();
			this.button31.CanFocus = true;
			this.button31.Name = "button31";
			this.button31.UseStock = true;
			this.button31.UseUnderline = true;
			this.button31.Label = "gtk-cancel";
			this.AddActionWidget (this.button31, -6);
			global::Gtk.ButtonBox.ButtonBoxChild w37 = ((global::Gtk.ButtonBox.ButtonBoxChild)(w36 [this.button31]));
			w37.Expand = false;
			w37.Fill = false;
			// Container child dialog1_ActionArea.Gtk.ButtonBox+ButtonBoxChild
			this.btnBack = new global::Gtk.Button ();
			this.btnBack.CanDefault = true;
			this.btnBack.CanFocus = true;
			this.btnBack.Name = "btnBack";
			this.btnBack.UseUnderline = true;
			this.btnBack.Label = global::Mono.Unix.Catalog.GetString ("_Back");
			w36.Add (this.btnBack);
			global::Gtk.ButtonBox.ButtonBoxChild w38 = ((global::Gtk.ButtonBox.ButtonBoxChild)(w36 [this.btnBack]));
			w38.Position = 1;
			w38.Expand = false;
			w38.Fill = false;
			// Container child dialog1_ActionArea.Gtk.ButtonBox+ButtonBoxChild
			this.btnNext = new global::Gtk.Button ();
			this.btnNext.CanDefault = true;
			this.btnNext.CanFocus = true;
			this.btnNext.Name = "btnNext";
			this.btnNext.UseUnderline = true;
			this.btnNext.Label = global::Mono.Unix.Catalog.GetString ("_Next");
			w36.Add (this.btnNext);
			global::Gtk.ButtonBox.ButtonBoxChild w39 = ((global::Gtk.ButtonBox.ButtonBoxChild)(w36 [this.btnNext]));
			w39.Position = 2;
			w39.Expand = false;
			w39.Fill = false;
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			this.DefaultWidth = 843;
			this.DefaultHeight = 463;
			this.Show ();
			this.cbAppTyp.Changed += new global::System.EventHandler (this.OnCbAppTypChanged);
			this.btnBack.Clicked += new global::System.EventHandler (this.OnBtnBackClicked);
			this.btnNext.Clicked += new global::System.EventHandler (this.OnBtnNextClicked);
		}
	}
}
