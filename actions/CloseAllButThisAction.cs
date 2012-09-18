using Gtk;

namespace Moscrif.IDE.Actions
{
	public class CloseAllButThisAction : Action
	{
		public CloseAllButThisAction() : base("closeallbutthis", MainClass.Languages.Translate("menu_close_all_but"),MainClass.Languages.Translate("menu_title_close_all_but"), null)
		{
		}

		protected override void OnActivated()
		{
			base.OnActivated();
			MainClass.MainWindow.EditorNotebook.CloseAllButThisPage();
		}
	}
}

