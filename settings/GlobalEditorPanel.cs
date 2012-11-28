using System;
using Gtk;
using Moscrif.IDE.Components;

namespace Moscrif.IDE.Option
{

	internal class GlobalEditorPanel : OptionsPanel
	{
		GlobalEditorWidget widget;

		public override Widget CreatePanelWidget()
		{
			return widget = new GlobalEditorWidget();
		}

		public override void ApplyChanges()
		{
			widget.Store();
		}

		public override void ShowPanel()
		{
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
			get { return MainClass.Languages.Translate("text_editor"); }
		}

		public override string Icon {
			get { return "editor-text.png"; }
		}

	}

	public partial class GlobalEditorWidget : Gtk.Bin
	{
		public GlobalEditorWidget()
		{
			this.Build();
					if(MainClass.Settings.SourceEditorSettings == null){
				MainClass.Settings.SourceEditorSettings = new  Moscrif.IDE.Option.Settings.SourceEditorSetting();
			}

			if (!String.IsNullOrEmpty(MainClass.Settings.SourceEditorSettings.EditorFont))
				fontbutton1.FontName = MainClass.Settings.SourceEditorSettings.EditorFont;

			chbShowTabs.Active = MainClass.Settings.SourceEditorSettings.ShowTab;
			chbShowSpaces.Active = MainClass.Settings.SourceEditorSettings.ShowSpaces;
			chbShowRuler.Active= MainClass.Settings.SourceEditorSettings.ShowRuler;

			spTabSpace.Value = MainClass.Settings.SourceEditorSettings.TabSpace;
			chbTabsToSpace.Active= MainClass.Settings.SourceEditorSettings.TabsToSpaces;
			chbShowEolMarker.Active = MainClass.Settings.SourceEditorSettings.ShowEolMarker;
			chbEnableAnimations.Active = MainClass.Settings.SourceEditorSettings.EnableAnimations;
			chbShowLineNumber.Active = MainClass.Settings.SourceEditorSettings.ShowLineNumber;
			spRulerColumn.Value = MainClass.Settings.SourceEditorSettings.RulerColumn;

			chbAgressivelyTriggerCL.Active = MainClass.Settings.SourceEditorSettings.AggressivelyTriggerCL ;
		}


		public void Store()
		{
			MainClass.Settings.SourceEditorSettings.EditorFont = fontbutton1.FontName;
			MainClass.Settings.SourceEditorSettings.TabSpace = (int)spTabSpace.Value;
			MainClass.Settings.SourceEditorSettings.TabsToSpaces = chbTabsToSpace.Active;

			MainClass.Settings.SourceEditorSettings.ShowLineNumber= chbShowLineNumber.Active;
			MainClass.Settings.SourceEditorSettings.ShowEolMarker = chbShowEolMarker.Active;
			MainClass.Settings.SourceEditorSettings.ShowTab = chbShowTabs.Active;
			MainClass.Settings.SourceEditorSettings.ShowSpaces = chbShowSpaces.Active;
			MainClass.Settings.SourceEditorSettings.ShowRuler = chbShowRuler.Active;
			MainClass.Settings.SourceEditorSettings.RulerColumn = (int)spRulerColumn.Value;


			MainClass.Settings.SourceEditorSettings.AggressivelyTriggerCL = chbAgressivelyTriggerCL.Active;

			/*MainClass.Settings.BackgroundColor.Blue = (byte)sbBlue.Value;
			MainClass.Settings.BackgroundColor.Red = (byte)sbRed.Value;
			MainClass.Settings.BackgroundColor.Green = (byte)sbGreen.Value;

			MainClass.Settings.PreCompile = chbPrecompile.Active;

			MainClass.Settings.EditorFont = fontbutton1.FontName;*/
		}
	}
}


