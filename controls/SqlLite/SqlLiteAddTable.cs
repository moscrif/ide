using System;
using System.Data;
using System.Collections.Generic;
using Mono.Data.Sqlite;
using MessageDialogs = Moscrif.IDE.Controls.MessageDialog;
using Gtk;
using Moscrif.IDE.Tool;


namespace Moscrif.IDE.Controls.SqlLite
{
	public partial class SqlLiteAddTable : Gtk.Dialog
	{
		//private IDbConnection dbConnection;
		Gtk.ListStore fieldsStore = new Gtk.ListStore(typeof(string), typeof(string),typeof(FieldTable));
		private List<FieldTable> fields = new List<FieldTable>();
		private string database;
		private SqlLiteDal sqlLiteDal;


		public SqlLiteAddTable(string database)
		{
			this.TransientFor = MainClass.MainWindow;
			this.Build();
			this.Title="New Table";
			//this.dbConnection = dbConnection;
			this.database = database;

			sqlLiteDal= new SqlLiteDal(this.database);

			tvFields.Model =fieldsStore;

			tvFields.AppendColumn(MainClass.Languages.Translate("name"), new Gtk.CellRendererText(), "text", 0);
			tvFields.AppendColumn(MainClass.Languages.Translate("type"), new Gtk.CellRendererText(), "text",1);

		}

		protected virtual void OnButtonOkClicked (object sender, System.EventArgs e)
		{
			if (String.IsNullOrEmpty(entrTableName.Text)){
				MessageDialogs md =
					new MessageDialogs(MessageDialogs.DialogButtonType.Ok,MainClass.Languages.Translate("please_enter_table_name"),"" , Gtk.MessageType.Error,this);
				md.ShowDialog();

				return;
			}

			if (fields.Count<1){
				MessageDialogs md =
					new MessageDialogs(MessageDialogs.DialogButtonType.Ok,MainClass.Languages.Translate("no_fileds_define"),"" , Gtk.MessageType.Error,this);
				md.ShowDialog();
				return;
			}

			string tableName = entrTableName.Text;
			string sql = string.Format("CREATE TABLE {0}( ",tableName);

			for (int r=0; r<fields.Count;r++){
				string type =fields[r].Type;
				string defValue =fields[r].DefaultValue;

				sql = sql+fields[r].Name+" "+type;

				if (fields[r].NotNULL)
					sql =sql+" "+"NOT NULL";

				if (!String.IsNullOrEmpty(defValue)){
					if ((type.IndexOf("NUMERIC")>-1) || (type.IndexOf("INTEGER")>-1))
					{
						sql =sql+" DEFAULT "+defValue;
					} else {
						sql =sql+" DEFAULT '"+defValue+"'";
					}
				}
				if (r<fields.Count-1)
					sql = sql+",";
			}
			sql = sql+") ;";
			sqlLiteDal.RunSqlScalar(sql);

			//CreateTable();
			this.Respond(ResponseType.Ok);
		}

		protected virtual void OnBtnDeleteFieldClicked (object sender, System.EventArgs e)
		{
			TreeSelection ts = tvFields.Selection;

			TreeIter ti = new TreeIter();
			ts.GetSelected(out ti);

			TreePath[] tp = ts.GetSelectedRows();
			if (tp.Length < 1)
				return ;

			FieldTable cd = (FieldTable)tvFields.Model.GetValue(ti, 2);
			if (cd == null) return;

			MessageDialogs md = new MessageDialogs(MessageDialogs.DialogButtonType.YesNo, MainClass.Languages.Translate("delete_field", cd.Name), "", Gtk.MessageType.Question,this);
			int result = md.ShowDialog();
			if (result != (int)Gtk.ResponseType.Yes)
				return;

			fields.Remove(cd);
			fieldsStore.Remove(ref ti);
		}

		protected virtual void OnBtnCreateFieldClicked (object sender, System.EventArgs e)
		{
			SqlLiteAddFiled sqlAddField = new SqlLiteAddFiled(this);
			int result = sqlAddField.Run();
			if (result == (int)ResponseType.Ok){
				FieldTable ft = sqlAddField.FieldTable;
				if (ft!= null ){

					FieldTable cdFind = fields.Find(x=>x.Name.ToUpper() ==ft.Name.ToUpper());
					if (cdFind != null){

						MessageDialogs md = new MessageDialogs(MessageDialogs.DialogButtonType.Ok, MainClass.Languages.Translate("fileds_exist", ft.Name), "", Gtk.MessageType.Error,this);
						md.ShowDialog();
						sqlAddField.Destroy();
						return;
					}

					fieldsStore.AppendValues(ft.Name,ft.Type,ft);
					fields.Add(ft);
				}
			}
			sqlAddField.Destroy();
		}
		/*
		private void CreateTable()
		{
			string tableName = entrTableName.Text;
			IDbConnection dbcon = dbConnection;

			IDbCommand dbcmd = dbcon.CreateCommand();
			if (dbcon == null)
				return;

			string sql = string.Format("CREATE TABLE {0}( ",tableName);

			for (int r=0; r<fields.Count;r++){
				sql = sql+fields[r].Name+" "+fields[r].Type;
				if (r<fields.Count-1)
					sql = sql+",";
			}
			sql = sql+")";

			Console.WriteLine(sql);

			dbcmd.CommandText = sql;
			IDataReader reader = null;
			try {
				dbcmd.ExecuteScalar();// ExecuteReader();
			} catch (Exception ex) {
				MessageDialogs ms = new MessageDialogs(MessageDialogs.DialogButtonType.Ok, "Error", ex.Message, MessageType.Error);
				ms.ShowDialog();
			} finally {
				if (reader != null) reader.Close();
				reader = null;
				dbcmd.Dispose();
				dbcmd = null;
				dbcon.Close();
				dbcon = null;
			}
		}*/

	}
}

