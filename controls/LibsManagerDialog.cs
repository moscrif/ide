using System;
using System.IO;
using System.Collections.Generic;
using Gtk;
using Moscrif.IDE.Workspace;
using Moscrif.IDE.Devices;
using Moscrif.IDE.Components;
using MessageDialogs = Moscrif.IDE.Controls.MessageDialog;
using Moscrif.IDE.Iface.Entities;

namespace Moscrif.IDE.Controls
{
	public partial class LibsManagerDialog : Gtk.Dialog
	{
		private List<string> projectLibs;
		Gtk.ListStore libstListStore = new Gtk.ListStore(typeof(string), typeof(string), typeof(bool), typeof(bool),typeof(string));

		private string libsInProject;

		public LibsManagerDialog(string libsInProject,Gtk.Window parentWindow)
		{
			this.TransientFor = parentWindow;
			this.Build();
			this.libsInProject = libsInProject;
			//this.TransientFor = MainClass.MainWindow;

			if(!String.IsNullOrEmpty(libsInProject)){
				libsInProject = libsInProject.Replace(",", " ");
				string[] libs = libsInProject.Split(' ');
				projectLibs = new List<string>(libs);

			} else {
				projectLibs = new List<string>();
			}
			tvLibs.Model = libstListStore;

			CellRendererToggle crt = new CellRendererToggle();
			crt.Activatable = true;
			crt.Toggled += delegate(object o, ToggledArgs args) {
				TreeIter iter;
				if (libstListStore.GetIter (out iter, new TreePath(args.Path))) {

					bool old = (bool) libstListStore.GetValue(iter,2);
				 	string libSelect = (string) libstListStore.GetValue(iter,0);
					bool libMissing = (bool) libstListStore.GetValue(iter,3);
					string frameworkLibPath = (string) libstListStore.GetValue(iter,4);
					string state = (string) libstListStore.GetValue(iter,1);

					if(!MainClass.LicencesSystem.CheckFunction(libSelect,this)){
						return;
					}

					int resultAction =0;

					if(old){
						projectLibs.Remove(libSelect);
					}
					else{

						if(libMissing){

							MessageDialogs md =
								new MessageDialogs(new string[]{MainClass.Languages.Translate("as_link"),MainClass.Languages.Translate("copy_f2"),MainClass.Languages.Translate("cancel")}, MainClass.Languages.Translate("add_library_to_workspace",libSelect), "", Gtk.MessageType.Question,this);
							resultAction = md.ShowDialog();

							//Console.WriteLine("resultAction->{0}",resultAction);

							switch (resultAction)
							{
							    case -1: //"As Link"
								try{
									MainClass.Tools.CreateLinks(frameworkLibPath,MainClass.Workspace.RootDirectory, true,true);
									state = MainClass.Languages.Translate("linked");
								} catch (Exception ex){
									MessageDialogs mdEror =
										new MessageDialogs(MessageDialogs.DialogButtonType.Ok, ex.Message,"", Gtk.MessageType.Error);
									mdEror.ShowDialog();
									return ;
								}
								break;
							    case -2://"Copy"
								string fullLibDir = System.IO.Path.Combine(MainClass.Workspace.RootDirectory,libSelect);
								MainClass.Tools.CopyDirectory(frameworkLibPath,fullLibDir, true, true);
								state = MainClass.Languages.Translate("copied");
							        break;
							    default: //"Cancel"
							        break;
							}
						}
						projectLibs.Add(libSelect);

					}
					//LoadLibs();
					libstListStore.SetValue(iter,2,!old);
					libstListStore.SetValue(iter,1,state);
					//}
				}
			};


			tvLibs.AppendColumn("", crt , "active", 2);
			tvLibs.AppendColumn(MainClass.Languages.Translate("lib_name"), new Gtk.CellRendererText(), "text", 0);
			tvLibs.AppendColumn(MainClass.Languages.Translate("workspace_location"), new Gtk.CellRendererText(), "text", 1);

			LoadLibs();

		}

		private void LoadLibs(){
			libstListStore.Clear();

			DirectoryInfo dirWorkspace = new DirectoryInfo(MainClass.Workspace.RootDirectory);
			DirectoryInfo[] di = dirWorkspace.GetDirectories("*",SearchOption.TopDirectoryOnly);
			List<DirectoryInfo> listWorkspaceLibs = new List<DirectoryInfo>(di);

			foreach (string lib in MainClass.Settings.LibsDefine) {
				bool isSelect = false;
				if(projectLibs.Count>0){
					int i = projectLibs.FindIndex(x=>x==lib.Trim());
					if (i>-1){
						isSelect = true;
					}
				}

				string state = MainClass.Languages.Translate("not_included");
				bool missing = true;
				DirectoryInfo libsDirectory = listWorkspaceLibs.Find(x=> x.Name == lib);

				if(libsDirectory != null){
					missing = false;
					state = MainClass.Languages.Translate("copied");
					if ((libsDirectory.Attributes & FileAttributes.ReparsePoint) == FileAttributes.ReparsePoint){// links
						//Console.WriteLine(System.IO.Path.GetFullPath(libsDirectory.FullName));
						state = MainClass.Languages.Translate("linked");
					}
				}

				string framneworkLibPath = System.IO.Path.Combine(MainClass.Settings.LibDirectory,lib);
				if(!Directory.Exists(framneworkLibPath ) ){
					missing = false;
					state = MainClass.Languages.Translate("error_missing_libs");
				}

				Gtk.TreeIter ti = libstListStore.AppendValues(lib, state,isSelect,missing,framneworkLibPath);
			}
		}


		public string LibsString{
			get{
				if((projectLibs == null ) || (projectLibs.Count< 1))
					return "";
				string res = string.Join(" ", projectLibs.ToArray());
				return res;
			}
		}



	}
}

