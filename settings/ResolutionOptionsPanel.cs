using System;
using Gtk;
using System.IO;
using Moscrif.IDE.Controls;
using MessageDialogs = Moscrif.IDE.Controls.MessageDialog;
using Moscrif.IDE.Tool;
using Moscrif.IDE.Devices;
using Moscrif.IDE.Iface.Entities;
using System.Collections.Generic;

namespace Moscrif.IDE.Settings
{

	internal class ResolutionOptionsPanel : OptionsPanel
	{
		ResolutionOptionWidget widget;

		public override Widget CreatePanelWidget ()
		{
			return widget = new  ResolutionOptionWidget (ParentDialog);
		}

		public override void ShowPanel()
		{
		}

		public override void ApplyChanges ()
		{
			widget.Store ();
		}

		public override bool ValidateChanges ()
		{
			return true;
		}

		public override string Label {
			get { return MainClass.Languages.Translate("resolution_f1"); }
		}

		public override string Icon {
			get { return "emulator-skin.png"; }
		}

	}

	public partial class ResolutionOptionWidget : Gtk.Bin
	{
		//private Condition resolution;
		private int maxCond = 0;
		List<int> resolutions = new List<int>();

		Gtk.ListStore resolStore = new Gtk.ListStore(typeof(int), typeof(string),typeof(string),typeof(Rule),typeof(bool));
		Gtk.Window parentWindow;

		public ResolutionOptionWidget(Gtk.Window parent)
		{
			parentWindow = parent;
			this.Build();

			Gtk.CellRendererText collumnRenderer = new Gtk.CellRendererText();
			Gtk.CellRendererText collumnResolRenderer = new Gtk.CellRendererText();

			tvResolution.AppendColumn(MainClass.Languages.Translate("id"), new Gtk.CellRendererText(), "text", 0);
			tvResolution.AppendColumn(MainClass.Languages.Translate("name"), collumnRenderer, "text", 1);
			tvResolution.AppendColumn(MainClass.Languages.Translate("specific"), new Gtk.CellRendererText(), "text", 2);
			tvResolution.AppendColumn(MainClass.Languages.Translate("resolution_f1"), collumnResolRenderer, "text", 1);
			tvResolution.Model = resolStore;
			tvResolution.Columns[0].Visible= false;

			tvResolution.Columns[1].SetCellDataFunc(collumnRenderer, new Gtk.TreeCellDataFunc(RenderFileNme));
			//tvResolution.Columns[2].SetCellDataFunc(collumnRenderer, new Gtk.TreeCellDataFunc(RenderFileNme));
			tvResolution.Columns[3].SetCellDataFunc(collumnResolRenderer, new Gtk.TreeCellDataFunc(RenderResolution));

			foreach (Rule rl in MainClass.Settings.Resolution.Rules ){
				resolStore.AppendValues(rl.Id,rl.Name,rl.Specific,rl);
				if (maxCond < rl.Id) maxCond = rl.Id;
			}

		}

		private void RenderResolution(Gtk.TreeViewColumn column, Gtk.CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
		{
			int type = (int) model.GetValue (iter, 0);
			Rule rule = (Rule) model.GetValue (iter, 3);

			Pango.FontDescription fd = new Pango.FontDescription();

			/*if (type < 0) {
				fd.Weight = Pango.Weight.Bold;

			} else {
				fd.Weight = Pango.Weight.Normal;
			}
			(cell as Gtk.CellRendererText).FontDesc = fd;*/

			if(rule!=null){
				(cell as Gtk.CellRendererText).Text = String.Format("{0}x{1}",rule.Width,rule.Height);
			} else {
				(cell as Gtk.CellRendererText).Text ="";
			}
		}

		private void RenderFileNme(Gtk.TreeViewColumn column, Gtk.CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
		{
			int type = (int) model.GetValue (iter, 0);

			Pango.FontDescription fd = new Pango.FontDescription();

			if (type < 0) {
				fd.Weight = Pango.Weight.Bold;

			} else {
				fd.Weight = Pango.Weight.Normal;
			}
			(cell as Gtk.CellRendererText).FontDesc = fd;
		}


		public void Store()
		{

		}

		protected void OnBtnAddRulesClicked (object sender, System.EventArgs e)
		{
			ResolutionDialog ed = new ResolutionDialog(parentWindow);

			int result = ed.Run();
			if (result == (int)ResponseType.Ok){
				Rule res = ed.Resolution;
				if (res!= null ){

					Rule cdFind = MainClass.Settings.Resolution.Rules.Find(x=>x.Name.ToUpper() ==res.Name.ToUpper());
					if (cdFind != null){

						MessageDialogs md = new MessageDialogs(MessageDialogs.DialogButtonType.Ok, MainClass.Languages.Translate("resolution_is_exist", cdFind.Name), "", Gtk.MessageType.Error,parentWindow);
						md.ShowDialog();
						ed.Destroy();
						return;
					}

					maxCond ++;
					res.Id = maxCond;
					resolStore.AppendValues(res.Id,res.Name,res.Specific,res);
					MainClass.Settings.Resolution.Rules.Add(res);

					if(ed.CreateFile){
						try{
							string newFile = System.IO.Path.Combine(MainClass.Paths.DisplayDir, res.Name + ".ini");
							EmulatorDisplay dd= EmulatorDisplay.Create(newFile,res.Width,res.Height);
						}catch(Exception ex){
							Tool.Logger.Error(ex.Message,null);
							MessageDialogs ms = new MessageDialogs(MessageDialogs.DialogButtonType.Ok, "Error", ex.Message, MessageType.Error,parentWindow);
							ms.ShowDialog();
							return;
						}

					}

				}
			}
			ed.Destroy();
		}

		protected void OnBtnDeleteRuleClicked (object sender, System.EventArgs e)
		{
			TreeSelection ts = tvResolution.Selection;

			TreeIter ti = new TreeIter();
			ts.GetSelected(out ti);

			TreePath[] tp = ts.GetSelectedRows();
			if (tp.Length < 1)
				return ;

			Rule cd = (Rule)tvResolution.Model.GetValue(ti, 3);
			if (cd == null) return;

			if(cd.Id == -1){
				MessageDialogs mdError = new MessageDialogs(MessageDialogs.DialogButtonType.Ok, MainClass.Languages.Translate("delete_resolution_system"), "", Gtk.MessageType.Error,parentWindow);
				mdError.ShowDialog();
				return;
			}

			MessageDialogs md = new MessageDialogs(MessageDialogs.DialogButtonType.YesNo, MainClass.Languages.Translate("delete_resolution", cd.Name), "", Gtk.MessageType.Question,parentWindow);
			int result = md.ShowDialog();
			if (result != (int)Gtk.ResponseType.Yes)
				return;

			MainClass.Settings.Resolution.Rules.Remove(cd);
			resolStore.Remove(ref ti);
		}

		protected void OnBtnEditRuleClicked (object sender, System.EventArgs e)
		{
			EditRule();
		}

		protected void OnTvResolutionRowActivated (object o, Gtk.RowActivatedArgs args)
		{
			EditRule();
		}

		protected void OnButton118Clicked (object sender, System.EventArgs e)
		{
			MessageDialogs md = new MessageDialogs(MessageDialogs.DialogButtonType.YesNo, MainClass.Languages.Translate("question_default_resolution"), "", Gtk.MessageType.Question,parentWindow);
			int result = md.ShowDialog();
	
			if (result != (int)Gtk.ResponseType.Yes)
				return;
	
			MainClass.Settings.GenerateResolution();
			MainClass.Settings.GeneratePlatformResolutions();
			resolStore.Clear();
			maxCond = 0;

			foreach (Rule rl in MainClass.Settings.Resolution.Rules ){
				resolStore.AppendValues(rl.Id,rl.Name,rl.Specific,rl);
				if (maxCond < rl.Id) maxCond = rl.Id;
			}
		}

		private void EditRule(){
			TreeSelection ts = tvResolution.Selection;

			TreeIter ti = new TreeIter();
			ts.GetSelected(out ti);

			TreePath[] tp = ts.GetSelectedRows();
			if (tp.Length < 1)
				return ;

			Rule cd = (Rule)tvResolution.Model.GetValue(ti, 3);
			if (cd == null) return;

			if(cd.Id ==-1){
				MessageDialogs mdError = new MessageDialogs(MessageDialogs.DialogButtonType.Ok, MainClass.Languages.Translate("edit_resolution_system"), "", Gtk.MessageType.Error,parentWindow);
				mdError.ShowDialog();
				return;
			}

			int width= cd.Width;
			int height =cd.Height;

			ResolutionDialog ed = new ResolutionDialog(cd,parentWindow);
			int result = ed.Run();
			if (result == (int)ResponseType.Ok){
				Rule res = ed.Resolution;
				if (res!= null ){

					Rule cdFind = MainClass.Settings.Resolution.Rules.Find(x=>x.Id ==res.Id);
					if (cdFind == null){

						MessageDialogs md = new MessageDialogs(MessageDialogs.DialogButtonType.Ok, MainClass.Languages.Translate("resolution_is_exist", cdFind.Name), "", Gtk.MessageType.Error,parentWindow);
						md.ShowDialog();
						ed.Destroy();
						return;
					}
					cdFind = res;
					resolStore.SetValues(ti,cdFind.Id,cdFind.Name,cdFind.Specific,cdFind);

					try{
						string[] listFi = Directory.GetFiles(MainClass.Paths.DisplayDir, "*.ini");

						foreach (string fi in listFi){
							EmulatorDisplay dd = new EmulatorDisplay(fi);
							if (dd.Load()){
								if (dd.Height == height && dd.Width == width){
									dd.Height = res.Height;
									dd.Width = res.Width;
									dd.Save();
								}
							}
						}


					}catch(Exception ex){
						Tool.Logger.Error(ex.Message,null);
						MessageDialogs ms = new MessageDialogs(MessageDialogs.DialogButtonType.Ok, "Error", ex.Message, MessageType.Error,parentWindow);
						ms.ShowDialog();
						return;
					}
				}
			}
			ed.Destroy();
		}
	}
}

