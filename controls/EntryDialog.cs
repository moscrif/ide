using System;
using Gtk;
namespace Moscrif.IDE.Controls
{
	public class EntryDialog : Dialog
	{
		Entry localEntry1 = null;
		bool onlyInt = false;
		SpinButton localSpin = null;

		public EntryDialog(string textEntry, string textLabel,Gtk.Window parent)
		{
			if (parent == null){
				parent =MainClass.MainWindow;
			}
			BuildControl(textEntry,textLabel,false,parent);
		}

		public EntryDialog(string textEntry, string textLabel)
		{
			BuildControl(textEntry,textLabel,false,MainClass.MainWindow);
		}

		public EntryDialog(string textEntry, string textLabel, bool onlyInt)
		{
			BuildControl(textEntry,textLabel,onlyInt,MainClass.MainWindow);
		}


		private void BuildControl(string textEntry, string textLabel, bool onlyInt,Gtk.Window parent){

			this.onlyInt =onlyInt;

			this.TransientFor = parent;

			HBox hbox = new HBox(false, 8);
			hbox.BorderWidth = 8;
			this.VBox.PackStart(hbox, false, false, 0);

			Image stock = new Image(Stock.DialogQuestion, IconSize.Dialog);
			hbox.PackStart(stock, false, false, 0);

			Table table = new Table(2, 2, false);
			table.RowSpacing = 4;
			table.ColumnSpacing = 4;
			hbox.PackStart(table, true, true, 0);

			Label label = new Label(textLabel);
			table.Attach(label, 0, 1, 0, 1);

			if (!onlyInt){
				localEntry1 = new Entry();
				localEntry1.Text = textEntry;//textEntry;
				//localEntry1.Changed += delegate(object sender, EventArgs e) { textEntry = localEntry1.Text; };
				table.Attach(localEntry1, 1, 2, 0, 1);
				label.MnemonicWidget = localEntry1;

				localEntry1.KeyPressEvent+= new KeyPressEventHandler(OnKeyPress);

				localEntry1.GrabFocus();
			} else {
				localSpin = new SpinButton(1,10000,10);
				localSpin.Digits = 0;
				localSpin.Numeric = true;

				localSpin.KeyPressEvent+= new KeyPressEventHandler(OnKeyPress);

				table.Attach(localSpin, 1, 2, 0, 1);
				label.MnemonicWidget = localSpin;
				localSpin.GrabFocus();
			}

			this.AddButton(MainClass.Languages.Translate("cancel"), ResponseType.Cancel);
			this.AddButton(MainClass.Languages.Translate("ok"), ResponseType.Ok);

			this.ShowAll();
		}

		[GLib.ConnectBefore]
		private void OnKeyPress(object o, KeyPressEventArgs args){

			if (args.Event.Key == Gdk.Key.Return)
				this.Respond(ResponseType.Ok);

		}

		//private string textEntry;
		public string TextEntry
		{

			get {
				if (onlyInt)
					return localSpin.Text.ToString();
				else
					return localEntry1.Text; }
		}
		
	}
	
}
