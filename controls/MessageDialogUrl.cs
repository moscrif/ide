using System;
using Gtk;

namespace Moscrif.IDE.Controls
{

	public class MessageDialogUrl : Dialog
	{

		public MessageDialogUrl()
		{
			InicializeComponent();
		}

		public MessageDialogUrl(DialogButtonType buttons, string primaryLabel, string secondaryLabel,string secondaryUrl, MessageType messageType,Gtk.Window parent)
		{
			InicializeComponent();
			//if (parent == null) parent =MainClass.MainWindow;

			if (parent != null)
				TransientFor =parent;

			Buttons = buttons;
			PrimaryText = primaryLabel;
			linkBtn.Label = secondaryLabel;
			linkBtn.LinkUrl = secondaryUrl;
			MessageType = messageType;
		}


		public MessageDialogUrl(DialogButtonType buttons, string primaryLabel, string secondaryLabel,string secondaryUrl, MessageType messageType)
		{
			InicializeComponent();
			TransientFor = MainClass.MainWindow;

			Buttons = buttons;
			PrimaryText = primaryLabel;
			linkBtn.Label = secondaryLabel;
			linkBtn.LinkUrl = secondaryUrl;
			MessageType = messageType;
		}

		public MessageDialogUrl(string[] labelButtons, string primaryLabel, string secondaryLabel,string secondaryUrl, MessageType messageType)
		{
			InicializeComponent();

			TransientFor = MainClass.MainWindow;

			Buttons = buttons;
			PrimaryText = primaryLabel;
			linkBtn.Label = secondaryLabel;
			linkBtn.LinkUrl = secondaryUrl;
			MessageType = messageType;

			Widget[] oldButtons = ActionArea.Children;
			foreach (Widget w in oldButtons)
				ActionArea.Remove(w);
			int i =1;
			foreach (string s in labelButtons){
				AddButton(s,-i);
				i++;
			}

		}

		public int ShowDialog(){
			int result =   this.Run();
			this.Destroy();
			return result;
		}

		private void InicializeComponent()
		{
			Resizable = false;
			HasSeparator = false;
			BorderWidth = 12;
			//Modal = true;
			
			label = new Label();
			label.LineWrap = true;
			label.Selectable = true;
			label.UseMarkup = true;
			label.SetAlignment(0.0f, 0.0f);
			
			linkBtn = new Moscrif.IDE.Components.LinkButton();
			linkBtn.SetAlignment(0.5f, 0.5f);
			
			icon = new Image(Stock.DialogInfo, IconSize.Dialog);
			icon.SetAlignment(0.5f, 0.0f);
			
			StockItem item = Stock.Lookup(icon.Stock);
			Title = item.Label;
			
			HBox hbox = new HBox(false, 12);
			VBox vbox = new VBox(false, 12);
			
			vbox.PackStart(label, false, false, 0);
			vbox.PackStart(linkBtn, true, true, 0);
			
			hbox.PackStart(icon, false, false, 0);
			hbox.PackStart(vbox, true, true, 0);
			
			VBox.PackStart(hbox, false, false, 0);
			hbox.ShowAll();
			
			Buttons = MessageDialogUrl.DialogButtonType.OkCancel;
		}

		Label label;
		Image icon;
		Moscrif.IDE.Components.LinkButton linkBtn;

		public MessageType MessageType
		{
			get {
				if (icon.Stock == Stock.DialogInfo)
					return MessageType.Info; else if (icon.Stock == Stock.DialogQuestion)
					return MessageType.Question; else if (icon.Stock == Stock.DialogWarning)
					return MessageType.Warning;
				else
					return MessageType.Error;
			}
			set {
				StockItem item = Stock.Lookup(icon.Stock);
				bool setTitle = (Title == "") || (Title == item.Label);
				
				if (value == MessageType.Info)
					icon.Stock = Stock.DialogInfo; else if (value == MessageType.Question)
					icon.Stock = Stock.DialogQuestion; else if (value == MessageType.Warning)
					icon.Stock = Stock.DialogWarning;
				else
					icon.Stock = Stock.DialogError;
				
				if (setTitle) {
					item = Stock.Lookup(icon.Stock);
					Title = item.Label;
				}
			}
		}

		public string primaryText;
		public string PrimaryText
		{
			get { return primaryText; }
			set {
				primaryText = value;
				label.Markup = "<b>" + value + "</b>";
			}
		}

		public string SecondaryText
		{
			get { return linkBtn.Label; }
			set { linkBtn.Label = value; }
		}

		DialogButtonType buttons;
		public DialogButtonType Buttons
		{
			get { return buttons; }
			set {
				Widget[] oldButtons = ActionArea.Children;
				foreach (Widget w in oldButtons)
					ActionArea.Remove(w);
				
				buttons = value;
				switch (buttons) {
				case DialogButtonType.None:
					// nothing
					break;
				
				case DialogButtonType.Ok:
					AddButton(Stock.Ok, ResponseType.Ok);
					break;
				
				case DialogButtonType.Close:
					AddButton(Stock.Close, ResponseType.Close);
					break;
				
				case DialogButtonType.Cancel:
					AddButton(Stock.Cancel, ResponseType.Cancel);
					break;
				
				case DialogButtonType.YesNo:
					AddButton(Stock.No, ResponseType.No);
					AddButton(Stock.Yes, ResponseType.Yes);
					break;
				
				case DialogButtonType.OkCancel:
					AddButton(Stock.Cancel, ResponseType.Cancel);
					AddButton(Stock.Ok, ResponseType.Ok);
					break;
				
				case DialogButtonType.YesNoCancel:
					AddButton(Stock.Cancel, ResponseType.Cancel);
					AddButton(Stock.No, ResponseType.No);
					AddButton(Stock.Yes, ResponseType.Yes);
					break;
				}
			}
		}

		public enum DialogButtonType
		{
			None,
			Ok,
			Close,
			Cancel,
			YesNo,
			OkCancel,
			YesNoCancel
		}
	}
}
