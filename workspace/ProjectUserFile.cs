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
	public class ProjectUserFile
	{
		// nazovprojektu.msp.nazovPc-Username.user
		public ProjectUserFile(string filePath)
		{
			FilePath = filePath;

		}

		public ProjectUserFile()
		{

		}

		static internal ProjectUserFile OpenProjectUserFile(string filePath)
		{
			if (System.IO.File.Exists(filePath)){

				try{

					using (FileStream fs = File.OpenRead(filePath)) {
						XmlSerializer serializer = new XmlSerializer(typeof(ProjectUserFile));

					ProjectUserFile p = (ProjectUserFile)serializer.Deserialize(fs);
						p.FilePath = filePath;

					return p;
					}
				}catch{//(Exception ex){

					ProjectUserFile p =new ProjectUserFile(filePath);
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

		[XmlArray("combinePublishes")]
		[XmlArrayItem("publish")]
		public List<CombinePublish> CombinePublish;

		[XmlAttribute("publishPageIndex")]
		public int PublishPage ;

	}


	/*
	 		WindowsIdentity id = WindowsIdentity.GetCurrent();
		Console.WriteLine(id.Name);
		Console.WriteLine(Environment.UserName );

		Console.WriteLine(id.AuthenticationType);
		if(id.User!= null)
			Console.WriteLine(id.User.Value);
	 **/
}

