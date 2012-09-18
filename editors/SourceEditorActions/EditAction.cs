using System;
using Mono.TextEditor;
using Moscrif.IDE.Components;

namespace  Moscrif.IDE.Editors.SourceEditorActions
{
	internal class EditAction
	{
		private readonly TextEdit editor;

		public EditAction(TextEdit editor)
		{
			this.editor = editor;
		}

		public void CommentUncomment (object obj, EventArgs args)
		{
			//int posCaret = this.editor.Caret.Offset;
			//Console.WriteLine("--------------");
			bool allComent = true;

			foreach(LineSegment ls in this.editor.SelectedLines){
				//Console.WriteLine(ls.Offset);
				//Console.WriteLine("{0}-{1}",ls.Offset,ls.EndOffset);

				string text = editor.Document.GetTextBetween(ls.Offset,ls.EndOffset);

				if(!text.Trim().StartsWith("//"))
					allComent = false;
			}

			if(!allComent){ // nezacinaju vsetky // tak ich vsade pridam
				foreach(LineSegment ls in this.editor.SelectedLines){
					editor.Insert(ls.Offset,"//");
				}
			} else { // zacinaju vsetky // tak ich odoberem

				foreach(LineSegment ls in this.editor.SelectedLines){
					string text = editor.Document.GetTextBetween(ls.Offset,ls.EndOffset);
					int indx =  text.IndexOf("//");
					if(indx>-1)
						editor.Remove(ls.Offset+indx,2);
				}
			}
			editor.QueueDraw();
		}

		public void GoToDefinition (object obj, EventArgs args)
		{
			//int posCaret = this.editor.Caret.Offset;

			//int startWord =FindPrevWordOffset(posCaret);
			//int endtWord =FindnextWordOffset(posCaret);

			string  activeWord = this.editor.GetCarretWord();

			string linkUrl = String.Format("http://moscrif.com/api?class={0}",activeWord);
			if (!String.IsNullOrEmpty(linkUrl)){
				System.Diagnostics.Process.Start(linkUrl);
			}

		}

		public void CopyText(object obj, EventArgs args)
		{
			//ClipboardActions ca = new ClipboardActions();
			//ClipboardActions.Copy(this.editor.GetTextEditorData());

			//this.editor.Document.
			if(this.editor== null) return;
			if(String.IsNullOrEmpty(this.editor.SelectedText)) return;

			Gtk.Clipboard clipboard = this.editor.GetClipboard(Gdk.Selection.Clipboard);
			clipboard.Text=this.editor.SelectedText;  // RequestText (new Gtk.ClipboardTextReceivedFunc (PasteReceived));

		}

		public void CutText(object obj, EventArgs args)
		{
			ClipboardActions.Cut(this.editor.GetTextEditorData());

		}

		public void PasteText(object obj, EventArgs args)
		{
			Gtk.Clipboard clipboard = this.editor.GetClipboard(Gdk.Selection.Clipboard);
			clipboard.RequestText (new Gtk.ClipboardTextReceivedFunc (PasteReceived));

		}
		void PasteReceived (Gtk.Clipboard clipboard, string text)
		{
			ISegment sg = this.editor.SelectionRange;

			if((sg != null) && sg.Length>0){

				this.editor.Replace(sg.Offset, sg.Length,text);
				int newPositionCaret = sg.Offset + text.Length;

				DocumentLocation dl =this.editor.Document.OffsetToLocation(newPositionCaret);

				Caret caret = this.editor.Caret;
				caret.Location = dl;

				//editor.ScrollToCaret();

			}else{
				this.editor.InsertAtCaret(text);
			};
		}


	}
}

