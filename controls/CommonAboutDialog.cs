using System;
using System.Text;

using Gdk;
using Gtk;
using GLib;
using Pango;
using System.IO;
using Moscrif.IDE.Components;

namespace Moscrif.IDE.Controls
{
	internal class ScrollBox : DrawingArea
	{
		Pixbuf image;
		Gdk.GC backGc;


		public ScrollBox()
		{
			this.Realized += new EventHandler(OnRealized);
			this.ModifyBg(Gtk.StateType.Normal,new Gdk.Color(255, 255, 255)); //new Gdk.Color(49, 49, 74));
			
			image = new Gdk.Pixbuf(System.IO.Path.Combine(MainClass.Paths.ResDir, "moscrif.png"));
			this.SetSizeRequest(600, image.Height - 1);
		}

		private void DrawImage()
		{
			if (image == null)
				return;
			
			int w, h;
			this.GdkWindow.GetSize(out w, out h);
			this.GdkWindow.DrawPixbuf(backGc, image, 0, 0, (w - image.Width) / 2, 0, -1, -1, RgbDither.Normal, 0,
			0);
			
		}


		protected override bool OnExposeEvent(Gdk.EventExpose evnt)
		{
			int w, h;
			this.GdkWindow.GetSize(out w, out h);
			this.DrawImage();
			return false;
		}

		protected void OnRealized(object o, EventArgs args)
		{
			int x, y;
			int w, h;
			GdkWindow.GetOrigin(out x, out y);
			GdkWindow.GetSize(out w, out h);
			
			
			backGc = new Gdk.GC(GdkWindow);
			backGc.RgbBgColor = new Gdk.Color(49, 49, 74);
		}

		protected override void OnDestroyed()
		{
			base.OnDestroyed();
			backGc.Dispose();
		}
		
	}

	internal class CommonAboutDialog : Dialog
	{
		ScrollBox aboutPictureScrollBox;

		public CommonAboutDialog()
		{
			Title = MainClass.Languages.Translate("moscrif_ide_title_f1");
			TransientFor = MainClass.MainWindow;
			AllowGrow = false;
			HasSeparator = false;
			Modal = true;
			
			VBox.BorderWidth = 0;
			
			aboutPictureScrollBox = new ScrollBox();
			
			VBox.PackStart(aboutPictureScrollBox, false, false, 0);
			
			Notebook notebook = new Notebook();
			notebook.BorderWidth = 6;
			notebook.AppendPage(new AboutTabPage(), new Label(Title));
			notebook.AppendPage(new VersionInformationTabPage(), new Label(MainClass.Languages.Translate("components")));
			var buildInfo = LoadBuildInfo();
			if (buildInfo != null)
				notebook.AppendPage(buildInfo, new Label(MainClass.Languages.Translate("components")));
			VBox.PackStart(notebook, true, true, 4);
			
			AddButton (Stock.Close, ResponseType.Close);
			
			//ShowAll();
		}

		public new int Run()
		{
			ShowAll();
			int tmp = base.Run();
			Destroy();
			return tmp;
		}

		Widget LoadBuildInfo()
		{
			var biFile = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(GetType().Assembly.Location), "buildinfo");
			if (!File.Exists(biFile)) {
				
				return null;
			}
			
			try {
				var buf = new TextBuffer(null);
				buf.Text = File.ReadAllText(biFile);
				
				return new ScrolledWindow { BorderWidth = 6, ShadowType = ShadowType.EtchedIn, Child = new TextView(buf) { Editable = false, LeftMargin = 4, RightMargin = 4, PixelsAboveLines = 4, PixelsBelowLines = 4 } };
			} catch {//(IOException ex) {
				
				return null;
			}
		}
	}
}
