using System;
using Gtk;
using Moscrif.IDE.Editors.ImageView;
using MessageDialogs = Moscrif.IDE.Controls.MessageDialog;

namespace Moscrif.IDE.Components
{
	public class ImageToolBarControl : Toolbar
	{

		ToggleToolButton btnShowHideBarier;
		//ToolButton btnAddBarierPoint= new ToolButton();
		//ToolButton btnMovieBarierPoint= new ToolButton();

		ToggleToolButton btnEditBarierPoint ;
		ToggleToolButton btnDeleteBarierPoint ;

		ToolButton btnDeleteBarier;


		ComboBoxEntry cbeZoom;
		ImageCanvas ic;

		public event EventHandler DeleteBarierLayerEvent;

		public ImageToolBarControl(ImageCanvas ic)
		{
			this.ic = ic;
			btnShowHideBarier = new ToggleToolButton("barier-show.png");//RadioToolButton(group1);//,"barier-show.png");
			btnShowHideBarier.Label = MainClass.Languages.Translate("show_barier_layer");
			btnShowHideBarier.Name = "btnShowHideBarier";
			//btnShowHideBarier.Relief = ReliefStyle.None;
			btnShowHideBarier.CanFocus = false;
			btnShowHideBarier.BorderWidth = 1;
			//btnShowHideBarier.WidthRequest = 75;
			btnShowHideBarier.TooltipText = MainClass.Languages.Translate("show_barier_layer_tt");

			btnShowHideBarier.Events = Gdk.EventMask.AllEventsMask;// | EventMask.ButtonReleaseMask | EventMask.PointerMotionMask
			btnShowHideBarier.Toggled+= delegate(object sender, EventArgs e) {

				this.ic.ShowBarierLayer =  btnShowHideBarier.Active;
				SetSensitive(btnShowHideBarier.Active);
				//Console.WriteLine("btnShowHideBarier.Toggled");

			};

			btnEditBarierPoint =  new ToggleToolButton("barier-add.png");//new RadioToolButton(group2,"barier-add.png");
			btnEditBarierPoint.Name = "btnEditBarierPoint";
			btnEditBarierPoint.Label = MainClass.Languages.Translate("edit_barier_point");
			//btnEditBarierPoint.Relief = ReliefStyle.None;
			btnEditBarierPoint.CanFocus = false;
			btnEditBarierPoint.BorderWidth = 1;
			btnEditBarierPoint.TooltipText = MainClass.Languages.Translate("edit_barier_point_tt");
			btnEditBarierPoint.Toggled+= delegate(object sender, EventArgs e) {
				if(btnEditBarierPoint.Active){
					btnDeleteBarierPoint.Active = false;
					//btnMovieBarierPoint.Active = false;
				}
			};

			btnDeleteBarierPoint = new ToggleToolButton("barier-delete.png");//new RadioToolButton(group2,"barier-delete.png");
			btnDeleteBarierPoint.Name = "btnDeleteBarierPoint";
			btnDeleteBarierPoint.Label = MainClass.Languages.Translate("delete_barier_point");
			//btnDeleteBarierPoint.Relief = ReliefStyle.None;
			btnDeleteBarierPoint.CanFocus = false;
			btnDeleteBarierPoint.BorderWidth = 1;
			btnDeleteBarierPoint.TooltipText = MainClass.Languages.Translate("delete_barier_point_tt");
			btnDeleteBarierPoint.Toggled+= delegate(object sender, EventArgs e) {
				if(btnDeleteBarierPoint.Active){
					btnEditBarierPoint.Active = false;
					//btnAddBarierPoint.Active = false;
					//btnMovieBarierPoint.Active = false;
				}
			};

			btnDeleteBarier = new ToolButton("barier-delete-all.png");
			btnDeleteBarier.Name = "btnDeleteBarier";

			btnDeleteBarier.Label =MainClass.Languages.Translate("delete_barier");
			btnDeleteBarier.CanFocus = false;
			btnDeleteBarier.BorderWidth = 1;
			btnDeleteBarier.TooltipText = MainClass.Languages.Translate("delete_barier_tt");
			btnDeleteBarier.Clicked += delegate(object sender, EventArgs e) {
				MessageDialogs md = new MessageDialogs(MessageDialogs.DialogButtonType.YesNo, "", MainClass.Languages.Translate("delete_barier_question"), Gtk.MessageType.Question);
				int result = md.ShowDialog();

				if (result != (int)Gtk.ResponseType.Yes){
					return;
				}

				if(DeleteBarierLayerEvent!= null)
					DeleteBarierLayerEvent(null,null);

			};

			SetSensitive(false);

			this.Insert (btnShowHideBarier, 0);

			this.Insert (btnEditBarierPoint, 1);
			this.Insert (btnDeleteBarierPoint, 2);
			this.Insert (btnDeleteBarier, 3);

			Gtk.ToolButton btnZoomIn = new Gtk.ToolButton("zoom-in.png");
			btnZoomIn.Label = MainClass.Languages.Translate("zoom_in");
			btnZoomIn.TooltipText = MainClass.Languages.Translate("zoom_in");
			//btnZoomIn.Relief = Gtk.ReliefStyle.None;
			btnZoomIn.CanFocus = false;
			btnZoomIn.Clicked+= delegate {
				ZoomIn();
			};

			Gtk.ToolButton btnZoomOriginal = new Gtk.ToolButton("zoom-original.png");
			btnZoomOriginal.Label = MainClass.Languages.Translate("zoom_original");
			btnZoomOriginal.TooltipText = MainClass.Languages.Translate("zoom_original");
			//btnZoomOriginal.Relief = Gtk.ReliefStyle.None;
			btnZoomOriginal.CanFocus = false;
			btnZoomOriginal.Clicked+= delegate {
				cbeZoom.Active = 11;
			};
			//btnZoomOriginal.WidthRequest = btnZoomOriginal.HeightRequest = 19;

			Gtk.ToolButton btnZoomOut = new Gtk.ToolButton("zoom-out.png");
			btnZoomOut.TooltipText = MainClass.Languages.Translate("zoom_out");
			btnZoomOut.Label = MainClass.Languages.Translate("zoom_out");
			//btnZoomOut.Relief = Gtk.ReliefStyle.None;
			btnZoomOut.CanFocus = false;
			btnZoomOut.Clicked+= delegate {
				ZoomOut();

			};

			string[] zoomCollection = new string[] { "3600%", "2400%", "1600%", "1200%", "800%", "700%", "600%", "500%", "400%", "300%", "200%", "100%", "66%", "50%", "33%", "25%", "16%", "12%", "8%", "5%" };
			cbeZoom = new ComboBoxEntry(zoomCollection);
			cbeZoom.Active = 11;
			cbeZoom.WidthRequest = 70;
			cbeZoom.Changed+= delegate(object sender, EventArgs e) {
				UpdateScale();
			};

			cbeZoom.Entry.FocusOutEvent+= delegate(object o, FocusOutEventArgs args) {
				//Console.WriteLine("FocusOutEvent");
				UpdateScale();
			};

			cbeZoom.Entry.FocusInEvent+= delegate(object o, FocusInEventArgs args) {

			};
			cbeZoom.Entry.Changed+= delegate(object sender, EventArgs e) {

				//UpdateScale();
			};

			ToolItem tic = new ToolItem();
			tic.Add(cbeZoom);

			this.Insert(new SeparatorToolItem(),4);
			this.Insert(btnZoomIn,5);
			this.Insert(btnZoomOut,6);
			this.Insert(tic,7);
			this.Insert(btnZoomOriginal,8);


		}

		private string lastvalid;

		private void UpdateScale(){
			string text =  cbeZoom.ActiveText.Trim ('%');
			//Console.WriteLine(text);
				double percent;

			if (!double.TryParse (text, out percent)) {
				cbeZoom.Entry.Text = lastvalid;
				return;
			}
			if (percent > 3600)
				cbeZoom.Active = 0;

			lastvalid = cbeZoom.ActiveText;
			ic.SetScale(percent/100);
		}


		private void SetSensitive(bool sensitive){

			//btnAddBarierPoint.Sensitive = sensitive;
			//btnMovieBarierPoint.Sensitive = sensitive;
			btnDeleteBarierPoint.Sensitive = sensitive;
			btnDeleteBarier.Sensitive = sensitive;
			btnEditBarierPoint.Sensitive = sensitive;

			btnDeleteBarierPoint.ShowAll();
			btnDeleteBarier.ShowAll();
			btnEditBarierPoint.ShowAll();
		}

		public void ZoomIn(){
			if (cbeZoom.Active>0)
				cbeZoom.Active= cbeZoom.Active-1;
		}
		public void ZoomOut(){
			if (cbeZoom.Active<18)
				cbeZoom.Active= cbeZoom.Active+1;
		}

		public ToolStateEnum ToolState{
			get {
				if(btnShowHideBarier. Active){
					//if(btnAddBarierPoint.Active){
					//	return ToolStateEnum.addpoint;
					//} else
					if(btnDeleteBarierPoint.Active){
						return ToolStateEnum.deletepoint;
					//}else if(btnMovieBarierPoint.Active){
					//	return ToolStateEnum.moviepoint;
					}else if(btnEditBarierPoint.Active){
						return ToolStateEnum.editpoint;
					}

				}
				return ToolStateEnum.nothing;
			}
		}

		public enum ToolStateEnum{
			nothing,
			addpoint,
			editpoint,
			deletepoint,
			moviepoint

		}
	}
}

