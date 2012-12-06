using System;
using System.IO;
using Gtk;
using Gdk;
using Moscrif.IDE.Iface;
using System.Collections;
using System.Collections.Generic;

namespace Moscrif.IDE.Components
{
	public class NavigationBar  : Gtk.HBox
	{

		public enum NavigationType {
			favorites,
			libs,
			publish,
			emulator

		}

		ComboBoxEntry cbeNavigation;
		Button btnFavorite;
		Pixbuf pixbufYes = null;
		Pixbuf pixbufNo = null;
		ListStore navigationStore = new ListStore(typeof(string));

		private NavigationType navigationType;

		public delegate void ChangePathHandler(string newPath);
		public event ChangePathHandler OnChangePath;

		public NavigationBar(): this(NavigationType.favorites ){}

		public NavigationBar(NavigationType navigationType)
		{
			this.navigationType = navigationType;
			cbeNavigation = new ComboBoxEntry();
			cbeNavigation.Entry.Completion = new EntryCompletion ();

			cbeNavigation.Entry.KeyReleaseEvent += cbeNavigationKeyReleas;

			cbeNavigation.Events = EventMask.AllEventsMask;

			cbeNavigation.Entry.Completion.Model = navigationStore;
			cbeNavigation.Model = navigationStore;
			cbeNavigation.Entry.ActivatesDefault = true;
			cbeNavigation.TextColumn = 0;

			IList<RecentFile> lRecentProjects; 
			switch (this.navigationType)
			{
			   case NavigationType.favorites:
				lRecentProjects = MainClass.Settings.RecentFiles.GetFavorite();
				break;
			   case NavigationType.libs:
				lRecentProjects = MainClass.Settings.FavoriteFiles.GetLibsFavorite();
				break;
			   case NavigationType.publish:
				lRecentProjects = MainClass.Settings.FavoriteFiles.GetPublishFavorite();
				break;
			   case NavigationType.emulator:
				lRecentProjects = MainClass.Settings.FavoriteFiles.GetEmulatorFavorite();
				break;
			   default:
			      	lRecentProjects = MainClass.Settings.RecentFiles.GetFavorite();
				break;
			}			


				//= this.recentFiles.GetFavorite();//MainClass.Settings.RecentFiles.GetFavorite();
			foreach(RecentFile rf in lRecentProjects){
				navigationStore.AppendValues(rf.FileName);
		
			}
			cbeNavigation.Changed+= cbeNavigationChanged;

			pixbufYes = null;
			string fileYes = System.IO.Path.Combine(MainClass.Paths.ResDir, "starSelect.png");
			string fileNo = System.IO.Path.Combine(MainClass.Paths.ResDir, "starUnselect.png");

			if (System.IO.File.Exists(fileYes)) {
				pixbufYes = new Pixbuf(fileYes);
				btnFavorite = new Button(new Gtk.Image(pixbufYes));
			}
			if (System.IO.File.Exists(fileNo)) {
				pixbufNo = new Pixbuf(fileNo);
				btnFavorite = new Button(new Gtk.Image(pixbufNo));
			} else {
				btnFavorite = new Button();
			}

			btnFavorite.TooltipText = MainClass.Languages.Translate("close");
			btnFavorite.Relief = ReliefStyle.None;
			btnFavorite.CanFocus = false;
			btnFavorite.WidthRequest = btnFavorite.HeightRequest =24;

			btnFavorite.Clicked+= btnFavoriteClick;  

			PackEnd (cbeNavigation, true, true, 0);
			PackStart (btnFavorite, false, false, 0);
		}
		

		void cbeNavigationChanged (object sender, EventArgs e)
		{
			if(cbeNavigation.Active >-1){
				if(OnChangePath != null){
					if(Directory.Exists(cbeNavigation.ActiveText)){
						OnChangePath(cbeNavigation.ActiveText);
					}
				}
			}
		}

		void cbeNavigationKeyReleas (object o, KeyReleaseEventArgs args)
		{
			if (args.Event.Key == Gdk.Key.Return){
				if(OnChangePath != null){
					if(Directory.Exists(cbeNavigation.ActiveText)){
						OnChangePath(cbeNavigation.ActiveText);
					}
				}
			}
		}


		//D:\Work\test\MoscrifSDK-Windows\framework
		void btnFavoriteClick (object sender, EventArgs e)
		{
			string txt = cbeNavigation.ActiveText;

			if(!CheckFavorites(txt)){

				//IList<RecentFile> lRecentProjects; 
				switch (this.navigationType)
				{
				   case NavigationType.favorites:
					MainClass.Settings.RecentFiles.AddFavorite(txt,txt);
					break;
				   case NavigationType.libs:
					MainClass.Settings.FavoriteFiles.AddLibsFavorite(txt,txt);
					break;
				   case NavigationType.publish:
					MainClass.Settings.FavoriteFiles.AddPublishFavorite(txt,txt);
					break;
				   case NavigationType.emulator:
					MainClass.Settings.FavoriteFiles.AddEmulatorFavorite(txt,txt);
					break;
				}
				//this.recentFiles.AddFavorite(txt,txt);

				//MainClass.Settings.RecentFiles.AddFavorite(txt,txt);
				btnFavorite.Image = new Gtk.Image(pixbufYes);
				navigationStore.AppendValues(txt);
			} else {
				//this.recentFiles.NotifyFileRemoved(txt);
				switch (this.navigationType)
				{
				   case NavigationType.favorites:
					MainClass.Settings.RecentFiles.NotifyFileRemoved(txt);
					break;
				   case NavigationType.libs:
					MainClass.Settings.FavoriteFiles.NotifyFileRemoved(txt);
					break;
				   case NavigationType.publish:
					MainClass.Settings.FavoriteFiles.NotifyFileRemoved(txt);
					break;
				   case NavigationType.emulator:
					MainClass.Settings.FavoriteFiles.NotifyFileRemoved(txt);
					break;
				}

				btnFavorite.Image = new Gtk.Image(pixbufNo);

				//bool isFind = false;
				TreeIter ti = new TreeIter();
				navigationStore.Foreach((model, path, iterr) => {
					string pathProject = navigationStore.GetValue(iterr,0).ToString();

					if (pathProject.ToUpper() == txt.ToUpper()){
						ti = iterr;
						//isFind = true;
						return true;
					}
						return false;
				});

				navigationStore.Remove(ref ti);
			}

		}

		public string  ActivePath{
			get{
				return cbeNavigation.ActiveText;
			}

		}

		private bool CheckFavorites(string txt){


			//List<RecentFile> lRecentProjects = (List<RecentFile>)this.recentFiles.GetFavorite();
			List<RecentFile> lRecentProjects;
			switch (this.navigationType)
			{
			   case NavigationType.favorites:
				lRecentProjects = (List<RecentFile>)MainClass.Settings.RecentFiles.GetFavorite();
				break;
			   case NavigationType.libs:
				lRecentProjects = (List<RecentFile>)MainClass.Settings.FavoriteFiles.GetLibsFavorite();
				break;
			   case NavigationType.publish:
				lRecentProjects = (List<RecentFile>)MainClass.Settings.FavoriteFiles.GetPublishFavorite();
				break;
			   case NavigationType.emulator:
				lRecentProjects = (List<RecentFile>)MainClass.Settings.FavoriteFiles.GetEmulatorFavorite();
				break;
			   default:
			      	lRecentProjects = (List<RecentFile>)MainClass.Settings.RecentFiles.GetFavorite();
				break;
			}

			//List<RecentFile> lRecentProjects = (List<RecentFile>)MainClass.Settings.RecentFiles.GetFavorite();

			int indx =  lRecentProjects.FindIndex(x=>x.FileName.ToUpper()==txt.ToUpper());
			if(indx<0){
				btnFavorite.Image = new Gtk.Image(pixbufNo);
				return false;
			} else {
				btnFavorite.Image = new Gtk.Image(pixbufYes);
				return true;
			}

		}

		public void SetPath(string path){
			CheckFavorites(path);
			cbeNavigation.Entry.Text = path;
		}

	}
}

