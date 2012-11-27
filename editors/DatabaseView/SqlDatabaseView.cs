using System;
using Gtk;
using System.Data;
using Mono.Data.Sqlite;
using MessageDialogs = Moscrif.IDE.Controls.MessageDialog;

namespace Moscrif.IDE.Editors.DatabaseView
{
	public class SqlDatabaseView : Table,IDataBaseView//VBox
	{
		private string filename;

		//private VBox vbox;
		private Button btnExecute;
		private TreeView treeView;
		ListStore tableModel = new ListStore(typeof(int), typeof(string), typeof(string), typeof(string), typeof(string), typeof(string));

		private TextView comandControl;
		private TextView outputControl;

		public SqlDatabaseView(string fileName): base(7,1,false)
		{
			this.filename = fileName;
			this.RowSpacing = 5;

			ScrolledWindow sw = new ScrolledWindow ();
			sw.ShadowType = ShadowType.EtchedIn;
			sw.SetPolicy (PolicyType.Automatic, PolicyType.Automatic);
			sw.HeightRequest = 75;

			comandControl = new TextView();
			comandControl.HeightRequest = 75;

			comandControl.KeyPressEvent += new KeyPressEventHandler(ComandKeyPress);

			sw.Add(comandControl);

			
			Label lblSql = new Label(MainClass.Languages.Translate("sql_script"));
			lblSql.Xalign=0;
			lblSql.Yalign=0.5f;
			this.Attach(lblSql,0,1,0,1,AttachOptions.Fill,AttachOptions.Shrink,3,3);
			this.Attach(sw,0,1,1,2,AttachOptions.Fill,AttachOptions.Fill,3,3);

			btnExecute = new Button();
			btnExecute.Label = MainClass.Languages.Translate("execute_query");
			btnExecute.WidthRequest = 175;
			btnExecute.HeightRequest = 25;
			btnExecute.Clicked += delegate(object sender, EventArgs e) {
				string sql = comandControl.Buffer.Text;
				if (!String.IsNullOrEmpty(sql))
					RunSql(sql);
			};

			this.Attach(btnExecute,0,1,2,3,AttachOptions.Shrink,AttachOptions.Shrink,0,0);

			ScrolledWindow swTv = new ScrolledWindow();
			swTv.ShadowType = ShadowType.EtchedIn;
			swTv.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);

			treeView = new TreeView(tableModel);
			treeView.RulesHint = true;
			treeView.EnableSearch = false;

			swTv.Add(treeView);

			Label lblResult = new Label(MainClass.Languages.Translate("sql_result"));
			lblResult.Xalign=0;
			lblResult.Yalign=0.5f;

			this.Attach(lblResult,0,1,3,4,AttachOptions.Fill,AttachOptions.Shrink,3,3);
			this.Attach(swTv,0,1,4,5,AttachOptions.Expand|AttachOptions.Fill,AttachOptions.Expand|AttachOptions.Fill,3,3);

			ScrolledWindow swOc = new ScrolledWindow ();
			swOc.ShadowType = ShadowType.EtchedIn;
			swOc.SetPolicy (PolicyType.Automatic, PolicyType.Automatic);
			swOc.HeightRequest = 50;

			outputControl = new TextView();
			outputControl.Editable = false;
			outputControl.HeightRequest = 150;
			swOc.Add (outputControl);

			Label lblOutput = new Label(MainClass.Languages.Translate("sql_output"));
			lblOutput.Xalign=0;
			lblOutput.Yalign=0.5f;

			this.Attach(lblOutput,0,1,5,6,AttachOptions.Fill,AttachOptions.Shrink,3,3);
			this.Attach(swOc,0,1,6,7,AttachOptions.Fill,AttachOptions.Fill,3,3);

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
					TreeViewColumn column = new TreeViewColumn(reader.GetName(i).Replace("_","__"), rendererText, "text", i);
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

