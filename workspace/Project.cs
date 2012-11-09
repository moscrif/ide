using System;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.IO;
using Moscrif.IDE.Devices;
using MessageDialogs = Moscrif.IDE.Controls.MessageDialog;
using Moscrif.IDE.Iface.Entities;
using System.Security.Principal;
using Moscrif.IDE.Controls;
using System.Timers;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using Gtk;

namespace Moscrif.IDE.Workspace
{
	public class Project
	{

		public const string KEY_ICON = "icon";

		public const string KEY_ICON_BADA1 = "icon1";
		public const string KEY_SPLASH_BADA1 = "splash1";
		public const string KEY_ICON_BADA2 = "icon2";
		public const string KEY_SPLASH_BADA2 = "splash2";
		public const string KEY_ICON_BADA3 = "icon3";
		public const string KEY_SPLASH_BADA3 = "splash3";

		public const string KEY_MANIFEST = "manifest";
		public const string KEY_SPLASH = "splash";
		public const string KEY_APPLICATIONFILE = "applicationFile";
		public const string KEY_APPLICATIONID = "applicationId";

		public const string KEY_CERTIFICATE = "certificate";
		public const string KEY_KEY = "key";
		public const string KEY_PASSWORD = "password";

		public const string KEY_KEYSTORE = "keystore";
		public const string KEY_STOREPASSWORD = "storepassword";
		public const string KEY_ALIAS = "alias";
		public const string KEY_KEYPASSWORD = "keypassword";

		public const string KEY_INSTALLOCATION = "installLocation";

		public const string KEY_CODESIGNINGIDENTITY = "codeSigningIdentity";
		public const string KEY_SUPPORTEDDEVICES = "supportedDevices";

		/*public const string KEY_ICON3GS = "icon3gs";
		public const string KEY_ICON4 = "icon4+";
		public const string KEY_ICONPAD = "iconTablet";
		public const string KEY_SPLASH3GS = "splash3gs";
		public const string KEY_SPLASH4 = "splash4+";
		public const string KEY_SPLASHPAD = "splashTablet";*/

		public const string KEY_IP4ICON = "iPhone4-icon";
		public const string KEY_IP4SPLASH = "iPhone4-splash";
		public const string KEY_IP5ICON = "iPhone5-icon";
		public const string KEY_IP5SPLASH = "iPhone5-splash";
		public const string KEY_IPADICON = "iPad-icon";
		public const string KEY_IPADSPLASH = "iPad-splash";
		public const string KEY_INEWPADICON = "newiPad-icon";
		public const string KEY_INEWPADSPLASH = "newiPad-splash";

		public const string KEY_BUNDLEIDENTIFIER = "bundleIdentifier";
		public const string KEY_PERMISSION = "permission";

		public Project()
		{
			FilesProperty = new List<FileItem>();
			//BadaSettings = new Device();

			DevicesSettings = new List<Device>();
			ConditoinsDefine = new List<Condition>();

			//if (Resolution == null)
			//	GenerateResolution();

			if (String.IsNullOrEmpty(ProjectOutput))
				ProjectOutput = MainClass.Workspace.OutputDirectory;
		}

		public Project(string filePath)
		{
			FilePath = filePath;
			FilesProperty = new List<FileItem>();
			//BadaSettings = new Device();

			DevicesSettings = new List<Device>();
			ConditoinsDefine = new List<Condition>();

			//if (Resolution == null)
			//	GenerateResolution();

			if (String.IsNullOrEmpty(ProjectOutput))
				ProjectOutput = MainClass.Workspace.OutputDirectory;			
		}

		public void GetAllFiles(ref List<string> filesList, string path, string extension){
			GetAllFiles(ref filesList, path, extension,false);
		}


		public void GetAllFiles(ref List<string> filesList, string path, string extension, bool skipMsc)
		{
			//this.AbsolutProjectDir
			if (!Directory.Exists(path)){
				Tool.Logger.LogDebugInfo(String.Format("Directory Not Exist-> {0}",path),null);
				return;
			}

			DirectoryInfo di = new DirectoryInfo(path);

			if((MainClass.Workspace == null) || (MainClass.Workspace.ActualProject == null)) return;

			//DirectoryInfo diOutput = new DirectoryInfo(MainClass.Workspace.ActualProject.OutputMaskToFullPath) ;
			DirectoryInfo diOutput = null;

			if(!String.IsNullOrEmpty(ProjectOutput))
				diOutput = new DirectoryInfo(this.OutputMaskToFullPath) ;

			foreach (DirectoryInfo d in di.GetDirectories()){
				int indx = -1;
				indx = MainClass.Settings.IgnoresFolders.FindIndex(x => x.Folder == d.Name && x.IsForPublish);

				if(!String.IsNullOrEmpty(ProjectOutput)){
					if(di.FullName == diOutput.FullName)
						continue;
				}

				if(indx<0){
					GetAllFiles(ref filesList, d.FullName,extension);
				}
			}

			foreach (FileInfo f in di.GetFiles()) {

				if (skipMsc && (f.Extension.ToUpper() == ".MSC")) continue;
				int indx = -1;
				indx = MainClass.Settings.IgnoresFiles.FindIndex(x => x.Folder == f.Name && x.IsForPublish);

				if(!String.IsNullOrEmpty(extension) ){
					if(f.Extension.ToLower() == extension.ToLower()){
						if(indx<0)
							filesList.Add( f.FullName );
					}
				}else {
					if(indx<0)
						filesList.Add( f.FullName );
				}

			}

		}

		public FileItem ToggleExcludeFile(string filePath)
		{
			FileItem fi = FindFile(filePath);
			if (fi == null) {
				fi = new FileItem(filePath,true);
				FilesProperty.Add(fi);
			} else
				fi.IsExcluded = !fi.IsExcluded;
			return fi;
			/*FileItem fi = new FileItem();
			fi.IsExclude = true;
			fi.FilePath = filePath;
			FilesProperty.Add(fi);*/
		}

		[XmlArray("conditionDefines")]
		[XmlArrayItem("define")]
		public List<Condition> ConditoinsDefine;

		// nazovprojektu.msp.nazovPc-Username.user
		private ProjectUserFile projectUserSetting;

		[XmlIgnore]
		public ProjectUserFile ProjectUserSetting{
			get{
				if(projectUserSetting == null){
					string mspFile= System.IO.Path.GetFileName(FilePath);
					string mspPath = System.IO.Path.GetDirectoryName(FilePath);

					string userName = String.Format("{0}.{1}-{2}.user",mspFile,Environment.MachineName,Environment.UserName);
					string userPath = System.IO.Path.Combine(mspPath,userName);

					if(System.IO.File.Exists(userPath)){
						projectUserSetting = ProjectUserFile.OpenProjectUserFile(userPath);
					} else {
						projectUserSetting =new ProjectUserFile(userPath);
					}
				}
				return projectUserSetting;
			}
		}


		/*

		[XmlArray("combinePublishes")]
		[XmlArrayItem("publish")]
		public List<CombinePublish> CombinePublish;

		[XmlAttribute("publishPageIndex")]
		public int PublishPage ;

		*/

		[XmlAttribute("includeAllResolution")]
		public bool IncludeAllResolution = false;

		[XmlAttribute("applicationType")]
		public string ApplicationType = "application";

		private AppFile appFile;

		[XmlIgnore]
		public AppFile AppFile
		{
			get {
				//if (appFile != null)
				//	return appFile;
				appFile = new AppFile( MainClass.Workspace.GetFullPath(AppFilePath));

				return appFile;
			}
			set {
				appFile = value;
			}
		}

		private string absolutProjectDir = String.Empty;

		[XmlIgnore]
		public string AbsolutProjectDir
		{
			get {
				//if(String.IsNullOrEmpty(absolutProjectDir)){

					string prjDir = System.IO.Path.GetFileNameWithoutExtension(AbsolutAppFilePath);
					absolutProjectDir = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(AbsolutAppFilePath),prjDir);
				//}
				return absolutProjectDir;
				//return System.IO.Path.Combine(MainClass.Workspace.RootDirectory,ProjectDir);
				//return MainClass.Workspace.GetFullPath(ProjectDir);
			}
		}

		[XmlIgnore]
		public string FilePath
		{
			get;
			set;
		}

		//[XmlAttribute("projectName")]
		[XmlIgnore]
		public string ProjectName{
			get{
				return AppFile.Name;
			}
		}

		//[XmlAttribute("projectDir")]
	/*	[XmlIgnore]
		public string ProjectDir{
			get{


				//return AppFile.Name;
			}
		}*/

		public bool NewSkin =false;

		[XmlAttribute("projectOutput")]
		public string ProjectOutput;

		[XmlAttribute("facebookAppID")]
		public string FacebookAppID;

		[XmlElement("publishTyp")]
		public int TypPublish = 0;

		[XmlIgnore]
		public string OutputMaskToFullPath{

			get{
				return ConvertProjectMaskPathToFull(ProjectOutput);
			}
		}

		public string ConvertProjectMaskPathToFull(string mask){

			if(MainClass.Workspace!=null && !String.IsNullOrEmpty(MainClass.Workspace.FilePath)){
	
				string path = MainClass.Workspace.ConvertOutputMaskToFullPath(mask);
				path = path.Replace("$(publish_dir)", MainClass.Settings.PublishDirectory);

				if(path.Contains("$(project_dir)"))	
					path = path.Replace("$(project_dir)", this.AbsolutProjectDir);
	
				return path;
			}

			return MainClass.Paths.WorkDir;

		}

		[XmlAttribute("projectArtefact")]
		public string ProjectArtefac;

		[XmlAttribute("appFile")]
		public string AppFilePath;


		[XmlIgnore]
		public string RelativeAppFilePath
		{
			get {
				return Tool.FileUtility.GetSystemPath(AppFilePath);
				/*string absPath = AppFilePath.Replace('/',Path.DirectorySeparatorChar);
				absPath = absPath.Replace('\\',Path.DirectorySeparatorChar);
				return absPath;*/
			}
		}

		private string absolutAppFilePath =string.Empty;

		[XmlIgnore]
		public string AbsolutAppFilePath
		{
			get {
				//if(String.IsNullOrEmpty(absolutAppFilePath)){
				absolutAppFilePath =MainClass.Workspace.GetFullPath(this.AppFilePath);
				//}

				return  absolutAppFilePath;
			}
		}

		/*[XmlAttribute("startupFile")]
		public string StartFile;*/


		[XmlIgnore]
		public string StartFile{
			get{
				if(!string.IsNullOrEmpty(AppFile.Main)){
					string file = System.IO.Path.Combine(this.absolutProjectDir,AppFile.Main);
					return MainClass.Workspace.GetRelativePath(file);
				} else return "";

			}
			set{
				string file = System.IO.Path.GetFileName(value);
				AppFile.Main = file;
			}

		}

		[XmlArray("fileProperties")]
		[XmlArrayItem("property")]
		public List<FileItem> FilesProperty;

		[XmlArray("devices")]
		[XmlArrayItem("device")]
		public List<Device> DevicesSettings;


		public void GenerateDevices(){
			GenerateDevices("","");
		}

		public List<PublishProperty> GeneratePublishPropertisMask(int deviceTyp){
			List<PublishProperty> list = new List<PublishProperty>();

			if(deviceTyp == (int)DeviceType.Android_1_6){
				list = new List<PublishProperty>();
				list.Add(new PublishProperty(KEY_ICON));
				list.Add(new PublishProperty(KEY_SPLASH));
				list.Add(new PublishProperty(KEY_BUNDLEIDENTIFIER));
				list.Add(new PublishProperty(KEY_ALIAS));
				list.Add(new PublishProperty(KEY_KEYSTORE));
				list.Add(new PublishProperty(KEY_KEYPASSWORD));
				list.Add(new PublishProperty(KEY_STOREPASSWORD));
				list.Add(new PublishProperty(KEY_SUPPORTEDDEVICES));
				list.Add(new PublishProperty(KEY_PERMISSION));

			}
			if(deviceTyp == (int)DeviceType.Android_2_2){
				list = new List<PublishProperty>();
				list.Add(new PublishProperty(KEY_ICON));
				list.Add(new PublishProperty(KEY_SPLASH));
				list.Add(new PublishProperty(KEY_BUNDLEIDENTIFIER));
				list.Add(new PublishProperty(KEY_ALIAS));
				list.Add(new PublishProperty(KEY_KEYSTORE));
				list.Add(new PublishProperty(KEY_KEYPASSWORD));
				list.Add(new PublishProperty(KEY_STOREPASSWORD));
				list.Add(new PublishProperty(KEY_SUPPORTEDDEVICES));
				list.Add(new PublishProperty(KEY_INSTALLOCATION));
				list.Add(new PublishProperty(KEY_PERMISSION));
			}
			
			if((deviceTyp == (int)DeviceType.Bada_1_0) || (deviceTyp == (int)DeviceType.Bada_1_1) || (deviceTyp == (int)DeviceType.Bada_1_2)
			   || (deviceTyp == (int)DeviceType.Bada_2_0) ){
				list = new List<PublishProperty>();
				list.Add(new PublishProperty(KEY_ICON));
				list.Add(new PublishProperty(KEY_SPLASH));
				/*dpd.Device.PublishPropertisMask.Add(new PublishProperty(KEY_ICON_BADA1));
					dpd.Device.PublishPropertisMask.Add(new PublishProperty(KEY_SPLASH_BADA1));
					dpd.Device.PublishPropertisMask.Add(new PublishProperty(KEY_ICON_BADA2));
					dpd.Device.PublishPropertisMask.Add(new PublishProperty(KEY_SPLASH_BADA2));*/
				/*
					 TOTO len pre badu 1.2 - ked bude
					dpd.Device.PublishPropertisMask.Add(new PublishProperty(KEY_ICON_BADA1));
					dpd.Device.PublishPropertisMask.Add(new PublishProperty(KEY_SPLASH_BADA1));*/
				list.Add(new PublishProperty(KEY_MANIFEST));
			}
			
			if(deviceTyp == (int)DeviceType.Symbian_9_4){
				list = new List<PublishProperty>();
				list.Add(new PublishProperty(KEY_ICON));
				list.Add(new PublishProperty(KEY_SPLASH));
				list.Add(new PublishProperty(KEY_APPLICATIONID));
				list.Add(new PublishProperty(KEY_CERTIFICATE));
				list.Add(new PublishProperty(KEY_KEY));
				list.Add(new PublishProperty(KEY_PASSWORD));
			}
			
			if(deviceTyp == (int)DeviceType.iOS_5_0){
				list = new List<PublishProperty>();
				list.Add(new PublishProperty(KEY_ICON));
				list.Add(new PublishProperty(KEY_SPLASH));
				list.Add(new PublishProperty(KEY_CODESIGNINGIDENTITY));
				list.Add(new PublishProperty(KEY_SUPPORTEDDEVICES));
				
				list.Add(new PublishProperty(KEY_IP4ICON));
				list.Add(new PublishProperty(KEY_IP4SPLASH ));
				list.Add(new PublishProperty(KEY_IP5ICON ));
				list.Add(new PublishProperty(KEY_IP5SPLASH ));
				list.Add(new PublishProperty(KEY_IPADICON ));
				list.Add(new PublishProperty(KEY_IPADSPLASH ));
				list.Add(new PublishProperty(KEY_INEWPADICON ));
				list.Add(new PublishProperty(KEY_INEWPADSPLASH ));
				/*list.Add(new PublishProperty(KEY_ICON3GS));
				list.Add(new PublishProperty(KEY_ICON4));
				list.Add(new PublishProperty(KEY_ICONPAD));
				list.Add(new PublishProperty(KEY_SPLASH3GS));
				list.Add(new PublishProperty(KEY_SPLASH4));
				list.Add(new PublishProperty(KEY_SPLASHPAD));*/
			}

			if(deviceTyp == (int)DeviceType.WindowsMobile_5 || (deviceTyp == (int)DeviceType.WindowsMobile_6) || (deviceTyp == (int)DeviceType.PocketPC_2003SE)){
				list = new List<PublishProperty>();
				list.Add(new PublishProperty(KEY_ICON));
				list.Add(new PublishProperty(KEY_SPLASH));
			}
			if(deviceTyp == (int)DeviceType.Windows){
				list = new List<PublishProperty>();
				list.Add(new PublishProperty(KEY_ICON));
				list.Add(new PublishProperty(KEY_SPLASH));
			}
			if(deviceTyp == (int)DeviceType.MacOs){
				list = new List<PublishProperty>();
				list.Add(new PublishProperty(KEY_ICON));
				list.Add(new PublishProperty(KEY_SPLASH));
			}
			return list;
		}

		public void GenerateDevices(string skin, string theme, string[] fonts){
			if(this.DevicesSettings == null)
				this.DevicesSettings = new List<Device>();

			foreach (Rule rl in MainClass.Settings.Platform.Rules){

				Device dvc = this.DevicesSettings.Find(x => x.TargetPlatformId == rl.Id);
				if (dvc == null) {
					dvc = new Device();
					dvc.TargetPlatformId = rl.Id;
					dvc.Includes.Skin.Name = skin;
					dvc.Includes.Skin.Theme = theme;

					if(fonts == null)
						dvc.Includes.Fonts = new string[]{};
					else
						dvc.Includes.Fonts = fonts;

					this.DevicesSettings.Add(dvc);
				}

				if ((dvc.PublishPropertisMask == null) || dvc.PublishPropertisMask.Count<1){
					dvc.PublishPropertisMask = GeneratePublishPropertisMask(rl.Id );
				}
			}
		}

		public void GenerateDevices(string skin, string theme){
			GenerateDevices(skin,theme,null);
		}

		internal void Save()
		{

			XmlWriterSettings xmlSettings = new XmlWriterSettings();

			xmlSettings.NewLineChars = "\n";//Environment.NewLine;
			xmlSettings.Indent = true;
			xmlSettings.CloseOutput = true;

			XmlSerializer x_serial = new XmlSerializer( typeof( Project ) );
			using (XmlWriter wr = XmlWriter.Create( this.FilePath, xmlSettings )) {
			    x_serial.Serialize( wr, this );
			}

			XmlSerializer x_serial_PU = new XmlSerializer( typeof( ProjectUserFile ) );
			using (XmlWriter wr = XmlWriter.Create( this.ProjectUserSetting.FilePath, xmlSettings )) {
			    x_serial_PU.Serialize( wr, this.ProjectUserSetting );
			}
		}


		private ProgressDialog progressDialog;
		public bool Export(string outputFile, bool Archive){

			/*if (System.IO.File.Exists(outputFile)){

				MessageDialogs md =
					new MessageDialogs(MessageDialogs.DialogButtonType.YesNo, MainClass.Languages.Translate("file_notcreate_is_exist") ,outputFile, Gtk.MessageType.Error);
				 return false;
			}*/


			string[] filePaths = Directory.GetFiles(AbsolutProjectDir, "*",SearchOption.AllDirectories);
			List<string> fonts = new List<string>();

			int lng = filePaths.Length;


			int folderOffset = MainClass.Workspace.RootDirectory.Length;//compressDir.Length + (compressDir.EndsWith("\\") ? 0 : 1);
			string signingOld = String.Empty;
			Device device = this.DevicesSettings.Find(x=>x.Devicetype == DeviceType.iOS_5_0);
			if(device!= null){
				PublishProperty ppSigning = device.PublishPropertisMask.Find(x=>x.PublishName== KEY_CODESIGNINGIDENTITY);
				if(ppSigning!= null){
					signingOld = ppSigning.PublishValue;
					ppSigning.PublishValue = "";
					try{
						Save();
					} catch (Exception ex){
						Tool.Logger.Error(ex.Message);
					}
				}
			}

			if(Archive){
				foreach (string lib in this.AppFile.Libs){
					string libPath = System.IO.Path.Combine(MainClass.Workspace.RootDirectory,lib);

					if(Directory.Exists(libPath)){
						try{
							filePaths = Directory.GetFiles(libPath, "*",SearchOption.AllDirectories);
							lng = lng  + filePaths.Length;
						}catch(Exception ex) {
							Tool.Logger.Error(ex.Message);
						}
					}
				}


				filePaths = Directory.GetFiles(MainClass.Workspace.RootDirectory, "*.ttf",SearchOption.TopDirectoryOnly);

				if(MainClass.Platform.IsMac){// for Mac UpperCase
					string[]  filePaths2 = Directory.GetFiles(MainClass.Workspace.RootDirectory, "*.TTF",SearchOption.TopDirectoryOnly);				

					var list = new List<string>();
					list.AddRange(filePaths);
					list.AddRange(filePaths2);
					filePaths = list.ToArray();

				}

				foreach (Device d in this.DevicesSettings){
					foreach (string font in d.Includes.Fonts){ 

						string fontPath = System.IO.Path.Combine(MainClass.Workspace.RootDirectory ,font);
						if( System.IO.File.Exists(fontPath) && fonts.FindIndex(x=>x == fontPath)<0 ){

							System.IO.FileInfo fi = new FileInfo(fontPath);
							if(fi.DirectoryName == MainClass.Workspace.RootDirectory) // only font from workspace
								fonts.Add(fontPath);
						} 
					}
				}
				lng = lng +fonts.Count; 
			}

			progressDialog = new ProgressDialog("Compressed...",ProgressDialog.CancelButtonType.None,lng+2,MainClass.MainWindow);

			/*Timer timer = new Timer();
			timer.Interval = 240;
			timer.Elapsed += new ElapsedEventHandler(OnTimeElapsed);
			timer.Start();*/


			FileStream fsOut = File.Create(outputFile);
			ZipOutputStream zipStream = new ZipOutputStream(fsOut);
			zipStream.SetLevel(4);

			string[] files = new string[] {this.AbsolutAppFilePath,this.FilePath};

			foreach (string file in files){
				FileInfo fi = new FileInfo(file);

				string entryName = System.IO.Path.GetFileName(file); //file.Substring(folderOffset);
				entryName = ZipEntry.CleanName(entryName); // Removes drive from name and fixes slash direction
				ZipEntry newEntry = new ZipEntry(entryName);
				newEntry.DateTime = fi.LastWriteTime; // Note the zip format stores 2 second granularity

				// Specifying the AESKeySize triggers AES encryption. Allowable values are 0 (off), 128 or 256.
				//   newEntry.AESKeySize = 256;

				// To permit the zip to be unpacked by built-in extractor in WinXP and Server2003, WinZip 8, Java, and other older code,
				// you need to do one of the following: Specify UseZip64.Off, or set the Size.
				// If the file may be bigger than 4GB, or you do not need WinXP built-in compatibility, you do not need either,
				// but the zip will be in Zip64 format which not all utilities can understand.
				//   zipStream.UseZip64 = UseZip64.Off;
				newEntry.Size = fi.Length;

				zipStream.PutNextEntry(newEntry);

				// Zip the file in buffered chunks
				// the "using" will close the stream even if an exception occurs
				progressDialog.Update(entryName);
				byte[ ] buffer = new byte[4096];
				using (FileStream streamReader = File.OpenRead(file)) {
					ICSharpCode.SharpZipLib.Core.StreamUtils.Copy(streamReader, zipStream, buffer);
				}
				zipStream.CloseEntry();
			}

			foreach (string file in fonts){
				FileInfo fi = new FileInfo(file);

				string entryName = System.IO.Path.GetFileName(file); //file.Substring(folderOffset);
				entryName = ZipEntry.CleanName(entryName); // Removes drive from name and fixes slash direction
				ZipEntry newEntry = new ZipEntry(entryName);
				newEntry.DateTime = fi.LastWriteTime; // Note the zip format stores 2 second granularity

				// Specifying the AESKeySize triggers AES encryption. Allowable values are 0 (off), 128 or 256.
				//   newEntry.AESKeySize = 256;

				// To permit the zip to be unpacked by built-in extractor in WinXP and Server2003, WinZip 8, Java, and other older code,
				// you need to do one of the following: Specify UseZip64.Off, or set the Size.
				// If the file may be bigger than 4GB, or you do not need WinXP built-in compatibility, you do not need either,
				// but the zip will be in Zip64 format which not all utilities can understand.
				//   zipStream.UseZip64 = UseZip64.Off;
				newEntry.Size = fi.Length;

				zipStream.PutNextEntry(newEntry);

				// Zip the file in buffered chunks
				// the "using" will close the stream even if an exception occurs
				progressDialog.Update(entryName);
				byte[ ] buffer = new byte[4096];
				using (FileStream streamReader = File.OpenRead(file)) {
					ICSharpCode.SharpZipLib.Core.StreamUtils.Copy(streamReader, zipStream, buffer);
				}
				zipStream.CloseEntry();
			}


			CompressFolder(this.AbsolutProjectDir, zipStream, folderOffset);

			if(Archive){
				foreach (string lib in this.AppFile.Libs){
					string libPath = System.IO.Path.Combine(MainClass.Workspace.RootDirectory,lib);
					if(System.IO.Directory.Exists(libPath)){
						CompressFolder(libPath, zipStream, folderOffset);
					}
				}
			}

			if(!String.IsNullOrEmpty(signingOld)){
				device = this.DevicesSettings.Find(x=>x.Devicetype == DeviceType.iOS_5_0);
				if(device!= null){
					PublishProperty ppSigning = device.PublishPropertisMask.Find(x=>x.PublishName== KEY_CODESIGNINGIDENTITY);
					if(ppSigning!= null){
						ppSigning.PublishValue = signingOld;
						try{
							Save();
						} catch (Exception ex){
							Tool.Logger.Error(ex.Message);
						}
					}
				}
			}

			zipStream.IsStreamOwner = true;
			zipStream.Close();
			progressDialog.Destroy();
			return true;

		}

		private void CompressFolder(string path, ZipOutputStream zipStream, int folderOffset) {
		
			string[] files = Directory.GetFiles(path);

			foreach (string filename in files) {

				FileInfo fi = new FileInfo(filename);

				int indx = -1;
				indx = MainClass.Settings.IgnoresFiles.FindIndex(x => x.Folder == fi.Name && x.IsForIde);

				if(indx >-1) continue;

				string entryName = filename.Substring(folderOffset); // Makes the name in zip based on the folder
				entryName = ZipEntry.CleanName(entryName); // Removes drive from name and fixes slash direction
				ZipEntry newEntry = new ZipEntry(entryName);
				newEntry.DateTime = fi.LastWriteTime; // Note the zip format stores 2 second granularity

				// Specifying the AESKeySize triggers AES encryption. Allowable values are 0 (off), 128 or 256.
				//   newEntry.AESKeySize = 256;

				// To permit the zip to be unpacked by built-in extractor in WinXP and Server2003, WinZip 8, Java, and other older code,
				// you need to do one of the following: Specify UseZip64.Off, or set the Size.
				// If the file may be bigger than 4GB, or you do not need WinXP built-in compatibility, you do not need either,
				// but the zip will be in Zip64 format which not all utilities can understand.
				//   zipStream.UseZip64 = UseZip64.Off;
				newEntry.Size = fi.Length;

				zipStream.PutNextEntry(newEntry);

				// Zip the file in buffered chunks
				// the "using" will close the stream even if an exception occurs
				progressDialog.Update(entryName);
				byte[ ] buffer = new byte[4096];
				using (FileStream streamReader = File.OpenRead(filename)) {
					ICSharpCode.SharpZipLib.Core.StreamUtils.Copy(streamReader, zipStream, buffer);
				}
				zipStream.CloseEntry();
			}
			string[ ] folders = Directory.GetDirectories(path);
			foreach (string folder in folders) {
				string folderName = System.IO.Path.GetFileName(folder);

				int indx = MainClass.Settings.IgnoresFolders.FindIndex(x => x.Folder == folderName && x.IsForIde);

				if(indx <0 )
					CompressFolder(folder, zipStream, folderOffset);
			}
		}

		private void OnTimeElapsed(object o, ElapsedEventArgs args){

			/*Gtk.Application.Invoke(delegate {

				if(progressDialog != null)
					progressDialog.AutomaticUpdate();
			});*/

			if(progressDialog != null)
				progressDialog.AutomaticUpdate();
			//Console.WriteLine("aaaaaa");

			while (Application.EventsPending ())
				Application.RunIteration ();
		}


		private FileItem FindFile(string filePath)
		{
			return FilesProperty.Find(x => x.SystemFilePath
				== filePath);

		}

		/*private void GenerateResolution()
		{
			Resolution = new Condition();
			Resolution.Name = MainClass.Settings.Resolution.Name;
			Resolution.Id = MainClass.Settings.Resolution.Id;
			Resolution.Rules = new List<Rule>(MainClass.Settings.Resolution.Rules.ToArray());
		}*/

		public List<string> ConvertArtefacToNames(string platform)
		{

			if (String.IsNullOrEmpty(this.ProjectArtefac)) {
				string name = System.IO.Path.GetFileNameWithoutExtension(this.AppFilePath);

				this.ProjectArtefac = String.Format("{0}_$({1})_$({2})", name, MainClass.Settings.Platform.Name, MainClass.Settings.Resolution.Name);
			}
			return new List<string>();
		}

		static internal Project OpenProject(string filePath,bool showError)
		{
			return OpenProject(filePath,true,false);
		}

		static internal Project OpenProject(string filePath)
		{
			return OpenProject(filePath,true);
		}

		static internal Project OpenProject(string filePath,bool showError,bool throwError)
		{
			if (System.IO.File.Exists(filePath)){

				try{

					using (FileStream fs = File.OpenRead(filePath)) {
						XmlSerializer serializer = new XmlSerializer(typeof(Project));

						Project p = (Project)serializer.Deserialize(fs);
							p.FilePath = filePath;

						if(!System.IO.File.Exists(p.AbsolutAppFilePath)){
								if(showError){
									MessageDialogs md = new MessageDialogs(MessageDialogs.DialogButtonType.Ok, MainClass.Languages.Translate("project_is_corrupted_f2", filePath), "", Gtk.MessageType.Error,null);
									md.ShowDialog();
								} else if(throwError){
									throw new Exception(MainClass.Languages.Translate("project_is_corrupted_f2", filePath));
								}
							return null;
						}
						return p;
					}
				}catch(Exception ex){

					Tool.Logger.Error(ex.Message);

					if(throwError){
						throw ex;
					}

					MessageDialogs md = new MessageDialogs(MessageDialogs.DialogButtonType.OkCancel, MainClass.Languages.Translate("project_is_corrupted", filePath), MainClass.Languages.Translate("delete_corupted_project"), Gtk.MessageType.Question,null);
					int res = md.ShowDialog();
					if(res == (int)Gtk.ResponseType.Ok) {
						try{
							System.IO.File.Delete(filePath);
						}catch(Exception exx){
							MessageDialogs mdError = new MessageDialogs(MessageDialogs.DialogButtonType.OkCancel, "Error", exx.Message, Gtk.MessageType.Error,null);
							mdError.ShowDialog();
							}
						}
					return null;

				}
			}else {
				if(showError){
					MessageDialogs md = new MessageDialogs(MessageDialogs.DialogButtonType.Ok, MainClass.Languages.Translate("project_not_exit_f1", filePath), "", Gtk.MessageType.Error,null);
					md.ShowDialog();
				} if(throwError){
					throw new Exception(MainClass.Languages.Translate("project_is_corrupted_f2", filePath));
				}
				return null;
			}
		}

		/*public void GetAllFiles(ref List<string> filesList,string path)
		{
			if (!Directory.Exists(path))
				return;

			DirectoryInfo di = new DirectoryInfo(path);

			foreach (DirectoryInfo d in di.GetDirectories()){
				int indx = -1;
				indx = MainClass.Settings.IgnoresFolders.FindIndex(x => x.Folder == d.Name && x.IsForIde);

				if(indx<0){
					GetAllFiles(ref filesList, d.FullName);
				}
			}

			foreach (FileInfo f in di.GetFiles()) {

				//if (f.Extension == ".ms") continue;

				filesList.Add( f.FullName );
			}

		}*/

		public void GeneratePublishCombination()
		{
			if(this.ProjectUserSetting.CombinePublish != null)
				this.ProjectUserSetting.CombinePublish.Clear();

			if (String.IsNullOrEmpty(this.ProjectArtefac)){
				string name = System.IO.Path.GetFileNameWithoutExtension(this.AppFilePath);
				this.ProjectArtefac = String.Format("{0}_$({1})_$({2})", name, MainClass.Settings.Platform.Name, MainClass.Settings.Resolution.Name);
			}

			List<Condition> fullCondition = new List<Condition>();

			//fullCondition.Add(MainClass.Settings.Platform.Clone());

			/*if ((this.SelectResolution == null) ||
				(this.SelectResolution.Count < 1)) {

				this.SelectResolution = new List<int>();
				SelectResolution.Add(-1);
			}*/

			// pridam rozlisenie,
			//fullCondition.Add(this.Resolution.Clone());

			fullCondition.Add(MainClass.Settings.Resolution.Clone());

			// pridam Conditions, ale len tie ktore su vmene pre aplikaciu
			foreach (Condition con in this.ConditoinsDefine) {
				int i = this.ProjectArtefac.IndexOf(String.Format("$({0})", con.Name));
				if (i > -1)
					fullCondition.Add(con.Clone());
			}

			int indx = 0;
			List<CombinePublish> listCC = new List<CombinePublish>();

			// kombinacie si predplnim aplikaciami
			foreach (Rule rl in MainClass.Settings.Platform.Rules) {

				CombinePublish cc = new CombinePublish();

				CombineCondition cr = new CombineCondition();
				cr.RuleId = rl.Id;
				cr.RuleName = rl.Specific;
				cr.ConditionId = MainClass.Settings.Platform.Id;
				cr.ConditionName = MainClass.Settings.Platform.Name;
				cc.combineRule = new List<CombineCondition>();
				cc.combineRule.Add(cr);
				//cc.ProjectName = this.ProjectArtefac;
				cc.ProjectName = this.ProjectArtefac.Replace(String.Format("$({0})", cr.ConditionName), cr.RuleName);
				listCC.Add(cc);
			}

			// samotne kombibnacie
			while (indx < fullCondition.Count) {
				List<CombinePublish> listCCcopy = MainClass.Tools.Clone<CombinePublish>(listCC);
				listCC.Clear();
				foreach (CombinePublish cc in listCCcopy)

						foreach (Rule rl in fullCondition [indx].Rules){
							CombinePublish cc2 = cc.Clone();

							CombineCondition cr = new CombineCondition();
							cr.RuleId = rl.Id;
							if (fullCondition [indx].Name == MainClass.Settings.Resolution.Name)
								cr.RuleName = rl.Specific;
							else
								cr.RuleName = rl.Name;
							cr.ConditionId = fullCondition [indx].Id;
							cr.ConditionName = fullCondition [indx].Name;
							cc2.combineRule.Add(cr);
							cc2.ProjectName = cc2.ProjectName.Replace(String.Format("$({0})", cr.ConditionName), cr.RuleName);
							listCC.Add(cc2);
						}
				indx++;
			}
			ProjectUserSetting.CombinePublish = new List<CombinePublish>(listCC.ToArray());
			// return listCC;
		}

	}

	public class FileItem
	{
		public FileItem()
		{
		}

		public FileItem(string Path,bool Exluded)
		{
			this.FilePath = Path;
			this.IsExcluded = Exluded;
		}

		[XmlAttribute("path")]
		public string FilePath;

		[XmlIgnore]
		public string SystemFilePath{
			get{
				return Tool.FileUtility.GetSystemPath(FilePath);
			}
		}

		[XmlAttribute("isDirectory")]
		public bool IsDirectory;

		[XmlAttribute("excluded")]
		public Boolean IsExcluded;

		[XmlArray("conditions")]
		[XmlArrayItem("condition")]
		public List<ConditionRule> ConditionValues;
	}

	public enum TypeFile
	{
		AppFile,
		Directory,
		SourceFile,
		StartFile,
		ExcludetFile
	}

}


