using System;
using Gtk;
using Moscrif.IDE.Workspace;
using Moscrif.IDE.Components;

namespace Moscrif.IDE.Settings
{

	internal class TestOptionsPanel : OptionsPanel
	{
		TestOptionsWidget widget;
		Workspace.Workspace workspace;

		public override Widget CreatePanelWidget()
		{
			return widget = new TestOptionsWidget(workspace.Tag);
		}

		public override void ShowPanel()
		{
		}

		public override void ApplyChanges()
		{
			workspace.Tag = widget.GetTagWorkspace();
			MainClass.MainWindow.SaveWorkspace();
			
			widget.Store();
		}

		public override bool ValidateChanges()
		{
			return true;
		}

		public override void Initialize(PreferencesDialog dialog, object dataObject)
		{
			workspace = dataObject as Workspace.Workspace;
			base.Initialize(dialog, dataObject);
		}

		public override string Label {
			get { return "Test"; }
		}

	}



	internal partial class TestOptionsWidget : Gtk.Bin
	{
		public TestOptionsWidget(int tagWorkspace)
		{
			this.Build();
			if (MainClass.Settings.BackgroundColor != null) {
				sbRed.Value = MainClass.Settings.BackgroundColor.Red;
				sbGreen.Value = MainClass.Settings.BackgroundColor.Green;
				sbBlue.Value = MainClass.Settings.BackgroundColor.Blue;
			}

			
			spinbutton4.Value = tagWorkspace;
			
			
			FileEntry fe = new FileEntry();
			//("a",true);
			fe.Name = "a";
			fe.IsFolder = true;
			
			table2.Attach(fe, 0, 1, 6, 7, AttachOptions.Shrink, AttachOptions.Shrink, 0, 0);
		}

		public void Store()
		{
			//MainClass.Settings.BackgroundColor.Blue = (byte)sbBlue.Value;
			//MainClass.Settings.BackgroundColor.Red = (byte)sbRed.Value;
			//MainClass.Settings.BackgroundColor.Green = (byte)sbGreen.Value;

		}
		public int GetTagWorkspace()
		{
			
			return (int)spinbutton4.Value;
			
		}
	}
}

