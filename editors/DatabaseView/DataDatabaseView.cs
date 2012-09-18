using System;
using Gtk;
using System.Data;
using Mono.Data.Sqlite;
using MessageDialogs = Moscrif.IDE.Controls.MessageDialog;
using Moscrif.IDE.Tool;

namespace Moscrif.IDE.Editors.DatabaseView
{
	public class DataDatabaseView : VBox,IDataBaseView
	{
		private string filename;

		private HBox hbox;
		private Label lblTable;
		private ComboBox cbTable;
		private ListStore tablesComboModel = new ListStore(typeof(string), typeof(string));

		private TreeView treeView;
		ListStore tableModel = new ListStore (typeof(int),typeof(string),typeof(string),typeof(string),typeof(string),typeof(string));
		private SqlLiteDal sqlLiteDal;
		int countTables;

		public DataDatabaseView(string fileName)
		{
			this.filename =fileName;

			hbox = new HBox();
			sqlLiteDal = new SqlLiteDal(fileName);

			lblTable = new Label(MainClass.Languages.Translate("tables"));
			hbox.PackStart(lblTable,false,false,10);

			cbTable = new ComboBox();
			cbTable.Changed += new EventHandler(OnComboProjectChanged);
			CellRendererText textRenderer = new CellRendererText();
			cbTable.PackStart(textRenderer, true);
			cbTable.AddAttribute(textRenderer, "text", 0);
			cbTable.Model = tablesComboModel;
			cbTable.WidthRequest = 200;

			hbox.PackStart(cbTable,false,false,2);
			hbox.PackEnd(new Label(""),true,true,2);

			this.PackStart(hbox,false,false,5);

			ScrolledWindow sw = new ScrolledWindow ();
			sw.ShadowType = ShadowType.EtchedIn;
			sw.SetPolicy (PolicyType.Automatic, PolicyType.Automatic);

			treeView = new TreeView (tableModel);
			treeView.RulesHint = true;
			//treeView.SearchColumn = (int) Column.Description;
			sw.Add (treeView);

			this.PackStart(sw,true,true,5);

			this.ShowAll();

			GetTables();
		}


		void OnComboProjectChanged(object o, EventArgs args)
		{
			ComboBox combo = o as ComboBox;
			if (o == null)
				return;

			TreeIter iter;
			if (combo.GetActiveIter(out iter)) {
				string table = (string)combo.Model.GetValue(iter, 0);
				GetTableData(table);

			}
		}

		private void GetTableData(string table_name)
		{

			tableModel.Clear();
			foreach (TreeViewColumn col in treeView.Columns) {
        			treeView.RemoveColumn (col);
			}

			SqliteConnection dbcon =  (SqliteConnection)sqlLiteDal.GetConnect();//GetConnect();
			if (dbcon == null)
				return;

			SqliteCommand dbcmd = dbcon.CreateCommand();
			
			string sql = String.Format("SELECT *  FROM '{0}';", table_name);
			dbcmd.CommandText = sql;
			SqliteDataReader reader = null;
			try {
				reader = dbcmd.ExecuteReader();

				int numberCollumns = reader.FieldCount;

				if (numberCollumns <1)return;

				Type[] type = new Type[numberCollumns];

				CellRendererText rendererText = new CellRendererText();
				for (int i=0; i<numberCollumns;i++){
					type[i] =typeof(string);
					TreeViewColumn column = new TreeViewColumn(reader.GetName(i), rendererText, "text", i);
					column.SortColumnId = i;
					column.Resizable = true;
					column.Reorderable = true;
					treeView.AppendColumn(column);
				}

				tableModel = new ListStore(type);

				while (reader.Read()) {

					string[] obj = new string[numberCollumns];

					for (int j=0; j<numberCollumns;j++){
						obj[j] = reader.GetValue(j).ToString();
					}
					tableModel.AppendValues(obj);
				}
				treeView.Model = tableModel;

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
				conn = null;
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

		/*private IDbConnection GetConnect()
		{
			IDbConnection dbcon = new SqliteConnection();
			try {
				string connectionString = "URI=file:" + filename+",version=3";
				dbcon = (IDbConnection)new SqliteConnection(connectionString);
				dbcon.Open();
			} catch (Exception ex) {

				MessageDialogs ms = new MessageDialogs(MessageDialogs.DialogButtonType.Ok, "Error", ex.Message, MessageType.Error);
				ms.ShowDialog();
				dbcon = null;
			}
			return dbcon;
		}*/


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



