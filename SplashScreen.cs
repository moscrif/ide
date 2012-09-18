using System;
using System.IO;
using System.Reflection;
using System.Collections;
using Gtk;
using Moscrif.IDE.Iface;
using Moscrif.IDE.Iface.Entities;

namespace Moscrif.IDE
{
	public class SplashScreenForm : Gtk.Window, IDisposable
	{
		static SplashScreenForm splashScreen;
		static ProgressBar progress;
		static VBox vbox;
		static ColorButton cbBackground;
		static ComboBox cbKeyBinding;
		static Button btnOk;
	//	ProgressTracker tracker = new ProgressTracker();
		Gdk.Pixbuf bitmap;
		static Gtk.Label label;

		Gtk.Menu popupColor = new Gtk.Menu();
		const string NOTHING ="Nothing";
		const string WIN ="Visual Studio";
		const string MACOSX ="XCode";
		const string JAVA ="Eclipse";
		const string VisualC ="Visual C++";


		public static SplashScreenForm SplashScreen
		{
			get {
				if (splashScreen == null)
					splashScreen = new SplashScreenForm(true);
				return splashScreen;
			}
		}

		public SplashScreenForm(bool showSetting) : base(Gtk.WindowType.Toplevel)
		{
			Console.WriteLine("splash.bild.start-{0}",DateTime.Now);
			waitingSplash =showSetting;

			AppPaintable = true;
			this.Decorated = false;
			this.WindowPosition = WindowPosition.Center;
			this.TypeHint = Gdk.WindowTypeHint.Splashscreen;
			try {
				bitmap = new Gdk.Pixbuf(System.IO.Path.Combine( MainClass.Paths.ResDir, "moscrif.png"));
			} catch (Exception ex) {
				Tool.Logger.Error(ex.Message);
				Tool.Logger.Error("Can't load splash screen pixbuf 'moscrif.png'.");
			}
			progress = new ProgressBar();
			progress.Fraction = 0.00;
			progress.HeightRequest = 6;
			
			vbox = new VBox();
			vbox.BorderWidth = 12;
			label = new Gtk.Label();
			label.UseMarkup = true;
			label.Xalign = 0;
			//vbox.PackEnd(progress, false, true, 0);

			if(showSetting){

				Table table= new Table(3,3,false);
	
				Label lbl1 = new Label("Color Scheme :");
				Label lbl2 = new Label("Keybinding :");

				table.Attach(lbl1,0,1,0,1,AttachOptions.Shrink,AttachOptions.Shrink,0,0);
				table.Attach(lbl2,0,1,1,2,AttachOptions.Shrink,AttachOptions.Shrink,0,0);
	
				cbBackground = new ColorButton();
				table.Attach(cbBackground,1,2,0,1,AttachOptions.Fill,AttachOptions.Shrink,0,0);

				cbKeyBinding = Gtk.ComboBox.NewText ();//new ComboBox();
				cbKeyBinding.Name="cbKeyBinding";

				if(MainClass.Settings.BackgroundColor==null){
					MainClass.Settings.BackgroundColor = new Moscrif.IDE.Settings.Settings.BackgroundColors(218,218,218);
					/*if(MainClass.Platform.IsMac)
						MainClass.Settings.BackgroundColor = new Moscrif.IDE.Settings.Settings.BackgroundColors(218,218,218);
					else
						MainClass.Settings.BackgroundColor = new Moscrif.IDE.Settings.Settings.BackgroundColors(224,41,47);
						*/
				}

				cbKeyBinding.AppendText(WIN);
				cbKeyBinding.AppendText(MACOSX);
				cbKeyBinding.AppendText(JAVA);
				cbKeyBinding.AppendText(VisualC);

				if(MainClass.Platform.IsMac){
					cbKeyBinding.Active = 1;
				} else {
					cbKeyBinding.Active = 0;
				}
	
				Gdk.Pixbuf default_pixbuf = null;
				string file = System.IO.Path.Combine(MainClass.Paths.ResDir, "stock-menu.png");
				//if (System.IO.File.Exists(file)) {

					try {
						default_pixbuf = new Gdk.Pixbuf(file);
					} catch (Exception ex) {
						Tool.Logger.Error(ex.Message);
					}

	
					popupColor = new Gtk.Menu();
					CreateMenu();
	
					Gtk.Button btnClose = new Gtk.Button(new Gtk.Image(default_pixbuf));
					btnClose.TooltipText = MainClass.Languages.Translate("select_color");
					btnClose.Relief = Gtk.ReliefStyle.None;
					btnClose.CanFocus = false;
					btnClose.WidthRequest = btnClose.HeightRequest = 20;
	
					popupColor.AttachToWidget(btnClose,new Gtk.MenuDetachFunc(DetachWidget));
					btnClose.Clicked += delegate {
						popupColor.Popup(null,null, new Gtk.MenuPositionFunc (GetPosition) ,3,Gtk.Global.CurrentEventTime);
					};
					table.Attach(btnClose,2,3,0,1, AttachOptions.Shrink, AttachOptions.Shrink, 0, 0);
					popupColor.ShowAll();
				//}
	
				cbBackground.Color = new Gdk.Color(MainClass.Settings.BackgroundColor.Red,
							MainClass.Settings.BackgroundColor.Green,MainClass.Settings.BackgroundColor.Blue);
	
	
				table.Attach(cbKeyBinding,1,2,1,2,AttachOptions.Fill,AttachOptions.Shrink,0,0);
	
				btnOk = new Gtk.Button();
				btnOk.Label = "_Ok";
				btnOk.UseUnderline = true;
				btnOk.Clicked+=  OnButtonOkClicked;
	
				table.Attach(btnOk,0,1,2,3,AttachOptions.Fill,AttachOptions.Shrink,0,0);
	
				vbox.PackEnd(table, false, true, 3);
			}
			vbox.PackEnd(label, false, true, 3);
			this.Add(vbox);

			if (bitmap != null)
				this.Resize(bitmap.Width, bitmap.Height);
			Console.WriteLine("splash.bild.end-{0}",DateTime.Now);
			this.ShowAll();
		}

		protected void OnButtonOkClicked (object sender, System.EventArgs e)
		{

			MainClass.Settings.BackgroundColor.Red = (byte)cbBackground.Color.Red;
			MainClass.Settings.BackgroundColor.Green= (byte)cbBackground.Color.Green;
			MainClass.Settings.BackgroundColor.Blue= (byte)cbBackground.Color.Blue;

			string active = cbKeyBinding.ActiveText;
			string file = System.IO.Path.Combine(MainClass.Paths.SettingDir, "keybinding");

			switch (active) {
			case WIN:{
				KeyBindings.CreateKeyBindingsWin(file);
				break;
			}
			case MACOSX:{
				KeyBindings.CreateKeyBindingsMac(file);
				break;
			}
			case JAVA:{
				KeyBindings.CreateKeyBindingsJava(file);
				break;
			}
			case VisualC:{
				KeyBindings.CreateKeyBindingsVisualC(file);
				break;
			}
			default:
				break;
			}

			MainClass.Settings.SaveSettings();
			waitingSplash =false;

		}

		private void DetachWidget(Gtk.Widget attach_widget, Gtk.Menu menu){}
		static void GetPosition(Gtk.Menu menu, out int x, out int y, out bool push_in){

			menu.AttachWidget.GdkWindow.GetOrigin(out x, out y);
			//Console.WriteLine("GetOrigin -->>> x->{0} ; y->{1}",x,y);

			x =menu.AttachWidget.Allocation.X+x;//+menu.AttachWidget.WidthRequest;
			y =menu.AttachWidget.Allocation.Y+y+menu.AttachWidget.HeightRequest;

			push_in = true;
		}

		private void CreateMenu(){

			Gtk.MenuItem miRed = new Gtk.MenuItem("Red");
			miRed.Activated += delegate(object sender, EventArgs e) {
					if (sender.GetType() == typeof(Gtk.MenuItem)){
						cbBackground.Color =  new Gdk.Color(224, 41, 47);
					}
				};
			popupColor.Add(miRed);
			Gtk.MenuItem miBlue = new Gtk.MenuItem("Blue");
			miBlue.Activated += delegate(object sender, EventArgs e) {
					if (sender.GetType() == typeof(Gtk.MenuItem)){
						cbBackground.Color =  new Gdk.Color(164, 192,222);
					}
				};
			popupColor.Add(miBlue);
			Gtk.MenuItem miUbuntu = new Gtk.MenuItem("Ubuntu");
			miUbuntu.Activated += delegate(object sender, EventArgs e) {
					if (sender.GetType() == typeof(Gtk.MenuItem)){
						cbBackground.Color =  new Gdk.Color(240, 119,70);
					}
				};
			popupColor.Add(miUbuntu);

			Gtk.MenuItem miOsx = new Gtk.MenuItem("Mac");
			miOsx.Activated += delegate(object sender, EventArgs e) {
					if (sender.GetType() == typeof(Gtk.MenuItem)){
						cbBackground.Color =  new Gdk.Color(218,218 ,218);
					}
				};
			popupColor.Add(miOsx);
		}


		protected override void OnDestroyed()
		{
			base.OnDestroyed();
			if (bitmap != null) {
				bitmap.Dispose();
				bitmap = null;
			}
		}

		protected override bool OnExposeEvent(Gdk.EventExpose evt)
		{
			if (bitmap != null) {
				Gdk.GC gc = Style.LightGC(StateType.Normal);
				GdkWindow.DrawPixbuf(gc, bitmap, 0, 0, 0, 0, bitmap.Width, bitmap.Height, Gdk.RgbDither.None, 0,
				0);
				
				using (Pango.Layout pl = new Pango.Layout(PangoContext)) {
					Pango.FontDescription des = this.Style.FontDescription.Copy();
					pl.FontDescription = des;
					//pl.SetMarkup("<b><span foreground='#cccccc'>" + BuildVariables.PackageVersionLabel + "</span></b>");
					int w, h;
					pl.GetPixelSize(out w, out h);
					GdkWindow.DrawLayout(gc, bitmap.Width - w - 75, 90, pl);
					des.Dispose();
				}
			}
			return base.OnExposeEvent(evt);
		}

		private bool waitingSplash = false;
		public bool WaitingSplash {
			get {
				return waitingSplash;
			}
		}


		public static void SetProgress(double Percentage)
		{
			progress.Fraction = Percentage;
			RunMainLoop();
		}

		public void SetMessage(string Message)
		{
			if (bitmap == null) {
				label.Text = Message;
			} else {
				label.Markup = "<span size='small' foreground='white'>" + Message + "</span>";
			}
			RunMainLoop();
		}

		static void RunMainLoop()
		{
			//DispatchService.RunPendingEvents();
		}

		/*void IProgressMonitor.BeginTask(string name, int totalWork)
		{
			tracker.BeginTask(name, totalWork);
			SetMessage(tracker.CurrentTask);
		}

		void IProgressMonitor.BeginStepTask(string name, int totalWork, int stepSize)
		{
			tracker.BeginStepTask(name, totalWork, stepSize);
			SetMessage(tracker.CurrentTask);
		}

		void IProgressMonitor.EndTask()
		{
			tracker.EndTask();
			SetProgress(tracker.GlobalWork);
			SetMessage(tracker.CurrentTask);
		}

		void IProgressMonitor.Step(int work)
		{
			tracker.Step(work);
			SetProgress(tracker.GlobalWork);
		}

		TextWriter IProgressMonitor.Log
		{
			get { return Console.Out; }
		}

		void IProgressMonitor.ReportWarning(string message)
		{
		}

		void IProgressMonitor.ReportSuccess(string message)
		{
		}

		void IProgressMonitor.ReportError(string message, Exception exception)
		{
		}

		bool IProgressMonitor.IsCancelRequested
		{
			get { return false; }
		}
				 */
		/*public event MonitorHandler CancelRequested
		{
			add { }
			remove { }
		}*/

		// The returned IAsyncOperation object must be thread safe
		/*IAsyncOperation IProgressMonitor.AsyncOperation
		{
			get { return null; }
		}*/

	/*	object IProgressMonitor.SyncRoot
		{
			get { return this; }
		}*/

		void IDisposable.Dispose()
		{
			Destroy();
			splashScreen = null;
		}
	}
}

