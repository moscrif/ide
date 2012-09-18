
using System;
using Gtk;
using Pango;
using System.Collections.Generic;
using System.IO;

namespace Moscrif.IDE.Controls
{
	public class OutputConsole : Gtk.HBox
	{
		//Gtk.ScrolledWindow
		Gtk.TextBuffer buffer;
		Gtk.TextView textEditorControl;
		TextMark endMark;
		FontDescription customFont;
		Gtk.ScrolledWindow sw;
		//Gtk.VButtonBox vbt;
		Gtk.VBox vbt;

		TextTag tag;
		TextTag bold;
		TextTag errorTag;
		TextTag consoleLogTag;
		int ident = 0;
		List<TextTag> tags = new List<TextTag>();
		Stack<string> indents = new Stack<string>();

		Queue<QueuedUpdate> updates = new Queue<QueuedUpdate>();
		QueuedTextWrite lastTextWrite;
		GLib.TimeoutHandler outputDispatcher;
		bool outputDispatcherRunning = false;

		const int MAX_BUFFER_LENGTH = 200 * 1024;

		public OutputConsole()
		{
			buffer = new Gtk.TextBuffer(new Gtk.TextTagTable());
			textEditorControl = new Gtk.TextView(buffer);
			textEditorControl.Editable = true;
			
			sw = new ScrolledWindow();
			
			sw.ShadowType = ShadowType.Out;
			sw.Add(textEditorControl);
			
			this.PackStart(sw, true, true, 0);
			
			vbt = new VBox();

			Gdk.Pixbuf clear_pixbuf = MainClass.Tools.GetIconFromStock("file-new.png", IconSize.SmallToolbar);
			Button btnClear = new Button(new Gtk.Image(clear_pixbuf));
			btnClear.TooltipText = MainClass.Languages.Translate("clear");
			btnClear.Relief = ReliefStyle.None;
			btnClear.CanFocus = false;
			btnClear.WidthRequest = btnClear.HeightRequest = 24;
			btnClear.Clicked += delegate(object sender, EventArgs e) {
				Clear();
			};
			
			Gdk.Pixbuf save_pixbuf = MainClass.Tools.GetIconFromStock("save.png", IconSize.SmallToolbar);
			Button btnSave = new Button(new Gtk.Image(save_pixbuf));
			btnSave.TooltipText = MainClass.Languages.Translate("save");
			btnSave.Relief = ReliefStyle.None;
			btnSave.CanFocus = false;
			btnSave.WidthRequest = btnSave.HeightRequest = 24;
			btnSave.Clicked += delegate(object sender, EventArgs e) {
				Save();
			};
			
			vbt.WidthRequest = 24;
			vbt.PackStart(btnClear, false, false, 0);
			vbt.PackStart(btnSave, false, false, 0);

			this.PackEnd(vbt, false, false, 0);
			
			bold = new TextTag("bold");
			bold.Weight = Pango.Weight.Bold;
			buffer.TagTable.Add(bold);
			
			errorTag = new TextTag("error");
			errorTag.Foreground = "red";
			buffer.TagTable.Add(errorTag);
			
			consoleLogTag = new TextTag("consoleLog");
			consoleLogTag.Foreground = "darkgrey";
			buffer.TagTable.Add(consoleLogTag);
			
			tag = new TextTag("0");
			tag.LeftMargin = 10;
			buffer.TagTable.Add(tag);
			tags.Add(tag);
			
			endMark = buffer.CreateMark("end-mark", buffer.EndIter, false);

			outputDispatcher = new GLib.TimeoutHandler(OutputDispatchHandler);

			customFont =  Pango.FontDescription.FromString(MainClass.Settings.ConsoleTaskFont);
			textEditorControl.ModifyFont(customFont);

			textEditorControl.KeyPressEvent += HandleTextEditorControlKeyPressEvent;
			this.ShowAll();
		}

		public void SetFont(string fontname){
			customFont =  Pango.FontDescription.FromString(fontname);
			textEditorControl.ModifyFont(customFont);
		}

		public void Redraw(){

			while (Application.EventsPending ())
				Application.RunIteration ();

			textEditorControl.QueueDraw();
			textEditorControl.ShowAll();
			textEditorControl.GrabFocus();
			WriteError("---------\n");
			WriteText("**********\n");
			textEditorControl.HideAll();
			textEditorControl.ShowAll();
			textEditorControl.QueueDraw();
			textEditorControl.ShowAll();
			textEditorControl.GrabFocus();
			//textEditorControl.
		}


		public void Save(){

			Gtk.FileChooserDialog fc = new Gtk.FileChooserDialog(MainClass.Languages.Translate("save_path"), MainClass.MainWindow, FileChooserAction.Save, MainClass.Languages.Translate("cancel"), ResponseType.Cancel, MainClass.Languages.Translate("save"), ResponseType.Accept);
			fc.SetCurrentFolder(MainClass.Workspace.RootDirectory);

			if (fc.Run() == (int)ResponseType.Accept) {
				System.IO.StreamWriter write = new System.IO.StreamWriter(fc.Filename);
				write.Write(buffer.Text);
				write.Close();
			}
			fc.Destroy();
		}

		void HandleTextEditorControlKeyPressEvent(object o, KeyPressEventArgs args)
		{
			if (args.Event.Key != Gdk.Key.Return)
				return;
			WriteText("OK");
		}
		public void Clear()
		{
			lock (updates) {
				updates.Clear();
				lastTextWrite = null;
				outputDispatcherRunning = false;
			}
			
			buffer.Clear();
		}

		//mechanism to to batch copy text when large amounts are being dumped
		bool OutputDispatchHandler()
		{
			lock (updates) {
				lastTextWrite = null;
				if (updates.Count == 0) {
					outputDispatcherRunning = false;
					return false;
				} else if (!outputDispatcherRunning) {
					updates.Clear();
					return false;
				} else {
					while (updates.Count > 0) {
						QueuedUpdate up = updates.Dequeue();
						up.Execute(this);
					}
				}
			}
			return true;
		}

		void AddQueuedUpdate(QueuedUpdate update)
		{
			lock (updates) {
				updates.Enqueue(update);
				if (!outputDispatcherRunning) {
					GLib.Timeout.Add(50, outputDispatcher);
					outputDispatcherRunning = true;
				}
				lastTextWrite = update as QueuedTextWrite;
			}
		}

		protected void UnsafeBeginTask(string name, int totalWork)
		{
			if (name != null && name.Length > 0) {
				Indent();
				indents.Push(name);
			} else
				indents.Push(null);
			
			if (name != null) {
				UnsafeAddText(Environment.NewLine + name + Environment.NewLine, bold);
			}
		}

		public void BeginTask(string name, int totalWork)
		{
			QueuedBeginTask bt = new QueuedBeginTask(name, totalWork);
			AddQueuedUpdate(bt);
		}

		public void EndTask()
		{
			QueuedEndTask et = new QueuedEndTask();
			AddQueuedUpdate(et);
		}

		protected void UnsafeEndTask()
		{
			if (indents.Count > 0 && indents.Pop() != null)
				Unindent();
		}

		public void WriteText(string text)
		{
			//raw text has an extra optimisation here, as we can append it to existing updates
			lock (updates) {
				if (lastTextWrite != null) {
					if (lastTextWrite.Tag == null) {
						lastTextWrite.Write(text);
						return;
					}
				}
			}
			QueuedTextWrite qtw = new QueuedTextWrite(text, null);
			AddQueuedUpdate(qtw);
		}
		public void WriteError(string text)
		{
			QueuedTextWrite w = new QueuedTextWrite(text, errorTag);
			AddQueuedUpdate(w);
		}

		protected void UnsafeAddText(string text, TextTag extraTag)
		{
			//don't allow the pad to hold more than MAX_BUFFER_LENGTH chars
			int overrun = (buffer.CharCount + text.Length) - MAX_BUFFER_LENGTH;
			if (overrun > 0) {
				TextIter start = buffer.StartIter;
				TextIter end = buffer.GetIterAtOffset(overrun);
				buffer.Delete(ref start, ref end);
			}
			
			TextIter it = buffer.EndIter;
			ScrolledWindow window = textEditorControl.Parent as ScrolledWindow;
			bool scrollToEnd = true;
			if (window != null) {
				scrollToEnd = window.Vadjustment.Value >= window.Vadjustment.Upper - 2 * window.Vadjustment.PageSize;
			}
			if (extraTag != null)
				buffer.InsertWithTags(ref it, text, tag, extraTag);
			else
				buffer.InsertWithTags(ref it, text, tag);
			
			if (scrollToEnd) {
				it.LineOffset = 0;
				buffer.MoveMark(endMark, it);
				textEditorControl.ScrollToMark(endMark, 0, false, 0, 0);
			}
		}

		void Indent()
		{
			ident++;
			if (ident >= tags.Count) {
				tag = new TextTag(ident.ToString());
				tag.LeftMargin = 10 + 15 * (ident - 1);
				buffer.TagTable.Add(tag);
				tags.Add(tag);
			} else {
				tag = tags[ident];
			}
		}

		void Unindent()
		{
			if (ident >= 0) {
				ident--;
				tag = tags[ident];
			}
		}

		protected override void OnDestroyed()
		{
			base.OnDestroyed();
			
			lock (updates) {
				updates.Clear();
				lastTextWrite = null;
			}
			//IdeApp.Preferences.CustomOutputPadFontChanged -= HandleCustomFontChanged;
			if (customFont != null) {
				customFont.Dispose();
				customFont = null;
			}
		}

		private abstract class QueuedUpdate
		{
			public abstract void Execute(OutputConsole pad);
		}

		private class QueuedTextWrite : QueuedUpdate
		{
			private System.Text.StringBuilder Text;
			public TextTag Tag;
			public override void Execute(OutputConsole pad)
			{
				pad.UnsafeAddText(Text.ToString(), Tag);
			}

			public QueuedTextWrite(string text, TextTag tag)
			{
				Text = new System.Text.StringBuilder(text);
				Tag = tag;
			}

			public void Write(string s)
			{
				Text.Append(s);
				if (Text.Length > MAX_BUFFER_LENGTH)
					Text.Remove(0, Text.Length - MAX_BUFFER_LENGTH);
			}
		}

		private class QueuedBeginTask : QueuedUpdate
		{
			public string Name;
			public int TotalWork;
			public override void Execute(OutputConsole pad)
			{
				pad.UnsafeBeginTask(Name, TotalWork);
			}

			public QueuedBeginTask(string name, int totalWork)
			{
				TotalWork = totalWork;
				Name = name;
			}
		}

		private class QueuedEndTask : QueuedUpdate
		{
			public override void Execute(OutputConsole pad)
			{
				pad.UnsafeEndTask();
			}
		}
	}
}

