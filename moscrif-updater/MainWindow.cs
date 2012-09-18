using System;
using Gtk;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Timers;
using System.Threading;
using System.Collections.Generic;
using Moscrif.Updater;
using Moscrif.Updater.Dialogs;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using MessageDialogs = Moscrif.Updater.Dialogs.MessageDialog;
using System.Text;
using System.Xml;
using System.Xml.Linq;

public partial class MainWindow: Gtk.Window
{	
	bool cancelUnpack = false;
	/*
	emu.win.zip ->> emulator for win35 and win 64
	emu.mac.zip ->> emulator for mac os
	emu.lin.zip ->> emulator for linux
			
	ide.zip ->> ide
	lib.zip ->> libs directory ( framework)
	
	pub.win.zip ->> publish for win35 and win 64
	pub.mac.zip ->> publish for mac os
	pub.lin.zip ->> publish for lin	
	
	
	sam.zip ->> sample directort
	
	updater sa posiela ako moscrif-updater.exe.new a hodit ho k ide-cku
	
	do zipu priamo subory a adresare ktore sa updatuju, (nesmie to byt vsetko este v jednom adresary)
	------------------------------
	ostatne sa rozzipuju k ide-cku do adresara podla zip suboru 
	(napr. doc.zip do adresara doc)

	*/
	
	string  fileEmulatorPath = "";
	string  fileLibsPath = "";
	string  filePublishPath = "";
	private bool isFinish;
	private bool isError;
	
	private bool test = false;
	
	public MainWindow(): base (Gtk.WindowType.Toplevel) {
		
		Build();
		//textview2.Visible=false;
		//endMark = textview2.Buffer.CreateMark("end-mark", textview2.Buffer.EndIter, false);

		string settingFile = System.IO.Path.Combine(MainClass.Paths.SettingDir, "moscrif.mss");
		if(!File.Exists(settingFile)){
			SetLabel("Setting file not found!");
			btnQuit.Visible = true;
			return;	
		}
		try{			
			Console.WriteLine(settingFile);
			XmlDocument xml = new XmlDocument();
			//xml.LoadXml(settingFile);
			StreamReader reader = new StreamReader(settingFile);

			xml.LoadXml(reader.ReadToEnd());
					
			XmlNodeList xnList = xml.SelectNodes("/Settings");
			
			filePublishPath= xnList[0].Attributes["publishDir"].Value ;
			fileLibsPath= xnList[0].Attributes["frameworkDir"].Value ;
			fileEmulatorPath= xnList[0].Attributes["emulatorDir"].Value ;
		} catch(Exception ex){
			Console.WriteLine(ex.Message);
			SetLabel("Setting file is corrupted");
			btnQuit.Visible = true;
			return;
		}
				
		//timer.Interval = 250;
		//timer.Elapsed += new ElapsedEventHandler(OnTimeElapsed);
		ZipConstants.DefaultCodePage = Encoding.UTF8.CodePage;
		
	
		/*for (int i = 0; i < args.Length; i++){
			WriteMesage(args[i]);
		}*/
		
		/*if (!MainClass.Auto){
			messageLabel.LabelProp = "ERROR.";
			WriteMesage("Incorect parameter.");
			return;
		}*/
		
		//tag = new TextTag("0");		
		if(MainClass.Platform.IsWindows){			
			WriteMesage("Process -->");
			Process []pArry = Process.GetProcesses();
			foreach(Process p in pArry)
			{
				string s = p.ProcessName;		
				s = s.ToLower();
				
				WriteMesage("\t"+s);
				if (s.CompareTo("moscrif-ide") ==0){
					
					try{
						p.Kill();
					} catch{
						SetLabel("Moscrif IDE is already running!");
						btnQuit.Visible = true;
						return;
					}
				}
			}			
		}
		
		string moscriftIdePath = System.IO.Path.Combine(MainClass.Paths.AppPath, "moscrif-ide.exe");
		
		if (!System.IO.File.Exists(moscriftIdePath)){
			SetLabel("Moscrif IDE not found!");
			btnQuit.Visible = true;
			return;	
		}
		string version = "";
		
		FileVersionInfo moFI = FileVersionInfo.GetVersionInfo(moscriftIdePath);
		
		VersionChecker vers = new VersionChecker();
		
		if (moFI== null){
			WriteMesage("ERROR: Unknown version.");
			btnQuit.Visible = true;
			return;
		} else {	
			version = moFI.FileVersion;
			string[] versions = version.Split('.');
			if (versions.Length != 4){
				WriteMesage("ERROR: Invalid Version.");
				btnQuit.Visible = true;
				return;
			}			
			string webVersion = vers.VersionConverter(version);
			
			WriteMesage("Old Version "+ webVersion);
			//return;
		}		
		string newVersion;
		string message ;
		try{
			if(!String.IsNullOrEmpty(MainClass.Token) )
				message = vers.CheckVersionSynch(version,MainClass.Token,out newVersion,false,test);
			else 			
				message = vers.CheckVersionSynch(version,"",out newVersion,false,test);
		}catch(Exception ex){
			SetLabel(ex.Message);
			WriteMesage(ex.Message);
			btnQuit.Visible = true;
			return;		
		}
		bool nextVersion = true;
		
		while(nextVersion){
			nextVersion = false;
			newVersion = System.IO.Path.GetFileNameWithoutExtension(newVersion);
			
			if(String.IsNullOrEmpty(newVersion) ){
				SetLabel("Your version is up-to-date.");
				WriteMesage( message);	
				btnQuit.Visible = true;
				return;
			}
			
			WriteMesage( message);
			WriteMesage("New Version "+ newVersion);
			//pd = new ProgressDialog("Download",ProgressDialog.CancelButtonType.None,1,null);//MainClass.MainWindow		
			Reset(1,"Downloading version : " + newVersion);
			string fullpath = System.IO.Path.Combine(MainClass.Paths.TempDir,newVersion+".zip");//Temp
			//timer.Start();
	//		data = null;
			//Stream str ;
			isFinish = false;
			isError = false;
			try{
				if(!String.IsNullOrEmpty(MainClass.Token) ){
					//vers.GetVersionSynch(newVersion,MainClass.Token,out str);
					GetVersionAsynch(newVersion,MainClass.Token,fullpath,test);
					//	GetVersionHandler
				}
				else 			
					GetVersionAsynch(newVersion,"",fullpath,test);		
				
				int indx = 0;
				while (!isFinish)//(client.IsBusy)
				    {
				        //progressBar.Text=indx.ToString();
					AutomaticUpdate();
					indx++;
					while (Application.EventsPending ())
						Application.RunIteration ();
					Thread.Sleep(150);
				    }
				
			} catch(Exception ex){
				SetLabel(ex.Message);
				WriteMesage(ex.Message);
				//timer.Enabled = false;	
				//timer.Stop();
				btnQuit.Visible = true;
				return;	
			}		
			if((!File.Exists(fullpath)) || (isError)){
				WriteMesage("Error downloading file.");
				SetLabel("Error downloading file!");
				//timer.Enabled = false;	
				//timer.Stop();
				btnQuit.Visible = true;
				//if(pd != null) pd.Destroy();
				return;
			}
			/*if (data!= null){	//str					
				FileStream writeStream = new FileStream(fullpath, FileMode.Create, FileAccess.Write);				
				Copy(data,writeStream);
				writeStream.Close();
				writeStream.Dispose();			
			} else {
				WriteMesage("Error downloading file.");
				SetLabel("Error downloading file!");
				timer.Enabled = false;	
				timer.Stop();
				btnQuit.Visible = true;
				//if(pd != null) pd.Destroy();
				return;
			}*/
			//timer.Enabled = false;	
				//timer.Stop();
			//if(pd != null) pd.Destroy();
			Reset(1,"Download Finish");
			
			string outputPath = System.IO.Path.Combine(MainClass.Paths.Temp,newVersion);
					
			if(!Directory.Exists(outputPath)){
			
				try{
					Directory.CreateDirectory(outputPath);
				}catch{
					SetLabel("Error creating output directory!");
					WriteMesage("ERROR: FILE CANNOT BY CREATE.");
					btnQuit.Visible = true;
					return;
				}
			}

			List<string> listFile = new List<string>();
			try{
				listFile = UnzipFile(fullpath,outputPath);
			} catch{
				SetLabel("Error unpacking new version!");
				WriteMesage("ERROR:FILE CANNOT BY UNPACK.");
				btnQuit.Visible = true;
				return;
			}
			Console.WriteLine("btnQuit.Visible 4 -> {0}",btnQuit.Visible);
			if ((listFile == null) || (listFile.Count<1)){
				SetLabel("Error unpacking new version!");
				WriteMesage("ERROR:FILE CANNOT BY UNPACK.");
				btnQuit.Visible = true;
				return;
			}
			Console.WriteLine("btnQuit.Visible 5 -> {0}",btnQuit.Visible);
			foreach(string file in listFile){
				
				string fileName = System.IO.Path.GetFileName(file).ToLower();
				string fileNameWE = System.IO.Path.GetFileNameWithoutExtension(file).ToLower();
				string fileOutputPath =	"";
				
				switch (fileName){
	
					case "emu.win.zip":
						if (MainClass.Platform.IsWindows){
							fileOutputPath = fileEmulatorPath;
						} else continue;
						break;			
					case "emu.mac.zip":
						if (MainClass.Platform.IsMac){
							fileOutputPath = fileEmulatorPath;
						} else continue;
						break;				
					case "emu.lin.zip":
						if (MainClass.Platform.IsX11){
							fileOutputPath = fileEmulatorPath;
						} else continue;
						break;
		    			case "ide.zip":
		        			fileOutputPath = MainClass.Paths.AppPath;
		        			break;
		    			case "lib.zip":
		        			fileOutputPath = fileLibsPath;
			        		break;
		    			/*case "pub.zip":
		        			fileOutputPath = filePublishPath;
			        		break;	*/
					case "pub.win.zip":
						if (MainClass.Platform.IsWindows){
							fileOutputPath = filePublishPath;
						} else continue;
						break;			
					case "pub.mac.zip":
						if (MainClass.Platform.IsMac){
							fileOutputPath = filePublishPath;
						} else continue;
						break;				
					case "pub.lin.zip":
						if (MainClass.Platform.IsX11){
							fileOutputPath = filePublishPath;
						} else continue;
						break;					
					
		    			case "sam.zip":
		        			fileOutputPath = MainClass.Paths.SampleDir;
			        		break;					
					default:
						fileOutputPath =System.IO.Path.Combine(MainClass.Paths.AppPath,fileNameWE);
						break;
				}
				WriteMesage("fileName :"+file);
				try{
					UnzipFile(file,fileOutputPath);	
					if (cancelUnpack){ 
						WriteMesage("SKIP ");
						goto nav;
					}
					
				}catch (Exception ex) {
					WriteMesage("Error unpacking file: "+ex.Message);
					SetLabel("Error unpacking file '"+file + "'!");
				}
			}
			
			
			string fixVersion = "";
			if(newVersion.Length == 7)
				fixVersion = newVersion[6].ToString();
				
			string versionPath = System.IO.Path.Combine(MainClass.Paths.AppPath,"version.txt");
					
			using (StreamWriter fileVersion = new StreamWriter(versionPath)) {
				fileVersion.Write(fixVersion);
				fileVersion.Close();
				fileVersion.Dispose();
			}
			
			nav:;
			try{	
				WriteMesage("Delete temp file.");
				SetLabel("Deleting temporary files.");
				System.IO.File.Delete(fullpath);
				System.IO.Directory.Delete(outputPath,true);			
			}catch{
				
			}
			
			try{
				version = newVersion;
				Console.WriteLine("newVersion -> {0}",newVersion);
				Console.WriteLine("version -> {0}",version);
				if(!String.IsNullOrEmpty(MainClass.Token) )
					message = vers.CheckVersionSynch(version,MainClass.Token,out newVersion,true,test);
				else 			
					message = vers.CheckVersionSynch(version,"",out newVersion,true,test);
				
				if(String.IsNullOrEmpty(newVersion) ){
					nextVersion = false;
				} else {
					nextVersion = true;
				}
								
			}catch{//(Exception ex){
				//SetLabel(ex.Message);
				//WriteMesage(ex.Message);
				//btnQuit.Visible = true;
				//return;		
				nextVersion = false;
			}
		}
		
		btnQuit.Visible = true;
		SetLabel("Update is completed.");		
	}

		WebClient client;
		public delegate void GetVersionEndTaskHandler(object sender, string mesage, Stream newVersion,string systemError);		
		
		public void GetVersionAsynch(string version,string token, string fullpath,bool test){
			//string file = null;
			string URL = "http://moscrif.com/ide/getVersion.ashx?v={0}";			
			client = new WebClient();
			
			client.DownloadProgressChanged+= delegate(object sender, DownloadProgressChangedEventArgs e) {
				
				//Console.WriteLine("----> {0}",e.ProgressPercentage);;
				/*progressBar.Text = e.ProgressPercentage.ToString();
				progressBar.QueueDraw();
			
				while (Application.EventsPending ())
					Application.RunIteration ();*/
			};
			
			if(String.IsNullOrEmpty(token)){
				URL = String.Format(URL,version);
			} else {
				URL = String.Format(URL+"&t={1}",version,token);
			}					
			
			//URL = String.Format(URL,version,token);
			
			if(test){
				URL = URL+"&test=1";
			}
			
			Console.WriteLine("URL ->{0}",URL);			
			
			//client.DownloadStringCompleted += delegate(object sender, DownloadStringCompletedEventArgs e) {   //OpenReadCompleted += delegate(object sender, OpenReadCompletedEventArgs e) {//UploadStringCompleted+= delegate(object sender, UploadStringCompletedEventArgs e) {
			client.OpenReadCompleted += delegate(object sender, OpenReadCompletedEventArgs e) {//UploadStringCompleted+= delegate(object sender, UploadStringCompletedEventArgs e) {							
				if (e.Cancelled){
					isFinish = true;
					return;
				}

				if (e.Error != null){
					isFinish = true;
					return;
				}

				if(File.Exists(fullpath))
					File.Delete(fullpath);
			
				FileStream writeStream = new FileStream(fullpath, FileMode.Create, FileAccess.Write);							
				try{
					Copy(e.Result,writeStream);
					writeStream.Close();
					writeStream.Dispose();	
				}catch{
					isError = true;					
				} 
				isFinish = true;
			};	
			client.OpenReadAsync(new Uri(URL));	
			//client.DownloadStringAsync(new Uri(URL));			
		
		//	while (Application.EventsPending ())
		//		Application.RunIteration ();
		
		}	
	
	
	private void OnTimeElapsed(object o, ElapsedEventArgs args){
		AutomaticUpdate();
	}
	
	public void Copy(Stream source, Stream target) {
		
		byte[] buffer = new byte[2054];//new byte[0x10000];
		int bytes;
		try {
			while ((bytes = source.Read(buffer, 0, buffer.Length)) > 0) {
				target.Write(buffer, 0, bytes);
			}
		}
		finally {
			target.Flush();
			// Or target.Close(); if you're done here already.
		}		
	}
	
	public List<string> UnzipFile(string zippPath,string outputPath){
		
		List<string> listFile = new List<string>();
		if (!System.IO.File.Exists(zippPath)){
			
			WriteMesage("ERROR :  FILE NOT FOUND :"+ zippPath );
			return listFile;
			
		}
	
		//ProgressDialog progD = new ProgressDialog("Unpacking",ProgressDialog.CancelButtonType.None, 2048,null);
		Reset(1,"Unpacking");
		
		using (ZipInputStream s = new ZipInputStream(File.OpenRead(zippPath))) {
			
			ZipEntry theEntry;
			while ((theEntry = s.GetNextEntry()) != null) {				
				
				string directoryName = System.IO.Path.GetDirectoryName(theEntry.Name);
				string fileName = System.IO.Path.GetFileName(theEntry.Name);
					WriteMesage("directoryName: "+ directoryName);
					WriteMesage("fileName: "+ fileName );					
				
				
				if (!String.IsNullOrEmpty(fileName)) {											
					
					string newDirectoryPath = System.IO.Path.Combine(outputPath, directoryName);
					string newFilePath = System.IO.Path.Combine(newDirectoryPath, fileName);
					WriteMesage("File: "+ fileName);
					
					try{
						int count = (int)(s.Length/2048)+2;
											
						WriteMesage("Length: "+ s.Length );													
						WriteMesage("Count: "+ count );
					
						//WriteMesage("File: " +newDirectoryPath);
						WriteMesage("New Path: " +newFilePath);
						
						if (!Directory.Exists(newDirectoryPath)){
							Directory.CreateDirectory(newDirectoryPath);
						}
						
						//if ( (s.Length == null) || (s.Length <1)) continue;
							
						//progD.Reset(count,theEntry.Name);
						Reset(count,theEntry.Name);				
					
						//progD.SetLabel(fileName);
						SetLabel(fileName);
					
						using (FileStream streamWriter = File.Create(newFilePath)){//theEntry.Name)) {
					
							int size = 1048576;//2048;
							byte[] data = new byte[1048576];//[2048];
							while (true) {
								//progD.Update(fileName);
								Update(fileName);
							
								size = s.Read(data, 0, data.Length);
								if (size > 0) {
									streamWriter.Write(data, 0, size);
								} else {
									break;
								}
							}
						}
					}catch(Exception ex){
						MessageDialogs md =
							new MessageDialogs(MessageDialogs.DialogButtonType.YesNo, "I can not unpack file. " +fileName ," Continue ?", Gtk.MessageType.Error,null);
						int res = md.ShowDialog();
						WriteMesage(String.Format("ERROR IN FILE {0}: {1}",fileName,ex.Message) );
						if (res == (int)Gtk.ResponseType.No){
							cancelUnpack = true;
							return listFile;
						}
					}
					listFile.Add(newFilePath);
				}
			}		
		}
		//progD.Destroy();
		//Reset(1,"Update finish.");
		return listFile;
	}
	
	
	public void SaveStreamToFile(string fileFullPath, Stream stream){
		
		if (stream.Length == 0) return;
    		// Create a FileStream object to write a stream to a file
    		using (FileStream fileStream = System.IO.File.Create(fileFullPath, (int)stream.Length))
    		{
        		// Fill the bytes[] array with the stream data
        		byte[] bytesInStream = new byte[stream.Length];
	        	stream.Read(bytesInStream, 0, (int)bytesInStream.Length);
        		// Use FileStream object to write to the specified file
        		fileStream.Write(bytesInStream, 0, bytesInStream.Length);
     		}
	}
	
	private void WriteMesage(string message){
	/*	TextIter it = textview2.Buffer.EndIter;
		//textview2.Buffer.Inse InsertWithTags(ref it,message+"\n", tag);
		textview2.Buffer.Insert(ref it,message+"\n");
		
		textview2.Buffer.MoveMark(endMark, it);
		textview2.ScrollToMark(endMark, 0, false, 0, 0);*/
		Console.WriteLine(message);
	}
	
	protected void OnDeleteEvent(object sender, DeleteEventArgs a) {
		Application.Quit();
		a.RetVal = true;
	}
	
	private int totalCount;
	private int currentCount;
	
	
	public void Reset(int totalCount, string message){
		this.totalCount = totalCount;	
		currentCount = 0;
		progressBar.Fraction = 0.0;
		messageLabel.Text = message;
		
		while (Application.EventsPending ())
			Application.RunIteration ();
	}	
		
	public void SetLabel(string message){
		messageLabel.Text = message;

		//ShowAll ();

		while (Application.EventsPending ())
			Application.RunIteration ();
	}

	public bool Update (string message)
	{
		currentCount ++;

		messageLabel.Text = message;
		progressBar.Text = String.Format ("{0} of {1}", currentCount, totalCount);
		progressBar.Fraction = (double) currentCount / totalCount;

		//ShowAll ();

		while (Application.EventsPending ())
			Application.RunIteration ();

		return false;
	}

	public void AutomaticUpdate ()
	{
		
		/*Gtk.Application.Invoke(delegate {

			currentCount ++;
			if (currentCount > 10)
				currentCount = 0;

			progressBar.Fraction = (double)currentCount/10;// / totalCount;
			progressBar.QueueDraw();
		});*/
		
		currentCount ++;
		if (currentCount > 10)
			currentCount = 0;

		progressBar.Fraction = (double)currentCount/10;// / totalCount;
		//ShowAll ();

		//while (Application.EventsPending ())
		//	Application.RunIteration ();
	
	}	
	
	protected virtual void OnButton2517Clicked (object sender, System.EventArgs e)
	{
		Application.Quit();
	}
	
	
}
