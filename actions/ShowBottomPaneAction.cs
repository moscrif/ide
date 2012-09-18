using System;
using Gtk;
using System.Reflection;
using Moscrif.IDE.Components;
using Moscrif.IDE.Controls;

namespace Moscrif.IDE.Actions
{
	public class ShowBottomPaneAction: Gtk.ToggleAction
	{
		private bool ignoreEvent = false;
		public ShowBottomPaneAction():base("showbottonpane",MainClass.Languages.Translate("menu_show_bottom_pane"),MainClass.Languages.Translate("menu_title_show_bottom_pane"),"workspace-bottom.png"){
			ignoreEvent = true;
			this.Active =MainClass.Workspace.ShowBottomPane;
			ignoreEvent = false;
		}

		protected override void OnToggled ()
		{
			if(!ignoreEvent){
				base.OnToggled ();
	
				MainClass.Workspace.ShowBottomPane = this.Active;
				if(!MainClass.Workspace.ShowBottomPane){
					MainClass.MainWindow.SetBodyParameter();
				}

				MainClass.MainWindow.ReloadPanel();
			}
		}
	}
}

