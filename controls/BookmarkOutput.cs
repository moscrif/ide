using System;
using System.Collections.Generic;
using Gtk;
using Moscrif.IDE.Tool;
using Pango;
using Moscrif.IDE.Iface.Entities;
//using Moscrif.IDE.Task;

namespace Moscrif.IDE.Controls
{
	public class BookmarkOutput : Gtk.ScrolledWindow//,ITaskOutput
	{
		private TreeStore outputModel = new TreeStore(typeof(string),typeof(int),typeof(string));//, typeof(string), typeof(string));
		private TreeView treeView = null;
		
		public BookmarkOutput()
		{
			this.ShadowType = ShadowType.Out;
			treeView = new TreeView();
			treeView.Selection.Mode = Gtk.SelectionMode.Single;
			
			treeView.Model = outputModel;

			FontDescription customFont =  Pango.FontDescription.FromString(MainClass.Settings.ConsoleTaskFont);
			treeView.ModifyFont(customFont);

			TreeViewColumn tvcState = new TreeViewColumn (MainClass.Languages.Translate("Line"),  new CellRendererText(), "text", 1);
			tvcState.MinWidth = 25;
			treeView.AppendColumn(tvcState);

			TreeViewColumn tvcName = new TreeViewColumn (MainClass.Languages.Translate("file"),  new CellRendererText(), "text", 0);
			tvcName.MinWidth = 100;
			treeView.AppendColumn(tvcName);

			TreeViewColumn tvcText = new TreeViewColumn (MainClass.Languages.Translate("name"),  new CellRendererText(), "text", 2);
			tvcText.MinWidth = 100;
			treeView.AppendColumn(tvcText);

			treeView.HeadersVisible = true;
			treeView.EnableTreeLines = true;
			
			treeView.RowActivated += new RowActivatedHandler(OnRowActivate);
			treeView.EnableSearch =false;
			treeView.HasFocus = false;

			this.Add(treeView);
			
			this.ShowAll();
		}

		public void RefreshBookmark(){
			Clear();

			foreach(FileSetting  fs in MainClass.Workspace.WorkspaceUserSetting.FilesSetting){

				foreach(MyBookmark b in fs.Bookmarks2){
					WriteTask(fs.FileName,b.Line+1,b.Name);
					//Console.WriteLine("bookmark -> {0};{1}",fs.FileName,line);
				}
			}
		}


		private void OnRowActivate(object o, RowActivatedArgs args)
		{
			TreeIter ti = new TreeIter();

			try {
				outputModel.GetIter(out ti, args.Path);
				//if (ti != TreeIter.Zero) {
				
				string pFile = outputModel.GetValue(ti, 0).ToString();
				int pLine = Convert.ToInt32(outputModel.GetValue(ti, 1));
				if (!String.IsNullOrEmpty(pFile)) {
					MainClass.MainWindow.GoToFile(pFile, (object)pLine);
				}
				//}
			} catch {
			}
		}

		public void SetFont(string fontname){
			FontDescription customFont =  Pango.FontDescription.FromString(MainClass.Settings.ConsoleTaskFont);
			treeView.ModifyFont(customFont);
		}

		public void WriteTask(string file, int line,string name)
		{
			try {
				outputModel.AppendValues(file, line,name);

			} catch(Exception ex) {
				Logger.Error(ex.Message,null);
			}

		}

		public void Clear()
		{
			outputModel.Clear();
		}
	}
}

