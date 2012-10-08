using System;
using System.Text;

namespace Moscrif.IDE.Controls//controls
{
	public partial class UniqueSequenceDialog : Gtk.Dialog
	{
		protected virtual void OnTvInputKeyReleaseEvent(object o, Gtk.KeyReleaseEventArgs args)
		{
			tvOutput.Buffer.Text = GetUnicodeSequence(tvInput.Buffer.Text);
		}

		public UniqueSequenceDialog()
		{
			this.Build();
			this.TransientFor = MainClass.MainWindow;
			this.Title = MainClass.Languages.Translate("moscrif_ide_title_f1");
		}

		private string GetUnicodeSequence(string input)
		{
			if (String.IsNullOrEmpty(input))
				return String.Empty;
			Encoding encoding = System.Text.UnicodeEncoding.BigEndianUnicode;
			int charSizeInBytes = System.Text.UnicodeEncoding.CharSize;
			string output = "";
			byte[] bytes = encoding.GetBytes(input);
			//int counter = 0;
			for (int i = 0; i < bytes.Length;) {
				char ch = input[i / charSizeInBytes];
				if (ch <= 'z') {
					output += ch;
					i += charSizeInBytes;
				} else {
					output += "\\u";
					for (int siz = 0; siz < charSizeInBytes; siz++) {
						output += String.Format("{0:X2}", bytes[i]);
						i++;
					}
				}
				//counter++;
				//if (counter % charSizeInBytes == 0 &amp;&amp; counter < bytes.Length)
				//{
				// output += "\\u";
				//}
			}
			return output;
		}
		
		protected virtual void OnButtonOkClicked (object sender, System.EventArgs e)
		{
			//this.Respond(Gtk.ResponseType.Ok);
		}
		
		
	}
}

