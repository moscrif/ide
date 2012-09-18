using System;
using System.Data;
using System.Collections.Generic;
using Mono.Data.Sqlite;
using MessageDialogs = Moscrif.IDE.Controls.MessageDialog;
using Gtk;
using Moscrif.IDE.Tool;

namespace Moscrif.IDE.Controls.SqlLite
{
	public partial class SqlLiteEditTable : Gtk.Dialog
	{
		Gtk.ListStore fieldsStore = new Gtk.ListStore(typeof(string), typeof(string),typeof(string), typeof(string),typeof(FieldTable));
		private List<FieldTable> fields = new List<FieldTable>();
		private string tableName;
		private string database;

		private SqlLiteDal sqlLiteDal;

		public SqlLiteEditTable(string filePath,string tableName)
		{
			this.TransientFor = MainClass.MainWindow;
			this.Build();
			this.tableName = tableName;
			this.database = filePath;

			sqlLiteDal = new SqlLiteDal(this.database);

			tvFields.Model =fieldsStore;

			tvFields.AppendColumn(MainClass.Languages.Translate("name"), new Gtk.CellRendererText(), "text", 0);
			tvFields.AppendColumn(MainClass.Languages.Translate("type"), new Gtk.CellRendererText(), "text",1);
			tvFields.AppendColumn(MainClass.Languages.Translate("not_null"), new Gtk.CellRendererText(), "text",2);
			tvFields.AppendColumn(MainClass.Languages.Translate("default_value"), new Gtk.CellRendererText(), "text",3);

			entrTableName.Text = tableName;
			this.Title = MainClass.Languages.Translate("edit_table_f1",tableName);

			GetTableStructure();
		}

		private void GetTableStructure()
		{
			fieldsStore.Clear();
			fields.Clear();

			SqliteConnection conn = (SqliteConnection)sqlLiteDal.GetConnect();
		        SqliteDataReader dr = null;

			string sql = String.Format("PRAGMA table_info( '{0}' );", tableName);

			//DataTable dt = null;
			try {
				//dt = sqlLiteDal.RunSqlReader(sql);

				using (SqliteCommand cmd = conn.CreateCommand())
		                {
		                    cmd.CommandText = sql;
		                    dr = cmd.ExecuteReader();
		                }

				if (dr.HasRows)
		                {
		                    while (dr.Read())
		                    {
					string name = dr[1].ToString();
					string type = dr[2].ToString();
					bool notnull = Convert.ToBoolean(dr[3]);
					string dfltValue = dr[4].ToString();
					//bool pk = Convert.ToBoolean(dr[5]);

					FieldTable ft = new FieldTable(name,type);
					ft.NotNULL =notnull;
					ft.DefaultValue =dfltValue;

					fields.Add(ft);
					fieldsStore.AppendValues( name, type,notnull.ToString(),dfltValue,ft);
		                    }
		                }


				/*if(dt != null ){
					foreach ( DataRow row in dt.Rows )
        				{
						string name = row[1].ToString();
						string type = row[2].ToString();

						bool notnull = Convert.ToBoolean(row[3]);
						string dfltValue = row[4].ToString();
						bool pk = Convert.ToBoolean(row[5]);

						FieldTable ft = new FieldTable(name,type);
						ft.NotNULL =notnull;
						ft.DefaultValue =dfltValue;

						fields.Add(ft);
						fieldsStore.AppendValues( name, type,notnull.ToString(),dfltValue,ft);
					}
				}*/

			} catch (Exception ex) {

				MessageDialogs ms = new MessageDialogs(MessageDialogs.DialogButtonType.Ok, "Error", ex.Message, MessageType.Error,this);
				ms.ShowDialog();

			} finally {
				if (dr != null) dr.Close();
				dr = null;
				conn.Close();
				conn = null;
			}
		}


		protected virtual void OnBtnEditFieldClicked (object sender, System.EventArgs e)
		{
			TreeSelection ts = tvFields.Selection;

			TreeIter ti = new TreeIter();
			ts.GetSelected(out ti);

			TreePath[] tp = ts.GetSelectedRows();
			if (tp.Length < 1)
				return ;

			FieldTable oldFT = (FieldTable)tvFields.Model.GetValue(ti, 4);
			if (oldFT == null) return;

			SqlLiteAddFiled sqlAddField = new SqlLiteAddFiled(oldFT,this);
			int result = sqlAddField.Run();
			if (result == (int)ResponseType.Ok){
				FieldTable newFT = sqlAddField.FieldTable;
				if (newFT!= null ){


					string tempTable= "temp_"+tableName+"_backup";
					string sqlBegin = "BEGIN TRANSACTION ;";

					string sqlRename = String.Format(" ALTER TABLE {0} RENAME TO {1} ;",tableName,tempTable);

					string sqlReCreate =  string.Format(" CREATE TABLE {0}( ",tableName);
					string oldColums ="";
					string newColums ="";

					for (int r=0; r<fields.Count;r++){

						if(fields[r].Name!=oldFT.Name){
							string dfltValue = fields[r].DefaultValue;
							string type = fields[r].Type;

							sqlReCreate = sqlReCreate+fields[r].Name+" "+type;

							if (fields[r].NotNULL)
								sqlReCreate =sqlReCreate+" NOT NULL";

							if (!String.IsNullOrEmpty(dfltValue)){
								if ((type.IndexOf("NUMERIC")>-1) || (type.IndexOf("INTEGER")>-1)){
									sqlReCreate =sqlReCreate+" DEFAULT "+dfltValue;
								} else {

									if (!dfltValue.StartsWith("'"))  dfltValue = "'"+dfltValue;
									if (!dfltValue.EndsWith("'"))  dfltValue = dfltValue+"'";
									sqlReCreate =sqlReCreate+" DEFAULT "+dfltValue;
								}
							}

							oldColums = oldColums +fields[r].Name;
							newColums = newColums +fields[r].Name;
						} else {

							string dfltValue = newFT.DefaultValue;
							string type = newFT.Type;

							sqlReCreate = sqlReCreate+newFT.Name+" "+type;

							if (newFT.NotNULL)
								sqlReCreate =sqlReCreate+" NOT NULL";

							if (!String.IsNullOrEmpty(dfltValue)){
								if ((type.IndexOf("NUMERIC")>-1) || (type.IndexOf("INTEGER")>-1)){
									sqlReCreate =sqlReCreate+" DEFAULT "+dfltValue;
								} else {
									if (!dfltValue.StartsWith("'"))  dfltValue = "'"+dfltValue;
									if (!dfltValue.EndsWith("'"))  dfltValue = dfltValue+"'";

									sqlReCreate =sqlReCreate+" DEFAULT "+dfltValue;
								}
							}

							oldColums = oldColums +fields[r].Name;
							newColums = newColums +newFT.Name;
						}

						if (r<fields.Count-1){
							sqlReCreate = sqlReCreate+",";
							oldColums = oldColums+",";
							newColums = newColums+",";
						}
					}
					sqlReCreate = sqlReCreate+") ;";

					string sqlInsertInto =  string.Format(" INSERT INTO {0}( {1} ) SELECT {2} FROM {3} ;",tableName,newColums,oldColums,tempTable);
					string sqlDropTable = string.Format(" DROP TABLE {0} ;",tempTable);
					string sqlEnd ="COMMIT ;";
					string sql = sqlBegin+ "\n" +sqlRename+ "\n"+sqlReCreate+ "\n"+sqlInsertInto+ "\n"+sqlDropTable+"\n"+sqlEnd;
					//Console.WriteLine(sql);

					if(sqlLiteDal.RunSqlScalar(sql)){

						//fields.Remove(oldFT);

						//fieldsStore.SetValue(ti,

						GetTableStructure();
					} else {
						// nepodarilo sa vymazanie, dam naspet
						//fields.Add(oldFT);
					}

				}
			}
			sqlAddField.Destroy();
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
					string sql = String.Format("ALTER TABLE {0} ADD {1} {2} ",tableName,ft.Name,ft.Type);

					if (ft.NotNULL)
						sql =sql+" "+"NOT NULL";

					if (!String.IsNullOrEmpty(ft.DefaultValue)){
						if ((ft.Type.IndexOf("NUMERIC")>-1) || (ft.Type.IndexOf("INTEGER")>-1))
						{
							sql =sql+" DEFAULT "+ft.DefaultValue;
						} else {
							string dfltValue =ft.DefaultValue;
							if (!dfltValue.StartsWith("'"))  dfltValue = "'"+dfltValue;
							if (!dfltValue.EndsWith("'"))  dfltValue = dfltValue+"'";

							sql =sql+" DEFAULT "+dfltValue;
						}
					}

					sql = sql+" ;";

					if(sqlLiteDal.RunSqlScalar(sql)){
						fieldsStore.AppendValues(ft.Name,ft.Type,ft);
						fields.Add(ft);
					}
				}
			}
			sqlAddField.Destroy();
		}
		
		protected virtual void OnBtnDeleteFieldClicked (object sender, System.EventArgs e)
		{
			TreeSelection ts = tvFields.Selection;

			TreeIter ti = new TreeIter();
			ts.GetSelected(out ti);

			TreePath[] tp = ts.GetSelectedRows();
			if (tp.Length < 1)
				return ;

			FieldTable cd = (FieldTable)tvFields.Model.GetValue(ti, 4);
			if (cd == null) return;

			MessageDialogs md = new MessageDialogs(MessageDialogs.DialogButtonType.YesNo, MainClass.Languages.Translate ("delete_field", cd.Name), "", Gtk.MessageType.Question,this);
			int result = md.ShowDialog();
			if (result != (int)Gtk.ResponseType.Yes)
				return;

			string tempTable= "temp_"+tableName+"_backup";
			string sqlBegin = "BEGIN TRANSACTION ;";

			string sqlRename = String.Format(" ALTER TABLE {0} RENAME TO {1} ;",tableName,tempTable);

			string sqlReCreate =  string.Format(" CREATE TABLE {0}( ",tableName);
			string colums ="";

			int newCount = 0;
			for (int r=0; r<fields.Count;r++){

				if(fields[r].Name!=cd.Name){
					string dfltValue = fields[r].DefaultValue;
					string type = fields[r].Type;

					sqlReCreate = sqlReCreate+fields[r].Name+" "+type;

					if (fields[r].NotNULL)
						sqlReCreate =sqlReCreate+" NOT NULL";

					if (!String.IsNullOrEmpty(dfltValue)){
						if ((type.IndexOf("NUMERIC")>-1) || (type.IndexOf("INTEGER")>-1))
						{
							sqlReCreate =sqlReCreate+" DEFAULT "+dfltValue;
						} else {

							if (!dfltValue.StartsWith("'"))  dfltValue = "'"+dfltValue;
							if (!dfltValue.EndsWith("'"))  dfltValue = dfltValue+"'";

							sqlReCreate =sqlReCreate+" DEFAULT "+dfltValue;
						}
					}
					colums = colums +fields[r].Name;
					if (newCount<fields.Count-2){
						sqlReCreate = sqlReCreate+",";
						colums = colums+",";
					}
					newCount++;
				}
			}
			sqlReCreate = sqlReCreate+") ;";

			string sqlInsertInto =  string.Format(" INSERT INTO {0}( {1} ) SELECT {1} FROM {2} ;",tableName,colums,tempTable);
			string sqlDropTable = string.Format(" DROP TABLE {0} ;",tempTable);
			string sqlEnd ="COMMIT ;";
			string sql = sqlBegin+ "\n" +sqlRename+ "\n"+sqlReCreate+ "\n"+sqlInsertInto+ "\n"+sqlDropTable+"\n"+sqlEnd;
			//Console.WriteLine(sql);

			if(sqlLiteDal.RunSqlScalar(sql)){

				//fields.Remove(cd);
				//fieldsStore.Remove(ref ti);
				GetTableStructure();
			} else {
				// nepodarilo sa vymazanie, dam naspet
				//fields.Add(cd);
			}

		}

		protected virtual void OnButton28Clicked (object sender, System.EventArgs e)
		{
		}
		
		protected virtual void OnBtnRenameTableClicked (object sender, System.EventArgs e)
		{
			EntryDialog ed = new EntryDialog(tableName,MainClass.Languages.Translate("new_name"),this);
			int result = ed.Run();
			if (result == (int)ResponseType.Ok) {

				string newName = ed.TextEntry;

				if(!String.IsNullOrEmpty(newName)){

					string sql = string.Format("ALTER TABLE {0} RENAME TO {1} ;",tableName,newName);
					if( sqlLiteDal.RunSqlScalar(sql)){
						entrTableName.Text=newName;
						tableName = newName;
					}
				}
			}
			ed.Destroy();
		}


	}
}

