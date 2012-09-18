using System;
using Gtk;
using Moscrif.IDE.Controls;

namespace Moscrif.IDE.Actions
{
	public class SettingPlatformAction: Gtk.Action
	{
		public SettingPlatformAction():base("settingplatform",MainClass.Languages.Translate("menu_setting_platform"),MainClass.Languages.Translate("menu_title_setting_platform"),null){

		}

		protected override void OnActivated ()
		{
			base.OnActivated ();
			SettingPlatformDialog spd = new SettingPlatformDialog(false);
			int result = spd.Run();

			if (result == (int)ResponseType.Ok) {
				MainClass.MainWindow.ActionUiManager.ReloadKeyBind();
				MainClass.MainWindow.SetSettingColor();
			}
			spd.Destroy();
		}

	}
}

