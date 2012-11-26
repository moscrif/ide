using System;
using Gtk;
using System.Reflection;
using Moscrif.IDE.Components;
using Moscrif.IDE.Controls;

namespace Moscrif.IDE.Actions
{
	public class ShowLeftPaneAction : Gtk.ToggleAction
	{
		private bool ignoreEvent = false;
		public ShowLeftPaneAction():base("showleftpane",MainClass.Languages.Translate("menu_show_left_pane"),MainClass.Languages.Translate("menu_title_show_left_pane"),"workspace-left.png"){
			ignoreEvent = true;
			this.Active =MainClass.Workspace.ShowLeftPane;
			ignoreEvent = false;
		}

		protected override void OnToggled ()
		{
			if(!ignoreEvent){
				base.OnToggled ();

				MainClass.Workspace.ShowLeftPane = this.Active;
				MainClass.MainWindow.ReloadPanel();
			}
		}

	}
}

