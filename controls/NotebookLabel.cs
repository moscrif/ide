using System;
using Gdk;
using Gtk;
using Moscrif.IDE.Components;
using Moscrif.IDE.Editors;

namespace Moscrif.IDE.Controls
{
	public class NotebookLabel : HBox
	{

		public Label lblDisplay= new Label();
		public string caption = "page";

		public NotebookLabel(string stockIcon, string caption)
		{
			this.BorderWidth = 1;

			Gtk.Image image = new Gtk.Image(MainClass.Tools.GetIconFromStock(stockIcon, Gtk.IconSize.Menu));
			image.SetPadding(2,2);
			this.PackStart(image, false, false, 0);

			lblDisplay.Text = caption;

			this.PackStart(lblDisplay, false, false, 0);

			this.ShowAll();
		}

		public object Tag {
			get;set;
		}

		protected override void OnDestroyed ()
		{
			base.OnDestroyed ();
		}


		public void SetLabel(string newName){

			caption = newName;
			lblDisplay.Text = caption;
		}
		
	}
}


