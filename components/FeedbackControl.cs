using System;
using System.IO;
using Gtk;
using Gdk;

namespace Moscrif.IDE.Components
{
	public class FeedbackControl: Gtk.Table
	{
		private Entry entrSubject;
		private TextView tvDescription;
		private ComboBox cbType;
		private ListStore projectModel;

		public FeedbackControl() : base(3,2,false)
		{
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
	}
}

