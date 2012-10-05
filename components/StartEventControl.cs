using System;
using System.Xml;
using System.Net;
using System.IO;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using Moscrif.IDE.Actions;
using Moscrif.IDE.Settings;
using Moscrif.IDE.Iface.Entities;
using Moscrif.IDE.Tool;
using Gtk;
using Gdk;
using MessageDialogs = Moscrif.IDE.Controls.MessageDialog;
using Moscrif.IDE.Iface;


namespace Moscrif.IDE.Components
{
	public partial class StartEventControl : Gtk.EventBox
	{
		Pixbuf logoPixbuf;
		Pixbuf bgPixbuf;
		Thread filllStartPageThread;
		string webCacheName =System.IO.Path.Combine(MainClass.Paths.SettingDir, ".webcache");
		WebCache webCacheFile;


		public StartEventControl()
		{
			try{
				string file = System.IO.Path.Combine(MainClass.Paths.ResDir, "background.png");
				bgPixbuf = new Gdk.Pixbuf (file);
	
				this.Build();
	
				btnRssLoad.Label = MainClass.Languages.Translate("loading");
				btnTwitLoad.Label = MainClass.Languages.Translate("loading");
	
				this.ModifyBg (Gtk.StateType.Normal, Style.White);
	
				string file2 = System.IO.Path.Combine(MainClass.Paths.ResDir, "moscrif.png");
				logoPixbuf = new Gdk.Pixbuf (file2);
	
				webCacheFile = WebCache.OpenWebCache(webCacheName);
	
				//imgLogo.Pixbuf = logoPixbuf;
	
				LinkButton lb1 = new LinkButton();
				lb1.Label = MainClass.Languages.Translate("open_workspace");
				lb1.Description = MainClass.Languages.Translate("open_exist_workspace");
				lb1.WidthRequest = 150;
				lb1.Clicked+= delegate(object sender, EventArgs e) {
					new OpenWorkspace().Activate();
				};
				tblAction.Attach(lb1,0,1,0,1,AttachOptions.Fill,AttachOptions.Shrink,0,0);
	
				LinkButton lb2 = new LinkButton();
				lb2.Label = MainClass.Languages.Translate("new_workspace");
				lb2.Description =MainClass.Languages.Translate("create_new_workspace");
				lb2.WidthRequest = 150;
				lb2.Clicked+= delegate(object sender, EventArgs e) {
					new NewWorkspaceAction().Activate();
				};
				tblAction.Attach(lb2,0,1,1,2,AttachOptions.Fill,AttachOptions.Shrink,0,0);
	
				LinkButton lb3 = new LinkButton();
				lb3.Label = MainClass.Languages.Translate("new_project");
				lb3.Description = MainClass.Languages.Translate("create_new_file");
				lb3.WidthRequest = 150;
				lb3.Clicked+= delegate(object sender, EventArgs e) {
					new NewProjectWizzardAction().Activate();
				};
				tblAction.Attach(lb3,0,1,2,3,AttachOptions.Fill,AttachOptions.Shrink,0,0);
	
				LinkButton lb31 = new LinkButton();
				lb31.Label = MainClass.Languages.Translate("import_project");
				lb31.Description = MainClass.Languages.Translate("import_project_f1");
				lb31.WidthRequest = 150;
				lb31.Clicked+= delegate(object sender, EventArgs e) {
					new ImportZipProjectAction().Activate();
				};
				tblAction.Attach(lb31,0,1,3,4,AttachOptions.Fill,AttachOptions.Shrink,0,0);
	
	
				LinkButton lb4 = new LinkButton();
				lb4.Label = MainClass.Languages.Translate("open_file");
				lb4.Description = MainClass.Languages.Translate("open_exist_file");
				lb4.WidthRequest = 150;
				lb4.Clicked+= delegate(object sender, EventArgs e) {
					new OpenAction().Activate();
				};
				tblAction.Attach(lb4,0,1,4,5,AttachOptions.Fill,AttachOptions.Shrink,0,0);
	
				LinkButton lb5 = new LinkButton();
				lb5.Label = MainClass.Languages.Translate("login_logout");
				lb5.Description = MainClass.Languages.Translate("login_logout");
				lb5.WidthRequest = 150;
				lb5.Clicked+= delegate(object sender, EventArgs e) {
					MainClass.MainWindow.LoginLogout();
	
				};
				tblAction.Attach(lb5,0,1,5,6,AttachOptions.Fill,AttachOptions.Shrink,0,0);
	
				getRecentWorkspace();
	
				getSamples();
				filllStartPageThread = new Thread(new ThreadStart(FilllStartPage));
				//filllStartPageThread.Priority = ThreadPriority.Normal;
				filllStartPageThread.Name = "FilllStartPage";
				filllStartPageThread.IsBackground = true;
				filllStartPageThread.Start();

			} catch(Exception ex){
				Tool.Logger.Error(ex.Message,null);
			}
		}

		private void getRecentWorkspace(){

			IList<RecentFile> lRecentProjects = MainClass.Settings.RecentFiles.GetWorkspace();
			int no =0;
			foreach(RecentFile rf in lRecentProjects){

				LinkButton lb = new LinkButton();
				lb.Label = System.IO.Path.GetFileName(rf.DisplayName);
				lb.HoverMessage =rf.DisplayName;
				lb.WidthRequest = 150;
				string fileName = rf.FileName;
				lb.Clicked+= delegate(object sender, EventArgs e) {
					Workspace.Workspace workspace = Workspace.Workspace.OpenWorkspace(fileName);
					if (workspace != null)
						MainClass.MainWindow.ReloadWorkspace(workspace,false,true);
				};
				tblRecentWork.Attach(lb,(uint)0,(uint)1,(uint)(no),(uint)(no+1),AttachOptions.Fill,AttachOptions.Shrink,0,0);
				no++;
				if (no>3) break;
			}
		}

		private void getSamples(){

			DirectoryInfo dir = new DirectoryInfo(MainClass.Paths.SampleDir);

			LinkButton lbGM = new LinkButton();
			lbGM.Label =MainClass.Languages.Translate("more_sample_label");
			lbGM.LinkUrl =MainClass.Settings.SamplesBaseUrl;
			lbGM.Description =MainClass.Languages.Translate("more_sample_tt");

			LinkButton lbOS = new LinkButton();
			lbOS.Label =MainClass.Languages.Translate("open_sample_label");
			lbOS.LinkUrl =MainClass.Paths.SampleDir;
			lbOS.Description =MainClass.Languages.Translate("open_sample_tt");

			if (!dir.Exists ){
				tblSamples.Attach(lbGM,(uint)0,(uint)1,(uint)(0),(uint)(1),AttachOptions.Fill,AttachOptions.Shrink,0,0);

				return;
			}

			DirectoryInfo[] listdi = dir.GetDirectories("*",SearchOption.TopDirectoryOnly);

			IComparer fileComparer = new CompareDirByDate();
			Array.Sort(listdi, fileComparer);

			int no =0;
			int x = 0;
			int y = 0;

			for(int i =listdi.Length-1 ; i>-1 ; i-- ){

				DirectoryInfo di =listdi[i];

				string  zipFile = System.IO.Path.Combine(di.FullName,di.Name+".zip");
				if(!File.Exists(zipFile)) continue;

				string  pngFile = System.IO.Path.Combine(di.FullName,di.Name+".png");
				//if(!File.Exists(zipFile)) continue;
				string  txtFile = System.IO.Path.Combine(di.FullName,di.Name+".txt");

				//FileInfo[] zipFile = di.GetFiles(di.Name+".zip",SearchOption.TopDirectoryOnly);
				//FileInfo[] txtFile = di.GetFiles(di.Name+".txt",SearchOption.TopDirectoryOnly);
				//FileInfo[] pngFile = di.GetFiles(di.Name+".png",SearchOption.TopDirectoryOnly);

				//if (zipFile.Length < 1 ) continue;

				LinkImageButton lb = new LinkImageButton();
				//lb.Label = System.IO.Path.GetFileName(di.Name);

				//if (txtFile.Length > 0){
				if(File.Exists(txtFile)){
					string descr = "";

					try{
						using (StreamReader file = new StreamReader(txtFile)) {
							descr = file.ReadToEnd();
							file.Close();
							file.Dispose();
						}
					} catch (Exception ex){
						Tool.Logger.Error(ex.Message);
						descr = di.Name;
					}

					if (!String.IsNullOrEmpty(descr)){
						descr = System.Text.RegularExpressions.Regex.Replace(descr, "<[^>]*>", string.Empty);
						descr = descr.Replace("\r","").Replace("\n","");
						descr = descr.Replace("&","");
						string hover =descr;
						if(descr.Length> 75)
							descr = descr.Substring(0,75)+"...";
						//lb.Description =descr;
						lb.HoverMessage =hover;
					}
				}

				if (pngFile.Length > 0)
					lb.Icon =pngFile;

				lb.WidthRequest = 53;
				string fileName = zipFile;
				lb.Clicked+= delegate(object sender, EventArgs e) {

					string prj = System.IO.Path.GetFileNameWithoutExtension(fileName);
					MessageDialogs md = new MessageDialogs(MessageDialogs.DialogButtonType.YesNo, prj, MainClass.Languages.Translate("do_you_want_import",prj),  Gtk.MessageType.Question);
					int result = md.ShowDialog();

					if (result != (int)Gtk.ResponseType.Yes)
						return ;

					MainClass.MainWindow.ImportProject(fileName,true);
				};

				tblSamples.Attach(lb,(uint)x,(uint)(x+1),(uint)(y),(uint)(y+1),AttachOptions.Fill|AttachOptions.Expand,AttachOptions.Shrink,0,0);

				no++;
				x++;
				if(x>2){
					x=0;
					y++;
				}
				if(y>3) break;

				//if (no>8) break;
			}
			hbMorSample.PackStart(lbGM,true,true,0);
			hbMorSample.PackStart(lbOS,true,true,0);
			//tblSamples.Attach(lbGM,(uint)0,(uint)1,(uint)(no),(uint)(no+1),AttachOptions.Fill|AttachOptions.Expand,AttachOptions.Shrink,0,0);

		}

		private void FilllStartPage(){
			try {
			//Gtk.Application.Invoke(delegate
			//{
				//Thread.Sleep(1);
				//getRss();
				frmRss.Visible = false;
				frmRss.HideAll();
				frmRss.Destroy();
				getTweet();

			//});
			}catch(ThreadAbortException){
				Thread.ResetAbort ();
			}finally{

			}
		}

				//protected void OnExposeEvent (EventExpose evnt)
		//public void Expose(object o, Gtk.ExposeEventArgs args)
		protected override bool OnExposeEvent (EventExpose evnt)
		{
			if (bgPixbuf != null) {
				var gc = Style.BackgroundGC ( State);
				//var lRect = new Rectangle (Allocation.X, Allocation.Y, logoPixbuf.Width, logoPixbuf.Height);
				//if (evnt.Region.RectIn (lRect) != OverlapType.Out)
				//	GdkWindow.DrawPixbuf (gc, logoPixbuf, 0, 0, lRect.X, lRect.Y, lRect.Width, lRect.Height, RgbDither.None, 0, 0);

				//var bgRect = new Rectangle (Allocation.X + logoPixbuf.Width, Allocation.Y, Allocation.Width - logoPixbuf.Width, bgPixbuf.Height);
				//if (evnt.Region.RectIn (bgRect) != OverlapType.Out)
				//	for (int x = bgRect.X; x < bgRect.Right; x += bgPixbuf.Width)
				//		GdkWindow.DrawPixbuf (gc, bgPixbuf, 0, 0, x, bgRect.Y, bgPixbuf.Width, bgRect.Height, RgbDither.None, 0, 0);

				var bgRect = new Rectangle (Allocation.X , Allocation.Y, Allocation.Width ,Allocation.Height);
				if (evnt.Region.RectIn (bgRect) != OverlapType.Out)
					for (int y = bgRect.Y; y < bgRect.Bottom; y += bgPixbuf.Height)
						for (int x = bgRect.X; x < bgRect.Right; x += bgPixbuf.Width)
							GdkWindow.DrawPixbuf (gc, bgPixbuf, 0, 0, x, y, bgPixbuf.Width,bgPixbuf.Height, RgbDither.None, 0, 0);

				/*var bgRect = new Rectangle (Allocation.X, Allocation.Bottom-logoPixbuf.Height, Allocation.Width - logoPixbuf.Width, bgPixbuf.Height);
				if (evnt.Region.RectIn (bgRect) != OverlapType.Out)
					for (int x = bgRect.Right- bgPixbuf.Width; x < bgRect.X; x -= bgPixbuf.Width)
						GdkWindow.DrawPixbuf (gc, bgPixbuf, 0, 0, x, bgRect.Y, bgPixbuf.Width, bgRect.Height, RgbDither.None, 0, 0);
				 */

				/*var bgRect = new Rectangle (Allocation.X , Allocation.Y, Allocation.Width ,Allocation.Height);
				if (evnt.Region.RectIn (bgRect) != OverlapType.Out)
					for (int y =bgRect.Bottom-bgPixbuf.Height; y <  bgRect.Y; y -= bgPixbuf.Height)
						for (int x =bgRect.Right- bgPixbuf.Width; x <  bgRect.X; x -= bgPixbuf.Width)
							GdkWindow.DrawPixbuf (gc, bgPixbuf, 0, 0, x, y, bgPixbuf.Width,bgPixbuf.Height, RgbDither.None, 0, 0);
				 */

				var lRect = new Rectangle (Allocation.Right- logoPixbuf.Width, Allocation.Bottom-logoPixbuf.Height, Allocation.Width ,Allocation.Height);
				if (evnt.Region.RectIn (lRect) != OverlapType.Out)
					GdkWindow.DrawPixbuf (gc, logoPixbuf, 0, 0, lRect.X, lRect.Y, logoPixbuf.Width, logoPixbuf.Height, RgbDither.None, 0, 0);
				

			}

			foreach (Widget widget in Children)
				PropagateExpose (widget, evnt);

			return true;
		}
		//

		/*protected override bool OnExposeEvent (EventExpose evnt)
		{
			if (logoPixbuf != null) {
				var gc = Style.BackgroundGC (State);
				var lRect = new Rectangle (Allocation.X, Allocation.Y, logoPixbuf.Width, logoPixbuf.Height);
				if (evnt.Region.RectIn (lRect) != OverlapType.Out)
					GdkWindow.DrawPixbuf (gc, logoPixbuf, 0, 0, lRect.X, lRect.Y, lRect.Width, lRect.Height, RgbDither.None, 0, 0);

				var bgRect = new Rectangle (Allocation.X + logoPixbuf.Width, Allocation.Y, Allocation.Width - logoPixbuf.Width, bgPixbuf.Height);
				if (evnt.Region.RectIn (bgRect) != OverlapType.Out)
					for (int x = bgRect.X; x < bgRect.Right; x += bgPixbuf.Width)
						GdkWindow.DrawPixbuf (gc, bgPixbuf, 0, 0, x, bgRect.Y, bgPixbuf.Width, bgRect.Height, RgbDither.None, 0, 0);
			}

			foreach (Widget widget in Children)
				PropagateExpose (widget, evnt);

			return true;
		}*/


		private List<WebObject> listTwet = new List<WebObject>();
		private void getTweet(){

			XmlDocument rssDoc;
			XmlNode nodeRss = new XmlDocument();
			XmlNode nodeChannel = new XmlDocument();
			XmlNode nodeItem;

			//while (Application.EventsPending ())
			//	Application.RunIteration ();

			int no = 0;

			try{
				XmlTextReader reader = new XmlTextReader(MainClass.Settings.TweetUrl);
 				rssDoc = new XmlDocument();
				rssDoc.Load(reader);
 				for (int i = 0; i < rssDoc.ChildNodes.Count; i++) {
                    			if (rssDoc.ChildNodes[i].Name == "statuses"){
						nodeChannel = rssDoc.ChildNodes[i];
					}
				}
				/*for (int i = 0; i < nodeRss.ChildNodes.Count; i++){
					if (nodeRss.ChildNodes[i].Name == "channel"){
						nodeChannel = nodeRss.ChildNodes[i];
					}
				}*/
				for (int i = 0; i < nodeChannel.ChildNodes.Count; i++)
      				{
					if (nodeChannel.ChildNodes[i].Name == "status")
  					{
						nodeItem = nodeChannel.ChildNodes[i];
						string label =nodeItem["text"].InnerText;
						label = label.Replace("&","");
						string url =MainClass.Settings.TweetBaseUrl+ nodeItem["id"].InnerText;

						listTwet.Add(new WebObject(label,url,"",label));

						no++;
					}
					if (no >MainClass.Settings.MaxRssTweetMessageCount) break;
				}
			} catch{

			}

			Gtk.Application.Invoke(delegate
			{
				if(listTwet.Count>1){
					webCacheFile.ListTweet =listTwet;
				} else {
					if ((webCacheFile.ListTweet == null) && (webCacheFile.ListTweet.Count<1)){
						btnTwitLoad.Label=MainClass.Languages.Translate("error_loat_tweet");
						webCacheFile.SaveWebCache();
						return;
					}
				}

				tblRss.NRows =(uint)webCacheFile.ListTweet.Count+1;
				btnTwitLoad.Destroy();

				for(int i = 0 ; i< webCacheFile.ListTweet.Count;i++){
					LinkButton lb = new LinkButton();
					lb.Label =webCacheFile.ListTweet[i].Title;
					lb.LinkUrl =webCacheFile.ListTweet[i].Url;
					lb.Description =webCacheFile.ListTweet[i].HoverMessage;

					tblTwitt.Attach(lb,(uint)0,(uint)1,(uint)(i),(uint)(i+1),AttachOptions.Fill,AttachOptions.Shrink,0,0);
				}
				tblTwitt.ShowAll();
				webCacheFile.SaveWebCache();
			});

		}
		private List<WebObject> listRss = new List<WebObject>();
		private void getRss(){
			XmlDocument rssDoc;
			XmlNode nodeRss = new XmlDocument();
			XmlNode nodeChannel = new XmlDocument();
			XmlNode nodeItem;

			int no = 0;

			//while (Application.EventsPending ())
			//	Application.RunIteration ();

			try{
				XmlTextReader reader = new XmlTextReader(MainClass.Settings.RssUrl);
 				rssDoc = new XmlDocument();
				rssDoc.Load(reader);
 				for (int i = 0; i < rssDoc.ChildNodes.Count; i++) {
                    			if (rssDoc.ChildNodes[i].Name == "rss"){
						nodeRss = rssDoc.ChildNodes[i];
					}
				}
				for (int i = 0; i < nodeRss.ChildNodes.Count; i++){
					if (nodeRss.ChildNodes[i].Name == "channel")                {
						nodeChannel = nodeRss.ChildNodes[i];
					}
				}
				for (int i = 0; i < nodeChannel.ChildNodes.Count; i++)
      				{
					if (nodeChannel.ChildNodes[i].Name == "item")
  					{
						nodeItem = nodeChannel.ChildNodes[i];
						//LinkButton lb = new LinkButton();
						string title =nodeItem["title"].InnerText;
						string url =nodeItem["link"].InnerText;

						string descr =nodeItem["description"].InnerText;

						descr = System.Text.RegularExpressions.Regex.Replace(descr, "<[^>]*>", string.Empty);
						descr = descr.Replace("\r","").Replace("\n","");
						descr = descr.Replace("&","");
						string hover =descr;
						//if(descr.Length>title.Length)
						if(descr.Length > title.Length){
							int length =title.Length;

							if ((length < 55) && (descr.Length>55)) length = 55;

							descr = descr.Substring(0,length-1)+"...";
						}

						listRss.Add(new WebObject(title,url,descr,hover));
						//listRss.Add(lb);
						no++;
					}
					if (no >MainClass.Settings.MaxRssTweetMessageCount-1) break;
				}
			} catch{

			}
			Gtk.Application.Invoke(delegate
			{

				if(listRss.Count>1){
					//for(int i = 0 ; i< listRss.Count;i++){
						//tblRss.Attach(listRss[i],(uint)0,(uint)1,(uint)(i),(uint)(i+1),AttachOptions.Fill,AttachOptions.Shrink,0,0);
					//}
					//tblRss.ShowAll();
					webCacheFile.ListRss =listRss;
				} else {
					if ((webCacheFile.ListRss == null) && (webCacheFile.ListRss.Count<1)){
						btnRssLoad.Label=MainClass.Languages.Translate("error_loat_rss");
						return;
					}
				}
				tblRss.NRows =(uint)webCacheFile.ListRss.Count+1;
				btnRssLoad.Destroy();
				for(int i = 0 ; i< webCacheFile.ListRss.Count;i++){
					LinkButton lb = new LinkButton();
					lb.Label =webCacheFile.ListRss[i].Title;
					lb.LinkUrl =webCacheFile.ListRss[i].Url;
					lb.Description = webCacheFile.ListRss[i].Description;
					lb.HoverMessage =webCacheFile.ListRss[i].HoverMessage;

					tblRss.Attach(lb,(uint)0,(uint)1,(uint)(i),(uint)(i+1),AttachOptions.Fill,AttachOptions.Shrink,0,0);
				}
				tblRss.ShowAll();
			});
		}

		protected virtual void OnBtnOpenWorkspaceClicked (object sender, System.EventArgs e)
		{
			new OpenWorkspace().Activate();
		}

		protected virtual void OnBtnNewProjectClicked (object sender, System.EventArgs e)
		{
			new NewProjectAction().Activate();
		}

		protected virtual void OnBtnOpenFileClicked (object sender, System.EventArgs e)
		{
			new OpenAction().Activate();
		}
		
		protected virtual void OnBtnNewWorkspaceClicked (object sender, System.EventArgs e)
		{
			new NewWorkspaceAction().Activate();
		}

	}
}