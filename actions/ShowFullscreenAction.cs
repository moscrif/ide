using System;
using Gtk;
using System.Reflection;
using Moscrif.IDE.Components;
using Moscrif.IDE.Controls;

namespace Moscrif.IDE.Actions
{
	public class ShowFullscreenAction : Gtk.ToggleAction
	{
		public ShowFullscreenAction():base("showfullscreen",MainClass.Languages.Translate("menu_fullscreen"),MainClass.Languages.Translate("menu_title_fullscreen"),null){

		}

		protected override void OnToggled ()
		{
			if(this.Active){
				MainClass.MainWindow.Fullscreen();
			} else{
				MainClass.MainWindow.Unfullscreen();
			} 
		}

	}
}

