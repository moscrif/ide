using System;
using System.Collections.Generic;
using Gtk;
using System.Reflection;


namespace Moscrif.IDE.Actions
{
	public class OpenApiReferenceAction: Gtk.Action
	{
		public OpenApiReferenceAction(): base("apireference", MainClass.Languages.Translate("menu_open_api_ref"), MainClass.Languages.Translate("menu_open_api_ref"),null)
		{
		}


		protected override void OnActivated ()
		{
			base.OnActivated();

			string path = System.IO.Path.Combine(MainClass.Paths.AppPath,"api-reference");


			string linkUrl = System.IO.Path.Combine(path,"index.html");
			if(System.IO.File.Exists(linkUrl)){
				System.Diagnostics.Process.Start(linkUrl);
			}

		}
	}
}

