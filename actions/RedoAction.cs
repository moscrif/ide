using System.IO;
using Gtk;
namespace Moscrif.IDE.Actions
{
	public class RedoAction : Action
	{
		public RedoAction()  : base("redo",  MainClass.Languages.Translate("menu_redo"),  MainClass.Languages.Translate("menu_title_redo"), "redo.png")
		{
		}

		protected override void OnActivated()
		{

			base.OnActivated();

			MainClass.MainWindow.EditorNotebook.Redo();

		}
	}
}

