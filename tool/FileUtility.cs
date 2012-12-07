using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Moscrif.IDE.Iface.Entities;

namespace Moscrif.IDE.Tool
{
	public static class FileUtility
	{
  
		internal readonly static char[] separators = { Path.DirectorySeparatorChar, Path.VolumeSeparatorChar };

		/// <summary>
		/// Removes trailing '.' character from a path.
		/// </summary>
		/// <param name="path">The path from which to remove the trailing
		/// '.' character.</param>
		/// <returns>A path without any trailing '.'.</returns>
		public static string TrimTrailingDotCharacter(string path)
		{
			if (path.Length > 0 && path [path.Length - 1] == '.')
				return path.Remove(path.Length - 1, 1);
			else
				return path;
		}

		/// <summary>
		/// Removes trailing '\' or '/' from a path.
		/// </summary>
		/// <param name="path">The path from which to remove the trailing
		/// directory separator.</param>
		/// <returns>A path without any trailing directory separator.</returns>
		public static string TrimTrailingDirectorySeparator(string path)
		{
			if (path.Length == 0)
				return path;
			if ((path [path.Length - 1] == separators [0]) || (path [path.Length - 1] == separators [1])){
				path = path.Remove(path.Length - 1, 1);
			}
			return path;
		}


		/// <summary>
		/// Removes starting '\' or '/' from a path.
		/// </summary>
		/// <param name="path">The path from which to remove the starting
		/// directory separator.</param>
		/// <returns>A path without any starting directory separator.</returns>
		public static string TrimStartingDirectorySeparator(string path) {
			if (path.Length == 0)
				return path;
			if ((path [0] == separators [0]) || (path [0] == separators [1])) {
				path = path.Remove(0, 1);
				TrimStartingDirectorySeparator(path);
			}

			return path;
		}


		/// <summary>
		/// Removes starting '.' character from a path.
		/// </summary>
		/// <param name="path">The path from which to remove the starting
		/// '.' character.</param>
		/// <returns>A path without any starting '.'.</returns>
		public static string TrimStartingDotCharacter(string path)
		{
			if (path.Length > 0 && path [0] == '.') {

				path = path.Remove(0, 1);
				TrimStartingDotCharacter(path);
				//return path;
			}
			return path;
		}


		/// <summary>
		/// Converts a given absolute path and a given base path to a path that leads
		/// from the base path to the absoulte path. (as a relative path)
		/// </summary>
		/// <remarks>
		/// <para>The returned relative path will be of the form:</para>
		/// <para><code>
		/// .\Test
		/// .\Test\Test.prjx
		/// .\Test.prjx
		/// .\
		/// ..\bin
		/// ..\..\bin\debug
		/// </code>
		/// </para>
		/// </remarks>
		public static string AbsoluteToRelativePath(string baseDirectoryPath, string absPath)
		{
			//absPath = absPath.Replace('/',Path.DirectorySeparatorChar);
			//absPath = absPath.Replace('\\',Path.DirectorySeparatorChar);

			// Remove trailing '.'
			baseDirectoryPath = TrimTrailingDotCharacter(baseDirectoryPath);

			// Remove trailing directory separators.
			baseDirectoryPath = TrimTrailingDirectorySeparator(baseDirectoryPath);
			absPath = TrimTrailingDirectorySeparator(absPath);

			// Remove ".\" occurrences.
			absPath = absPath.Replace(String.Concat(".", separators [0]), String.Empty);
			absPath = absPath.Replace(String.Concat(".", separators [1]), String.Empty);

			string[] bPath = baseDirectoryPath.Split(separators);
			string[] aPath = absPath.Split(separators);
			int indx = 0;
			for (; indx < Math.Min(bPath.Length, aPath.Length); ++indx)
        		//if(!bPath[indx].Equals(aPath[indx]))
				if (String.Compare(bPath [indx], aPath [indx], true) != 0)
					break;

			if (indx == 0)
				return absPath;

			StringBuilder erg = new StringBuilder();

			if (indx == bPath.Length) {
				erg.Append('.');
				erg.Append(Path.DirectorySeparatorChar);
			} else
				for (int i = indx; i < bPath.Length; ++i) {
					erg.Append("..");
					erg.Append(Path.DirectorySeparatorChar);
				}
			erg.Append(String.Join(Path.DirectorySeparatorChar.ToString(), aPath, indx, aPath.Length - indx));

			return erg.ToString();
		}

		/// <summary>
		/// Converts a given relative path and a given base path to a path that leads
		/// to the relative path absoulte.
		/// </summary>
		public static string RelativeToAbsolutePath(string baseDirectoryPath, string relPath)
		{
			relPath = relPath.Replace('/', Path.DirectorySeparatorChar);
			relPath = relPath.Replace('\\', Path.DirectorySeparatorChar);

			if (separators [0] != separators [1] && relPath.IndexOf(separators [1]) != -1)
        			return relPath;

			string[] bPath = baseDirectoryPath.Split(separators [0]);
			string[] rPath = relPath.Split(separators [0]);
			int indx = 0;

			for (; indx < rPath.Length; ++indx)
			        if (!rPath[indx].Equals("..")) {
			          break;
			        }
			if (indx == 0)
        			return baseDirectoryPath + separators[0] + String.Join(Path.DirectorySeparatorChar.ToString(), rPath, 1, rPath.Length - 1);
			

			string erg = String.Join(Path.DirectorySeparatorChar.ToString(), bPath, 0, Math.Max(0, bPath.Length - indx));

			erg += separators [0] + String.Join(Path.DirectorySeparatorChar.ToString(), rPath, indx, rPath.Length - indx);

			return erg;
		}

		public static string GetSystemPath(string path)
		{
			if(String.IsNullOrEmpty(path))
				return path;

			string sysPath = path.Replace('/', Path.DirectorySeparatorChar);
			sysPath = sysPath.Replace('\\', Path.DirectorySeparatorChar);
			return sysPath;
		}

		public static MatchCollection FindInFileRegEx(string filePath, string expresion)
		{

			if (!System.IO.File.Exists(filePath))
				return null;

			FileAttributes fa = File.GetAttributes(filePath);
			if ((fa & FileAttributes.System) == FileAttributes.System) {
				return null;
			}

			if ((fa & FileAttributes.ReadOnly) == FileAttributes.ReadOnly) {
				return null;
			}

			try{
				StreamReader reader = new StreamReader(filePath);
				string content = reader.ReadToEnd();
				reader.Close();
				Regex regexBar = new Regex(expresion, RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled | RegexOptions.CultureInvariant);

				MatchCollection mcAll =regexBar.Matches(content);//Regex.Matches(content, expresion);
				
				return mcAll;
			}catch {
				return null;
			}
		}

		public static List<FindResult> FindInFile(string filePath, string expresion, bool caseSensitve, bool wholeWorlds, string replaceExpresion)
		{
			if (!System.IO.File.Exists(filePath))
				return null;

			FileAttributes fa = File.GetAttributes(filePath);
			if ((fa & FileAttributes.System) == FileAttributes.System) {
				return null;
			}

			if ((fa & FileAttributes.ReadOnly) == FileAttributes.ReadOnly) {
				return null;
			}

			List<FindResult> result = new List<FindResult>();
			StreamReader reader = new StreamReader(filePath);
			bool isChanged = false;

			StringBuilder sb = new StringBuilder();

			int indx = 0;
			do {
				string line = reader.ReadLine();

				if(String.IsNullOrEmpty(line)){
					indx++;
					continue;
				}

				var comparison = caseSensitve ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
				int idx = 0;
				int delta = 0;

				while ((idx = line.IndexOf (expresion, idx, line.Length - idx, comparison)) >= 0) {
					if (!wholeWorlds || IsWholeWordAt(line, idx, expresion.Length))
					if (replaceExpresion != null) {

						Replace(ref line, idx + delta, expresion.Length, replaceExpresion);
						isChanged = true;
						//yield return new SearchResult (provider, idx + delta, replacePattern.Length);
						result.Add(new FindResult((object)indx, (object)line));
						delta += replaceExpresion.Length - expresion.Length;
					} else
					result.Add(new FindResult( (object)(indx+1),(object)line));
					//yield return new SearchResult (provider, idx, pattern.Length);
					idx += expresion.Length;
				}
				sb.AppendLine(line);


				/*	if(caseSensitve && !wholeWorlds){
			if(line.Contains(expresion) )
				result.Add(indx,line);

		} else if(!caseSensitve && !wholeWorlds ){
			if(lineUpper.Contains(expresion.ToUpper()) )
				result.Add(indx,line);

		} else if(!caseSensitve && wholeWorlds){
			if(lineUpper.Split(separators).Contains(expresion.ToUpper()))
				result.Add(indx,line);

		} else if(caseSensitve && wholeWorlds){
			if(line.Split(separators).Contains(expresion))
				result.Add(indx,line);
		}*/

				//line.Contains("string", StringComparison.CurrentCultureIgnoreCase);
				indx++;

			} while (reader.Peek() != -1);
			reader.Close();

			if (isChanged)
				try {
					StreamWriter writer = new StreamWriter(filePath);
					writer.Write(sb.ToString());
					writer.Close();
				} catch (Exception ex) {
					Tool.Logger.Error(ex.Message);
				}


			return result;
		}

		public static bool IsWordSeparator(char ch)
		{
			return !Char.IsLetterOrDigit(ch) && ch != '_';
		}

		public static bool ContainsPath(string baseDirectoryPath, string secondPath)
		{
			// Remove trailing '.'
			baseDirectoryPath = TrimTrailingDotCharacter(baseDirectoryPath);

			// Remove trailing directory separators.
			baseDirectoryPath = TrimTrailingDirectorySeparator(baseDirectoryPath);
			secondPath = TrimTrailingDirectorySeparator(secondPath);

			// Remove ".\" occurrences.
			secondPath = secondPath.Replace(String.Concat(".", separators [0]), String.Empty);
			secondPath = secondPath.Replace(String.Concat(".", separators [1]), String.Empty);

			string[] bPath = baseDirectoryPath.Split(separators);
			string[] aPath = secondPath.Split(separators);
			int indx = 0;
			for (; indx < Math.Min(bPath.Length, aPath.Length); ++indx)
				if (String.Compare(bPath [indx], aPath [indx], true) != 0)//if(!bPath[indx].Equals(aPath[indx]))
					break;

			if (indx == bPath.Length)
				return true;
			return false;

		}

		public static void GetAllFiles(ref List<string> filesList,string path)
		{
			if (!Directory.Exists(path))
				return;

			DirectoryInfo di = new DirectoryInfo(path);

			foreach (DirectoryInfo d in di.GetDirectories()) {
				int indx = -1;
				indx = MainClass.Settings.IgnoresFolders.FindIndex(x => x.Folder == d.Name && x.IsForIde);

				if (indx < 0)
					GetAllFiles(ref filesList, d.FullName);
			}

			foreach (FileInfo f in di.GetFiles()){
				int indx = -1;
				indx = MainClass.Settings.IgnoresFiles.FindIndex(x => x.Folder == f.Name && x.IsForIde);
				if(indx >-1)continue;
				
				filesList.Add(f.FullName);
			}

		}

		public static string DeleteItem(string fileName, bool isDir)
		{
			if (isDir) {

				if (!System.IO.Directory.Exists(fileName))
					return "";

				try {			
					List<string> listFile = new List<string>();
					GetAllFiles(ref listFile, fileName);

					System.IO.Directory.Delete(fileName, true);

					foreach (string file in listFile)
						MainClass.MainWindow.EditorNotebook.ClosePage(file, true);

				} catch (Exception ex){
					return ex.Message;
				}

			} else {

				if (!System.IO.File.Exists(fileName))
					return "";

				try {
					System.IO.File.Delete(fileName);
					MainClass.MainWindow.EditorNotebook.ClosePage(fileName, true);

				} catch (Exception ex) {
					return ex.Message;
				}
			}
			return "";
		}

		public static string RenameItem(string fileName, bool isDir, string newName,out string newPath)
		{
			newPath = "";
			if (isDir) {

				if (!System.IO.Directory.Exists(fileName)){
					return "";
				}

				string path = System.IO.Path.GetDirectoryName(fileName);
				newPath = System.IO.Path.Combine(path, newName);

				if (System.IO.Directory.Exists(newPath)){
					return "";
				}

				try {
					List<string> listFile = new List<string>();
					FileUtility.GetAllFiles(ref listFile, fileName);

					foreach (string file in listFile)
					MainClass.MainWindow.EditorNotebook.ClosePage(file, true);


					System.IO.Directory.Move(fileName, newPath);
				} catch(Exception ex) {
					return ex.Message;
				}
			} else {// Rename File
				if (!System.IO.File.Exists(fileName)){

					return "";
				}

				string path = System.IO.Path.GetDirectoryName(fileName);
				string extension = System.IO.Path.GetExtension(fileName);
				string newExt = System.IO.Path.GetExtension(newName);

				if (string.IsNullOrEmpty(newExt))
					newName = newName + extension;

				newPath = System.IO.Path.Combine(path, newName);
				if (System.IO.Directory.Exists(newPath)){
					return "";
				}

				try {
					System.IO.File.Move(fileName, newPath);
					MainClass.MainWindow.EditorNotebook.RenameFile(fileName, newPath);
				} catch (Exception ex){
					return ex.Message;
				}
			}
			return "";
		}

		public static void CreateDirectory(string newFile)
		{
			string fileName = System.IO.Path.GetFileName(newFile);
			string dir = System.IO.Path.GetDirectoryName (newFile);

			if (System.IO.Directory.Exists(newFile))
				newFile = System.IO.Path.Combine(dir,  fileName+"_1");

			Directory.CreateDirectory(newFile);

		}

		public static void CreateFile(string newFile, string content)
		{
			string dir = System.IO.Path.GetDirectoryName(newFile);
			string nameExtens = System.IO.Path.GetExtension(newFile);
			string nameClear = System.IO.Path.GetFileNameWithoutExtension(newFile);

			if (System.IO.File.Exists(newFile))
				newFile = System.IO.Path.Combine(dir,  nameClear+"_1"+nameExtens);

			string ext = System.IO.Path.GetExtension(newFile);
			if (ext == ".db"){
				using (FileStream fs = System.IO.File.Create(newFile))
				fs.Close();
				SqlLiteDal sqlld = new SqlLiteDal(newFile);
				//Console.WriteLine(content);
				sqlld.RunSqlScalar(content);

			} else {
				using (StreamWriter file = new StreamWriter(newFile)) {
					file.Write(content);
					file.Close();
					file.Dispose();
				}
			}
		}

		#region private
		private static bool IsWholeWordAt(string text, int offset, int length)
		{
			return (offset <= 0 || IsWordSeparator(text [offset - 1])) &&
				(offset + length >= text.Length || IsWordSeparator(text [offset + length]));
		}
		
		private static void Replace(ref string text, int offset, int length, string replacement)
		{
			text = text.Remove(offset, length);
			text = text.Insert(offset, replacement);
			/*if (document != null) {
			Gtk.Application.Invoke (delegate {
				document.Editor.Replace (offset, length, replacement);
			});
			return;
		}*/
		}
		#endregion

	}
}
