using System;
using System.IO;
using System.Net;
using System.Collections.Generic;
using Gtk;
using Gdk;
using Moscrif.IDE.Iface.Entities;
//using Gtk.Html;

namespace Moscrif.IDE.Editors
{
	public class WebViewer : IEditor 
	{


		private ScrolledWindow control = null;

		private Gtk.ActionGroup editorAction = null;
		private string fileName = String.Empty;
		//HTML html;
		//string currentUrl;

		public WebViewer(string filePath)
		{
			fileName =filePath;
			//html = new HTML();
			//html.LinkClicked += new LinkClickedHandler (OnLinkClicked);
			//control.Add (html);
			control.ShowAll();

			LoadHtml(filePath);
			//LoadHtml("http://www.mothiva.sk");
		}


		/* void OnLinkClicked (object obj, LinkClickedArgs args)
                {
                        string newUrl;
                        
                        // decide absolute or relative
                        if (args.Url.StartsWith("http://")) 
                                newUrl = args.Url;
                        else
                                newUrl = currentUrl + args.Url;

	                try {
                                LoadHtml (newUrl);
	                } catch { }
                        currentUrl = newUrl;
                }*/

		void LoadHtml (string URL)
                {
                        /*HttpWebRequest web_request = (HttpWebRequest) WebRequest.Create (URL);
                        HttpWebResponse web_response = (HttpWebResponse) web_request.GetResponse ();
			Stream stream = web_response.GetResponseStream ();
			byte [] buffer = new byte [8192];
			
			HTMLStream html_stream = html.Begin ();
			int count;
			
			while ((count = stream.Read (buffer, 0, 8192)) != 0){
				html_stream.Write (buffer, count);
			}
			html.End (html_stream, HTMLStreamStatus.Ok);*/
                }
		#region IEditor implementation
		
		public object GetSelected(){
			return null;
		}
		
		public void Rename(string newName){

			fileName = newName;
		}

		public bool RefreshSettings(){
			return true;
		}		

		public bool Save ()
		{
			return true;
			//throw new NotImplementedException ();
		}

		public bool SaveAs (string newPath)
		{
			return true;
			//throw new NotImplementedException ();
		}

		public void Close()
		{

		}		

		public bool SearchExpression (SearchPattern expresion){
			return false;
			//throw new NotImplementedException ();
		}

		public bool SearchNext(SearchPattern expresion){
			return false;
			//throw new NotImplementedException ();
		}

		public bool SearchPreviu(SearchPattern expresion){
			return false;
			//throw new NotImplementedException ();
		}

		public bool Replace(SearchPattern expresion){
			return false;
			//throw new NotImplementedException ();
		}

		public bool ReplaceAll(SearchPattern expresion){
			return false;
			//throw new NotImplementedException ();
		}

		public List<FindResult> FindReplaceAll(SearchPattern expresion)
		{
			return null;
		}		

		public bool Undo(){
			return false;
			//throw new NotImplementedException ();
		}

	
		public bool Redo(){
			return false;
			//throw new NotImplementedException ();
		}

		public string Caption {
			get { return System.IO.Path.GetFileName(fileName); }
		}

		public string FileName {
			get { return fileName; }
		}

		public bool Modified {
			get { return false; }
		}

		public void GoToPosition(object position){
		}

		public Gtk.Widget Control {
			get { return control; }
		}

		public Gtk.ActionGroup EditorAction
		{
			get {return editorAction;}
		}

		public void ActivateEditor(bool updateStatus){

		}

		public event EventHandler<ModifiedChangedEventArgs> ModifiedChanged;
		public event EventHandler<WriteStatusEventArgs> WriteStatusChange;

		#endregion

		void OnModifiedChanged(bool newModified)
		{
			ModifiedChangedEventArgs mchEventArg = new ModifiedChangedEventArgs(false);//modified);

			if (ModifiedChanged != null)
				ModifiedChanged(this, mchEventArg);
		}
		
		void OnWriteToStatusbar(string message)
		{
			WriteStatusEventArgs mchEventArg = new WriteStatusEventArgs(message);
			if (WriteStatusChange != null)
				WriteStatusChange(this, mchEventArg);
		}
	}
}

