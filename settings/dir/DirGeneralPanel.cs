using System;
using Gtk;
using Moscrif.IDE.Workspace;
namespace Moscrif.IDE.Settings
{

	internal class  DirGeneralPanel : OptionsPanel
	{
		DirGeneralWidget widget;
		FilePropertisData fpd ;

		public override Widget CreatePanelWidget()
		{
			return widget = new DirGeneralWidget(fpd);
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
				fpd=(FilePropertisData)dataObject;
		}

		public override string Label {
			get { return MainClass.Languages.Translate("general_preferencies_dir"); }
		}

		public override string Icon {
			get { return "folder-properties.png"; }
		}
	}

	public partial class DirGeneralWidget : Gtk.Bin
	{
		FilePropertisData fpd ;
		Project project;
		FileItem fi_old;
		string filepath;

		public DirGeneralWidget(FilePropertisData fpd)
		{
			this.Build();

			this.fpd = fpd;
			project = this.fpd.Project;
			filepath = this.fpd.Filename;

			fi_old = project.FilesProperty.Find(x => x.SystemFilePath == filepath);
			if (fi_old == null){
				fi_old =new FileItem(filepath,false);
				project.FilesProperty.Add(fi_old);
			}

			lblName.LabelProp = System.IO.Path.GetFileName(filepath);
			lblRelativePath.LabelProp = filepath;
			lblProject.LabelProp =project.ProjectName;
			lblFullPath.LabelProp = MainClass.Workspace.GetFullPath(filepath);

		}

		public void Store()
		{

		}
	}
}

