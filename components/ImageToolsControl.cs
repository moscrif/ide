using System;
using Gtk;
using Moscrif.IDE.Editors.ImageView;
using MessageDialogs = Moscrif.IDE.Controls.MessageDialog;

namespace Moscrif.IDE.Components
{
	public class ImageToolsControl : Table
	{


		/*OLD */

		
		HButtonBox toolbutton;
		ToggleButton btnShowHideBarier;
		//ToggleButton btnAddBarierPoint= new ToggleButton();
		//ToggleButton btnMovieBarierPoint= new ToggleButton();

		ToggleButton btnEditBarierPoint ;
		ToggleButton btnDeleteBarierPoint ;
		Button btnDeleteBarier;
		Table tblButton;

		ComboBoxEntry cbeZoom;
		ImageCanvas ic;

		public event EventHandler DeleteBarierLayerEvent;


		public ImageToolsControl(ImageCanvas ic): base (1,6,false)
		{
			this.NRows = 1;
			this.NColumns = 10;
			this.ic = ic;
			tblButton = new Table(1,5,false);

			toolbutton = new HButtonBox();
			toolbutton.LayoutStyle = ButtonBoxStyle.Start;

			btnShowHideBarier = new ToggleButton();
			btnShowHideBarier.Add(MainClass.Tools.CreatePicLabelWidget("barier-show.png",MainClass.Languages.Translate("show_barier_layer")));
			btnShowHideBarier.Name = "btnShowHideBarier";
			btnShowHideBarier.Relief = ReliefStyle.None;
			btnShowHideBarier.CanFocus = false;
			btnShowHideBarier.BorderWidth = 1;
			//btnShowHideBarier.WidthRequest = 75;
			btnShowHideBarier.TooltipText = MainClass.Languages.Translate("show_barier_layer_tt");
			btnShowHideBarier.Toggled+= delegate(object sender, EventArgs e) {

				this.ic.ShowBarierLayer =  btnShowHideBarier.Active;
				SetSensitive(btnShowHideBarier.Active);

			};

			btnEditBarierPoint = new ToggleButton();
			btnEditBarierPoint.Name = "btnEditBarierPoint";
			btnEditBarierPoint.Add(MainClass.Tools.CreatePicLabelWidget("barier-add.png",MainClass.Languages.Translate("edit_barier_point")));
			btnEditBarierPoint.Relief = ReliefStyle.None;
			btnEditBarierPoint.CanFocus = false;
			btnEditBarierPoint.BorderWidth = 1;
			btnEditBarierPoint.TooltipText = MainClass.Languages.Translate("edit_barier_point_tt");
			btnEditBarierPoint.Toggled+= delegate(object sender, EventArgs e) {
				if(btnEditBarierPoint.Active){
					btnDeleteBarierPoint.Active = false;
					//btnMovieBarierPoint.Active = false;
				}
			};

			btnDeleteBarierPoint = new ToggleButton();
			btnDeleteBarierPoint.Name = "btnDeleteBarierPoint";
			btnDeleteBarierPoint.Add(MainClass.Tools.CreatePicLabelWidget("barier-delete.png",MainClass.Languages.Translate("delete_barier_point")));
			btnDeleteBarierPoint.Relief = ReliefStyle.None;
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

			btnDeleteBarier = new Button();
			btnDeleteBarier.Name = "btnDeleteBarier";
			btnDeleteBarier.Add(MainClass.Tools.CreatePicLabelWidget("barier-delete-all.png",MainClass.Languages.Translate("delete_barier")));
			btnDeleteBarier.Relief = ReliefStyle.None;
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

			btnShowHideBarier.Active = false;
			//btnAddBarierPoint.Active = false;
			//btnMovieBarierPoint.Active = false;
			btnDeleteBarierPoint.Active = false;
			btnEditBarierPoint.Active = false;
			SetSensitive(false);


			tblButton.Attach(btnShowHideBarier,0,1,0,1,AttachOptions.Shrink,AttachOptions.Shrink,0,0);
			//tblButton.Attach(btnAddBarierPoint,1,2,0,1,AttachOptions.Shrink,AttachOptions.Shrink,0,0);
			//tblButton.Attach(btnMovieBarierPoint,2,3,0,1,AttachOptions.Shrink,AttachOptions.Shrink,0,0);
			tblButton.Attach(btnEditBarierPoint,1,2,0,1,AttachOptions.Shrink,AttachOptions.Shrink,0,0);
			tblButton.Attach(btnDeleteBarierPoint,2,3,0,1,AttachOptions.Shrink,AttachOptions.Shrink,0,0);
			tblButton.Attach(btnDeleteBarier,3,4,0,1,AttachOptions.Shrink,AttachOptions.Shrink,0,0);
			tblButton.ShowAll();


			this.Attach(tblButton,0,1,0,1,AttachOptions.Shrink,AttachOptions.Shrink,0,0);
			this.Attach(new HSeparator(),1,2,0,1,AttachOptions.Shrink,AttachOptions.Shrink,0,0);


			Gtk.Button btnZoomIn = new Gtk.Button(new Image("zoom-in.png",IconSize.Button));
			btnZoomIn.TooltipText = MainClass.Languages.Translate("zoom_in");
			btnZoomIn.Relief = Gtk.ReliefStyle.None;
			btnZoomIn.CanFocus = false;
			btnZoomIn.Clicked+= delegate {
				ZoomIn();

			};
			//btnZoomIn.WidthRequest = btnZoomIn.HeightRequest = 19;

			Gtk.Button btnZoomOriginal = new Gtk.Button(new Image("zoom-original.png",IconSize.Button));
			btnZoomOriginal.TooltipText = MainClass.Languages.Translate("zoom_original");
			btnZoomOriginal.Relief = Gtk.ReliefStyle.None;
			btnZoomOriginal.CanFocus = false;
			btnZoomOriginal.Clicked+= delegate {
				cbeZoom.Active = 11;
			};
			//btnZoomOriginal.WidthRequest = btnZoomOriginal.HeightRequest = 19;

			Gtk.Button btnZoomOut = new Gtk.Button(new Image("zoom-out.png",IconSize.Button));
			btnZoomOut.TooltipText = MainClass.Languages.Translate("zoom_out");
			btnZoomOut.Relief = Gtk.ReliefStyle.None;
			btnZoomOut.CanFocus = false;
			btnZoomOut.Clicked+= delegate {
				ZoomOut();

			};
			//btnZoomOut.WidthRequest = btnZoomOut.HeightRequest = 19;

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

			this.Attach(btnZoomOut,2,3,0,1,AttachOptions.Shrink,AttachOptions.Shrink,0,0);
			this.Attach(cbeZoom,3,4,0,1,AttachOptions.Shrink,AttachOptions.Shrink,0,0);
			this.Attach(btnZoomIn,4,5,0,1,AttachOptions.Shrink,AttachOptions.Shrink,0,0);
			this.Attach(btnZoomOriginal,5,6,0,1,AttachOptions.Shrink,AttachOptions.Shrink,0,0);
			//this.PackEnd(cbeZoom,false,false,1);
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
				if(btnShowHideBarier.Active){
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

