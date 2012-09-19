
// This file has been generated by the GUI designer. Do not modify.
namespace Moscrif.IDE.Settings
{
	public partial class DeviceWidget
	{
		private global::Gtk.VBox vbox2;
		private global::Moscrif.IDE.Components.SkinThemeControl skinThemeControl;
		private global::Gtk.Table table1;
		private global::Gtk.ScrolledWindow GtkScrolledWindow;
		private global::Gtk.TreeView nvFonts;
		private global::Gtk.Label label4;
		
		protected virtual void Build ()
		{
			global::Stetic.Gui.Initialize (this);
			// Widget Moscrif.IDE.Settings.DeviceWidget
			global::Stetic.BinContainer.Attach (this);
			this.Name = "Moscrif.IDE.Settings.DeviceWidget";
			// Container child Moscrif.IDE.Settings.DeviceWidget.Gtk.Container+ContainerChild
			this.vbox2 = new global::Gtk.VBox ();
			this.vbox2.Name = "vbox2";
			// Container child vbox2.Gtk.Box+BoxChild
			this.skinThemeControl = null;
			this.vbox2.Add (this.skinThemeControl);
			global::Gtk.Box.BoxChild w1 = ((global::Gtk.Box.BoxChild)(this.vbox2 [this.skinThemeControl]));
			w1.Position = 0;
			w1.Expand = false;
			w1.Fill = false;
			// Container child vbox2.Gtk.Box+BoxChild
			this.table1 = new global::Gtk.Table (((uint)(1)), ((uint)(2)), false);
			this.table1.Name = "table1";
			this.table1.RowSpacing = ((uint)(5));
			this.table1.ColumnSpacing = ((uint)(8));
			this.table1.BorderWidth = ((uint)(8));
			// Container child table1.Gtk.Table+TableChild
			this.GtkScrolledWindow = new global::Gtk.ScrolledWindow ();
			this.GtkScrolledWindow.HeightRequest = 126;
			this.GtkScrolledWindow.Name = "GtkScrolledWindow";
			this.GtkScrolledWindow.ShadowType = ((global::Gtk.ShadowType)(1));
			// Container child GtkScrolledWindow.Gtk.Container+ContainerChild
			this.nvFonts = new global::Gtk.TreeView ();
			this.nvFonts.CanFocus = true;
			this.nvFonts.Name = "nvFonts";
			this.GtkScrolledWindow.Add (this.nvFonts);
			this.table1.Add (this.GtkScrolledWindow);
			global::Gtk.Table.TableChild w3 = ((global::Gtk.Table.TableChild)(this.table1 [this.GtkScrolledWindow]));
			w3.LeftAttach = ((uint)(1));
			w3.RightAttach = ((uint)(2));
			w3.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.label4 = new global::Gtk.Label ();
			this.label4.TooltipMarkup = "List of Fonts to be Embeded into Output Installation";
			this.label4.Name = "label4";
			this.label4.Xalign = 1F;
			this.label4.LabelProp = global::Mono.Unix.Catalog.GetString ("Fonts :");
			this.table1.Add (this.label4);
			global::Gtk.Table.TableChild w4 = ((global::Gtk.Table.TableChild)(this.table1 [this.label4]));
			w4.XOptions = ((global::Gtk.AttachOptions)(4));
			w4.YOptions = ((global::Gtk.AttachOptions)(4));
			this.vbox2.Add (this.table1);
			global::Gtk.Box.BoxChild w5 = ((global::Gtk.Box.BoxChild)(this.vbox2 [this.table1]));
			w5.Position = 1;
			w5.Expand = false;
			w5.Fill = false;
			this.Add (this.vbox2);
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			this.Hide ();
		}
	}
}
