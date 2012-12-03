
// This file has been generated by the GUI designer. Do not modify.
namespace Moscrif.IDE.Option
{
	public partial class PreferencesDialog
	{
		private global::Gtk.HBox hbPage;
		private global::Gtk.ScrolledWindow GtkScrolledWindow;
		private global::Gtk.TreeView tvCategory;
		private global::Gtk.VBox vbox2;
		private global::Gtk.Label labelTitle;
		private global::Gtk.HSeparator hseparator2;
		private global::Gtk.HBox pageFrame;
		private global::Gtk.Button buttonCancel;
		private global::Gtk.Button buttonOk;
		
		protected virtual void Build ()
		{
			global::Stetic.Gui.Initialize (this);
			// Widget Moscrif.IDE.Option.PreferencesDialog
			this.Name = "Moscrif.IDE.Option.PreferencesDialog";
			this.WindowPosition = ((global::Gtk.WindowPosition)(1));
			// Internal child Moscrif.IDE.Option.PreferencesDialog.VBox
			global::Gtk.VBox w1 = this.VBox;
			w1.Name = "dialog1_VBox";
			w1.BorderWidth = ((uint)(2));
			// Container child dialog1_VBox.Gtk.Box+BoxChild
			this.hbPage = new global::Gtk.HBox ();
			this.hbPage.Name = "hbPage";
			this.hbPage.Spacing = 6;
			this.hbPage.BorderWidth = ((uint)(10));
			// Container child hbPage.Gtk.Box+BoxChild
			this.GtkScrolledWindow = new global::Gtk.ScrolledWindow ();
			this.GtkScrolledWindow.WidthRequest = 159;
			this.GtkScrolledWindow.Name = "GtkScrolledWindow";
			this.GtkScrolledWindow.ShadowType = ((global::Gtk.ShadowType)(2));
			// Container child GtkScrolledWindow.Gtk.Container+ContainerChild
			this.tvCategory = new global::Gtk.TreeView ();
			this.tvCategory.WidthRequest = 50;
			this.tvCategory.CanFocus = true;
			this.tvCategory.Name = "tvCategory";
			this.tvCategory.EnableSearch = false;
			this.GtkScrolledWindow.Add (this.tvCategory);
			this.hbPage.Add (this.GtkScrolledWindow);
			global::Gtk.Box.BoxChild w3 = ((global::Gtk.Box.BoxChild)(this.hbPage [this.GtkScrolledWindow]));
			w3.Position = 0;
			w3.Expand = false;
			// Container child hbPage.Gtk.Box+BoxChild
			this.vbox2 = new global::Gtk.VBox ();
			this.vbox2.Name = "vbox2";
			this.vbox2.Spacing = 6;
			this.vbox2.BorderWidth = ((uint)(1));
			// Container child vbox2.Gtk.Box+BoxChild
			this.labelTitle = new global::Gtk.Label ();
			this.labelTitle.Name = "labelTitle";
			this.labelTitle.LabelProp = global::Mono.Unix.Catalog.GetString ("<span weight=\"bold\" size=\"x-large\">Title</span>");
			this.labelTitle.UseMarkup = true;
			this.vbox2.Add (this.labelTitle);
			global::Gtk.Box.BoxChild w4 = ((global::Gtk.Box.BoxChild)(this.vbox2 [this.labelTitle]));
			w4.Position = 0;
			w4.Expand = false;
			w4.Fill = false;
			// Container child vbox2.Gtk.Box+BoxChild
			this.hseparator2 = new global::Gtk.HSeparator ();
			this.hseparator2.Name = "hseparator2";
			this.vbox2.Add (this.hseparator2);
			global::Gtk.Box.BoxChild w5 = ((global::Gtk.Box.BoxChild)(this.vbox2 [this.hseparator2]));
			w5.Position = 1;
			w5.Expand = false;
			w5.Fill = false;
			// Container child vbox2.Gtk.Box+BoxChild
			this.pageFrame = new global::Gtk.HBox ();
			this.pageFrame.Name = "pageFrame";
			this.pageFrame.Spacing = 6;
			this.vbox2.Add (this.pageFrame);
			global::Gtk.Box.BoxChild w6 = ((global::Gtk.Box.BoxChild)(this.vbox2 [this.pageFrame]));
			w6.Position = 2;
			this.hbPage.Add (this.vbox2);
			global::Gtk.Box.BoxChild w7 = ((global::Gtk.Box.BoxChild)(this.hbPage [this.vbox2]));
			w7.Position = 1;
			w1.Add (this.hbPage);
			global::Gtk.Box.BoxChild w8 = ((global::Gtk.Box.BoxChild)(w1 [this.hbPage]));
			w8.Position = 0;
			// Internal child Moscrif.IDE.Option.PreferencesDialog.ActionArea
			global::Gtk.HButtonBox w9 = this.ActionArea;
			w9.Name = "dialog1_ActionArea";
			w9.Spacing = 10;
			w9.BorderWidth = ((uint)(10));
			w9.LayoutStyle = ((global::Gtk.ButtonBoxStyle)(4));
			// Container child dialog1_ActionArea.Gtk.ButtonBox+ButtonBoxChild
			this.buttonCancel = new global::Gtk.Button ();
			this.buttonCancel.CanDefault = true;
			this.buttonCancel.CanFocus = true;
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.UseStock = true;
			this.buttonCancel.UseUnderline = true;
			this.buttonCancel.Label = "gtk-cancel";
			this.AddActionWidget (this.buttonCancel, -6);
			global::Gtk.ButtonBox.ButtonBoxChild w10 = ((global::Gtk.ButtonBox.ButtonBoxChild)(w9 [this.buttonCancel]));
			w10.Expand = false;
			w10.Fill = false;
			// Container child dialog1_ActionArea.Gtk.ButtonBox+ButtonBoxChild
			this.buttonOk = new global::Gtk.Button ();
			this.buttonOk.CanDefault = true;
			this.buttonOk.CanFocus = true;
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.UseUnderline = true;
			this.buttonOk.Label = global::Mono.Unix.Catalog.GetString ("_OK");
			w9.Add (this.buttonOk);
			global::Gtk.ButtonBox.ButtonBoxChild w11 = ((global::Gtk.ButtonBox.ButtonBoxChild)(w9 [this.buttonOk]));
			w11.Position = 1;
			w11.Expand = false;
			w11.Fill = false;
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			this.DefaultWidth = 712;
			this.DefaultHeight = 534;
			this.Show ();
			this.buttonOk.Clicked += new global::System.EventHandler (this.OnButtonOkClicked);
		}
	}
}
