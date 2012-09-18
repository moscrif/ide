using System;
using Gtk;
using System.Reflection;
using Moscrif.IDE.Components;
using Moscrif.IDE.Controls;

namespace Moscrif.IDE.Actions
{
	public class AboutAction : Gtk.Action
	{			
		public AboutAction():base("about",MainClass.Languages.Translate("menu_about"),MainClass.Languages.Translate("menu_title_about"),"about.png"){
						
		}

		protected override void OnActivated ()
		{
			base.OnActivated ();
			
			CommonAboutDialog cad = new CommonAboutDialog();
			cad.Run();

		}
		
		
	}
}

