using System;
using Gtk;
using System.Reflection;


namespace Moscrif.IDE.Actions
{
	public class QuitAction : Gtk.Action
	{			
		public QuitAction():base("quit",MainClass.Languages.Translate("menu_quit"),MainClass.Languages.Translate("menu_title_quit"),"quit.png"){
						
		}

		protected override void OnActivated ()
		{
			base.OnActivated ();
			if(MainClass.MainWindow.InicializeQuit())
				MainClass.MainWindow.QuitApplication();

			//Application.Quit ();
		}		
	}
}

