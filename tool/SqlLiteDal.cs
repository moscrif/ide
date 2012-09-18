using System;
using System.Data;
using System.Collections.Generic;
using Mono.Data.Sqlite;
using MessageDialogs = Moscrif.IDE.Controls.MessageDialog;

namespace Moscrif.IDE.Tool
{
	public class SqlLiteDal
	{
		private string database;
		public SqlLiteDal(string database)
		{
			this.database = database;
		}

		public IDbConnection GetConnect()
		{
			IDbConnection dbcon;// = new SqliteConnection();
			try {//
				/*if(MainClass.Platform.IsMac){
					string connectionString = "Data Source="+database;
					dbcon = new SQLiteConnection(connectionString);
					dbcon.Open();
				} else {*/
					//string connectionString = "URI=file:" + database+",version=3";
					string connectionString = "URI=file:" + database+",version=3";
					dbcon = (IDbConnection)new SqliteConnection(connectionString);
					dbcon.Open();
				//}
			} catch (Exception ex) {

				MessageDialogs ms = new MessageDialogs(MessageDialogs.DialogButtonType.Ok, "Error", ex.Message, Gtk.MessageType.Error,null);
				ms.ShowDialog();
				Tool.Logger.Error("GetConnect ->>" +ex.Message,null);
				dbcon = null;
			}
			return dbcon;
		}

		public bool RunSqlScalar(string sql){
			bool result = true;

			IDbConnection dbcon = GetConnect();

			IDbCommand dbcmd = dbcon.CreateCommand();
			if (dbcon == null)
				return false;

			//Console.WriteLine(sql);
			dbcmd.CommandText = sql;
			try {
				dbcmd.ExecuteScalar();
			} catch (Exception ex) {
				MessageDialogs ms = new MessageDialogs(MessageDialogs.DialogButtonType.Ok, "Error", ex.Message, Gtk.MessageType.Error);
				ms.ShowDialog();
				result = false;
			} finally {
				dbcmd.Dispose();
				dbcmd = null;
				dbcon.Close();
				dbcon = null;
			}
			return result;
		}



		public bool CheckExistTable(string tableName)
		{
			SqliteConnection conn = (SqliteConnection)GetConnect();
		        SqliteDataReader dr = null;

			try {
		                using (SqliteCommand cmd = conn.CreateCommand())
		                {
		                    cmd.CommandText = "SELECT name,sql FROM sqlite_master WHERE type='table' ORDER BY name;";
		                    dr = cmd.ExecuteReader();
		                }

				if (dr.HasRows)
		                {
				    int cnt = -1;
		                    while (dr.Read())
		                    {
		                     	string name = dr[0].ToString();
						if(name == tableName)
							return true;
					cnt++;
		                    }
		                }

			} catch {//(Exception ex) {
				//MessageDialogs ms = new MessageDialogs(MessageDialogs.DialogButtonType.Ok, "Error", ex.Message, MessageType.Error,null);
				//ms.ShowDialog();

			} finally {
				if (dr != null) dr.Close();
				dr = null;
				conn.Close();
				conn = null;
			}
			return false;
		}

		/*
		public DataTable RunSqlReader (string sql){
			return RunSqlReader(sql,MainClass.MainWindow);
		}


		public DataTable RunSqlReader (string sql,Gtk.Window parent){

				DataTable result = new DataTable();
	
				SqliteConnection sqlcon =(SqliteConnection)GetConnect();
	
				SqliteCommand dbcmd = sqlcon.CreateCommand();
				if (sqlcon == null)
					return result;
	
				dbcmd.CommandText = sql;
	
				SqliteDataAdapter dataAdapter = new SqliteDataAdapter(dbcmd);
	
				DataSet ds = new DataSet();
				try {
					dataAdapter.Fill(ds);
					result =ds.Tables[0];
	
				} catch (Exception ex) {
					MessageDialogs ms = new MessageDialogs(MessageDialogs.DialogButtonType.Ok, "Error", ex.Message, Gtk.MessageType.Error,parent);
					ms.ShowDialog();
					Logger.Error("RunSqlReader ->>"+ex.Message);
					result = null;
				} finally {
					//if (reader != null) reader.Close();
					//reader = null;
					dataAdapter.Dispose();
					dataAdapter = null;
					dbcmd.Dispose();
					dbcmd = null;
					sqlcon.Close();
					sqlcon = null;
				}
				return result;
		}*/

	}
}

