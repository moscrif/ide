using Gtk;

namespace Moscrif.IDE.Actions
{
	public class CloseAllPageAction : Action
	{
		public CloseAllPageAction() : base("closeall", MainClass.Languages.Translate("menu_close_all"), MainClass.Languages.Translate("menu_title_close_all"), "close.png")
		{
		}

		protected override void OnActivated()
		{
			base.OnActivated();
			MainClass.MainWindow.EditorNotebook.CloseAllPage();
		}
	}
}

