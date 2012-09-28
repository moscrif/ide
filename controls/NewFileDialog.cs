using System;
using System.IO;
using Gtk;
using Moscrif.IDE.Components;
using System.Collections.Generic;
using Moscrif.IDE.FileTemplates;

namespace Moscrif.IDE.Controls
{
	public partial class NewFileDialog : Gtk.Dialog
	{
		Gtk.ListStore fileListStore = new Gtk.ListStore(typeof(string),typeof(string), typeof(string), typeof(FileTemplate));
		//Table tblAtributes;
		public NewFileDialog()
		{
			this.TransientFor = MainClass.MainWindow;
			this.Build(); 
			notebook1.ShowTabs = false;

			//tblMain.Visible = false;

			CellRendererText textRenderer = new CellRendererText();
			//cbFileTemplates.Changed += new EventHandler(OnComboDeviceChanged);
			cbFileTemplates.PackStart(textRenderer, true);
			cbFileTemplates.AddAttribute(textRenderer, "text", 1);


			tvItem.Selection.Changed+= delegate(object sender, EventArgs e) {

				fileListStore.Clear();
				if(Directory.Exists(MainClass.Paths.FileTemplateDir)){

					string typeFile = GetSelectedTypFile();
					typeFile = String.Format("*{0}",typeFile);
					FileInfo[] fis = new DirectoryInfo(MainClass.Paths.FileTemplateDir).GetFiles(typeFile);

					fileListStore.AppendValues(MainClass.Languages.Translate("empty_file"),"",null);

					foreach (FileInfo fi in fis){
		                		FileTemplate t = new FileTemplate(fi,true);
	                			//if (String.Compare(t.TemplateType, "moscrif source code", true) != 0) continue;
	                				//templates.Items.Add(t);
						fileListStore.AppendValues(t.Name,t.Description,fi.FullName,t);
					}
				}
				cbFileTemplates.Model = fileListStore;
				cbFileTemplates.Active = 0;
			};
			FillType();
		}

		private FileTemplate GetSelectedTemplate()
		{
			Gtk.TreeIter iter;
			if (cbFileTemplates.GetActiveIter(out iter)){
				return  (FileTemplate)cbFileTemplates.Model.GetValue(iter, 3);
			} else return null;
		}

		private string GetSelectedTypFile()
		{
			TreeSelection ts = tvItem.Selection;

			TreeIter ti = new TreeIter();
			ts.GetSelected(out ti);

			TreePath[] tp = ts.GetSelectedRows();
			if (tp.Length < 1)
				return null;

			return  (string)tvItem.Model.GetValue(ti, 1);
		}


		private void GenerateEntry(ref Table table, FileTemplate.Attribute attribute,int xPos){
			AttributeEntry ae = new AttributeEntry(attribute);
			table.Attach(ae,0,1,(uint)(xPos-1),(uint)xPos,AttachOptions.Expand|AttachOptions.Fill,AttachOptions.Fill,0,0);
			//table.Attach(entr,1,2,(uint)(xPos-1),(uint)xPos,AttachOptions.Fill,AttachOptions.Shrink,3,3);
		}

		private void GenerateCheckBox(ref Table table,  FileTemplate.Attribute attribute,int xPos){

			AttributeCheckBox achb = new AttributeCheckBox(attribute);
			table.Attach(achb,0,1,(uint)(xPos-1),(uint)xPos,AttachOptions.Expand|AttachOptions.Fill,AttachOptions.Fill,0,0);
			//table.Attach(chb,1,2,(uint)(xPos-1),(uint)xPos,AttachOptions.Fill,AttachOptions.Shrink,3,3);
		}

		private void FillType()
		{
			Gtk.ListStore fileListStore = new Gtk.ListStore(typeof(Gdk.Pixbuf), typeof(string), typeof(string));
			
			tvItem.AppendColumn("", new Gtk.CellRendererPixbuf(), "pixbuf", 0);
			tvItem.AppendColumn(MainClass.Languages.Translate("file"), new Gtk.CellRendererText(), "text", 1);
			tvItem.AppendColumn(MainClass.Languages.Translate("description"), new Gtk.CellRendererText(), "text", 2);
			
			fileListStore.AppendValues(MainClass.Tools.GetIconFromStock("file-text.png", IconSize.LargeToolbar), ".txt", MainClass.Languages.Translate("new_file_text"));
			TreeIter selectIter = fileListStore.AppendValues(MainClass.Tools.GetIconFromStock("file-ms.png", IconSize.LargeToolbar), ".ms", MainClass.Languages.Translate("new_file_source_code"));
			fileListStore.AppendValues(MainClass.Tools.GetIconFromStock("file-ms.png", IconSize.LargeToolbar), ".mso", MainClass.Languages.Translate("new_file_json"));
			fileListStore.AppendValues(MainClass.Tools.GetIconFromStock("file-html.png", IconSize.LargeToolbar), ".xml", MainClass.Languages.Translate("XML file (.xml)"));
			fileListStore.AppendValues(MainClass.Tools.GetIconFromStock("file-database.png", IconSize.LargeToolbar), ".db", MainClass.Languages.Translate("new_file_sqlite"));

			tvItem.Model = fileListStore;
			tvItem.Selection.SelectIter(selectIter);
			entrName.GrabFocus();
		}


		private static Gdk.Pixbuf GetIcon(string name)
		{
			return Gtk.IconTheme.Default.LoadIcon(name, 24, (IconLookupFlags)0);
		}

	/*	public Gdk.Pixbuf GetIcon(string name, Gtk.IconSize size)
		{
			string stockid = name;
			//GetStockId (name);
			if (stockid != null) {
				Gtk.IconSet iconset = Gtk.IconFactory.LookupDefault(stockid);
				if (iconset != null) {
					return iconset.RenderIcon(Gtk.Widget.DefaultStyle, Gtk.TextDirection.None, Gtk.StateType.Normal, size, null, null);
				}
			}
			
			return null;
		}*/

		private string fileName;
		private string fileExtension;

		public string  FileName {
			get {

				string exten = System.IO.Path.GetExtension(fileName);

				if (String.IsNullOrEmpty(exten) || (exten != fileExtension) )
					fileName =fileName +fileExtension;

				return fileName;
			}
		}

		private FileTemplate fileTemplate;

		private string content;
		public string Content {
			get {
				return content;
			}
		}

		int pageCount = 1;
		protected virtual void OnButtonOkClicked(object sender, System.EventArgs e)
		{
			if(pageCount == 1){

				TreeSelection ts = tvItem.Selection;
				TreeIter ti = new TreeIter();
				ts.GetSelected(out ti);
	
				TreePath[] tp = ts.GetSelectedRows();
	
				fileName = entrName.Text;
	
				if (tp.Length <= 0 )  return;
				if (String.IsNullOrEmpty(fileName) )  return;
	
				fileExtension = tvItem.Model.GetValue(ti, 1).ToString();
	
				FileTemplate  ft = GetSelectedTemplate();
				if (ft  == null){
					content = "";
					this.Respond(ResponseType.Ok);
				} else {
					//hbox1.Destroy();
					//tblMain.Destroy();
					notebook1.Page = 1;
					fileTemplate = ft;
					hbox1.HideAll();
					tblMain.HideAll();
					vbox2.Remove(hbox1);
					vbox2.Remove(tblMain);

					//if(tblAtributes!=null)
					//	tblAtributes.Destroy();
					//Table tblAtributes = new Table((uint)ft.Attributes.Count+2,3,false);
					tblAtributes.NRows = (uint)ft.Attributes.Count+2;
					tblAtributes.NColumns =3;
					tblAtributes.BorderWidth = 10;
					tblAtributes.WidthRequest = 200;
					//tblAtributes.BorderWidth = 2;
				
					//vbox3.PackStart(tblAtributes,true,true,2);
					int i =1;
				        foreach (FileTemplate.Attribute attr in ft.Attributes) {
						if(attr.Type=="bool"){
							bool defValue = false;
							if(attr.Value!= null){
								Boolean.TryParse(attr.Value.ToString(), out defValue);
							}
							GenerateCheckBox(ref tblAtributes,attr,i);
						} else {
							GenerateEntry(ref tblAtributes,attr,i);
						}
						i++;
				        };
					tblAtributes.ShowAll();
					this.ShowAll();
					pageCount = 2;
				}
			} else if(pageCount == 2){
				content = FileTemplateUtilities.Apply(fileTemplate.Content, fileTemplate.GetAttributesAsDictionary());
				//Console.WriteLine(content);
				this.Respond(ResponseType.Ok);
			}

		}
		
		protected virtual void OnEntrNameKeyReleaseEvent (object o, Gtk.KeyReleaseEventArgs args)
		{
			if (args.Event.Key == Gdk.Key.Return)
				OnButtonOkClicked(null,null);
		}

		

		
	}
}

