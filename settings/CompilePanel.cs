using System;
using Gtk;

namespace Moscrif.IDE.Option
{
	internal class CompilePanel : OptionsPanel
	{
		CompileWidget widget;

		public override Widget CreatePanelWidget()
		{
			return widget = new CompileWidget();
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
			get { return MainClass.Languages.Translate("compilation"); }
		}

		public override string Icon {
			get { return "compile.png"; }
		}

	}


	public partial class CompileWidget : Gtk.Bin
	{
		public CompileWidget()
		{
			this.Build();
			chbPrecompile.Active = MainClass.Settings.PreCompile;
			chbFirstError.Active = MainClass.Settings.FirstErrorStopCompile;
			chbShowError.Active = MainClass.Settings.ShowErrorPane;
			chbClearConsole.Active =MainClass.Settings.ClearConsoleBeforRuning;
			chbOpenOutput.Active =MainClass.Settings.OpenOutputAfterPublish;
			chbSaveAfterRun.Active =MainClass.Settings.SaveChangesBeforeRun;
		}

		public void Store()
		{
			MainClass.Settings.PreCompile = chbPrecompile.Active;
			MainClass.Settings.FirstErrorStopCompile = chbFirstError.Active;
			MainClass.Settings.ShowErrorPane = chbShowError.Active;
			MainClass.Settings.ClearConsoleBeforRuning =chbClearConsole.Active;
			MainClass.Settings.OpenOutputAfterPublish=chbOpenOutput.Active;

			MainClass.Settings.SaveChangesBeforeRun = chbSaveAfterRun.Active;

			MainClass.MainWindow.SaveWorkspace();
		}
	}
}

