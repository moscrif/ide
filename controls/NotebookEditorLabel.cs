using System;
using Gdk;
using Gtk;
using Moscrif.IDE.Components;
using Moscrif.IDE.Editors;

namespace Moscrif.IDE.Controls
{
	public class NotebookEditorLabel : HBox
	{

		public Label lblDisplay= new Label();
		public string caption = "page";
		Gtk.Image image;

		public NotebookEditorLabel(EditorNotebook parent_netbook, IEditor se)
		{
			this.CanFocus=false;
			this.BorderWidth = 1;

			string stockIcon = "home.png";

			if (se.FileName != "StartPage"){
				stockIcon = MainClass.Tools.GetIconForExtension( System.IO.Path.GetExtension(se.Caption) );
			}

			image = new Gtk.Image(MainClass.Tools.GetIconFromStock(stockIcon, Gtk.IconSize.Menu));
			image.SetPadding(2,2);
			this.PackStart(image, false, false, 0);

			caption =se.Caption.Replace("_","__");

			lblDisplay.Text = caption;
			lblDisplay.CanFocus=false;

			this.PackStart(lblDisplay, false, false, 0);
			
			Pixbuf default_pixbuf = null;
			string file = System.IO.Path.Combine(MainClass.Paths.ResDir, "stock-close.png");
			if (System.IO.File.Exists(file)) {
				default_pixbuf = new Pixbuf(file);

				Button btnClose = new Button(new Gtk.Image(default_pixbuf));
				btnClose.TooltipText = MainClass.Languages.Translate("close");
				btnClose.Relief = ReliefStyle.None;
				btnClose.CanFocus = false;
				btnClose.WidthRequest = btnClose.HeightRequest = 18;
				btnClose.Clicked += delegate { parent_netbook.ClosePage(se); };
				
				this.PackEnd(btnClose, false, false, 0);
			}

			lblDisplay.TooltipText = se.FileName;

			this.ShowAll();
		}

		public void SetSaveState(bool modified){
			if (modified)
				lblDisplay.Text =caption+"*";
			else
				lblDisplay.Text =caption;
		}

		public void SetNewName(string newName){

			caption = System.IO.Path.GetFileName(newName).Replace("_","__");
			lblDisplay.Text = caption;
			lblDisplay.TooltipText = newName;

			string stockIcon = MainClass.Tools.GetIconForExtension( System.IO.Path.GetExtension(caption) );
			image = new Gtk.Image(MainClass.Tools.GetIconFromStock(stockIcon, Gtk.IconSize.Menu));
		}


		protected override void OnDestroyed ()
		{
			base.OnDestroyed ();
		}
		
	}
}

