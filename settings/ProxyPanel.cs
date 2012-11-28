using System;
using Gtk;

namespace Moscrif.IDE.Settings
{

	internal class ProxyPanel : OptionsPanel
	{
		ProxyWidget widget;
		
		public override Widget CreatePanelWidget()
		{
			return widget = new ProxyWidget();
		}
		
		public override void ApplyChanges()
		{
			widget.Store();
		}
		
		public override bool ValidateChanges()
		{
			return true;
		}
		
		public override void Initialize(PreferencesDialog dialog, object dataObject)
		{
			base.Initialize(dialog, dataObject);
		}
		
		public override string Label {
			get { return MainClass.Languages.Translate("proxy"); }
		}
		
		public override string Icon {
			get { return "compile.png"; }
		}
		
	}

	public partial class ProxyWidget : Gtk.Bin
	{
		public ProxyWidget()
		{
			this.Build();
			this.ShowAll();
		}
		public void Store()
		{


		}

	}
}

