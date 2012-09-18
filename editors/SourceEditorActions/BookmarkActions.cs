using System;
using Mono.TextEditor;
using Moscrif.IDE.Iface.Entities;

namespace Moscrif.IDE.Editors.SourceEditorActions
{
	internal class BookmarkActions
	{
		private readonly TextEditor editor;
		private FileSetting fileSetting;

		public BookmarkActions(TextEditor editor, FileSetting fileSetting)
		{
			this.editor = editor;
			this.fileSetting = fileSetting;
		}

		public void ToggleBookmark(object obj, EventArgs args)
		{
			LineSegment ls = editor.Document.GetLine(editor.Caret.Line);
			if (ls != null) {
				int lineNumber = editor.Document.OffsetToLineNumber(ls.Offset);

				ls.IsBookmarked = !ls.IsBookmarked;
				if(ls.IsBookmarked){
					//fileSetting.Bookmarks.Add(lineNumber);
					string text = this.editor.Document.GetTextBetween(ls.Offset,ls.EndOffset);
					fileSetting.Bookmarks2.Add(new MyBookmark(lineNumber,text.Trim()));

				} else {
					MyBookmark b = fileSetting.Bookmarks2.Find(x=>x.Line == lineNumber);
					if(b!=null)
						fileSetting.Bookmarks2.Remove(b);
				}
				//Console.WriteLine("fileSetting.Bookmarks->{0}",fileSetting.Bookmarks.Count);
				editor.Document.RequestUpdate(new LineUpdate(lineNumber));
				editor.Document.CommitDocumentUpdate();
				MainClass.MainWindow.BookmarkOutput.RefreshBookmark();
			}
		}

		public void NextBookmark(object obj, EventArgs args)
		{
			Mono.TextEditor.BookmarkActions.GotoNext(editor.GetTextEditorData());
		}

		public void PreviousBookmark(object obj, EventArgs args)
		{
			Mono.TextEditor.BookmarkActions.GotoPrevious(editor.GetTextEditorData());
		}

		public void ClearBookmarks(object obj, EventArgs args)
		{
			Mono.TextEditor.BookmarkActions.ClearAll(editor.GetTextEditorData());
		}
		
	}
}

