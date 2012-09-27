using System;
using System.IO;
using Moscrif.IDE.Workspace;
using Gtk;

namespace Moscrif.IDE.Controls
{
	public partial class CreateThemeDialog : Gtk.Dialog
	{
		Gtk.ListStore skinListStore = new Gtk.ListStore(typeof(string), typeof(string));
		private string skinDir;
		private string themeDir ;

		private string themePath;
		private string skinPath;

		public CreateThemeDialog(Project project )
		{
			this.Build();
			cbSkin.Model = skinListStore;
			skinDir = System.IO.Path.Combine(MainClass.Workspace.RootDirectory, "skin");
			themeDir = System.IO.Path.Combine(project.AbsolutProjectDir, MainClass.Settings.ThemeDir);

			FillSkinCombo(skinDir);
		}

		private void FillSkinCombo(string path)
		{
			if (!Directory.Exists(path))
				return;

			DirectoryInfo di = new DirectoryInfo(path);

			int i = 0;
			foreach (DirectoryInfo d in di.GetDirectories()) {

				int indx = -1;
				indx = MainClass.Settings.IgnoresFolders.FindIndex(x => x.Folder == d.Name && x.IsForIde);

				if (indx > -1)
					continue;

					skinListStore.AppendValues(d.Name, d.FullName);
			}
			cbSkin.Active = i;
		}

		protected virtual void OnButtonOkClicked (object sender, System.EventArgs e)
		{

			if (String.IsNullOrEmpty(entrThemeName.Text)) return;

			TreeIter iter;
			if (cbSkin.GetActiveIter(out iter)) {
				string skin = (string)cbSkin.Model.GetValue(iter, 0);
				skinPath = (string)cbSkin.Model.GetValue(iter, 1);

				string themeName = skin+"."+entrThemeName.Text;

				if(Directory.Exists(themeDir)){
					int cnt = Directory.GetDirectories(themeDir,themeName).Length;
					if (cnt>0)return;
				}

				themePath = System.IO.Path.Combine(themeDir,themeName);
				this.Respond(ResponseType.Ok);
			}

		}

		public string ThemePath{
			get{
				return themePath;

			}
		}

		public string SkinPath{
			get{
				return skinPath;

			}
		}

	}
}

