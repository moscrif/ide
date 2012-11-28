using System;
using System.IO;
using Moscrif.IDE.Option;
using Gtk;

namespace  Moscrif.IDE.Actions
{
	public class IdePreferencesAction : Gtk.Action
	{
		public IdePreferencesAction(): base("idepreferences", MainClass.Languages.Translate("menu_preferences"), MainClass.Languages.Translate("menu_title_preferences"), "preferences.png")
		{
		}

		protected override void OnActivated ()
		{
			base.OnActivated();

			PreferencesDialog pd = new PreferencesDialog(TypPreferences.GlobalSetting,MainClass.Languages.Translate("settings"));
			int result = pd.Run();
			if (result == (int)ResponseType.Ok) {


				MainClass.Settings.SaveSettings();

				MainClass.MainWindow.ReloadSettings(true);
				//string fileName = nfd.FileName;
				//MainClass.MainWindow.CreateFile(fileName);
			}
			pd.Destroy();
		}
	}
}

