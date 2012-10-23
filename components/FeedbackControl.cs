using System;
using System.IO;
using Gtk;
using Gdk;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using Moscrif.IDE.Iface.Entities;

namespace Moscrif.IDE.Components
{
	public class FeedbackControl: Gtk.Table
	{
		private Entry entrSubject;
		private TextView tvDescription;
		private ComboBox cbType;
		private ListStore projectModel;
		private TypFeedback typeFeedback;

		public FeedbackControl(TypFeedback type) : base(3,2,false)
		{
			typeFeedback = type;
			this.RowSpacing = 3;
			this.ColumnSpacing = 3;
			Label lblSubject =  GetLabel("Subject");
			this.Attach(lblSubject,0,1,0,1,AttachOptions.Fill,AttachOptions.Fill,0,0);

			entrSubject  = new Entry();
			this.Attach(entrSubject,1,2,0,1,AttachOptions.Fill|AttachOptions.Expand,AttachOptions.Fill,0,0);

			cbType = new ComboBox();
			projectModel = new ListStore(typeof(string), typeof(string));
			CellRendererText textRenderer = new CellRendererText();
			cbType.PackStart(textRenderer, true);
			cbType.AddAttribute(textRenderer, "text", 0);
			cbType.Model= projectModel;
			projectModel.AppendValues("IDE and Emulator","IDE and Emulator" );
			projectModel.AppendValues("Framework and Documentation","Framework and Documentation" );
			projectModel.AppendValues("Deployment, Devices and Publishing","Deployment, Devices and Publishing" );
			projectModel.AppendValues("Web moscrif.com","Web moscrif.com" );
			cbType.Active = 0;

			Label lblVersion =  GetLabel("Product Group");
			this.Attach(lblVersion,0,1,1,2,AttachOptions.Fill,AttachOptions.Fill,0,0);
			this.Attach(cbType,1,2,1,2,AttachOptions.Fill|AttachOptions.Expand,AttachOptions.Fill,0,0);

			Label lblDescription =  GetLabel("Description");
			this.Attach(lblDescription,0,1,2,3,AttachOptions.Fill,AttachOptions.Fill,0,0);

			tvDescription = new TextView();
			ScrolledWindow sw = new ScrolledWindow ();
			sw.ShadowType = ShadowType.Out;
			sw.Add(tvDescription);

			this.Attach(sw,1,2,2,3,AttachOptions.Fill|AttachOptions.Expand,AttachOptions.Fill|AttachOptions.Expand,0,0);

			this.ShowAll();
		}

		public string GetDescription(){
			return tvDescription.Buffer.Text;
		}

		private Label GetLabel(string text){
			Label label = new Label(text);
			label.Xpad = 5;
			label.Ypad = 5;
			label.UseUnderline = false;
			label.Selectable = false;
			label.Xalign = 0;
			return label;
		}

		public string GetData(){
			FeedbackData fd = new FeedbackData ();
			fd.Typ = (int)typeFeedback;
			fd.Subject = entrSubject.Text;
			fd.Product = cbType.ActiveText;

			if(MainClass.Platform.IsWindows){
				fd.System = "Windows 7 32bit";
			} else if(MainClass.Platform.IsMac) {
				fd.System = "Mac OS X 10.7 (Lion)";
			} else if(MainClass.Platform.IsX11) {
				fd.System = "X11";
			} else {
				fd.System = "Unknow";
			}

			string version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
			string newVersion = MainClass.Tools.VersionConverter(version);

			fd.Version = newVersion; //"2012q3a";

			fd.Description =tvDescription.Buffer.Text; 
			
			XmlSerializer x_serial = new XmlSerializer( fd.GetType());
			StringWriter textWriter = new StringWriter();
			
			x_serial.Serialize(textWriter, fd);
			return textWriter.ToString();
		}
		public enum TypFeedback{
			Issue = 2,
			Question = 3,
			FAQ = 4,
			Suggestion = 5
		}


		/*  Issue = 2,
  Question = 3,
  FAQ = 4,
  Suggestion = 5*/
	}
}

