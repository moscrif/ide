using System.IO;
using Gtk;
namespace Moscrif.IDE.Actions
{
	public class UndoAction : Action
	{
		public UndoAction()  : base("undo", MainClass.Languages.Translate("menu_undo"), MainClass.Languages.Translate("menu_title_undo"), "undo.png")
		{
		}

		protected override void OnActivated()
		{
			base.OnActivated();

			MainClass.MainWindow.EditorNotebook.Undo();

		}

	}
}

