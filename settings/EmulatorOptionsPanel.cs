using System;
using Gtk;
using System.IO;
using Moscrif.IDE.Controls;
using MessageDialogs = Moscrif.IDE.Controls.MessageDialog;
using Moscrif.IDE.Tool;
using Moscrif.IDE.Devices;
using Moscrif.IDE.Iface.Entities;

namespace Moscrif.IDE.Option
{

	internal class EmulatorOptionsPanel : OptionsPanel
	{
		DevicesOptionsWidget widget;

		public override Widget CreatePanelWidget()
		{
			return widget = new DevicesOptionsWidget(ParentDialog);
		}

		public override void ShowPanel()
		{
			widget.ShowWidget();
		}

		public override void ApplyChanges()
		{
			widget.Store();
		}

		public override bool ValidateChanges()
		{
			return widget.Valid();
		}

		public override void Initialize(PreferencesDialog dialog, object dataObject)
		{
			base.Initialize(dialog, dataObject);
		}

		public override string Label {
			get { return MainClass.Languages.Translate("emulator_f1"); }//Devices
		}

		public override string Icon {
			get { return "emulator.png"; }//emulator-skin.png
		}
		
	}

	public partial class DevicesOptionsWidget : Gtk.Bin
	{
		Gtk.ListStore fileListStore = new Gtk.ListStore(typeof(string), typeof(string),typeof(EmulatorDisplay),typeof(string));
		Gtk.ListStore resolStore = new Gtk.ListStore(typeof(string),typeof(int), typeof(string),typeof(string),typeof(Rule));

		private bool isChange = false;
		EmulatorDisplay selectedResolDisplay;
		TreeIter selectedTreeIter = new TreeIter();
		Gtk.Window parentWindow;

		//table1
		public DevicesOptionsWidget(Gtk.Window parent)
		{
			parentWindow = parent;
			this.Build();
			//ComboBox cbResolution = new ComboBox();

			//table1.Attach(cbResolution,1,2,0,1);
			CellRendererText textRenderer = new CellRendererText();

			//cbResolution.PackStart(textRenderer, true);

			cbResolution.AddAttribute(textRenderer, "text", 0);

			cbResolution.Model=resolStore;

			tvFiles.AppendColumn(MainClass.Languages.Translate("file"), new Gtk.CellRendererText(), "text", 0);
			tvFiles.AppendColumn(MainClass.Languages.Translate("path"), new Gtk.CellRendererText(), "text", 1);

			DirectoryInfo dir = new DirectoryInfo(MainClass.Paths.DisplayDir);

			string[] listFi = Directory.GetFiles(MainClass.Paths.DisplayDir, "*.ini");
			foreach (string fi in listFi){
				EmulatorDisplay dd = new EmulatorDisplay(fi);
				if (dd.Load()){
					fileListStore.AppendValues(System.IO.Path.GetFileName(fi),fi,dd);
				}
			}

			foreach (Rule rl in MainClass.Settings.Resolution.Rules ){
				resolStore.AppendValues(String.Format("{0} ({1}x{2})",rl.Name,rl.Width,rl.Height),rl.Id,rl.Name,rl.Specific,rl);
			}

			tvFiles.Model = fileListStore;

			tvFiles.Selection.Changed += delegate(object sender, EventArgs e)
			{
				if(isChange){
					Save(true);
				}

				selectedResolDisplay = GetSelectedDevicesDisplay();
				if (selectedResolDisplay == null ) return;

				sbHeight.Value = selectedResolDisplay.Height;
				sbWidth.Value =selectedResolDisplay.Width;

				entTitle.Text =selectedResolDisplay.Title;
				entAuthor.Text =selectedResolDisplay.Author;
				entUrl.Text =selectedResolDisplay.Url;
				chbTablet.Active =selectedResolDisplay.Tablet;

				Rule rlr = MainClass.Settings.Resolution.Rules.Find(x=>x.Height==selectedResolDisplay.Height && x.Width== selectedResolDisplay.Width);

				if(rlr != null){
					TreeIter ti = new TreeIter();
					bool isFind = false;
					resolStore.Foreach((model, path, iterr) => {
						Rule ruleIter = (Rule)resolStore.GetValue(iterr, 4);

						if (ruleIter == rlr){
							ti = iterr;
							isFind = true;
							return true;
						}
							return false;
					});
					if(isFind)
						cbResolution.SetActiveIter(ti);
					else cbResolution.Active =0;
				}
				isChange = false;
			};
			cbResolution.Changed += delegate(object sender, EventArgs e) {
				TreeIter ti = new TreeIter();
				if(cbResolution.GetActiveIter(out ti)){
					Rule ruleIter = (Rule)resolStore.GetValue(ti, 4);
					sbHeight.Value= ruleIter.Height;
					sbWidth.Value= ruleIter.Width;
				}

			};
		}

		public void ShowWidget(){

			isChange = false;
			TreeSelection ts = tvFiles.Selection;

			TreePath[] tp = ts.GetSelectedRows();

			fileListStore.Clear();
			string[] listFi = Directory.GetFiles(MainClass.Paths.DisplayDir, "*.ini");
			foreach (string fi in listFi){
				EmulatorDisplay dd = new EmulatorDisplay(fi);
				if (dd.Load()){
					fileListStore.AppendValues(System.IO.Path.GetFileName(fi),fi,dd);
				}
			}

			resolStore.Clear();
			foreach (Rule rl in MainClass.Settings.Resolution.Rules ){
				resolStore.AppendValues(String.Format("{0} ({1}x{2})",rl.Name,rl.Width,rl.Height),rl.Id,rl.Name,rl.Specific,rl);
			}

			isChange = false;
			if (tp.Length < 1){
				TreeIter ti = new TreeIter();
				if(fileListStore.GetIterFirst(out ti)){
					ts.SelectIter(ti);
				}
			} else {
				ts.SelectPath(tp[0]);
			}

		}

		public void Store()
		{
			
		}

		public bool Valid()
		{
			if(isChange){
				Save(true);
			}
			return true;
		}

		protected virtual void OnBtnAddClicked(object sender, System.EventArgs e)
		{
			EntryDialog ed = new EntryDialog("", MainClass.Languages.Translate("new_resolution_name"),parentWindow);
			
			int result = ed.Run();
			if (result == (int)ResponseType.Ok) {
				if (!String.IsNullOrEmpty(ed.TextEntry)) {
					string nameFile = MainClass.Tools.RemoveDiacritics(ed.TextEntry);
					
					string newFile = System.IO.Path.Combine(MainClass.Paths.DisplayDir, nameFile + ".ini");
					/*if (File.Exists(newFile)) {
						MessageDialogs ms = new MessageDialogs(MessageDialogs.DialogButtonType.Ok, "Error", "File cannot is create becaus is exist!", MessageType.Error);
						ms.ShowDialog();
						return;
					}*/
					try {
						EmulatorDisplay dd= EmulatorDisplay.Create(newFile);

						if (dd!= null){
							TreeIter ti = fileListStore.AppendValues(System.IO.Path.GetFileName(newFile), newFile,dd);
							tvFiles.Selection.SelectIter(ti);
						}

						//File.Create(newFile);
						
					} catch (Exception ex) {
						
						MessageDialogs ms = new MessageDialogs(MessageDialogs.DialogButtonType.Ok, "Error", ex.Message, MessageType.Error,parentWindow);
						ms.ShowDialog();
						return;
					}
				}
			}
			ed.Destroy();
			
		}

		protected virtual void OnBtnRemoveClicked(object sender, System.EventArgs e)
		{
			TreeSelection ts = tvFiles.Selection;

			TreeIter ti = new TreeIter();
			ts.GetSelected(out ti);

			TreePath[] tp = ts.GetSelectedRows();
			if (tp.Length < 1)
				return ;

			EmulatorDisplay dd = (EmulatorDisplay)tvFiles.Model.GetValue(ti, 2);


			MessageDialogs md = new MessageDialogs(MessageDialogs.DialogButtonType.YesNo, MainClass.Languages.Translate("question_delete_file", dd.FilePath), "", Gtk.MessageType.Question,parentWindow);
			int result = md.ShowDialog();

			if (result != (int)Gtk.ResponseType.Yes)
				return;

			if (!System.IO.File.Exists(dd.FilePath ))
				return;

			try {
				//fileListStore
				fileListStore.SetValue(ti,2,null);
				fileListStore.Remove(ref ti);
				System.IO.File.Delete(dd.FilePath);
			} catch {
				return;
			}
		}


		private EmulatorDisplay GetSelectedDevicesDisplay()
		{
			TreeSelection ts = tvFiles.Selection;

			TreeIter ti = new TreeIter();
			ts.GetSelected(out ti);

			TreePath[] tp = ts.GetSelectedRows();
			if (tp.Length < 1)
				return null;

			selectedTreeIter = ti;

			return  (EmulatorDisplay)tvFiles.Model.GetValue(ti, 2);
		}

		private void Save(bool question){

			if(question){
				MessageDialogs md = new MessageDialogs(MessageDialogs.DialogButtonType.YesNo, MainClass.Languages.Translate("question_save_resolution", selectedResolDisplay.Title), MainClass.Languages.Translate("changes_will_be_lost"), Gtk.MessageType.Question,parentWindow);
				int result = md.ShowDialog();
	
				if (result != (int)Gtk.ResponseType.Yes){
					isChange = false;
					return;
				}

			}

			/*TreeSelection ts = tvFiles.Selection;

			TreeIter ti = new TreeIter();
			ts.GetSelected(out ti);

			TreePath[] tp = ts.GetSelectedRows();
			if (tp.Length < 1)
				return ;

			ResolutionDisplay dd=  (ResolutionDisplay)tvFiles.Model.GetValue(ti, 2);
			*/
			if (selectedResolDisplay == null ) return;

			selectedResolDisplay.Height=(int)sbHeight.Value;
			selectedResolDisplay.Width=(int)sbWidth.Value;

			selectedResolDisplay.Title = String.IsNullOrEmpty(entTitle.Text)?" ":entTitle.Text;
			selectedResolDisplay.Author=String.IsNullOrEmpty(entAuthor.Text)?" ":entAuthor.Text;
			selectedResolDisplay.Url=String.IsNullOrEmpty(entUrl.Text)?" ":entUrl.Text;
			selectedResolDisplay.Tablet= chbTablet.Active;

			selectedResolDisplay.Save();
			fileListStore.SetValue(selectedTreeIter,2,selectedResolDisplay);
			//fileListStore.SetValue(ti,2,dd);

			isChange = false;
		}

		protected virtual void OnBtnSaveClicked(object sender, System.EventArgs e)
		{
			Save(false);
			/*TreeSelection ts = tvFiles.Selection;

			TreeIter ti = new TreeIter();
			ts.GetSelected(out ti);

			TreePath[] tp = ts.GetSelectedRows();
			if (tp.Length < 1)
				return ;

			ResolutionDisplay dd=  (ResolutionDisplay)tvFiles.Model.GetValue(ti, 2);

			if (dd == null ) return;

			dd.Height=(int)sbHeight.Value;
			dd.Width=(int)sbWidth.Value;

			dd.Title = String.IsNullOrEmpty(entTitle.Text)?" ":entTitle.Text;
			dd.Author=String.IsNullOrEmpty(entAuthor.Text)?" ":entAuthor.Text;
			dd.Url=String.IsNullOrEmpty(entUrl.Text)?" ":entUrl.Text;


			dd.Save();
			fileListStore.SetValue(ti,2,dd);*/
		}

		protected void OnEntTitleChanged (object sender, System.EventArgs e)
		{
			isChange = true;
		}

		protected void OnEntAuthorChanged (object sender, System.EventArgs e)
		{
			isChange = true;
		}

		protected void OnEntUrlChanged (object sender, System.EventArgs e)
		{
			isChange = true;
		}

		protected void OnCbResolutionChanged (object sender, System.EventArgs e)
		{
			isChange = true;
		}

		protected void OnChbTabletToggled (object sender, System.EventArgs e)
		{
			isChange = true;
		}
	}
}

