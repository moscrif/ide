using System;
using Gtk;
using Moscrif.IDE.Workspace;
namespace Moscrif.IDE.Settings
{
	internal class  FileGeneralPanel : OptionsPanel
	{
		FileGeneralWidget widget;
		FilePropertisData fcd ;

		public override Widget CreatePanelWidget()
		{
			return widget = new FileGeneralWidget(fcd);
		}

		public override void ShowPanel()
		{
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
			if (dataObject.GetType() == typeof(FilePropertisData))
				fcd=(FilePropertisData)dataObject;
		}

		public override string Label {
			get { return MainClass.Languages.Translate("general_preferencies_file"); }
		}

		public override string Icon {
			get { return "file-properties.png"; }
		}
	}


	public partial class FileGeneralWidget : Gtk.Bin
	{
		FilePropertisData fcd ;
		Project project;
		FileItem fi_old;
		string filepath;

		public FileGeneralWidget(FilePropertisData fpd )
		{
			this.Build();

			this.fcd = fpd;
			project = fcd.Project;
			filepath = fcd.Filename;

			fi_old = project.FilesProperty.Find(x => x.SystemFilePath == filepath);
			if (fi_old == null){
				fi_old =new FileItem(filepath,false);
				project.FilesProperty.Add(fi_old);
			}

			lblName.LabelProp = System.IO.Path.GetFileName(filepath);
			lblRelativePath.LabelProp = filepath;
			lblProject.LabelProp =project.ProjectName;
			lblFullPath.LabelProp = MainClass.Workspace.GetFullPath(filepath);
			chbExclude.Active = fi_old.IsExcluded;
		}

		public void Store()
		{
			fi_old.IsExcluded=chbExclude.Active;
		}
	}
}

