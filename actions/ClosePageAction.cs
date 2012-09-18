using Gtk;

namespace Moscrif.IDE.Actions
{
	public class ClosePageAction : Action
	{
		public ClosePageAction() : base("close", MainClass.Languages.Translate("menu_close"), MainClass.Languages.Translate("menu_title_close"), "close.png")
		{
		}

		protected override void OnActivated()
		{
			base.OnActivated();
			MainClass.MainWindow.EditorNotebook.CloseCurentPage();
		}
	}
}

