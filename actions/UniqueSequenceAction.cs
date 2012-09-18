using System;
using System.Collections.Generic;
using Gtk;
using Moscrif.IDE.Controls;
//using controls;

namespace  Moscrif.IDE.Actions
{
	public class UniqueSequenceAction  : Gtk.Action
	{
		public UniqueSequenceAction():base("unicodesequence",MainClass.Languages.Translate("menu_unicode_sequence"),MainClass.Languages.Translate("menu_title_unicode_sequence"),null)
		{
		}

		protected override void OnActivated ()
		{
			base.OnActivated();

			UniqueSequenceDialog pd = new UniqueSequenceDialog();
			int result = pd.Run();
			if (result == (int)ResponseType.Ok) {

				//string fileName = nfd.FileName;
				//MainClass.MainWindow.CreateFile(fileName);
			}
			pd.Destroy();
		}
	}
}

