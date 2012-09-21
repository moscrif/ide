using System;
using Gtk;
using Cairo;
using Gdk;
using System.Collections.Generic;
using Moscrif.IDE.Settings;
//using Rsvg;

using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;



namespace Moscrif.IDE.Editors.ImageView
{
	public class ImageCanvas : DrawingArea
	{
		public ImageCanvas(string fileName, List<BarierPoint> shapeListPoint)
		{
			this.fileName = fileName;
			if (shapeListPoint != null)
				this.shapeListPoint = shapeListPoint;
			else
				shapeListPoint = new List<BarierPoint>();

			if(MainClass.Settings.ImageEditors == null){
				MainClass.Settings.ImageEditors =  new Settings.Settings.ImageEditorSetting();
				MainClass.Settings.ImageEditors.LineWidth = 3;
				MainClass.Settings.ImageEditors.PointWidth = 5;

				MainClass.Settings.ImageEditors.LineColor = new Settings.Settings.BackgroundColors(10,10,255,32767);
				MainClass.Settings.ImageEditors.PointColor = new Settings.Settings.BackgroundColors(10,10,255,32767);
				MainClass.Settings.ImageEditors.SelectPointColor = new Settings.Settings.BackgroundColors(255,10,10,32767);
			}

			lineWitdth = MainClass.Settings.ImageEditors.LineWidth;
			pointWidth = MainClass.Settings.ImageEditors.PointWidth;

			Gdk.Color gdkLineColor = new Gdk.Color(MainClass.Settings.ImageEditors.LineColor.Red,
				MainClass.Settings.ImageEditors.LineColor.Green,MainClass.Settings.ImageEditors.LineColor.Blue);

			Gdk.Color gdkPointColor =  new Gdk.Color(MainClass.Settings.ImageEditors.PointColor.Red,
				MainClass.Settings.ImageEditors.PointColor.Green,MainClass.Settings.ImageEditors.PointColor.Blue);

			Gdk.Color gdkSelPointColor =  new Gdk.Color(MainClass.Settings.ImageEditors.SelectPointColor.Red,
				MainClass.Settings.ImageEditors.SelectPointColor.Green,MainClass.Settings.ImageEditors.SelectPointColor.Blue);

			colorLine = gdkLineColor.ToCairoColor(MainClass.Settings.ImageEditors.LineColor.Alpha);
			colorPoint = gdkPointColor.ToCairoColor(MainClass.Settings.ImageEditors.PointColor.Alpha);
			colorSelectPoint = gdkSelPointColor.ToCairoColor(MainClass.Settings.ImageEditors.SelectPointColor.Alpha);
		}

		public int WidthImage {get;set;}
		public int HeightImage {get;set;}

		private string fileName;

		List<BarierPoint> listPoint;
		List<BarierPoint> shapeListPoint;

		int width, height;

		int drawOffsetX, drawOffsetY;
		double scaling = 1;

		private BarierPoint movingPoint;
		private Cairo.Color colorLine;
		private Cairo.Color colorPoint;
		private Cairo.Color colorSelectPoint;
		private int lineWitdth;
		private int pointWidth;

		public List<BarierPoint> ListPoint
		{
			get { 
				return this.listPoint;
			}
		}

		public List<BarierPoint> ShapeListPoint
		{
			get { 
				List<BarierPoint> updateList = new List<BarierPoint>();
				foreach(BarierPoint bp in this.listPoint){
					BarierPoint updateBp = new BarierPoint();
					updateBp.X = bp.X -(1/HeightImage);
					updateBp.Y = -bp.Y +(1/WidthImage);//(bp.Y -(1/WidthImage))*-1
					updateList.Add(updateBp);
				}
				return updateList; 
			}
		}

		public void RefreshSetting(){

			lineWitdth = MainClass.Settings.ImageEditors.LineWidth;
			pointWidth = MainClass.Settings.ImageEditors.PointWidth;

			Gdk.Color gdkLineColor = new Gdk.Color(MainClass.Settings.ImageEditors.LineColor.Red,
				MainClass.Settings.ImageEditors.LineColor.Green,MainClass.Settings.ImageEditors.LineColor.Blue);

			Gdk.Color gdkPointColor =  new Gdk.Color(MainClass.Settings.ImageEditors.PointColor.Red,
				MainClass.Settings.ImageEditors.PointColor.Green,MainClass.Settings.ImageEditors.PointColor.Blue);

			Gdk.Color gdkSelPointColor =  new Gdk.Color(MainClass.Settings.ImageEditors.SelectPointColor.Red,
				MainClass.Settings.ImageEditors.SelectPointColor.Green,MainClass.Settings.ImageEditors.SelectPointColor.Blue);

			colorLine = gdkLineColor.ToCairoColor(MainClass.Settings.ImageEditors.LineColor.Alpha);
			colorPoint = gdkPointColor.ToCairoColor(MainClass.Settings.ImageEditors.PointColor.Alpha);
			colorSelectPoint = gdkSelPointColor.ToCairoColor(MainClass.Settings.ImageEditors.SelectPointColor.Alpha);

			try{
				GdkWindow.InvalidateRect(new Gdk.Rectangle(drawOffsetX, drawOffsetY, width, height), false);
			} catch{}

		}

		protected override bool OnExposeEvent(Gdk.EventExpose e)
		{
			Gdk.Pixbuf bg;
			try{
				//if(fileName.ToLower().EndsWith (".svg")){
					//bg = Rsvg.Pixbuf.FromFile(fileName);
				//	bg = Rsvg.Tool.PixbufFromFileAtSize(fileName,800,600);

				//} else{
					using (var fs = new System.IO.FileStream(fileName, System.IO.FileMode.Open))
						bg = new Gdk.Pixbuf(fs);
				//}

			}catch(Exception ex){
				Tool.Logger.Error(ex.Message,null);
				return true;
			}


			bg = bg.ApplyEmbeddedOrientation();
			this.HeightImage = bg.Height;
			this.WidthImage= bg.Width;
			if ( this.listPoint == null ){
				this.listPoint = new List<BarierPoint>();
				if( this.shapeListPoint != null ){
					foreach(BarierPoint bp in this.shapeListPoint){
						BarierPoint updateBp = new BarierPoint();
						updateBp.X = bp.X +(1/HeightImage);
						updateBp.Y = -bp.Y +(1/WidthImage);
						listPoint.Add(updateBp);
					}
				}
			}


			//Size imagesize = new Size (bg.Width, bg.Height);
			
			width = (int)(bg.Width * scaling);
			height = (int)(bg.Height * scaling);
			
			int x, y, w, h, d = 0;
			this.ParentWindow.GetGeometry(out x, out y, out w, out h, out d);
			
			drawOffsetX = (w - width) / 2;
			if (drawOffsetX < 0)
				drawOffsetX = 0;
			
			drawOffsetY = (h - height) / 2;
			if (drawOffsetY < 0)
				drawOffsetY = 0;


			using (Context cr = Gdk.CairoHelper.Create(e.Window)) {
				if(!MainClass.Platform.IsMac){
					FillChecks (cr, 0, 0,w,h);//w, h);
					cr.Save ();
					
					cr.DrawRectangle(new Cairo.Rectangle(drawOffsetX - 1, drawOffsetY - 1, width + 1, height + 1), new Cairo.Color(0, 0, 0), 1);
					cr.Rectangle(new Cairo.Rectangle(drawOffsetX - 1, drawOffsetY - 1, width, height));
					cr.Clip();
				}

				cr.Scale(scaling, scaling);
				
				CairoHelper.SetSourcePixbuf(cr, bg, drawOffsetX / scaling, drawOffsetY / scaling);
				
				cr.Paint();
				
				this.WidthRequest = width + 1;
				this.HeightRequest = height + 1;
				
				if (showBarierLayer) {
					Draw(cr, width, height);
				}
				cr.Scale(scaling, scaling);
			}
			return true;
			/*using (Context cr = Gdk.CairoHelper.Create (e.Window)) {
				int w, h;
				e.Window.GetSize (out w, out h);
				Draw (cr, w, h);
			}
			return true;*/
		}

		#region TEST

	void Draw3Circles (Context cr, int xc, int yc, double radius, double alpha)
	{
		double subradius = radius * (2 / 3.0 - 0.1);

		cr.Color = new Cairo.Color (1.0, 0.0, 0.0, alpha);
		OvalPath (cr, xc + radius / 3.0 * Math.Cos (Math.PI * 0.5), yc - radius / 3.0 * Math.Sin (Math.PI * 0.5), subradius, subradius);
		cr.Fill ();

		cr.Color = new Cairo.Color (0.0, 1.0, 0.0, alpha);
		OvalPath (cr, xc + radius / 3.0 * Math.Cos (Math.PI * (0.5 + 2 / 0.3)), yc - radius / 3.0 * Math.Sin (Math.PI * (0.5 + 2 / 0.3)), subradius, subradius);
		cr.Fill ();

		cr.Color = new Cairo.Color (0.0, 0.0, 1.0, alpha);
    		OvalPath (cr, xc + radius / 3.0 * Math.Cos (Math.PI * (0.5 + 4 / 0.3)), yc - radius / 3.0 * Math.Sin (Math.PI * (0.5 + 4 / 0.3)), subradius, subradius);
		cr.Fill ();
	}

	void Draw2 (Context cr, int width, int height)
	{
		double radius = 0.5 * Math.Min (width, height) - 10;
		int xc = width / 2;
		int yc = height / 2;

		Surface overlay = cr.Target.CreateSimilar (Content.ColorAlpha, width, height);
		Surface punch   = cr.Target.CreateSimilar (Content.Alpha, width, height);
		Surface circles = cr.Target.CreateSimilar (Content.ColorAlpha, width, height);

		FillChecks (cr, 0, 0, width, height);
		cr.Save ();

		// Draw a black circle on the overlay
		using (Context cr_overlay = new Context (overlay)) {
			cr_overlay.Color = new Cairo.Color (0.0, 0.0, 0.0);
			OvalPath (cr_overlay, xc, yc, radius, radius);
			cr_overlay.Fill ();

			using (Context cr_tmp = new Context (punch))
				Draw3Circles (cr_tmp, xc, yc, radius, 1.0);

			cr_overlay.Operator = Operator.DestOut;
			cr_overlay.SetSourceSurface (punch, 0, 0);
			cr_overlay.Paint ();

			using (Context cr_circles = new Context (circles)) {
				cr_circles.Operator = Operator.Over;
				Draw3Circles (cr_circles, xc, yc, radius, 0.5);
			}

			cr_overlay.Operator = Operator.Add;
			cr_overlay.SetSourceSurface (circles, 0, 0);
			cr_overlay.Paint ();
		}

		cr.SetSourceSurface (overlay, 0, 0);
		cr.Paint ();

		overlay.Destroy ();
		punch.Destroy ();
		circles.Destroy ();
	}
		#endregion


		void Draw(Context cr, int width, int height)
		{
			Surface overlay = cr.Target.CreateSimilar(Content.ColorAlpha, width, height);
			Surface addSurface = cr.Target.CreateSimilar(Content.ColorAlpha, width, height);

			//	FillChecks (cr, 0, 0,width, height);//w, h);
			//	cr.Save ();

			using (Context cr_overlay = new Context(overlay))
				if (listPoint != null && listPoint.Count > 0) {

					using (Context cr_addSurface = new Context(addSurface)) {
						
						BarierPoint actualPoint;
						BarierPoint nearstBP;

						for (int i = 0; i < listPoint.Count; i++) {

							actualPoint = listPoint[i];
							if (i == listPoint.Count-1) {
								nearstBP = listPoint[0];
							} else
								nearstBP = listPoint[i + 1];
							Cairo.Color clrPoint;

							if (actualPoint.Equals(movingPoint))
								clrPoint =colorSelectPoint;
							else
								clrPoint = colorPoint;

							cr_addSurface.Color = clrPoint;
							OvalPath(cr_addSurface, actualPoint.X * scaling, actualPoint.Y * scaling, pointWidth, pointWidth);
							cr_addSurface.Fill();
							
							cr_addSurface.Color = colorLine;
							LinePath(cr_addSurface, actualPoint.X * scaling, actualPoint.Y * scaling, nearstBP.X * scaling, nearstBP.Y * scaling);
							cr_addSurface.Fill();
						}

					cr_overlay.Operator = Operator.Add;
					cr_overlay.SetSourceSurface(addSurface, 0, 0);
					cr_overlay.Paint();
				}
			
			cr.Scale(1 / scaling, 1 / scaling);
			
			cr.SetSourceSurface(overlay, (int)(drawOffsetX), (int)(drawOffsetY));
			
			cr.Paint();

			}

			addSurface.Destroy();
			overlay.Destroy();
		}

		void FillChecks (Context cr, int x, int y, int width, int height)
		{
			int CHECK_SIZE = 32;

			cr.Save ();
			Surface check = cr.Target.CreateSimilar (Content.Color, 2 * CHECK_SIZE, 2 * CHECK_SIZE);

			// draw the check
			using (Context cr2 = new Context (check)) {
				cr2.Operator = Operator.Source;
				cr2.Color = new  Cairo.Color (0.4, 0.4, 0.4);
				cr2.Rectangle (0, 0, 2 * CHECK_SIZE, 2 * CHECK_SIZE);//0,0
				cr2.Fill ();

				cr2.Color = new Cairo.Color (0.7, 0.7, 0.7);
				cr2.Rectangle (x, y, CHECK_SIZE, CHECK_SIZE);
				cr2.Fill ();

				cr2.Rectangle (x + CHECK_SIZE, y + CHECK_SIZE, CHECK_SIZE, CHECK_SIZE);
				cr2.Fill ();
			}

			// Fill the whole surface with the check
			SurfacePattern check_pattern = new SurfacePattern (check);
			check_pattern.Extend = Extend.Repeat;
			cr.Source = check_pattern;
			cr.Rectangle (0, 0, width, height);//0,0
			cr.Fill ();

			check_pattern.Destroy ();
			check.Destroy ();
			cr.Restore ();
		}

		public void StartMovingPoint(int x, int y, Cairo.PointD offset)
		{
			
			if (!ConvertPointToCanvasPoint(offset, ref x, ref y))
				return;
			
			if (listPoint == null)
				listPoint = new List<BarierPoint>();
			
			BarierPoint bp = new BarierPoint(x, y);
			BarierPoint nearstBP = BarierPoint.ClosestPoint(bp, listPoint);
			if (nearstBP != null) {
				movingPoint = nearstBP;
				//GdkWindow.InvalidateRect(new Gdk.Rectangle(drawOffsetX, drawOffsetY, width, height), false);
			}
		}

		public bool ConvertPointToCanvasPoint(Cairo.PointD offset, ref int x, ref int y)
		{
			
			if (x < drawOffsetX)
				return false;
			if (y < drawOffsetY)
				return false;
			if (x > drawOffsetX + width)
				return false;
			if (y > drawOffsetY + height)
				return false;
			
			x = (int)((x / scaling) - /* + offset.X*/(drawOffsetX / scaling));
			y = (int)((y / scaling) - /*+ offset.Y*/(drawOffsetY / scaling));
			
			return true;
		}

		public void StepMovingPoint(int x, int y, Cairo.PointD offset)
		{
			
			if (movingPoint == null)
				return;
			
			if (!ConvertPointToCanvasPoint(offset, ref x, ref y))
				return;
			
			if (listPoint == null)
				listPoint = new List<BarierPoint>();
			
			movingPoint.X = x;
			movingPoint.Y = y;
			GdkWindow.InvalidateRect(new Gdk.Rectangle(drawOffsetX, drawOffsetY, width, height), false);
		}

		public void EndMovingPoint(int x, int y, Cairo.PointD offset)
		{
			
			if (movingPoint == null)
				return;
			
			if (!ConvertPointToCanvasPoint(offset, ref x, ref y)) {
				movingPoint = null;
				return;
			}
			
			if (listPoint == null)
				listPoint = new List<BarierPoint>();
			
			movingPoint.X = x;
			movingPoint.Y = y;
			movingPoint = null;
			GdkWindow.InvalidateRect(new Gdk.Rectangle(drawOffsetX, drawOffsetY, width, height), false);
		}

		public void EditPoint(int x, int y, Cairo.PointD offset)
		{
			int realX = x;
			int realY = y;
			if (!ConvertPointToCanvasPoint(offset, ref realX, ref realY))
				return;

			if (listPoint == null)
				listPoint = new List<BarierPoint>();

			BarierPoint bp = new BarierPoint(realX, realY);
			BarierPoint nearstBP = BarierPoint.ClosestPoint(bp, listPoint);

			double distance = 999;

			if(nearstBP != null){
				distance = BarierPoint.Distance(bp,nearstBP);
			}

			if(distance <5.5 ){
				StartMovingPoint(x,y,offset);
			} else {
				AddPoint(x,y,offset);
			}
		}

		public void AddPoint(int x, int y, Cairo.PointD offset)
		{

			if (!ConvertPointToCanvasPoint(offset, ref x, ref y))
				return;

			if (listPoint == null)
				listPoint = new List<BarierPoint>();

			BarierPoint bp = new BarierPoint(x, y);

			int indx =  BarierPoint.ClosestLine(bp,listPoint);

			if(listPoint.Count >8){

			}

			if((indx >listPoint.Count-1) || (indx<0)){
				listPoint.Add(bp);
			} else {
				listPoint.Insert(indx,bp);
			}
		
			GdkWindow.InvalidateRect(new Gdk.Rectangle(drawOffsetX, drawOffsetY, width, height), false);
		}

		public void DeletePoint(int x, int y, Cairo.PointD offset)
		{

			if (!ConvertPointToCanvasPoint(offset, ref x, ref y))
				return;
			
			if (listPoint == null)
				listPoint = new List<BarierPoint>();


			BarierPoint bp = new BarierPoint(x, y);
			BarierPoint nearstBP = BarierPoint.ClosestPoint(bp, listPoint);
			if (nearstBP != null) {
				listPoint.Remove(nearstBP);
				GdkWindow.InvalidateRect(new Gdk.Rectangle(drawOffsetX, drawOffsetY, width, height), false);
			}
		}

		public void DeleteBarier()
		{
			if (listPoint != null) {
				listPoint.Clear();
				GdkWindow.InvalidateRect(new Gdk.Rectangle(drawOffsetX, drawOffsetY, width, height), false);
			}
		}

		public void SetScale(double scale)
		{
			scaling = scale;
			GdkWindow.InvalidateRect(new Gdk.Rectangle(drawOffsetX, drawOffsetY, width, height), false);
		}


		private bool showBarierLayer;
		public bool ShowBarierLayer
		{
			set {
				showBarierLayer = value;
				GdkWindow.InvalidateRect(new Gdk.Rectangle(drawOffsetX, drawOffsetY, width, height), false);
			}
		}

		void OvalPath(Context cr, double xc, double yc, double xr, double yr)
		{
			Matrix m = cr.Matrix;

			cr.Translate(xc, yc);
			cr.Scale(1.0, yr / xr);
			cr.MoveTo(xr, 0.0);
			cr.Arc(0, 0, xr, 0, 2 * Math.PI);
			cr.ClosePath();
			
			cr.Matrix = m;
		}

		void LinePath(Context cr, double xs, double ys, double xe, double ye)
		{
			Matrix m = cr.Matrix;
			cr.Antialias = Antialias.Subpixel;
			cr.LineWidth = lineWitdth;
			cr.LineCap = LineCap.Round;
			cr.MoveTo(xs, ys);
			cr.LineTo(xe, ye);
			cr.Stroke();
			
			cr.ClosePath();
			
			cr.Matrix = m;
		}

	}
}

