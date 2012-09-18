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

namespace Moscrif.IDE.Workspace
{
	public class WorkspaceUserFile
	{
		// workspace.nazovPc-Username.user
		public WorkspaceUserFile(string filePath)
		{
			FilePath = filePath;
			FilesSetting = new List<FileSetting>();

		}

		public WorkspaceUserFile()
		{
			FilesSetting = new List<FileSetting>();
		}

		static internal WorkspaceUserFile OpenWorkspaceUserFile(string filePath)
		{
			if (System.IO.File.Exists(filePath)){

				try{

					using (FileStream fs = File.OpenRead(filePath)) {
						XmlSerializer serializer = new XmlSerializer(typeof(WorkspaceUserFile));

						WorkspaceUserFile p = (WorkspaceUserFile)serializer.Deserialize(fs);
						p.FilePath = filePath;

					return p;
					}
				}catch{//(Exception ex){

					WorkspaceUserFile p =new WorkspaceUserFile(filePath);
					return p;
					/*MessageDialogs md = new MessageDialogs(MessageDialogs.DialogButtonType.OkCancel, MainClass.Languages.Translate("project_is_corrupted", filePath), MainClass.Languages.Translate("delete_corupted_project"), Gtk.MessageType.Question,null);
					int res = md.ShowDialog();
					return null;*/

				}
			}else {
				MessageDialogs md = new MessageDialogs(MessageDialogs.DialogButtonType.Ok, MainClass.Languages.Translate("project_not_exit_f1", filePath), "", Gtk.MessageType.Error,null);
				md.ShowDialog();
				return null;
			}
		}

		[XmlIgnore]
		public string FilePath
		{
			get;
			set;
		}

		[XmlArray("filesSetting")]
		[XmlArrayItem("file")]
		public List<FileSetting> FilesSetting {
			get;
			set;
		}

	}

}

