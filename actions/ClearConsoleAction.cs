using System;
namespace Moscrif.IDE.Actions
{
	public class ClearConsoleAction : Gtk.ToggleAction
	{
		public ClearConsoleAction()  : base("clear", MainClass.Languages.Translate("menu_title_clear_console"), MainClass.Languages.Translate("menu_title_clear_console"), null)
		{
	    		this.Active = MainClass.Settings.ClearConsoleBeforRuning;
		}

		protected override void OnActivated()
		{
			base.OnActivated();

			if(MainClass.Settings.ClearConsoleBeforRuning == this.Active)
				MainClass.Settings.ClearConsoleBeforRuning = this.Active;
		}
	}
}

