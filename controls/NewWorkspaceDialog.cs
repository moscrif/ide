using System;
using Gtk;
using MessageDialogs = Moscrif.IDE.Controls.MessageDialog;
using Moscrif.IDE.Components;

namespace Moscrif.IDE.Controls
{
	public partial class NewWorkspaceDialog : Gtk.Dialog
	{
		private SkinThemeControl skinthemecontrol1;
		private FileMaskEntry feOutput ;
		private bool project = false;
		public string  WorkspaceName {
			get {
				return  MainClass.Tools.RemoveDiacritics(this.entWorkspace.Text).Replace(" ","_");
			}
		}

		public string  WorkspaceOutput {
			get {return this.feOutput.Path; }
		}

		public string  WorkspaceRoot {
			get {
				if (cbSubFolder.Active )
					return System.IO.Path.Combine(this.feRoot.Path,WorkspaceName);
				return this.feRoot.Path;
			}
		}

		public string  ProjectName {
			get {
				return MainClass.Tools.RemoveDiacritics(this.entrProjectName.Text).Replace(" ","_");
			}
		}

		public string Skin {
			get {
				return this.skinthemecontrol1.GetSkin();
			}
		}

		public string Theme {
			get {
				return this.skinthemecontrol1.GetTheme();
			}
		}


		public bool CopyLibs{
			get {return this.cbCopyLibs.Active; }
		}

		protected virtual void OnButtonOkClicked (object sender, System.EventArgs e)
		{
			if(WorkspaceName.Contains(" ")){
				MessageDialogs md =
				new MessageDialogs(MessageDialogs.DialogButtonType.Ok, MainClass.Languages.Translate("error_whitespace_work"),"", Gtk.MessageType.Error);
				md.ShowDialog();
				return;
			}

			if(WorkspaceRoot.Contains(" ")){
				MessageDialogs md =
				new MessageDialogs(MessageDialogs.DialogButtonType.Ok, MainClass.Languages.Translate("error_whitespace_work_path"),"", Gtk.MessageType.Error);
				md.ShowDialog();
				return;
			}


			if  (String.IsNullOrEmpty(WorkspaceName)){
					MessageDialogs md =
					new MessageDialogs(MessageDialogs.DialogButtonType.Ok, MainClass.Languages.Translate("please_set_workspace_name"),"", Gtk.MessageType.Error);
					md.ShowDialog();
					return;
			}

			if  (String.IsNullOrEmpty(WorkspaceOutput)){
					MessageDialogs md =
					new MessageDialogs(MessageDialogs.DialogButtonType.Ok, MainClass.Languages.Translate("please_set_workspace_output"),"", Gtk.MessageType.Error);
					md.ShowDialog();
					return;
			}

			if  (String.IsNullOrEmpty(WorkspaceRoot)){
					MessageDialogs md =
					new MessageDialogs(MessageDialogs.DialogButtonType.Ok, MainClass.Languages.Translate("please_set_workspace_root"),"", Gtk.MessageType.Error);
					md.ShowDialog();
					return;
			}

			if  (project){

				if  (String.IsNullOrEmpty(ProjectName)){
					MessageDialogs md =
					new MessageDialogs(MessageDialogs.DialogButtonType.Ok, MainClass.Languages.Translate("please_set_project_name"),"", Gtk.MessageType.Error);
					md.ShowDialog();
					return;
				}

				if(ProjectName.Contains(" ")){
					MessageDialogs md =
					new MessageDialogs(MessageDialogs.DialogButtonType.Ok, MainClass.Languages.Translate("error_whitespace_proj"),"", Gtk.MessageType.Error);
					md.ShowDialog();
					return ;
				}
			}

				this.Respond(ResponseType.Ok);
		}


		protected void OnEntWorkspaceKeyReleaseEvent (object o, Gtk.KeyReleaseEventArgs args)
		{
			if (args.Event.Key == Gdk.Key.Return){
				OnButtonOkClicked(null,null);
			}
		}

		protected void OnEntrProjectNameKeyReleaseEvent (object o, Gtk.KeyReleaseEventArgs args)
		{
			if (args.Event.Key == Gdk.Key.Return){
				OnButtonOkClicked(null,null);
			}
		}

	        private void OnConfigureGeometry (object o, EventArgs args)
	        {
	            var limits = new Gdk.Geometry () {
	                MinWidth = SizeRequest ().Width,
	                MaxWidth = Gdk.Screen.Default.Width
	            };

	            if (expander1.Expanded) {
	                limits.MinHeight = SizeRequest().Height + table2.SizeRequest().Height;
	                limits.MaxHeight = Gdk.Screen.Default.Height;
	               // limits.MinWidth = SizeRequest().Width + table2.SizeRequest().Width;
	               // limits.MaxWidth = Gdk.Screen.Default.Width;

	            } else {
	                limits.MinHeight = -1;
	                limits.MaxHeight = -1;
	                //limits.MinWidth = -1;
	                //limits.MaxWidth = -1;
	            }

	            SetGeometryHints (this, limits,
	                Gdk.WindowHints.MaxSize | Gdk.WindowHints.MinSize);
	        }


		public NewWorkspaceDialog(bool project)
		{
			this.TransientFor = MainClass.MainWindow;
			this.Build();
			skinthemecontrol1 = new SkinThemeControl();
			vbox3.PackEnd(skinthemecontrol1,false,true,0);
			skinthemecontrol1.ShowAll();
			expander1.Activated += OnConfigureGeometry;


			feOutput = new FileMaskEntry(MainClass.Settings.WorkspaceMaskDirectory, MainClass.Workspace,this);
			feOutput.IsFolder = true;
			//Console.WriteLine(" MainClass.Paths.WorkDir-> {0}", MainClass.Paths.WorkDir);
			if(!System.IO.Directory.Exists(MainClass.Paths.WorkDir)){
				try{
					System.IO.Directory.CreateDirectory(MainClass.Paths.WorkDir);
				}catch(Exception ex){
					Moscrif.IDE.Tool.Logger.Error(MainClass.Languages.Translate("work_dir_not_creat"));
					Moscrif.IDE.Tool.Logger.Error(ex.Message);
				}
			}

			feRoot.DefaultPath = MainClass.Paths.WorkDir;
			feOutput.DefaultPath = MainClass.Paths.WorkDir;
			feOutput.VisiblePath = System.IO.Path.Combine(MainClass.Settings.WorkspaceMaskDirectory[0],"output");

			if (project){
				vbox3.Visible = true;
				trmProject.Visible = true;
				//hseparator1.Visible = true;
				//expander2.Expanded = true;
				skinthemecontrol1.SetLabelWidth(85);
			} else {
				this.Resize(450,90);
			}
			table2.Attach(feOutput,1,2,0,1,AttachOptions.Fill,AttachOptions.Shrink,5,0);

			this.project = project;
		}

		protected void OnExpander1Activated (object sender, EventArgs e)
		{
			/*Console.WriteLine(expander1.Expanded);
			if(!expander1.Expanded){
				this.Resize(300,70);
				this.QueueDraw();
				this.QueueResize();
			}*/
		}


	}
}

