using System;
using Gtk;
using System.Data;
using Mono.Data.Sqlite;
using MessageDialogs = Moscrif.IDE.Controls.MessageDialog;
using Moscrif.IDE.Controls;
using Moscrif.IDE.Controls.SqlLite;
using Moscrif.IDE.Tool;

namespace Moscrif.IDE.Editors.DatabaseView
{
	public class StructureDatabaseView : VBox,IDataBaseView
	{
		private string filename;

		private HBox hbox;
		private Label lblTable;
		private ComboBox cbTable;
		private ListStore tablesComboModel = new ListStore(typeof(string), typeof(string));

		private TreeView treeView;
		ListStore tableModel = new ListStore(typeof(int), typeof(string), typeof(string), typeof(string), typeof(string), typeof(string));

		private SqlLiteDal sqlLiteDal;

		private TextView textControl;

		string curentTable;
		int countTables;

		public StructureDatabaseView(string fileName)
		{
			this.filename = fileName;
			
			hbox = new HBox();

			sqlLiteDal = new SqlLiteDal(fileName);

			lblTable = new Label( MainClass.Languages.Translate("tables"));
			hbox.PackStart(lblTable, false, false, 10);
			
			cbTable = new ComboBox();
			cbTable.Changed += new EventHandler(OnComboProjectChanged);

			CellRendererText textRenderer = new CellRendererText();
			cbTable.PackStart(textRenderer, true);
			cbTable.AddAttribute(textRenderer, "text", 0);
			cbTable.Model = tablesComboModel;
			cbTable.WidthRequest = 200;

			hbox.PackStart(cbTable, false, false, 2);
			hbox.PackEnd(new Label(""), true, true, 2);

			HButtonBox hbbAction = new HButtonBox();
			hbbAction.LayoutStyle = Gtk.ButtonBoxStyle.Start;

			Button btnAddTable = new Button(MainClass.Languages.Translate("add_table"));
			btnAddTable.Clicked+= delegate(object sender, EventArgs e) {

				SqlLiteAddTable addtable = new SqlLiteAddTable( filename );
				int result = addtable.Run();
				if (result == (int)ResponseType.Ok) {
					GetTables();
				}
				addtable.Destroy();
			};

			Button btnDeleteTable = new Button(MainClass.Languages.Translate("delete_table"));
			btnDeleteTable.Clicked+= delegate(object sender, EventArgs e) {

				if(!CheckSelectTable())  return;

				MessageDialogs md = new MessageDialogs(MessageDialogs.DialogButtonType.YesNo,MainClass.Languages.Translate("permanently_delete_table", curentTable), "", Gtk.MessageType.Question);
				int result = md.ShowDialog();
				if (result != (int)Gtk.ResponseType.Yes)
					return;

				DropTable();
				GetTables();
			};

			Button btnEditTable = new Button(MainClass.Languages.Translate("edit_table"));
			btnEditTable.Clicked+= delegate(object sender, EventArgs e) {

				if(!CheckSelectTable())  return;

				SqlLiteEditTable editTable = new SqlLiteEditTable(filename,curentTable);
				int result = editTable.Run();
				if (result == (int)ResponseType.Ok) {
					GetTables();
				}
				editTable.Destroy();
			};


			hbbAction.Add(btnAddTable);
			hbbAction.Add(btnDeleteTable);
			hbbAction.Add(btnEditTable);
			hbox.PackEnd(hbbAction, false, false, 10);

			this.PackStart(hbox, false, false, 5);

			ScrolledWindow sw = new ScrolledWindow();
			sw.ShadowType = ShadowType.EtchedIn;
			sw.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);

			treeView = new TreeView(tableModel);
			GenerateColumns();
			treeView.RulesHint = true;
			//treeView.SearchColumn = (int) Column.Description;
			sw.Add(treeView);

			this.PackStart(sw, true, true, 5);

			ScrolledWindow sw2 = new ScrolledWindow ();
			sw2.ShadowType = ShadowType.EtchedIn;
			sw2.SetPolicy (PolicyType.Automatic, PolicyType.Automatic);
			sw2.HeightRequest = 50;

			textControl = new TextView();
			textControl.Editable = false;
			textControl.HeightRequest = 50;
			sw2.Add (textControl);
			this.PackEnd(sw2, false, false, 5);


			this.ShowAll();
			GetTables();
			//cbTable.Active = 0;
		}


		void OnComboProjectChanged(object o, EventArgs args)
		{
			ComboBox combo = o as ComboBox;
			if (o == null)
				return;
			
			TreeIter iter;
			if (combo.GetActiveIter(out iter)) {
				string table = (string)combo.Model.GetValue(iter, 0);
				string schema = (string)combo.Model.GetValue(iter, 1);
				textControl.Buffer.Clear();
				textControl.Buffer.Text = schema;
				curentTable = table;
				GetTableStructure(table);
			}
		}


		private void GenerateColumns()
		{
			CellRendererText rendererText = new CellRendererText();
			//CellRendererSpin rendererSpin = new CellRendererSpin();
			TreeViewColumn column = new TreeViewColumn("cid", rendererText, "text", 0);
			column.SortColumnId = 0;
			column.Resizable = true;
			column.Reorderable = true;
			treeView.AppendColumn(column);

			rendererText = new CellRendererText();
			column = new TreeViewColumn(MainClass.Languages.Translate("name"), rendererText, "text", 1);
			column.SortColumnId = 1;
			column.Resizable = true;
			column.Reorderable = true;
			treeView.AppendColumn(column);

			rendererText = new CellRendererText();
			column = new TreeViewColumn(MainClass.Languages.Translate("type"), rendererText, "text", 2);
			column.SortColumnId = 2;
			column.Resizable = true;
			column.Reorderable = true;
			treeView.AppendColumn(column);

			rendererText = new CellRendererText();
			column = new TreeViewColumn(MainClass.Languages.Translate("not_null"), rendererText, "text", 3);
			column.Resizable = true;
			column.Reorderable = true;
			column.SortColumnId = 3;
			treeView.AppendColumn(column);

			rendererText = new CellRendererText();
			column = new TreeViewColumn(MainClass.Languages.Translate("default_value"), rendererText, "text", 4);
			column.SortColumnId = 4;
			column.Resizable = true;
			column.Reorderable = true;
			treeView.AppendColumn(column);

			rendererText = new CellRendererText();
			column = new TreeViewColumn(MainClass.Languages.Translate("primary_key"), rendererText, "text", 5);
			column.SortColumnId = 5;
			column.Resizable = true;
			column.Reorderable = true;
			treeView.AppendColumn(column);
		}

		private void GetTableStructure(string table_name)
		{
			tableModel.Clear();
			SqliteConnection conn = (SqliteConnection)sqlLiteDal.GetConnect();
		        SqliteDataReader dr = null;
			try {

		                //conn.Open();
		                using (SqliteCommand cmd = conn.CreateCommand())
		                {
		                    cmd.CommandText = String.Format("PRAGMA table_info( '{0}' );", table_name);
		                    dr = cmd.ExecuteReader();
		                }

				if (dr.HasRows)
		                {
		                    //int fieldcount = dr.FieldCount;
		                    while (dr.Read())
		                    {
		                        int cid = Convert.ToInt32(dr[0]);
					string name = dr[1].ToString();
					string type = dr[2].ToString();
					bool notnull = Convert.ToBoolean(dr[3]);
					string dfltValue = dr[4].ToString();
					bool pk = Convert.ToBoolean(dr[5]);
					tableModel.AppendValues(cid, name, type, notnull.ToString(), dfltValue, pk.ToString());

		                    }
		                }

			} catch (Exception ex) {
				MessageDialogs ms = new MessageDialogs(MessageDialogs.DialogButtonType.Ok, "Error", ex.Message, MessageType.Error,null);
				ms.ShowDialog();

			} finally {
				if (dr != null) dr.Close();
				dr = null;
				conn.Close();
				conn = null;
			}

			/*try {
			 *string sql = String.Format("PRAGMA table_info( '{0}' );", table_name);
				DataTable dt = sqlLiteDal.RunSqlReader(sql,null);
				if(dt != null ){
					foreach ( DataRow row in dt.Rows ){

						int cid = Convert.ToInt32(row[0]);
						string name = row[1].ToString();
						string type = row[2].ToString();
						bool notnull = Convert.ToBoolean(row[3]);
						string dfltValue = row[4].ToString();
						bool pk = Convert.ToBoolean(row[5]);
						tableModel.AppendValues(cid, name, type, notnull.ToString(), dfltValue, pk.ToString());
					}
				}

			} catch (Exception ex) {
				
				MessageDialogs ms = new MessageDialogs(MessageDialogs.DialogButtonType.Ok, "Error", ex.Message, MessageType.Error,null);
				ms.ShowDialog();

			} finally {

			}*/
		}

		private bool CheckSelectTable(){
			if (String.IsNullOrEmpty(curentTable)){
				MessageDialogs ms = new MessageDialogs(MessageDialogs.DialogButtonType.Ok, MainClass.Languages.Translate("please_select_table"), "", MessageType.Error,null);
				ms.ShowDialog();
				return false;
			}
			return true;
		}

		private void DropTable()
		{
			if(!CheckSelectTable())  return;

			string sql = String.Format("DROP TABLE {0} ;",curentTable);

			if (sqlLiteDal.RunSqlScalar(sql))
				GetTables();

		}

		private void GetTables()
		{
			tableModel.Clear();
			tablesComboModel.Clear();
			SqliteConnection conn = (SqliteConnection)sqlLiteDal.GetConnect();
		        SqliteDataReader dr = null;
			try {
		                //conn.Open();
		                using (SqliteCommand cmd = conn.CreateCommand())
		                {
		                    cmd.CommandText = "SELECT name,sql FROM sqlite_master WHERE type='table' ORDER BY name;";
		                    dr = cmd.ExecuteReader();
		                }

				if (dr.HasRows)
		                {
		                    //int fieldcount = dr.FieldCount;
					int cnt = -1;
		                    while (dr.Read())
		                    {
		                     	string name = dr[0].ToString();
					string schema = dr[1].ToString();
					tablesComboModel.AppendValues(name, schema);
					cnt++;
		                    }
					cbTable.Active = cnt;
					countTables = cnt;
		                }

			} catch (Exception ex) {

				MessageDialogs ms = new MessageDialogs(MessageDialogs.DialogButtonType.Ok, "Error", ex.Message, MessageType.Error,null);
				ms.ShowDialog();

			} finally {
				if (dr != null) dr.Close();
				dr = null;
				conn.Close();
				conn = null;;
			}
			/*string sql = "SELECT name,sql FROM sqlite_master WHERE type='table' ORDER BY name;";

			DataTable dt = null;
			try {

				int i = -1;
				dt = sqlLiteDal.RunSqlReader(sql,null);
				if(dt != null ){
					foreach ( DataRow row in dt.Rows ){
						string name = row[0].ToString();
						string schema = row[1].ToString();
						tablesComboModel.AppendValues(name, schema);
						i++;
					}
				}
				cbTable.Active = i;
				countTables = i;
			} catch (Exception ex) {

				MessageDialogs ms = new MessageDialogs(MessageDialogs.DialogButtonType.Ok, "Error", ex.Message, MessageType.Error,null);
				ms.ShowDialog();
				Logger.Error(ex.Message,null);
			} finally {

			}*/
		}
		
		#region IDataBaseView implementation
		public void RefreshData ()
		{
			int indx = cbTable.Active;
			GetTables();
			if(countTables > indx )
				cbTable.Active = indx;
		}
		#endregion
	}
}

