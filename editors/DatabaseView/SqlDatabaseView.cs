using System;
using Gtk;
using System.Data;
using Mono.Data.Sqlite;
using MessageDialogs = Moscrif.IDE.Controls.MessageDialog;

namespace Moscrif.IDE.Editors.DatabaseView
{
	public class SqlDatabaseView : VBox,IDataBaseView
	{
		private string filename;

		private VBox vbox;
		private Label lblSql;
		private Button btnExecute;
		private TreeView treeView;
		ListStore tableModel = new ListStore(typeof(int), typeof(string), typeof(string), typeof(string), typeof(string), typeof(string));

		private TextView comandControl;
		private TextView outputControl;

		public SqlDatabaseView(string fileName)
		{
			this.filename = fileName;
			
			vbox = new VBox();
			
			lblSql = new Label(MainClass.Languages.Translate("sql_script"));
			vbox.PackStart(lblSql, false, false, 0);

			ScrolledWindow sw = new ScrolledWindow ();
			sw.ShadowType = ShadowType.EtchedIn;
			sw.SetPolicy (PolicyType.Automatic, PolicyType.Automatic);
			sw.HeightRequest = 75;

			comandControl = new TextView();
			comandControl.HeightRequest = 75;

			comandControl.KeyPressEvent += new KeyPressEventHandler(ComandKeyPress);

			sw.Add(comandControl);

			vbox.PackStart(sw, false, false, 0);


			btnExecute = new Button();
			btnExecute.Label = MainClass.Languages.Translate("execute_query");
			btnExecute.WidthRequest = 75;
			btnExecute.Clicked += delegate(object sender, EventArgs e) {
				string sql = comandControl.Buffer.Text;
				if (!String.IsNullOrEmpty(sql))
					RunSql(sql);
			};

			vbox.PackEnd(btnExecute, false, false, 0);

			this.PackStart(vbox, false, false, 5);

			ScrolledWindow swTv = new ScrolledWindow();
			swTv.ShadowType = ShadowType.EtchedIn;
			swTv.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);

			treeView = new TreeView(tableModel);
			treeView.RulesHint = true;

			swTv.Add(treeView);

			this.PackStart(swTv, true, true, 5);

			ScrolledWindow swOc = new ScrolledWindow ();
			swOc.ShadowType = ShadowType.EtchedIn;
			swOc.SetPolicy (PolicyType.Automatic, PolicyType.Automatic);
			swOc.HeightRequest = 50;

			outputControl = new TextView();
			outputControl.Editable = false;
			outputControl.HeightRequest = 150;
			swOc.Add (outputControl);
			this.PackEnd(swOc, false, false, 5);


			this.ShowAll();
		}


		[GLib.ConnectBefore]
		private void ComandKeyPress(object o, KeyPressEventArgs args) {
				
		}

		private void RunSql(string sql)
		{
			outputControl.Buffer.Text = String.Empty;
			tableModel.Clear();
			foreach (TreeViewColumn col in treeView.Columns) {
        			treeView.RemoveColumn (col);
			}

			IDbConnection dbcon = GetConnect();
			if (dbcon == null)
				return;

			IDbCommand dbcmd = dbcon.CreateCommand();

			dbcmd.CommandText = sql;
			IDataReader reader = null;

			try {
				reader = dbcmd.ExecuteReader();

				int numberCollumns = reader.FieldCount;

				if (numberCollumns <1){
					outputControl.Buffer.Text = MainClass.Languages.Translate("succes_no_result");
					return;
				}

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
				outputControl.Buffer.Text =  MainClass.Languages.Translate("succes");

			} catch (Exception ex) {

				outputControl.Buffer.Text = ex.Message;

			} finally {

				if (reader != null) reader.Close();
				reader = null;
				dbcmd.Dispose();
				dbcmd = null;
				dbcon.Close();
				dbcon = null;
			}
		}

		private IDbConnection GetConnect()
		{
			IDbConnection dbcon = new SqliteConnection();
			try {
				string connectionString = "URI=file:" + filename+",version=3";
				dbcon = (IDbConnection)new SqliteConnection(connectionString);
				dbcon.Open();
			} catch (Exception ex) {

				outputControl.Buffer.Text = ex.Message;
				dbcon = null;
			}
			return dbcon;
		}
		
		
		#region IDataBaseView implementation
		public void RefreshData ()
		{

		}
		#endregion
	}
}

