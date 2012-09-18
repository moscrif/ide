using System;
using Moscrif.IDE.Tool;
using MessageDialogs = Moscrif.IDE.Controls.MessageDialog;


namespace Moscrif.IDE.Devices
{
	public class EmulatorDisplay
	{
		public EmulatorDisplay(string filePath)
		{
			this.FilePath = filePath;
		}

		public string FilePath;

		public int Width;
		public int Height;
		public string Title;
		public string Author;
		public string Url;
		public bool Tablet;

		public bool Load(){
			try {
				IniFile mpi = new IniFile(FilePath);
				this.Height = mpi.ReadInt("display", "height", 1);
				this.Width = mpi.ReadInt("display", "width", 1);

				this.Title = mpi.ReadString("info", "title", System.IO.Path.GetFileName(FilePath));
				this.Author = mpi.ReadString("info", "author", " ");
				this.Url = mpi.ReadString("info", "url", " ");

				/*string tmp =mpi.ReadString("device", "tablet", " ");

				if(String.IsNullOrEmpty(tmp))
					this.Tablet = false;
				 */
				int tmp =mpi.ReadInt("device", "tablet", 0);


				if(tmp ==1) {
					this.Tablet = true;
				} else
					this.Tablet = false;
				//this.Tablet = Convert.ToBoolean(mpi.ReadString("device", "tablet", " "));
				return true;

			} catch  {
				MessageDialogs ms = new MessageDialogs(MessageDialogs.DialogButtonType.Ok, MainClass.Languages.Translate("error"), MainClass.Languages.Translate("error_load_resolution_file", FilePath), Gtk.MessageType.Error,null);
				ms.ShowDialog();
				return false;
			}
		}

		public bool Save()
		{
			try {
				IniFile mpi = new IniFile(FilePath);
				
				mpi.WriteString("info", "title", this.Title);
				mpi.WriteString("info", "author", this.Author);
				mpi.WriteString("info", "url", this.Url);

				mpi.WriteInt("display", "height", this.Height);
				mpi.WriteInt("display", "width", this.Width);

				mpi.WriteInt("device", "tablet", this.Tablet?1:0);
				
				mpi.Flush();
				return true;
			} catch {
				MessageDialogs ms = new MessageDialogs(MessageDialogs.DialogButtonType.Ok, MainClass.Languages.Translate("error"), MainClass.Languages.Translate("error_save_resolution_file" , FilePath), Gtk.MessageType.Error);
				ms.ShowDialog();
				return false;
			}
		}

		static internal EmulatorDisplay Create(string filePath,int width,int height)
		{
			if (!System.IO.File.Exists(filePath)) {
				
				try{
					EmulatorDisplay dd = new EmulatorDisplay(filePath);

					dd.Width = width;
					dd.Height = height;

					dd.Author = "Moscrif Ide Generator";
					dd.Title = System.IO.Path.GetFileName(filePath);
					dd.Url ="";
					dd.Save();
					return dd;


				}catch(Exception ex){
					MessageDialogs md = new MessageDialogs(MessageDialogs.DialogButtonType.Ok, MainClass.Languages.Translate("error"), ex.Message, Gtk.MessageType.Error);
					md.ShowDialog();
					return null;

				}
			} else {
				//throw new FileNotFoundException();
				MessageDialogs md = new MessageDialogs(MessageDialogs.DialogButtonType.Ok, MainClass.Languages.Translate("error"),  MainClass.Languages.Translate("file_notcreate_is_exist"), Gtk.MessageType.Error);
				md.ShowDialog();
				return null;
			}
		}

		static internal EmulatorDisplay Create(string filePath)
		{
			return Create(filePath,480,800);
		}
		
	}
	
	
}

