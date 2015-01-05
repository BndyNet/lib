// =================================================================================
// Copyright (c) 2012 http://www.bndy.net.
// Created by Bndy at 12/15/2012 11:09:26 AM
// ---------------------------------------------------------------------------------
// The factory used to operate MsSql Server database.
// =================================================================================

using System;
using System.Data;
using System.Linq;
using System.Configuration;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Reflection;

using Net.Bndy;

namespace Net.Bndy.Data.SqlServer
{
	public class MsSqlFactory : DbFactory, IDbFactory, IDisposable
	{
		#region Uninherited Members

		private SqlConnection _connection;
		private SqlConnection Connection
		{
			get
			{
				if (_connection == null)
					_connection = new SqlConnection(this.ConnectionString);
				if (_connection.State != ConnectionState.Open)
					_connection.Open();
				return _connection;
			}
		}
		protected string ConnectionString { get; set; }

		/// <summary>
		/// Initializes a new instance of Net.Bndy.Data.SqlServer.MsSqlFactory class.
		///		Use the first Connection String in the Configuration file, When the connectionString parameter is null.
		/// </summary>
		/// <param name="connectionString">The connection used to open database.</param>
		public MsSqlFactory(string connectionString = null)
		{
			if (connectionString == null)
				this.ConnectionString = ConfigurationManager.ConnectionStrings[0].ConnectionString;
			else
				this.ConnectionString = connectionString;
		}
		public MsSqlFactory(string dbServer, string dbName, string dbUser, string dbPassword)
			: this(string.Format("Data Source={0}; Initial Catalog={1}; User ID={2}; Password={3}", dbServer, dbName, dbUser, dbPassword))
		{

		}

		public void InsertObject(object obj, bool createObjectTable)
		{
			Insert(obj.GetType().Name, obj.GetNativePropertiesDict());
		}
		public void UpdateObject(object dataObject, string condition)
		{
			Update(dataObject.GetType().Name, dataObject.GetNativePropertiesDict(), condition);
		}
		private SqlCommand GetCommand(string sql, params SqlParameter[] parameters)
		{
			var cmd = new SqlCommand(sql, Connection);
			cmd.Parameters.AddRange(parameters);
			return cmd;
		}
		~MsSqlFactory()
		{
			Dispose(true);
		}

		#endregion

		public bool CanOpen()
		{
			try
			{
				if (this.Connection.State != ConnectionState.Open)
					this.Connection.Open();

				var canOpen = this.Connection.State == ConnectionState.Open;
				this.Connection.Close();
				return canOpen;
			}
			catch
			{
				return false;
			}
		}
		public object Insert(string tblName, Dictionary<string, object> fieldValuePairs)
		{
			if (fieldValuePairs == null || fieldValuePairs.Count == 0) return false;

			List<string> columns = new List<string>();
			List<string> variables = new List<string>();
			foreach (string key in fieldValuePairs.Keys)
			{
				columns.Add(string.Format("{0}", key));
				variables.Add("@" + key);
			}

			bool isNew = false;
			if (!ExistsTable(tblName))
			{
				NewTable(tblName, columns.ToArray());
				isNew = true;
			}

			if (!isNew)
			{
				foreach (string c in columns)
				{
					AddColumn(tblName, c);
				}
			}

			string sql = string.Format("INSERT INTO [{0}]({1}) VALUES({2}); SELECT @@IDENTITY;", tblName,
				string.Join(",", columns), string.Join(",", variables));
			var cmd = GetCommand(sql);
			foreach (string v in variables)
			{
				SqlParameter p = new SqlParameter(v, SqlDbType.NVarChar);
				p.SqlValue = fieldValuePairs[v.Replace("@", "")] != null
					? fieldValuePairs[v.Replace("@", "")]
					: DBNull.Value;
				cmd.Parameters.Add(p);
			}

			return cmd.ExecuteScalar();
		}
		public bool Update(string tblName, Dictionary<string, object> fieldValuePairs, string condition)
		{
			string sqlFormat = "UPDATE [{0}] SET {1} WHERE {2}";

			List<string> lst = new List<string>();
			foreach (KeyValuePair<string, object> item in fieldValuePairs)
			{
				if (item.Value != null)
					lst.Add(string.Format("[{0}] = '{1}'", item.Key, item.Value.ToString().Replace("'", "''")));
				else
					lst.Add(string.Format("[{0}] = NULL", item.Key));
			}

			string sql = string.Format(sqlFormat, tblName, string.Join(",", lst.ToArray()), condition);

			return GetCommand(sql).ExecuteNonQuery() > 0;
		}
		public int Delete(string tblName, string condition)
		{
			string sql = string.Format("DELETE FROM [{0}] WHERE {1}", tblName, condition);
			var cmd = GetCommand(sql);

			return cmd.ExecuteNonQuery();
		}
		public int Count(string tblName, string condition)
		{
			if (string.IsNullOrWhiteSpace(condition)) condition = "1=1";
			return (int)(GetCommand(string.Format("SELECT COUNT(1) FROM [{1}] WHERE {2}", tblName, condition)).ExecuteScalar());
		}
		public bool ExistsTable(string tblName)
		{
			string sql = string.Format("SELECT COUNT(1) FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[{0}]') AND type in (N'U')", tblName);
			return (int)(GetCommand(sql).ExecuteScalar()) > 0;
		}
		public bool ExistsColumn(string colName, string tblName)
		{
			string sql = string.Format("SELECT COUNT(1) FROM sys.columns WHERE name = '{0}' AND object_id = OBJECT_ID(N'[dbo].[{1}]')", colName, tblName);
			return (int)(GetCommand(sql).ExecuteScalar()) > 0;
		}
		public bool ExistsRows(string sqlSelect)
		{
			return GetCommand(sqlSelect).ExecuteScalar() != null;
		}
		public void NewTable(string tblName, params string[] columnNames)
		{
			string sql = @"CREATE TABLE [dbo].[{0}]({1});
				ALTER TABLE [{0}] ADD CONSTRAINT [PK_{0}] PRIMARY KEY CLUSTERED ([RowId] ASC);
				ALTER TABLE [dbo].[{0}] ADD CONSTRAINT [DF_{0}_DateCreated]  DEFAULT (GETDATE()) FOR [DateCreated]
			";

			List<string> columns = new List<string>();
			columns.Add("[RowId] [int] IDENTITY(1,1) NOT NULL");
			columns.Add("[DateCreated] [DateTime] NULL");
			foreach (string c in columnNames)
			{
				columns.Add(string.Format("{0} NVARCHAR(MAX) NULL", c));
			}

			sql = string.Format(sql, tblName, string.Join(",", columns.ToArray()));
			GetCommand(sql).ExecuteNonQuery();
		}
		public void AddColumn(string tblName, string colName)
		{
			if (!ExistsColumn(colName, tblName))
			{
				string sql = string.Format("ALTER TABLE [{0}] ADD {1} NVARCHAR(MAX) NULL", tblName, colName);
				GetCommand(sql).ExecuteNonQuery();
			}
		}

		#region Implements the IDisposable interface
		private bool _disposed = false;
		private void Dispose(bool disposing)
		{
			if (!_disposed)
			{
				if (disposing)
				{
					// Release managed resources.
					if (_connection != null && _connection.State != ConnectionState.Closed)
					{
						_connection.Close();
						_connection.Dispose();
						_connection = null;
					}
				}

				// Here is used to release unmanaged resources.

				_disposed = true;
			}
		}
		public void Dispose()
		{
			Dispose(true);
		}
		#endregion

		public DataTable Select(string tblName, string condition = null)
		{
			if (!string.IsNullOrWhiteSpace(condition)) condition = "1=1";
			return Query(string.Format("SELECT * FROM [{0}] WHERE {1}", tblName, condition));
		}
		public int ExecuteNonQuery(string sqlText)
		{
			return GetCommand(sqlText).ExecuteNonQuery();
		}
		public object ExecuteScalar(string sqlText)
		{
			var obj = GetCommand(sqlText).ExecuteScalar();
			if (obj == DBNull.Value)
				return null;
			return obj;
		}
		public DataTable Query(string selectText)
		{
			DataTable result = new DataTable();
			SqlDataAdapter adapter = new SqlDataAdapter(GetCommand(selectText));
			adapter.Fill(result);
			return result;
		}

		public string[] GetColumnNames(string selectText)
		{
			List<string> result = new List<string>();

			var dt = Query(selectText);

			foreach (DataColumn item in dt.Columns)
			{
				result.Add(item.ColumnName);
			}

			return result.ToArray();
		}
		public string[] GetTableNames()
		{
			List<string> lst = new List<string>();
			var dt = Query("SELECT name FROM sys.objects WHERE type in (N'U') order by name asc");
			foreach (DataRow item in dt.Rows)
			{
				lst.Add(item[0].ToString());
			}
			return lst.ToArray();
		}
	}
}
