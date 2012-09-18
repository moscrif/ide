using System;
using Gtk;
namespace Moscrif.IDE.Actions
{
	public class FindAction : Gtk.Action
	{
		public FindAction(): base("find", MainClass.Languages.Translate("menu_find"),MainClass.Languages.Translate("menu_title_find"), "find.png")
		{
		}

		protected override void OnActivated ()
		{
			base.OnActivated ();

			MainClass.MainWindow.SetSearch();

		}
	}
}

