//
// RecentOpen.cs
//
// Author:
//   Mike Krüger <mkrueger@novell.com>
//
// Copyright (C) 2009 Novell, Inc (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//


using System;
using System.Xml;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Moscrif.IDE.Iface
{
	public class FdoRecentFiles : RecentFiles, IDisposable
	{
		RecentFileStorage recentFiles;
		
		const string projGroup = "Moscrif Projects";
		const string fileGroup = "Moscrif Files";
		const string workspaceGroup = "Moscrif Workspace";
		const string favoriteGroup = "Favorite Folder";

		const string libsGroup = "Libs Folder";
		const string publishGroup = "Publish Folder";
		const string emulatorGroup = "Emulator Folder";

		const int ItemLimit = 20;
		
		public FdoRecentFiles () : this (RecentFileStorage.DefaultPath)
		{
		}
		
		public FdoRecentFiles (string storageFile)
		{
			recentFiles = new RecentFileStorage (storageFile);
			recentFiles.RemoveMissingFiles (projGroup, fileGroup,workspaceGroup);
		}
		
		public override event EventHandler Changed {
			add { recentFiles.RecentFilesChanged += value; }
			remove { recentFiles.RecentFilesChanged -= value; }
		}
		
		public override IList<RecentFile> GetProjects ()
		{
			return Get (projGroup);
		}
		
		public override IList<RecentFile> GetFiles ()
		{
			return Get (fileGroup);
		}

		public override IList<RecentFile> GetWorkspace ()
		{
			return Get (workspaceGroup);
		}

		public override IList<RecentFile> GetFavorite ()
		{
			return Get (favoriteGroup);
		}

		public override IList<RecentFile> GetLibsFavorite ()
		{
			return Get (libsGroup);
		}

		public override IList<RecentFile> GetPublishFavorite ()
		{
			return Get (publishGroup);
		}

		public override IList<RecentFile> GetEmulatorFavorite ()
		{
			return Get (emulatorGroup);
		}
		
		IList<RecentFile> Get (string grp)
		{
			var gp = recentFiles.GetItemsInGroup (grp);
			return gp.Select (i => new RecentFile (i.LocalPath, i.Private, i.Timestamp)).ToList ();
		}
		
		public override void ClearProjects ()
		{
			recentFiles.ClearGroup (projGroup);
		}
		
		public override void ClearFiles ()
		{
			recentFiles.ClearGroup (fileGroup);
		}

		public override void ClearWorkspace ()
		{
			recentFiles.ClearGroup (workspaceGroup);
		}

		public override void ClearFavorite ()
		{
			recentFiles.ClearGroup (favoriteGroup);
		}

		public override void ClearLibsFavorite ()
		{
			recentFiles.ClearGroup (libsGroup);
		}

		public override void ClearPublishFavorite ()
		{
			recentFiles.ClearGroup (publishGroup);
		}

		public override void ClearEmulatorFavorite ()
		{
			recentFiles.ClearGroup (emulatorGroup);
		}

		public override void AddFile (string fileName, string displayName)
		{
			Add (fileGroup, fileName, displayName);
		}
		
		public override void AddProject (string fileName, string displayName)
		{
			Add (projGroup, fileName, displayName);
		}

		public override void AddWorkspace (string fileName, string displayName)
		{
			Add (workspaceGroup, fileName, displayName);
		}

		public override void AddFavorite (string fileName, string displayName)
		{
			Add (favoriteGroup, fileName, displayName);
		}

		public override void AddLibsFavorite (string fileName, string displayName)
		{
			Add (libsGroup, fileName, displayName);
		}

		public override void AddPublishFavorite (string fileName, string displayName)
		{
			Add (publishGroup, fileName, displayName);
		}

		public override void AddEmulatorFavorite (string fileName, string displayName)
		{
			Add (emulatorGroup, fileName, displayName);
		}

		void Add (string grp, string fileName, string displayName)
		{
			var mime = "text/plain";//DesktopService.GetMimeTypeForUri (fileName);
			var uri = RecentFileStorage.ToUri (fileName);
			var recentItem = new RecentItem (uri, mime, grp) { Private = displayName };
			recentFiles.AddWithLimit (recentItem, grp, ItemLimit);
		}
		
		public override void NotifyFileRemoved (string fileName)
		{
			recentFiles.RemoveItem (RecentFileStorage.ToUri (fileName));
		}
		
		public override void NotifyFileRenamed (string oldName, string newName)
		{
			recentFiles.RenameItem (RecentFileStorage.ToUri (oldName), RecentFileStorage.ToUri (newName));
		}
		
		public void Dispose ()
		{
			recentFiles.Dispose ();
			recentFiles = null;
		}
	}
		
	public abstract class RecentFiles
	{
		public abstract IList<RecentFile> GetFiles ();
		public abstract IList<RecentFile> GetProjects ();
		public abstract IList<RecentFile> GetWorkspace ();
		public abstract IList<RecentFile> GetFavorite();
		public abstract IList<RecentFile> GetLibsFavorite();
		public abstract IList<RecentFile> GetPublishFavorite();
		public abstract IList<RecentFile> GetEmulatorFavorite();
		public abstract event EventHandler Changed;
		public abstract void ClearProjects ();
		public abstract void ClearFiles ();
		public abstract void ClearWorkspace ();
		public abstract void ClearFavorite ();
		public abstract void ClearLibsFavorite();
		public abstract void ClearPublishFavorite();
		public abstract void ClearEmulatorFavorite();
		public abstract void AddFile (string fileName, string displayName);
		public abstract void AddProject (string fileName, string displayName);
		public abstract void AddWorkspace (string fileName, string displayName);
		public abstract void AddFavorite (string fileName, string displayName);
		public abstract void AddLibsFavorite (string fileName, string displayName);
		public abstract void AddPublishFavorite (string fileName, string displayName);
		public abstract void AddEmulatorFavorite (string fileName, string displayName);
		public abstract void NotifyFileRemoved (string filename);
		public abstract void NotifyFileRenamed (string oldName, string newName);
		
		/*public void AddFile (string fileName, string projectName//MonoDevelop.Projects.Project project)
		{
			var projectName = project != null? project.Name : null;
			var displayName = projectName != null?
				string.Format ("{0} [{1}]", Path.GetFileName (fileName), projectName)
				: Path.GetFileName (fileName);
			AddFile (fileName, displayName);
		}*/
	}
	
	public class RecentFile
	{
		string displayName, fileName;
		DateTime timestamp;
		
		public RecentFile (string fileName, string displayName, DateTime timestamp)
		{
			this.fileName = fileName;
			this.displayName = displayName;
			this.timestamp = timestamp;
		}

		public string FileName { get { return fileName; } }
		public string DisplayName {
			get {
				return string.IsNullOrEmpty (displayName)? Path.GetFileName (fileName) : displayName;
			}
		}
		
		public DateTime TimeStamp { get { return timestamp; } }
		
		public override string ToString ()
		{
			return FileName;
		}
	}
}

