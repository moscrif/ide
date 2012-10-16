using System;
using Gdk;
using Gtk;
using GLib;
using System.Runtime.InteropServices;// InteropService;
using Cairo;
using Moscrif.IDE.Editors.ImageView;
using MessageDialogs = Moscrif.IDE.Controls.MessageDialog;
using Moscrif.IDE.Tool;
using System.Collections.Generic;
using System.IO;
using Moscrif.IDE.Components;
using Moscrif.IDE.Iface.Entities;

namespace Moscrif.IDE.Editors
{
	public class ImageEditor : IEditor
	{

		private Gtk.VBox vbox = null;

		private Gtk.ActionGroup editorAction = null;
		private string fileName = String.Empty;
		private string fileBarierName = String.Empty;
		List<BarierPoint> listPoint;

		private bool modified = false;

		private ImageCanvas ic;
		//private ImageToolBarControl itc;
		private ImageToolBarControl itc;
		private string statusFormat ="X: {0}; Y: {1}; W {2}; H {3}";

		public ImageEditor(string filePath)
		{
			fileName =filePath;

			fileBarierName = fileName+".mso";
			if(System.IO.File.Exists(fileBarierName)){
				string barierFile;
				try {
					using (StreamReader file = new StreamReader(fileBarierName)) {
							barierFile = file.ReadToEnd();
							file.Close();
							file.Dispose();
					}
					if(!string.IsNullOrEmpty(barierFile)){
						//listPoint =  JsonConvert.DeserializeObject<List<BarierPoint>>(barierFile);
						System.Web.Script.Serialization.JavaScriptSerializer jss= new System.Web.Script.Serialization.JavaScriptSerializer();
						jss.RegisterConverters(new[]{new BarierPointJavaScriptConverter()} );
						listPoint = jss.Deserialize<List<BarierPoint>>(barierFile);
					}

				} catch (Exception ex) {
					MessageDialogs ms = new MessageDialogs(MessageDialogs.DialogButtonType.Ok, MainClass.Languages.Translate("file_cannot_open", fileBarierName), ex.Message, Gtk.MessageType.Error,null);
					ms.ShowDialog();
				}
			}


			editorAction = new Gtk.ActionGroup("imageeditor");

			//ic = new ImageCanvas(filePath,listPoint);
			ic = new ImageCanvas(filePath,listPoint) {
				Name = "canvas",
				CanDefault = true,
				CanFocus = true,
				Events = (Gdk.EventMask)16134
			};

			vbox = new Gtk.VBox();

			/*Gdk.Color col = new Gdk.Color(255,255,0);
			vbox.ModifyBg(StateType.Normal, col);*/

			itc = new ImageToolBarControl (ic);//new ImageToolBarControl(ic);
			itc.DeleteBarierLayerEvent+= delegate(object sender, EventArgs e) {

				if(System.IO.File.Exists(fileBarierName) ){
					try{
						System.IO.File.Delete(fileBarierName);
						OnModifiedChanged(false);
						ic.DeleteBarier();
					}catch (Exception ex){
						MessageDialogs mdd =
							new MessageDialogs(MessageDialogs.DialogButtonType.Ok, MainClass.Languages.Translate("cannot_delete_file",fileBarierName),ex.Message, Gtk.MessageType.Error);
						mdd.ShowDialog();
					}
				} else {
					ic.DeleteBarier();
				}
			};

			vbox.PackStart(itc,false,false,0);

			ScrolledWindow sw = new Gtk.ScrolledWindow();
			sw.ShadowType = Gtk.ShadowType.Out;

			sw.Hadjustment.ValueChanged += delegate(object sender, EventArgs e) {
				//Console.WriteLine("sw.Hadjustment -> {0}",sw.Hadjustment.Value);
			};

			sw.Vadjustment.ValueChanged += delegate(object sender, EventArgs e) {
				//Console.WriteLine("sw.Vadjustment -> {0}",sw.Vadjustment.Value);
			};

			Viewport vp = new Viewport () {
				ShadowType = ShadowType.None
			};

			vp.ScrollEvent+= delegate(object o, ScrollEventArgs args) {
				if (args.Event.State  == ModifierType.ControlMask) {
					switch (args.Event.Direction) {
						case ScrollDirection.Down:
						case ScrollDirection.Right:
							 itc.ZoomOut();
							return ;
						case ScrollDirection.Left:
						case ScrollDirection.Up:
							itc.ZoomIn();
							return ;
					}
				}
			};

			vp.MotionNotifyEvent+= delegate(object o, MotionNotifyEventArgs args) {

				Cairo.PointD offset = new Cairo.PointD(sw.Hadjustment.Value,sw.Vadjustment.Value);
				int x =(int)args.Event.X;
				int y =(int)args.Event.Y;
				if(ic.ConvertPointToCanvasPoint(offset, ref x, ref y)){
					OnWriteToStatusbar(String.Format(statusFormat,x,y,ic.WidthImage,ic.HeightImage));
				}

				if(itc.ToolState == ImageToolBarControl.ToolStateEnum.nothing ) return;

				if(itc.ToolState == ImageToolBarControl.ToolStateEnum.editpoint){

					ic.StepMovingPoint((int)args.Event.X,(int)args.Event.Y,offset);
					//OnModifiedChanged(true);
				}
			};

			vp.ButtonReleaseEvent+= delegate(object o, ButtonReleaseEventArgs args) {
				//Console.WriteLine("1_ButtonReleaseEvent");
				if (args.Event.Button != 1) return;

				if(itc.ToolState == ImageToolBarControl.ToolStateEnum.nothing ) return;
				Cairo.PointD offset = new Cairo.PointD(sw.Hadjustment.Value,sw.Vadjustment.Value);

				if(itc.ToolState == ImageToolBarControl.ToolStateEnum.editpoint){

					ic.EndMovingPoint((int)args.Event.X,(int)args.Event.Y,offset);
					OnModifiedChanged(true);
				}
			};

			vp.ButtonPressEvent+= delegate(object o, ButtonPressEventArgs args) {
				if (args.Event.Button != 1) return;

				if(itc.ToolState == ImageToolBarControl.ToolStateEnum.nothing ) return;

				Cairo.PointD offset = new Cairo.PointD(sw.Hadjustment.Value,sw.Vadjustment.Value);
				if(itc.ToolState == ImageToolBarControl.ToolStateEnum.addpoint){
					ic.AddPoint((int)args.Event.X,(int)args.Event.Y,offset);
					OnModifiedChanged(true);
				
				} else if(itc.ToolState == ImageToolBarControl.ToolStateEnum.deletepoint){
					ic.DeletePoint((int)args.Event.X,(int)args.Event.Y,offset);
					OnModifiedChanged(true);
				
				} else if(itc.ToolState == ImageToolBarControl.ToolStateEnum.moviepoint){

					ic.StartMovingPoint((int)args.Event.X,(int)args.Event.Y,offset);
					OnModifiedChanged(true);

				} else if(itc.ToolState == ImageToolBarControl.ToolStateEnum.editpoint){
					if (args.Event.State  == ModifierType.ShiftMask){
						ic.DeletePoint((int)args.Event.X,(int)args.Event.Y,offset);
						OnModifiedChanged(true);
						return;
					}
						ic.EditPoint((int)args.Event.X,(int)args.Event.Y,offset);
						OnModifiedChanged(true);
				}
			};


			sw.Add(vp);
			vp.Add (ic);

			vbox.PackEnd(sw,true,true,0);
			ic.Show ();
			vp.Show ();

			vbox.ShowAll();

		}


		#region IEditor implementation

		public void Rename(string newName){

			fileName = newName;
		}

		public bool RefreshSettings(){
			ic.RefreshSetting();
			return true;
		}


		public object GetSelected(){
			return null;
		}		
		
		public bool Save ()
		{
			if ((!modified) || (ic.ListPoint== null))
				return true;

			try {
				//string json = JsonConvert.SerializeObject(ic.ListPoint, Formatting.Indented);

					string json="";
				System.Web.Script.Serialization.JavaScriptSerializer oSerializer =
				         new System.Web.Script.Serialization.JavaScriptSerializer();

				oSerializer.RegisterConverters(new[]{new BarierPointJavaScriptConverter()} );

				string sJSON = oSerializer.Serialize(ic.ShapeListPoint);
				sJSON = sJSON.Replace("\"x\"","x");
				sJSON = sJSON.Replace("\"y\"","y");
				json  = sJSON;

				using (StreamWriter file = new StreamWriter(fileBarierName)) {
					file.Write(json);

					file.Close();
					file.Dispose();
				}
				OnModifiedChanged(false);

			} catch (Exception ex) {
				MessageDialogs ms = new MessageDialogs(MessageDialogs.DialogButtonType.Ok, MainClass.Languages.Translate("cannot_save_file",fileName), ex.Message, Gtk.MessageType.Error);
				ms.ShowDialog();
				return false;
			}
			// do save
			return true;

		}

		public bool SaveAs (string newPath)
		{
			if (System.IO.File.Exists(newPath)) {
				MessageDialogs md = new MessageDialogs(MessageDialogs.DialogButtonType.YesNo, "", MainClass.Languages.Translate("overwrite_file", newPath), Gtk.MessageType.Question);
				int result = md.ShowDialog();

				if (result != (int)Gtk.ResponseType.Yes)
					return false;
			}

			try {

				System.IO.File.Copy(fileName,newPath);

				fileName = newPath;

			} catch (Exception ex) {
				MessageDialogs ms = new MessageDialogs(MessageDialogs.DialogButtonType.Ok, MainClass.Languages.Translate("cannot_save_file", newPath), ex.Message, Gtk.MessageType.Error);
				ms.ShowDialog();
				return false;
			}
			// do save
			return true;
			//return true;
			//throw new NotImplementedException ();
		}

		public void Close()
		{

		}		

		public bool SearchExpression (SearchPattern expresion){
			return false;
			//throw new NotImplementedException ();
		}

		public bool SearchNext(SearchPattern expresion){
			return false;
			//throw new NotImplementedException ();
		}

		public bool SearchPreviu(SearchPattern expresion){
			return false;
			//throw new NotImplementedException ();
		}

		public bool Replace(SearchPattern expresion){
			return false;
			//throw new NotImplementedException ();
		}

		public bool ReplaceAll(SearchPattern expresion){
			return false;
			//throw new NotImplementedException ();
		}

		public List<FindResult> FindReplaceAll(SearchPattern expresion)
		{
			return null;
		}		

		public bool Undo(){
			return false;
			//throw new NotImplementedException ();
		}

		public bool Redo(){
			return false;
			//throw new NotImplementedException ();
		}

		public string Caption {
			get { return System.IO.Path.GetFileName(fileName); }
		}

		public string FileName {
			get { return fileName; }
		}

		public bool Modified {
			get { return modified; }
		}

		public void GoToPosition(object position){
		}

		public Gtk.Widget Control {
			get { return vbox; }
		}

		public Gtk.ActionGroup EditorAction
		{
			get {return editorAction;}
		}

		public void ActivateEditor(bool updateStatus){
			if(updateStatus){
				OnWriteToStatusbar(String.Format(statusFormat,0,0,ic.WidthImage,ic.HeightImage));
			}
		}

		public event EventHandler<ModifiedChangedEventArgs> ModifiedChanged;
		public event EventHandler<WriteStatusEventArgs> WriteStatusChange;

		void OnModifiedChanged(bool newModified)
		{
			if (newModified != modified)
				modified = newModified;
			else
				return;
			
			ModifiedChangedEventArgs mchEventArg = new ModifiedChangedEventArgs(modified);
			
			if (ModifiedChanged != null)
				ModifiedChanged(this, mchEventArg);
		}

		void OnWriteToStatusbar(string message)
		{

			WriteStatusEventArgs mchEventArg = new WriteStatusEventArgs(message);

			if (WriteStatusChange != null)
				WriteStatusChange(this, mchEventArg);
		}


		#endregion
}

public class CairoGraphic : Gtk.DrawingArea
{
    ImageSurface src;
    
    public CairoGraphic (ImageSurface src)
    {
        this.src = src;
    }
    
    protected override bool OnExposeEvent (Gdk.EventExpose args)
    {
        using (Context g = Gdk.CairoHelper.Create (args.Window)){
	    g.Source = new SurfacePattern (src);
	    g.Paint ();
	}
        return true;
    }
}


}


