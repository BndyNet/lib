// =================================================================================
// Copyright (c) 2012 http://www.bndy.net.
// Created by Bndy at 12/15/2012 11:03:00 AM
// ---------------------------------------------------------------------------------
// Summary & Change Logs.
// =================================================================================

using System;
using System.Data;
using System.Collections.Generic;

namespace Net.Bndy.Data
{
	/// <summary>
	/// The interface for operating database.
	/// </summary>
	public interface IDbFactory : IDisposable
	{
		/// <summary>
		/// Indicates whether the database can be opened.
		/// </summary>
		/// <returns>True if exsited, otherwise false</returns>
		bool CanOpen();
		/// <summary>
		/// Inserts an object to database.
		/// </summary>
		/// <param name="obj">The object with values</param>
		/// <param name="createObjectTable">True to create the table named obj name, otherwise false</param>
		void InsertObject(object obj, bool createObjectTable = true);
		/// <summary>
		/// Updates data
		/// </summary>
		/// <param name="dataObject">The object with values</param>
		/// <param name="condition">The condition, like the sql where sentence</param>
		void UpdateObject(object dataObject, string condition);
		/// <summary>
		/// Inserts values to database
		/// </summary>
		/// <param name="tblName">The table name</param>
		/// <param name="fieldValuePairs">The values</param>
		/// <returns></returns>
		object Insert(string tblName, Dictionary<string, object> fieldValuePairs);
		/// <summary>
		/// Updates data
		/// </summary>
		/// <param name="tblName">The table name</param>
		/// <param name="fieldValuePairs">The new values</param>
		/// <param name="condition">The condition, like sql where sentence</param>
		/// <returns></returns>
		bool Update(string tblName, Dictionary<string, object> fieldValuePairs, string condition);
		/// <summary>
		/// Deletes data
		/// </summary>
		/// <param name="tblName">The table name</param>
		/// <param name="condition">The condition, like sql where sentence</param>
		/// <returns></returns>
		int Delete(string tblName, string condition);
		/// <summary>
		/// Gets the count of rows matched the condition.
		/// </summary>
		/// <param name="tblName">The table name</param>
		/// <param name="condition">The condition, like sql where sentence</param>
		/// <returns>The count</returns>
		int Count(string tblName, string condition);
		/// <summary>
		/// Queries the table
		/// </summary>
		/// <param name="tblName">The table name</param>
		/// <param name="condition">The condition, like sql where sentence</param>
		/// <returns>Return the DataTable instance</returns>
		DataTable Select(string tblName, string condition = null);

		/// <summary>
		/// Determines whether the table exists.
		/// </summary>
		/// <param name="tblName">The table name</param>
		/// <returns>True if existed, otherwise false</returns>
		bool ExistsTable(string tblName);
		/// <summary>
		/// Determines whether the column exists.
		/// </summary>
		/// <param name="colName">The column name</param>
		/// <param name="tblName">The table name</param>
		/// <returns>True if existed, otherwise false</returns>
		bool ExistsColumn(string colName, string tblName);
		/// <summary>
		/// Determines whether the rows exist matched the condition.
		/// </summary>
		/// <param name="sqlSelect">The select sql.</param>
		/// <returns>True if the result has at lease one row, otherwise false</returns>
		bool ExistsRows(string sqlSelect);

		/// <summary>
		/// Executes the sql text.
		/// </summary>
		/// <param name="sqlText">The sql text</param>
		/// <returns>The affected rows</returns>
		int ExecuteNonQuery(string sqlText);
		/// <summary>
		/// Get the value of first row and first column from the result of sql.
		/// </summary>
		/// <param name="sqlText">The sql text</param>
		/// <returns>The value of first row and first column</returns>
		object ExecuteScalar(string sqlText);
		/// <summary>
		/// Queries by specified the select sql.
		/// </summary>
		/// <param name="selectText">The select sql</param>
		/// <returns>An DataTable instance</returns>
		DataTable Query(string selectText);
		/// <summary>
		/// Gets the column names from the select sql.
		/// </summary>
		/// <param name="sqlSelect">The select sql</param>
		/// <returns>An array of String</returns>
		string[] GetColumnNames(string sqlSelect);
		/// <summary>
		/// Gets the table names in database.
		/// </summary>
		/// <returns>An array of String</returns>
		string[] GetTableNames();
	}
}