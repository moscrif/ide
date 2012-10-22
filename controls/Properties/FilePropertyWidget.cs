using System;
using Gtk;
using Moscrif.IDE.Workspace;
using Moscrif.IDE.Settings;
using Moscrif.IDE.Iface.Entities;
using System.IO;

namespace Moscrif.IDE.Controls.Properties
{
	public class FilePropertyWidget : Gtk.HBox
	{
		private FileItem fiOld;
		private FilePropertisData fpdOld;

		public FilePropertyWidget(FilePropertisData fpd)
		{
			fpdOld = fpd;
			Project project = fpd.Project;
			fiOld = project.FilesProperty.Find(x => x.SystemFilePath == fpd.Filename);
			if (fiOld == null){
				fiOld =new FileItem(fpd.Filename,false);
				project.FilesProperty.Add(fiOld);
			}
			if (fiOld.ConditionValues == null)
				fiOld.ConditionValues = new System.Collections.Generic.List<ConditionRule>();

			string fullPath = MainClass.Workspace.GetFullPath(fiOld.FilePath);

			string size = "0";
			FileInfo fi = new FileInfo(fullPath);
			if(fi.Exists){
				size =((int)(fi.Length/1024)).ToString() + " KB";
			}

			Table mainTable = new Table(4,1,false);
			Table propertyTable = new Table(5,2,false);

			Label lblFN =GetLabel(System.IO.Path.GetFileName(fiOld.SystemFilePath)); //new Label(System.IO.Path.GetFileName(fiOld.SystemFilePath));
			Label lblSZ =GetLabel(System.IO.Path.GetFileName(size));// new Label(System.IO.Path.GetFileName(size));

			Entry entr = new Entry(fiOld.SystemFilePath);
			entr.IsEditable = false;

			Entry entrFullPath = new Entry(fullPath);
			entrFullPath.IsEditable = false;

			Label lblPrj =GetLabel(project.ProjectName);// new Label(project.ProjectName);


			CheckButton chbExclude = new CheckButton("");
			chbExclude.Active = fiOld.IsExcluded;

			chbExclude.Clicked+=  delegate(object sender, EventArgs e)
			{
				fiOld.IsExcluded = chbExclude.Active;
			};

			AddControl(ref propertyTable,0,lblFN,"Name ");
			AddControl(ref propertyTable,1,lblSZ,"Size ");
			AddControl(ref propertyTable,2,entr,"Relative Path ");
			AddControl(ref propertyTable,3,entrFullPath,"Full Path ");
			AddControl(ref propertyTable,4,lblPrj,"Project ");
			AddControl(ref propertyTable,5,chbExclude,"Exclude ");

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

			string extension = System.IO.Path.GetExtension(fpd.Filename);
			switch (extension) {
			case ".png":
			case ".jpg":
			case ".jpeg":
			case ".bmp":
			case ".gif":
			case ".tif":
			case ".svg":{
				Table ImageTable =GenerateImageControl();
				if(ImageTable!= null){
					Expander expImage = new Expander("Image");
					expImage.Expanded = true;
					expImage.Add(ImageTable);
					mainTable.Attach(expImage,0,1,2,3,AttachOptions.Expand|AttachOptions.Fill,AttachOptions.Fill,0,0);
				}
				break;
			}
			case ".db":
				//
				break;
			default:
				//
				break;
			}


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
			cboxPlatform.Changed += delegate(object sender, EventArgs e) {
				
				if (sender == null)
					return;
				
				ComboBox combo = sender as ComboBox;
				
				TreeIter iter;
				if (combo.GetActiveIter(out iter)) {
					int ruleId = (int)combo.Model.GetValue(iter, 0);
					int condId = (int)combo.Model.GetValue(iter, 1);
					if (ruleId != 0){
						ConditionRule cr = fiOld.ConditionValues.Find(x => x.ConditionId == condId);//cd.Id);
						if (cr != null)
							cr.RuleId = ruleId;
						else
							fiOld.ConditionValues.Add(new ConditionRule(condId,ruleId));
					}
					else {
						ConditionRule cr = fiOld.ConditionValues.Find(x => x.ConditionId == condId);//cd.Id);
						if (cr != null){
							fiOld.ConditionValues.Remove(cr);
						}
					}
				}
				
			};
			
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
		}

		private void AddControl(ref Table tbl, int top, Widget ctrl, string label){
			Label lbl = new Label(label);
			lbl.Xalign = 1;
			lbl.Yalign = 0.5F;
			lbl.WidthRequest = 100;

			tbl.Attach(lbl,0,1,(uint)top,(uint)(top+1),AttachOptions.Shrink,AttachOptions.Shrink,2,2);
			tbl.Attach(ctrl,1,2,(uint)top,(uint)(top+1),AttachOptions.Fill|AttachOptions.Expand,AttachOptions.Fill,2,2);
		}

		private Label GetLabel(string text){
			Label label = new Label(text);
			label.UseUnderline = false;
			label.Selectable = true;
			label.Xalign = 0;
			return label;
		}

		private Table GenerateImageControl(){
			string imagePath = MainClass.Workspace.GetFullPath(fiOld.FilePath);
			Table imageTable = new Table(4,2,false);
			int height = 0;
			int width = 0;

			Gdk.Pixbuf bg;
			try{
				using (var fs = new System.IO.FileStream(imagePath, System.IO.FileMode.Open))
					bg = new Gdk.Pixbuf(fs);
				
				//bg = bg.ApplyEmbeddedOrientation();
				height = bg.Height;
				width= bg.Width;
			}catch(Exception ex){
				Tool.Logger.Error(ex.Message,null);
				Console.WriteLine(ex.Message);
				return null;
			}
			Label lblDimensions = GetLabel(width.ToString()+ " x "+height.ToString());//new Label(width.ToString()+ " x "+height.ToString() );
			Label lblWidth =GetLabel(width.ToString()+ " pixels"); //new Label();
			Label lblHeight = GetLabel(height.ToString()+ " pixels");//new Label(height.ToString()+ " pixels");

			Image img= new Image(imagePath);
			img.Xalign = 0;

			int newWidth =0;
			int newHeight = 0;
			MainClass.Tools.RecalculateImageSize(width,height,75,75,ref newWidth,ref newHeight);
			img.Pixbuf = img.Pixbuf.ScaleSimple(newWidth,newHeight, Gdk.InterpType.Bilinear);

			AddControl(ref imageTable,0,lblDimensions,"Dimensions ");
			AddControl(ref imageTable,1,lblWidth,"Width ");
			AddControl(ref imageTable,2,lblHeight,"Height ");
			AddControl(ref imageTable,3,img,"Preview ");
		
			return imageTable;
		}

	}
}

