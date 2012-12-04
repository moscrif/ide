using System;
using Gtk;
using Moscrif.IDE.Components;
using Moscrif.IDE.Option;
using Moscrif.IDE.Controls;
using Moscrif.IDE.Iface.Entities;
using MessageDialogs = Moscrif.IDE.Controls.MessageDialog;

namespace Moscrif.IDE.Option
{

	internal class EditorPanel : OptionsPanel
	{
		EditorWidget widget;
		
		public override Widget CreatePanelWidget()
		{
			return widget = new EditorWidget(ParentDialog);
		}
		
		public override void ApplyChanges()
		{
			widget.Store();
		}
		
		public override bool ValidateChanges()
		{
			return true;
		}
		
		public override void Initialize(PreferencesDialog dialog, object dataObject)
		{
			base.Initialize(dialog, dataObject);
		}
		
		public override string Label {
			get { return MainClass.Languages.Translate("Files"); }
		}
		
		public override string Icon {
			get { return "file-ms.png"; }
		}
		
	}

	public partial class EditorWidget : Gtk.Bin	
	{
		private FileEntry feExternalPRogram = new FileEntry();
		DropDownRadioButton ddrbAction = new DropDownRadioButton();
		DropDownButton.ComboItemSet actionItems = new DropDownButton.ComboItemSet ();
		Gtk.ListStore resolStore = new Gtk.ListStore(typeof(string),typeof(ExtensionSetting));
		Gtk.Window parentWindow;

		ExtensionSetting selectedExtensionSetting;
		TreeIter selectedTreeIter = new TreeIter();

		public EditorWidget(Gtk.Window parent)
		{
			this.Build();
			this.parentWindow = parent;
			feExternalPRogram.IsFolder = false;
			DropDownButton.ComboItem textEdit = new DropDownButton.ComboItem("Text Editor",(int)ExtensionSetting.OpenTyp.TEXT);

			actionItems.Add(textEdit);
			actionItems.Add(new DropDownButton.ComboItem("Image Editor",(int)ExtensionSetting.OpenTyp.IMAGE));
			actionItems.Add(new DropDownButton.ComboItem("Database Editor",(int)ExtensionSetting.OpenTyp.DATABASE));
			actionItems.Add(new DropDownButton.ComboItem("System program",(int)ExtensionSetting.OpenTyp.SYSTEM));
			actionItems.Add(new DropDownButton.ComboItem("External Program",(int)ExtensionSetting.OpenTyp.EXTERNAL));

			ddrbAction.Changed+= delegate(object sender, DropDownButton.ChangedEventArgs e)
			{
				if(e.Item !=null){
					int indx = 0;
					Int32.TryParse(e.Item.ToString(),out indx);
					switch(indx){
					case 0:{
						frmExternal.Sensitive = false;
						break;
					}
					case 1:{
						frmExternal.Sensitive = false;
						break;
					}
					case 2:{
						frmExternal.Sensitive = false;						
						break;
					}
					case -1:{
						frmExternal.Sensitive = false;
						break;
					}
					case -2:{
						frmExternal.Sensitive = true;
						break;
					}
					}
				}
			};
			ddrbAction.SetItemSet(actionItems);
			ddrbAction.SelectItem(actionItems,textEdit);

			tblMain.Attach(ddrbAction,2,3,2,3,AttachOptions.Fill,AttachOptions.Shrink,0,0);
			tblExternal.Attach(feExternalPRogram,1,2,0,1,AttachOptions.Fill,AttachOptions.Shrink,0,0);

			tvExtension.AppendColumn(MainClass.Languages.Translate("name"), new Gtk.CellRendererText(), "text", 0);
			tvExtension.Model = resolStore;
			this.ShowAll();


			if((MainClass.Settings.ExtensionList == null)||(MainClass.Settings.ExtensionList.Count<1)){
				MainClass.Settings.GenerateExtensionList();
			}

			foreach(ExtensionSetting ex in MainClass.Settings.ExtensionList){
				resolStore.AppendValues(ex.Extension,ex);
			}
			tvExtension.Selection.Changed+= delegate(object sender, EventArgs e) {
				selectedExtensionSetting = GetSelectedExtensionSetting();
				if(selectedExtensionSetting==null) return;

				entrExtension.Text = selectedExtensionSetting.Extension;

				ddrbAction.SelectValue(actionItems,(int)selectedExtensionSetting.OpenType);

				if(!String.IsNullOrEmpty(selectedExtensionSetting.ExternalProgram)){
					feExternalPRogram.DefaultPath =selectedExtensionSetting.ExternalProgram; 
					feExternalPRogram.Path =selectedExtensionSetting.ExternalProgram; 
					entrParameters.Text = selectedExtensionSetting.Parameter;
				}
			};
			tvExtension.Selection.SelectPath(new TreePath("0"));
		}

		private ExtensionSetting GetSelectedExtensionSetting()
		{
			TreeSelection ts = tvExtension.Selection;
			
			TreeIter ti = new TreeIter();
			ts.GetSelected(out ti);
			
			TreePath[] tp = ts.GetSelectedRows();
			if (tp.Length < 1)
				return null;
			
			selectedTreeIter = ti;
			
			return  (ExtensionSetting)tvExtension.Model.GetValue(ti, 1);
		}

		public void Store(){

		}

		protected void OnBtnSaveClicked (object sender, EventArgs e)
		{
		/*	MessageDialogs md = new MessageDialogs(MessageDialogs.DialogButtonType.YesNo, MainClass.Languages.Translate("are_you_sure"), "", Gtk.MessageType.Question,parentWindow);
			int result = md.ShowDialog();
			if (result != (int)Gtk.ResponseType.Yes)
				return;
			*/
			TreeSelection ts = tvExtension.Selection;
			
			TreeIter ti = new TreeIter();
			ts.GetSelected(out ti);
			
			TreePath[] tp = ts.GetSelectedRows();
			if (tp.Length < 1)
				return ;
			
			selectedTreeIter = ti;
			selectedExtensionSetting = (ExtensionSetting)tvExtension.Model.GetValue(ti, 1);

			int seltyp =  (int)ddrbAction.CurrentItem;
			selectedExtensionSetting.OpenType = (ExtensionSetting.OpenTyp)seltyp;

			selectedExtensionSetting.Extension = entrExtension.Text;
			if(selectedExtensionSetting.OpenType == ExtensionSetting.OpenTyp.EXTERNAL){
				selectedExtensionSetting.ExternalProgram=feExternalPRogram.Path; 
				selectedExtensionSetting.Parameter= entrParameters.Text;
			}

			resolStore.SetValues(ti,selectedExtensionSetting.Extension,selectedExtensionSetting);

		}

		protected void OnBtnDeleteClicked (object sender, EventArgs e)
		{
			MessageDialogs md = new MessageDialogs(MessageDialogs.DialogButtonType.YesNo, MainClass.Languages.Translate("are_you_sure"), "", Gtk.MessageType.Question,parentWindow);
			int result = md.ShowDialog();
			if (result != (int)Gtk.ResponseType.Yes)
				return;

			TreeSelection ts = tvExtension.Selection;
			
			TreeIter ti = new TreeIter();
			ts.GetSelected(out ti);
			
			TreePath[] tp = ts.GetSelectedRows();
			if (tp.Length < 1)
				return ;
			
			selectedTreeIter = ti;
			
			ExtensionSetting es = (ExtensionSetting)tvExtension.Model.GetValue(ti, 1);

			MainClass.Settings.ExtensionList.Remove(selectedExtensionSetting);
			resolStore.Remove(ref ti);
			tvExtension.Selection.SelectPath(new TreePath("0"));

		}

		protected void OnBtnAddClicked (object sender, EventArgs e)
		{
			EntryDialog ed = new EntryDialog("",MainClass.Languages.Translate("new_conditions"),parentWindow);
			int result = ed.Run();
			if (result == (int)ResponseType.Ok){
				string newStr = ed.TextEntry;
				if (!String.IsNullOrEmpty(newStr) ){

					ExtensionSetting es= new ExtensionSetting();
					es.Extension =newStr;
					es.OpenType = ExtensionSetting.OpenTyp.TEXT;
					resolStore.AppendValues(es.Extension,es);
					MainClass.Settings.ExtensionList.Add(es);
				}
			}
			ed.Destroy();
		}
	}
}

