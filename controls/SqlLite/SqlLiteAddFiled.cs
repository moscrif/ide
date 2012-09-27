using System;
using MessageDialogs = Moscrif.IDE.Controls.MessageDialog;
using Gtk;


namespace Moscrif.IDE.Controls.SqlLite
{
	public partial class SqlLiteAddFiled : Gtk.Dialog
	{
		ListStore fieldTypeStore = new ListStore(typeof(string));

		protected virtual void OnBtnMyTypeClicked (object sender, System.EventArgs e)
		{
			EntryDialog ed = new EntryDialog("",MainClass.Languages.Translate("filed_typ"),this);
			int result = ed.Run();
			if (result == (int)ResponseType.Ok) {

				string newTyp = ed.TextEntry;

				if(!String.IsNullOrEmpty(newTyp)){
					bool isFind = false;
					fieldTypeStore.Foreach((model, path, iterr) => {
						string name = fieldTypeStore.GetValue(iterr, 0).ToString();

						if (name == newTyp){

							cbFieldType.SetActiveIter(iterr);
							isFind = true;
							return true;
						}
						return false;
					});
					if (!isFind){
						TreeIter ti =fieldTypeStore.AppendValues(newTyp);
						cbFieldType.SetActiveIter(ti);
					}
				}
				}
				ed.Destroy();
		}

		public SqlLiteAddFiled(FieldTable ft,Gtk.Window parent)
		{
			this.TransientFor =parent;

			this.Build();

			fieldTable = ft;

			CellRendererText textRenderer = new CellRendererText();
			cbFieldType.PackStart(textRenderer, true);

			entrFieldName.Text = ft.Name;

			if(!String.IsNullOrEmpty(ft.DefaultValue))
				entrDefault.Text = ft.DefaultValue;
			chbNulable.Active = ft.NotNULL;

			cbFieldType.Model = fieldTypeStore;

			int active = -1;

			if (String.IsNullOrEmpty(ft.Type))
				active = 0;
			fieldTypeStore.AppendValues("");

			if (ft.Type== "TEXT")
				active = 1;
			fieldTypeStore.AppendValues("TEXT");

			if (ft.Type== "NUMERIC")
				active = 2;
			fieldTypeStore.AppendValues("NUMERIC");

			if (ft.Type== "BLOB")
				active = 3;
			fieldTypeStore.AppendValues("BLOB");

			if (ft.Type== "INTEGER PRIMARY KEY")
				active = 4;
			fieldTypeStore.AppendValues("INTEGER PRIMARY KEY");

			/*if (ft.Type== "INTEGER PRIMARY KEY AUTOINCREMENT")
				active = 5;
			fieldTypeStore.AppendValues("INTEGER PRIMARY KEY AUTOINCREMENT");*/

			if (active==-1){
				fieldTypeStore.AppendValues(ft.Type);
				active = 5;
			}
			cbFieldType.Active = active;
		}

		public SqlLiteAddFiled(Gtk.Window parent)
		{
			this.TransientFor =parent;
			this.Build();

			CellRendererText textRenderer = new CellRendererText();
			cbFieldType.PackStart(textRenderer, true);

			cbFieldType.Model = fieldTypeStore;

			fieldTypeStore.AppendValues("");
			fieldTypeStore.AppendValues("TEXT");
			fieldTypeStore.AppendValues("NUMERIC");
			fieldTypeStore.AppendValues("BLOB");
			fieldTypeStore.AppendValues("INTEGER PRIMARY KEY");
		}


		protected virtual void OnButtonOkClicked (object sender, System.EventArgs e)
		{
			if (String.IsNullOrEmpty(entrFieldName.Text)){
				MessageDialogs md =
					new MessageDialogs(MessageDialogs.DialogButtonType.Ok,MainClass.Languages.Translate("please_enter_field_name"),"" , Gtk.MessageType.Error,this);
				md.ShowDialog();

				return;
			}
			TreeIter ti;

			string typ="";

			if(cbFieldType.GetActiveIter(out ti)){
				 typ =  fieldTypeStore.GetValue(ti,0).ToString();
			}

			fieldTable = new FieldTable(entrFieldName.Text,typ);
			fieldTable.NotNULL= chbNulable.Active;
			if (!String.IsNullOrEmpty(entrDefault.Text))
				fieldTable.DefaultValue = entrDefault.Text;
			this.Respond(ResponseType.Ok);

		}



		private FieldTable fieldTable;
		public FieldTable FieldTable{
			get{
				return fieldTable;
			}

		}
		
	}
}

