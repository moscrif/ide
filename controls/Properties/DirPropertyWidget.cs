using System;
using Gtk;
using Moscrif.IDE.Workspace;
using Moscrif.IDE.Option;
using Moscrif.IDE.Iface.Entities;
using System.IO;
using System.Collections.Generic;

namespace Moscrif.IDE.Controls.Properties
{
	public class DirPropertyWidget: Gtk.HBox
	{

		private FileItem fiOld;
		Project project;
		public DirPropertyWidget(FilePropertisData fpd)
		{
			project = fpd.Project;
			fiOld = project.FilesProperty.Find(x => x.SystemFilePath == fpd.Filename);
			if (fiOld == null){
				fiOld =new FileItem(fpd.Filename,false);
				fiOld.IsDirectory = true;
				project.FilesProperty.Add(fiOld);
			}
			if (fiOld.ConditionValues == null)
				fiOld.ConditionValues = new System.Collections.Generic.List<ConditionRule>();
			
			
			Table mainTable = new Table(2,1,false);
			Table propertyTable = new Table(5,2,false);
			
			Label lblFN =GetLabel(System.IO.Path.GetFileName(fiOld.SystemFilePath)); // new Label(System.IO.Path.GetFileName(fiOld.SystemFilePath));

			Entry entr = new Entry(fiOld.SystemFilePath);
			entr.IsEditable = false;

			Entry entrFullPath = new Entry(MainClass.Workspace.GetFullPath(fiOld.SystemFilePath));
			entrFullPath.IsEditable = false;

			Label lblPrj = GetLabel(project.ProjectName); //new Label(project.ProjectName);
		
			AddControl(ref propertyTable,0,lblFN,"Name ");
			AddControl(ref propertyTable,1,entr,"Relative Path ");
			AddControl(ref propertyTable,2,entrFullPath,"Full Path ");
			AddControl(ref propertyTable,3,lblPrj,"Project ");

			int rowCount = project.ConditoinsDefine.Count;
			Table conditionsTable = new Table((uint)(rowCount + 3),(uint)2,false);
			
			GenerateContent(ref conditionsTable, MainClass.Settings.Platform.Name, 1, MainClass.Settings.Platform,false);//tableSystem
			GenerateContent(ref conditionsTable, MainClass.Settings.Resolution.Name, 2,MainClass.Settings.Resolution,true); //project.Resolution);//tableSystem
			int i = 3;
			foreach (Condition cd in project.ConditoinsDefine) {
				GenerateContent(ref conditionsTable, cd.Name, i, cd,false);
				i++;
			}
			Expander exp1 = new Expander("General");
			exp1.Expanded = true;
			exp1.Add(propertyTable);

			Expander exp2 = new Expander("Conditions");
			exp2.Expanded = true;
			exp2.Add(conditionsTable);

			mainTable.Attach(exp1,0,1,0,1,AttachOptions.Expand|AttachOptions.Fill,AttachOptions.Fill,0,0);
			mainTable.Attach(exp2,0,1,1,2,AttachOptions.Expand|AttachOptions.Fill,AttachOptions.Fill,0,0);
			
			this.PackStart(mainTable,true,true,0);
			this.ShowAll();
		}

		private void GenerateContent(ref Gtk.Table tableSystem, string label, int xPos, Condition cd,bool isResolution)
		{
			
			ListStore lstorePlatform = new ListStore(typeof(int),typeof(int),typeof(string));
			
			int selectRule = 0;
			if (fiOld.ConditionValues.Count > 0) {
				ConditionRule cr = fiOld.ConditionValues.Find(x => x.ConditionId == cd.Id);
				if (cr != null)
					selectRule = cr.RuleId;
			}
			
			Label lblPlatform = new Label(label);
			lblPlatform.Name = "lbl_" + label;
			lblPlatform.Xalign = 1;
			lblPlatform.Yalign = 0.5F;
			lblPlatform.WidthRequest = 100;
			
			ComboBox cboxPlatform = new ComboBox();
			cboxPlatform.Name = "cd_" + label;
			
			CellRendererText textRenderer = new CellRendererText();
			cboxPlatform.PackStart(textRenderer, true);
			cboxPlatform.AddAttribute(textRenderer, "text", 2);
			
			//cboxPlatform.WidthRequest = 200;
			cboxPlatform.Model = lstorePlatform;

			tableSystem.Attach(lblPlatform, 0, 1, (uint)(xPos - 1), (uint)xPos, AttachOptions.Shrink, AttachOptions.Shrink, 2, 2);
			tableSystem.Attach(cboxPlatform, 1, 2, (uint)(xPos - 1), (uint)xPos, AttachOptions.Expand|AttachOptions.Fill, AttachOptions.Shrink, 2, 2);
			
			TreeIter selectIter = lstorePlatform.AppendValues(0, cd.Id, "Unset");
			
			foreach (Rule rl in cd.Rules){
				
				if(!isResolution){
					
					if (rl.Id == selectRule)
						selectIter = lstorePlatform.AppendValues(rl.Id, cd.Id, rl.Name);
					else
						lstorePlatform.AppendValues(rl.Id, cd.Id, rl.Name);
				} else {
					string name  = String.Format("{0} ({1}x{2})",rl.Name,rl.Width,rl.Height);
					if (rl.Id == selectRule)
						selectIter = lstorePlatform.AppendValues(rl.Id, cd.Id, name);
					else
						lstorePlatform.AppendValues(rl.Id, cd.Id, name);
				}
			}
			cboxPlatform.SetActiveIter(selectIter);

			cboxPlatform.Changed += delegate(object sender, EventArgs e) {
				
				if (sender == null)
					return;
				
				ComboBox combo = sender as ComboBox;
				
				TreeIter iter;
				if (combo.GetActiveIter(out iter)) {
					string fullPath = MainClass.Workspace.GetFullPath(fiOld.FilePath);
					int ruleId = (int)combo.Model.GetValue(iter, 0);
					int condId = (int)combo.Model.GetValue(iter, 1);
					if (ruleId != 0){
						ConditionRule cr = fiOld.ConditionValues.Find(x => x.ConditionId == condId);//cd.Id);
						if (cr != null)
							cr.RuleId = ruleId;
						else
							fiOld.ConditionValues.Add(new ConditionRule(condId,ruleId));
						//setConditionsRecursive(fullPath,cr, false);
					}
					else {
						ConditionRule cr = fiOld.ConditionValues.Find(x => x.ConditionId == condId);//cd.Id);
						if (cr != null){
							fiOld.ConditionValues.Remove(cr);
							//setConditionsRecursive(fullPath,cr, true);
						}
					}
					setConditionsRecursive(fullPath,condId, ruleId);
				}
				
			};
		}

		private Label GetLabel(string text){
			Label label = new Label(text);
			label.UseUnderline = false;
			label.Selectable = true;
			label.Xalign = 0;
			return label;
		}

		private void setConditionsRecursive(string path,int condId, int ruleId)
		{
			if (!Directory.Exists(path))
				return;
			
			DirectoryInfo di = new DirectoryInfo(path);
			
			foreach (DirectoryInfo d in di.GetDirectories()){
				int indx = -1;
				indx = MainClass.Settings.IgnoresFolders.FindIndex(x => x.Folder == d.Name && x.IsForIde);
				if(indx<0){
					string relativePath = MainClass.Workspace.GetRelativePath(d.FullName);
					FileItem fi = project.FilesProperty.Find(x => x.SystemFilePath == relativePath);
					if(fi == null){
						fi = new FileItem(relativePath, false);
						fi.ConditionValues = new List<ConditionRule>();
						fi.IsDirectory = true;
						project.FilesProperty.Add(fi);
					}


					if(fi.ConditionValues == null)
						fi.ConditionValues = new List<ConditionRule>();
					
					ConditionRule cr = fi.ConditionValues.Find(x => x.ConditionId == condId);
					if (ruleId != 0){
						if (cr != null)
							cr.RuleId = ruleId;
						else
							fi.ConditionValues.Add(new ConditionRule(condId,ruleId));
					}
					else {
						//cd.Id);
						if (cr != null){
							fi.ConditionValues.Remove(cr);
						}
					}
					setConditionsRecursive(d.FullName,condId, ruleId);
				}
			}
			
			foreach (FileInfo f in di.GetFiles()){
				if (!MainClass.Tools.IsIgnoredExtension(f.Extension)) {
				
					int indx = -1;
					indx = MainClass.Settings.IgnoresFiles.FindIndex(x => x.Folder == f.Name && x.IsForIde);
					if(indx >-1)continue; 


					string relativePath = MainClass.Workspace.GetRelativePath(f.FullName);
					FileItem fi = project.FilesProperty.Find(x => x.SystemFilePath == relativePath);
					if (fi == null) {
						fi = new FileItem(relativePath, false);
						fi.ConditionValues = new List<ConditionRule>();
						project.FilesProperty.Add(fi);
					}

					if(fi.ConditionValues == null)
						fi.ConditionValues = new List<ConditionRule>();
					
					ConditionRule cr = fi.ConditionValues.Find(x => x.ConditionId == condId);
					if (ruleId != 0){
						if (cr != null)
							cr.RuleId = ruleId;
						else
							fi.ConditionValues.Add(new ConditionRule(condId,ruleId));
					}
					else {
						if (cr != null){
							fi.ConditionValues.Remove(cr);
						}
					}
				
				}
			}
		}

		private void AddControl(ref Table tbl, int top, Widget ctrl, string label){
			Label lbl = new Label(label);
			lbl.Xalign = 1;
			lbl.Yalign = 0.5F;
			lbl.WidthRequest = 100;
			
			tbl.Attach(lbl,0,1,(uint)top,(uint)(top+1),AttachOptions.Shrink,AttachOptions.Shrink,2,2);
			tbl.Attach(ctrl,1,2,(uint)top,(uint)(top+1),AttachOptions.Fill|AttachOptions.Expand,AttachOptions.Fill,2,2);
		}
	}
}

