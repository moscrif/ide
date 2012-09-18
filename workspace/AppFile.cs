using System;
using System.IO;
using System.Collections.Generic;
using System.Xml;
using MessageDialogs = Moscrif.IDE.Controls.MessageDialog;

namespace Moscrif.IDE.Workspace
{
	public class AppFile
	{
		//private const string KEY_ID = "id";
		//private const string KEY_NAME = "name";
		private const string KEY_MAIN = "main";
		private const string KEY_TITLE = "title";
		private const string KEY_DESCRIPTION = "description";
		private const string KEY_AUHTOR = "author";
		private const string KEY_COPYRIGHT = "copyright";
		private const string KEY_HOMEPAGE = "homepage";
		private const string KEY_HIDDEN = "hidden";
		//private const string KEY_LIBUI = "libUi";
		private const string KEY_USES = "uses";
		private const string KEY_VERSION = "version";
		private const string KEY_ORIENTATION = "orientation";
		private const string KEY_REMOTE_CONSOLE = "remote-console";
		public string ApplicationFile
		{
			get;
			set;
		}

		//public string Id
		//{
		//	get;
		//	private set;
		//}

		public string Name
		{
			get{
				return System.IO.Path.GetFileNameWithoutExtension(ApplicationFile);
			}
			//get;
			//private set;
		}

		public string Main
		{
			get;
			set;
		}

		public string Title
		{
			get;
			set;
		}

		public string Description
		{
			get;
			set;
		}

		public string Author
		{
			get;
			set;
		}

		public string Copyright
		{
			get;
			set;
		}

		public string Homepage
		{
			get;
			set;
		}

		public string Hidden
		{
			get;
			set;
		}

		/*public string LibUi
		{
			get;
			set;
		}*/
		public string Uses
		{
			get;
			set;
		}

		public string Orientation
		{
			get;
			set;
		}

		public string Remote_Console
		{
			get;
			set;
		}

		public string Version
		{
			get;
			set;
		}

		public Dictionary<string, string> XValues
		{
			get;
			private set;
		}

		public string[] Libs {
			get {
				string tmp = Uses.Replace(",", " ");
				string[] libs = tmp.Split(' ');
				return libs;
			}
		}


		public string Directory
		{
			get {
				string dir = Path.GetDirectoryName(ApplicationFile);
				return Path.Combine(dir, Name);//Id
			}
		}

		public AppFile()
		{
			XValues = new Dictionary<string, string>();
		}

		public AppFile(string appFile)
		{
			if (!File.Exists(appFile))
				throw new Exception(MainClass.Languages.Translate("application_file_not_exist", appFile));
			XValues = new Dictionary<string, string>();
			ApplicationFile = appFile;
			Load();
		}

		public AppFile(string appFile, string projectName)//, string projectId)
		{
			if (File.Exists(appFile))
				throw new Exception(MainClass.Languages.Translate("application_file_exist", appFile));
			XValues = new Dictionary<string, string>();
			ApplicationFile = appFile;
			//Name = projectName;
			//Id = projectId;
		}

		/*     public List<FileInfo> GetAllFiles(FileTypes types = FileTypes.All)
        {
            DirectoryInfo di = new DirectoryInfo(Directory);

            List<FileInfo> files = new List<FileInfo>();
            FileInfo[] fis;

            if (types == FileTypes.All || types == FileTypes.Sources)
            {
                fis = di.GetFiles("*.msc");
                foreach (FileInfo fi in fis) files.Add(fi);
            }

            if (types == FileTypes.All || types == FileTypes.Data)
            {
                fis = di.GetFiles("*.db");
                foreach (FileInfo fi in fis) files.Add(fi);
            }

            if (types == FileTypes.All || types == FileTypes.Images)
            {
                fis = di.GetFiles("*.png");
                foreach (FileInfo fi in fis) files.Add(fi);
            }
            return files;
        }*/

		public void Load()
		{
			string line;
			using (StreamReader sr = new StreamReader(ApplicationFile))
				while ((line = sr.ReadLine()) != null) {
					line = line.Trim();
					if (String.IsNullOrEmpty(line))
						continue;
					if (line.StartsWith("#"))
						continue;
					int pos = line.IndexOf(':');
					if (pos < 0)
						continue;
					string key = line.Substring(0, pos).Trim().ToLower();
					string val = line.Substring(pos + 1).Trim();
					//if (key == KEY_ID)
					//	Id = val;
					//if (key == KEY_NAME)
					//	Name = val;
					if (key == KEY_MAIN)
						Main = val;
					if (key == KEY_TITLE)
						Title = val;
					if (key == KEY_DESCRIPTION)
						Description = val;
					if (key == KEY_AUHTOR)
						Author = val;
					if (key == KEY_COPYRIGHT)
						Copyright = val;
					if (key == KEY_HOMEPAGE)
						Homepage = val;
					if (key == KEY_USES)
						Uses = val;
					if (key == KEY_HIDDEN)
						Hidden = val;
					if (key == KEY_VERSION)
						Version = val;
					if (key == KEY_ORIENTATION)
						Orientation = val;
					if(key == KEY_REMOTE_CONSOLE)
						Remote_Console = val;

					if (key.StartsWith("x-"))
						XValues[key] = val;
				}
			//if (String.IsNullOrEmpty(Id))
			//	throw new Exception(String.Format("Application file '{0}' does not have ID!", ApplicationFile));
			//Pogram.MainForm.PrintLn(String.Format("Project '{0}' loaded succesfully...", ApplicationFile));
		}

		/*     public static Project Open(string fileName)
        {
            using (WaitCursor wait = new WaitCursor())
            {
                Project prj = null;
                try
                {
                    prj = new Project(fileName);
                }
                catch (Exception ex)
                {
                    Program.PrintErrorLn(String.Format("Error loading application!\n{0}", ex.Message));
                }
                if (prj != null)
                    Program.Workspace.Projects.Add(prj);
                return prj;
            }
        }*/

		public void Save()
		{
			using (StreamWriter sw = new StreamWriter(ApplicationFile, false)) {
				/*sw.WriteLine("#");
				sw.WriteLine("# Managed by Moscrif-Ide, last changes: {0} by {1} on {2}", DateTime.Now, Environment.UserName, Environment.MachineName);
				sw.WriteLine("#");
				sw.WriteLine();*/
				//sw.WriteLine("{0,15}: {1}", KEY_ID, Id);
				//sw.WriteLine("{0,15}: {1}", KEY_NAME, Name);
				sw.WriteLine("{0,15}: {1}", KEY_MAIN, Main);
				sw.WriteLine("{0,15}: {1}", KEY_TITLE, Title);
				sw.WriteLine("{0,15}: {1}", KEY_DESCRIPTION, Description);
				sw.WriteLine("{0,15}: {1}", KEY_AUHTOR, Author);
				sw.WriteLine("{0,15}: {1}", KEY_COPYRIGHT, Copyright);
				sw.WriteLine("{0,15}: {1}", KEY_HOMEPAGE, Homepage);
				sw.WriteLine("{0,15}: {1}", KEY_USES, Uses);
				sw.WriteLine("{0,15}: {1}", KEY_ORIENTATION,Orientation);	
				sw.WriteLine("{0,15}: {1}", KEY_REMOTE_CONSOLE,Remote_Console);
				sw.WriteLine("{0,15}: {1}", KEY_VERSION,Version);
				if (!String.IsNullOrEmpty(Hidden))
					sw.WriteLine("{0,15}: {1}", KEY_HIDDEN, "true");
				foreach (string key in XValues.Keys)
					sw.WriteLine("{0,15}: {1}", key, XValues[key]);
				}

		}

		public override string ToString()
		{
			return Name;
		}
	}
}

