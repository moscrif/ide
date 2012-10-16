using System;
using Gtk;
using System.Reflection;
using Moscrif.IDE.Components;
using Moscrif.IDE.Controls;

namespace Moscrif.IDE.Actions
{
	public class ShowRightPaneAction: Gtk.ToggleAction
	{
		private bool ignoreEvent = false;
		public ShowRightPaneAction():base("showrightpane",MainClass.Languages.Translate("menu_show_right_pane"),MainClass.Languages.Translate("menu_title_show_right_pane"),"workspace-right.png"){
			ignoreEvent = true;
			//this.Sensitive = false;
			this.Active =MainClass.Workspace.ShowRightPane;
			ignoreEvent = false;
		}

		protected override void OnToggled ()
		{
			if(!ignoreEvent){
				base.OnToggled ();
	
				MainClass.Workspace.ShowRightPane = this.Active;
				MainClass.MainWindow.ReloadPanel();
			}
		}
	}
}
