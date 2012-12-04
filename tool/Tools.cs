using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using MessageDialogs = Moscrif.IDE.Controls.MessageDialog;
using System.Timers;
using System.Text.RegularExpressions;
using Moscrif.IDE.Controls;
using Moscrif.IDE.Iface.Entities;

namespace Moscrif.IDE.Tool
{
	public class Tools
	{
		private List<string> validExtension = new List<string>(new string[]{".xml",".ms",".mso",".txt",".db",".png",".jpg",".bmp",".tab",".app",".msp",".msw",".mss"});
		private ProgressDialog progressDialog;

		/// <summary>
		/// Get icon for extension
		/// </summary>
		/// <param name="extension">Extension.</param>
		/// <returns>Return icon name</returns>
		public string GetIconForExtension(string extension)
		{
			string stockIcon = "";
			
			switch (extension) {
			case ".xml":
				stockIcon = "file-html.png";
				break;
			case ".ms":
				stockIcon = "file-ms.png";
				break;
			case ".mso":
				stockIcon = "file-ms.png";
				break;
			case ".txt":
				stockIcon = "file-text.png";
				break;
			case ".db":
				stockIcon = "file-database.png";
				break;
			case ".png":
			case ".jpg":
			case ".jpeg":
			case ".bmp":
			case ".gif":
			case ".tif":
			case ".svg":				
				stockIcon = "file-image.png";
				break;
			default:
				stockIcon = "file.png";
				break;
			}
			
			return stockIcon;
		}

		public ExtensionSetting FindFileTyp(string extension){
			if(MainClass.Settings.ExtensionList== null || MainClass.Settings.ExtensionList.Count<1){
				MainClass.Settings.GenerateExtensionList();
			}

			foreach(ExtensionSetting es in MainClass.Settings.ExtensionList){
				foreach(string str in es.Extensions){
					if(str.ToLower() == extension.ToLower()){
						return es;
					}
				}
			}
			return null;
		}

		/// <summary>
		/// Open folder in system explorel.
		/// </summary>
		/// <param name="path">Folder.</param>
		public void OpenFolder(string path){
			try {
				if (MainClass.Platform.IsWindows) {
					string windir = Environment.GetEnvironmentVariable("WINDIR");
					System.Diagnostics.Process process = new System.Diagnostics.Process();
					process.StartInfo.FileName = windir + @"\explorer.exe";
					process.StartInfo.Arguments = path;
					process.Start();
				} else {
					System.Diagnostics.Process.Start(path);
				}
			} catch(Exception ex) {

				Logger.Error(ex.Message,null);
			}
		}

		public void CopyDirectory(string src, string dst, bool ignoreDotDirecto,bool progress){

			if (progress){
				int fileCount = Directory.GetFiles(src, "*.*", SearchOption.AllDirectories).Length;
				int directoryCount = Directory.GetDirectories(src, "*.*", SearchOption.AllDirectories).Length;
				//Console.WriteLine("fileCount ->"+fileCount);
				//Console.WriteLine("directoryCount ->"+directoryCount);
				int allCount= fileCount+directoryCount;

				double step = 1 / (allCount * 1.0);
				MainClass.MainWindow.ProgressStart(step, MainClass.Languages.Translate("copy_f1"));
				progressDialog = new ProgressDialog(MainClass.Languages.Translate("copy_f2"),ProgressDialog.CancelButtonType.None,allCount,MainClass.MainWindow);
			}

			CopyDirectory(src,dst,ignoreDotDirecto);

			if (progress){
				MainClass.MainWindow.ProgressEnd();
				if(progressDialog!= null)
					progressDialog.Destroy();
			}
		}

		public void CreateLinks(string src, string dst, bool ignoreDotDirecto,bool onlySrc){

				if (MainClass.Platform.IsWindows){

					string junction =System.IO.Path.Combine(MainClass.Paths.AppPath  ,"junction.exe");
					Logger.Log("junction -> "+junction);
					if(File.Exists(junction)){
						string path = CreateLinks(src, dst, true,junction,onlySrc);
						Logger.Log("path 1 "+path);

						if(!File.Exists(path)) Logger.Log("NOT Exist ->"+path);

						if (!String.IsNullOrEmpty(path))
							try {

								string argum = String.Format("/c  \"{0}\"",path);//"/c " + path
								Logger.Log("RunProcess ->"+argum);
								MainClass.MainWindow.RunProcess("cmd.exe", argum , MainClass.Settings.LibDirectory, false, new EventHandler(
									delegate{
										Logger.Log("ENDProcess "+path);

										try{
											System.IO.File.Delete(path);
										}catch{	}
									}
								));

							} catch {}
					} else {
						throw new Exception(MainClass.Languages.Translate("cannot_create_links_librar")  + "\n"+MainClass.Languages.Translate("junction_not_found")  );
						/*MessageDialogs md =
						new MessageDialogs(MessageDialogs.DialogButtonType.Ok, MainClass.Languages.Translate("cannot_create_links_librar"),MainClass.Languages.Translate("junction_not_found"), Gtk.MessageType.Error,null);
						md.ShowDialog();*/

					}
				} else {
					string path = CreateLinksUnix(src, dst, true,onlySrc);
					if (!String.IsNullOrEmpty(path))
						try {
							MainClass.MainWindow.RunProcess("/bin/bash", path,String.Empty, false, new EventHandler(
									delegate{
											Logger.Log("ENDProcess "+path);
											//Console.WriteLine("ENDProcess ");
										try{
											System.IO.File.Delete(path);
										}catch{
											//Console.WriteLine("ERRORDELETE ");
										}
									}
								));
						} catch {
						}
				}
		}

		/// <summary>
		/// Recalculate Image Size
		/// </summary>
		/// <param name="imageWidth">Image width.</param>
		/// <param name="imageHeight">Image height</param>
		/// <param name="maxWidth">Max new  width</param>
		/// <param name="maxHeight">Max new  height</param>
		/// <param name="width">New width</param>
		/// <param name="height">New height</param>
		public void RecalculateImageSize(int imageWidth, int imageHeight, int maxWidth, int maxHeight, ref int width, ref int height)
		{
			if (maxWidth > imageWidth && maxHeight > imageHeight)
			{
				width = imageWidth;
				height = imageHeight;
				return;
			}
			
			int imageW = imageWidth;
			int imageH = imageHeight;
			
			double wIndex = (double)imageWidth / (double)maxWidth;
			double hIndex = (double)imageHeight / (double)maxHeight;
			
			if (hIndex > wIndex)
			{
				height = maxHeight;
				width = (int)((imageW * height) / imageH);
			}
			else
			{
				width = maxWidth;
				height = (int)((imageH * width) / imageW);
			}
		}

		/// <summary>
		/// Unzip File to output directory.
		/// </summary>
		/// <param name="zippPath">Path zip file.</param>
		/// <param name="outputPath">Output directory path.</param>
		/// <returns>Return tlist uzipped files</returns>
		public List<string> UnzipFile(string zippPath,string outputPath){

			ProgressDialog pd = new ProgressDialog("",ProgressDialog.CancelButtonType.None,1,MainClass.MainWindow);

			List<string> listFile = new List<string>();
			if (!System.IO.File.Exists(zippPath)){
				
				//WriteMesage("ERROR :  FILE NOT FOUND :"+ zippPath );
				return listFile;
	
			}
	
			ZipConstants.DefaultCodePage = Encoding.UTF8.CodePage;//Thread.CurrentThread.CurrentCulture.TextInfo.ANSICodePage;
	
			//ProgressDialog progD = new ProgressDialog("Unpacking",ProgressDialog.CancelButtonType.None, 2048,null);
			pd.Reset(1,"Unpacking");
			
			using (ZipInputStream s = new ZipInputStream(File.OpenRead(zippPath))) {
				
				ZipEntry theEntry;
				while ((theEntry = s.GetNextEntry()) != null) {
					
					string directoryName = System.IO.Path.GetDirectoryName(theEntry.Name);
					string fileName = System.IO.Path.GetFileName(theEntry.Name);
						//WriteMesage("directoryName: "+ directoryName);
						//WriteMesage("fileName: "+ fileName );

	
					if (!String.IsNullOrEmpty(fileName)) {
	
						string newDirectoryPath = System.IO.Path.Combine(outputPath, directoryName);
						string newFilePath = System.IO.Path.Combine(newDirectoryPath, fileName);
						int count = (int)(s.Length/2048)+2;
	
						try{
							if (!Directory.Exists(newDirectoryPath)){
								Directory.CreateDirectory(newDirectoryPath);
							}

							pd.Reset(count,theEntry.Name);
	
							using (FileStream streamWriter = File.Create(newFilePath)){//theEntry.Name)) {
						
								int size = 1048576;//2048;
								byte[] data = new byte[1048576];//[2048];
								while (true) {
									//progD.Update(fileName);
									pd.Update(fileName);
								
									size = s.Read(data, 0, data.Length);
									if (size > 0) {
										streamWriter.Write(data, 0, size);
									} else {
										break;
									}
								}
							}
						}catch{//(Exception ex){
							MessageDialogs md =
								new MessageDialogs(MessageDialogs.DialogButtonType.YesNo, MainClass.Languages.Translate("cannot_umpack_file",fileName) ,MainClass.Languages.Translate("continue"), Gtk.MessageType.Error,null);
							int res = md.ShowDialog();
							//WriteMesage(String.Format("ERROR IN FILE {0}: {1}",fileName,ex.Message) );
							if (res == (int)Gtk.ResponseType.No){
								//cancelUnpack = true;
								return listFile;
							}
						}
						listFile.Add(newFilePath);
					}
				}		
			}
			//progD.Destroy();
			pd.Destroy();
			return listFile;
		}

		/// <summary>
		/// Remove diacritics from text
		/// </summary>
		/// <param name="s">text.</param>
		/// <returns>Return text without diacritics</returns>
		public string RemoveDiacritics(string s)
		{
			s = s.Normalize(NormalizationForm.FormD);
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < s.Length; i++)
				if (CharUnicodeInfo.GetUnicodeCategory(s[i]) != UnicodeCategory.NonSpacingMark)
					sb.Append(s[i]);
			string report = sb.ToString();
			return report.Normalize(NormalizationForm.FormC);
		}

		/// <summary>
		/// Remove diacritics and non letter and non digit char from text
		/// </summary>
		/// <param name="s">text.</param>
		/// <returns>Return Text</returns>
		public string RemoveDiacriticsAndOther(string s)
		{
			s = s.Normalize(NormalizationForm.FormD);
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < s.Length; i++)
				if (CharUnicodeInfo.GetUnicodeCategory(s[i]) != UnicodeCategory.NonSpacingMark)
					sb.Append(s[i]);
			string report = sb.ToString();
			report = report.Normalize(NormalizationForm.FormC);
			//Regex regex = new Regex("([a-zA-Z0-9_])", RegexOptions.Compiled);
			//regex.Replace(report,
			return Regex.Replace(report,"([^a-zA-Z0-9_])",String.Empty);


		}

		/// <summary>
		/// Create Pixbuf from stockId.
		/// </summary>
		/// <param name="iconName">Image stock id.</param>
		/// <param name="size">Icon Size.</param>
		/// <returns>Return Pixbuf</returns>
		public Gdk.Pixbuf GetIconFromStock(string iconName, Gtk.IconSize size)
		{
			string stockid = iconName;
			if (stockid != null) {
				Gtk.IconSet iconset = Gtk.IconFactory.LookupDefault(stockid);
				if (iconset != null) {
					return iconset.RenderIcon(Gtk.Widget.DefaultStyle, Gtk.TextDirection.None, Gtk.StateType.Normal, size, null, null);
				}
			}
			
			return null;
		}

		/// <summary>
		/// Create component stockImage and Label
		/// </summary>
		/// <param name="stockId">Image stock id.</param>
		/// <param name="lab">Label.</param>
		/// <returns>Return HBox widget</returns>
		public Gtk.HBox CreatePicLabelWidget(string stockId, string lab) {
		      Gtk.HBox hb = new Gtk.HBox();

		      hb.PackStart(new Gtk.Image(stockId,Gtk.IconSize.Button ), false,false, 0);
		      hb.PackStart(new Gtk.Label(lab), true, true, 5);
			
		      return hb;
	    	}

		/// <summary>
		/// Checks whether extension ignored 
		/// </summary>
		/// <param name="extension">Extension to check.</param>
		/// <returns>True is ignored.</returns>
		public bool IsIgnoredExtension(string extension){

			List<string> systemIgnoreExtension = new List<string>();
			systemIgnoreExtension.Add(".zip");
			systemIgnoreExtension.Add(".msc");

			int indxIgnoreExtension = systemIgnoreExtension.FindIndex (x=>x == extension);
			if(indxIgnoreExtension < 0){
				return false;
			} else return true;

		}

		/// <summary>
		/// Return new empty path
		/// </summary>
		/// <returns>Return new empty path.</returns>
		public string GetEmptyPath ()
    		{
        		string bpath = "temp", path = "temp";
        		for (int i = 0; System.IO.File.Exists (path); i++)
	     			path = bpath + i.ToString ();

			return path;
    		}

		/// <summary>
		/// Return Publis tool directory for device
		/// </summary>
		/// <param name="platformSpecific">Identifi device.</param>
		/// <returns>Return publish directory with publish tool.</returns>
		public string GetPublishDirectory(string platformSpecific){

			if (String.IsNullOrEmpty(platformSpecific)) return null;

			return System.IO.Path.Combine(MainClass.Settings.PublishDirectory,platformSpecific);

		}

		/// <summary>
		/// Check exist publish directory with publish tool
		/// </summary>
		/// <param name="platformSpecific">Identifi device.</param>
		/// <returns>return true if dir exist  directory with publish tool.</returns>
		public bool CheckPublishDirectory(string platformSpecific){
			string platformDir = GetPublishDirectory(platformSpecific);

			if (String.IsNullOrEmpty(platformDir)) return false;

			return Directory.Exists(platformDir);

		}

		/// <summary>
		/// Clone List.
		/// </summary>
		/// <param name="listToClone">List To Clone.</param>
		/// <returns>Return clone list.</returns>
		public List<T> Clone<T>(List<T> listToClone) where T: ICloneable
		{
			List<T> newList = new List<T>(listToClone.Count);

			listToClone.ForEach((item) =>
    				{
        				newList.Add((T)item.Clone());
    				});
			return newList;
		}

		/// <summary>
		/// Convert assembly version to Moscrif version
		/// </summary>
		/// <param name="version">Assembly version (2012.3.0.0)</param>
		/// <returns>Return Moscrif version (2012q1.125) </returns>
		public string VersionConverter(string version){
			
			string[] versions = version.Split('.');
			/*if (versions.Length != 4){
				return "Invalid Version.";
			}*/
			string versionFile = System.IO.Path.Combine(MainClass.Paths.AppPath,"version.txt");
			string sFix = "";

			if(System.IO.File.Exists(versionFile)){
				try {
					using (StreamReader file = new StreamReader(versionFile)) {
						string text = file.ReadToEnd();
						sFix = text.Trim();
					}
				} catch {
				}						
			}
						
			string webVersion = String.Format("{0}q{1}{2}", versions[0], versions[1],sFix);
			return webVersion;			
		}
	
		#region private

		private static string CreateLinksUnix(string Src, string Dst, bool ignoreDotDirectory, bool onlySrc)
		{
			string path = System.IO.Path.Combine(Dst, "link" + DateTime.Now.ToString("yyyymmddhhMMss") + ".sh");
			
			String[] Files;
			if (Dst [Dst.Length - 1] != Path.DirectorySeparatorChar)
				Dst += Path.DirectorySeparatorChar;
			
			if (!Directory.Exists(Dst))
				Directory.CreateDirectory(Dst);
			
			if(onlySrc)
			Files = new string[] {Src};
			else
				Files = Directory.GetFileSystemEntries(Src);
			
			try {
				using (StreamWriter file = new StreamWriter(path)) {
					file.WriteLine("#!/bin/bash"); ///bin/bash    //sh
					//file.WriteLine("echo It worked!");
					foreach (string Element in Files) {
						string destPath = System.IO.Path.Combine(Dst, Path.GetFileName(Element));
						if (Directory.Exists(Element)) {
							if (ignoreDotDirectory)	// "."
								if (Path.GetFileName(Element) [0] == '.')
									continue;
							//ln -s {0} {1} symbolic
							//ln {0} {1} symbolic
							file.WriteLine(String.Format("ln -s {0} {1}", Element, destPath ));
						} else{
							file.WriteLine(String.Format("cp {0} {1}", Element, destPath ));
							//file.WriteLine(String.Format("ln -s {0} {1}", Element, destPath ));
						}
					}
					file.Close();
					file.Dispose();
				}
			} catch (Exception ex) {
				throw ex;
				//return false;
				//path = "";
			}
			return path;
		}
		
		private static string CreateLinks(string Src, string Dst, bool ignoreDotDirectory, string exeFile,bool onlySrc)
		{
			string path = System.IO.Path.Combine(Dst, "link" + DateTime.Now.ToString("yyyymmddhhMMss") + ".bat");
			
			String[] Files;
			if (Dst [Dst.Length - 1] != Path.DirectorySeparatorChar)
				Dst += Path.DirectorySeparatorChar;
			
			if (!Directory.Exists(Dst))
				Directory.CreateDirectory(Dst);
			
			if(onlySrc)
			Files = new string[] {Src};
			else
				Files = Directory.GetFileSystemEntries(Src);
			
			try {
				using (StreamWriter file = new StreamWriter(path)) {
					foreach (string Element in Files) {
						string destPath = System.IO.Path.Combine(Dst, Path.GetFileName(Element));
						Tool.Logger.Debug(Element+"->" );
						
						if (Directory.Exists(Element)) {
							Tool.Logger.Debug("Link" );
							if (ignoreDotDirectory)
								if (Path.GetFileName(Element) [0] == '.')
									continue;
							file.WriteLine(String.Format("\"{0}\" /accepteula \"{1}\" \"{2}\"",exeFile, destPath, Element));//mklink /J
							
							//file.WriteLine(String.Format("FSUTIL hardlink create {0} {1}", destPath, Element));//mklink /J
						} else{
							Tool.Logger.Debug("Copy" );
							//file.WriteLine(String.Format("{0} /accepteula {1} {2}",exeFile, destPath, Element));
							//file.WriteLine(String.Format("if not exist \"{0}\" (copy \"{1}\" \"{0}\")", destPath, Element));
							file.WriteLine(String.Format("copy \"{1}\" \"{0}\"", destPath, Element));
						}
						//file.WriteLine(String.Format("FSUTIL hardlink create {0} {1}", destPath, Element));
					}
					file.Close();
					file.Dispose();
				}
			} catch (Exception ex) {
				throw ex;
				//return false;
				//path = "";
			}
			return path;
		}
		
		private void CopyDirectory(string src, string dst, bool ignoreDotDirectory)
		{
			String[] Files;
			
			if (dst[dst.Length - 1] != Path.DirectorySeparatorChar)
				dst += Path.DirectorySeparatorChar;
			if (!Directory.Exists(dst))
				Directory.CreateDirectory(dst);
			Files = Directory.GetFileSystemEntries(src);
			
			foreach (string Element in Files){
				
				if (progressDialog != null){
					MainClass.MainWindow.ProgressStep();
					progressDialog.Update (Element);
				}
				
				string destPath= System.IO.Path.Combine( dst ,Path.GetFileName(Element));
				
				if (Directory.Exists(Element)) {
					if (ignoreDotDirectory)	// "."
					{
						int indx = MainClass.Settings.IgnoresFolders.FindIndex(x => x.Folder == Path.GetFileName(Element) && x.IsForIde);
						if (indx > -1) continue;
					}
					//if (Path.GetFileName(Element)[0] == '.')continue;
					CopyDirectory(Element,destPath, ignoreDotDirectory);
				} else {
					int indx = -1;
					indx = MainClass.Settings.IgnoresFiles.FindIndex(x => x.Folder == Path.GetFileName(Element) && x.IsForIde);
					
					if(indx >-1) continue;
					
					File.Copy(Element, destPath, true);
				}
			}
		}
		

		#endregion
	}
	
}

