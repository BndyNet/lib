// =================================================================================
// Copyright (c) 2014 http://www.bndy.net.
// Created by Bndy at 7/4/2014 10:33:57 AM
// ---------------------------------------------------------------------------------
// Extensions for object
// =================================================================================

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Net.Bndy.Data
{
	public static class ObjectExtension
	{
		/// <summary>
		/// Inserts the object into specified table of database.
		/// </summary>
		/// <param name="obj">The object.</param>
		/// <param name="tblName">Name of the table.</param>
		/// <param name="dbConnectionString">The database connection string.</param>
		public static void InsertTo(this object obj, string tblName, string dbConnectionString)
		{
			using (IDbFactory dbFactory = DbFactory.GetInstance(dbConnectionString))
			{
				dbFactory.Insert(tblName, obj.GetNativePropertiesDict());
			}
		}

		/// <summary>
		/// Generates the insert sql for object.
		/// </summary>
		/// <param name="obj">The object.</param>
		/// <param name="excludedProperties">The property names to exclude and which ignores case.</param>
		/// <returns>The Sql</returns>
		public static string ToInsertSql(
			this object obj,
			params string[] excludedProperties)
		{
			if (obj == null)
				return null;

			var tblName = "[TableName]";
			var dataDict = new Dictionary<string, string>();
			if (obj is IDictionary)
			{
				var d = ((IDictionary)obj);
				foreach (object key in d.Keys)
				{

					if (key != null)
					{
						var newKey = key.ToString();
						var val = d[key];
						if (val != null)
							dataDict.Add(newKey, val.ToString());
						else
							dataDict.Add(newKey, null);
					}
				}
			}
			else
			{
				dataDict = obj.ToDict();
				tblName = obj.GetType().Name;
			}

			var fields = new List<string>();
			var values = new List<string>();
			foreach (KeyValuePair<string, string> kv in dataDict)
			{
				if (excludedProperties.Count(__ => __.ToUpper() == kv.Key.ToUpper()) > 0)
					continue;

				fields.Add(string.Format("[{0}]", kv.Key));

				var value = kv.Value;
				if (value == null)
				{
					values.Add("NULL");
				}
				else
				{
					values.Add(value.ToString().Replace("'", "''"));
				}
			}

			var format = "INSERT INTO [{0}]({1}) VALUES('{2}')";

			return string.Format(format, tblName,
				string.Join(", ", fields), string.Join("', '", values)
				);
		}

		/// <summary>
		/// Generates the update sql for object.
		/// </summary>
		/// <param name="obj">The object.</param>
		/// <param name="condition">The condition as the part of where sql.</param>
		/// <param name="excludedProperties">The property names to exclude and which ignores case.</param>
		/// <returns>The Sql</returns>>
		public static string ToUpdateSql(
			this object obj,
			string condition,
			params string[] excludedProperties
			)
		{
			if (obj == null)
				return null;

			var tblName = "[TableName]";
			var dataDict = new Dictionary<string, string>();
			if (obj is IDictionary)
			{
				var d = ((IDictionary)obj);
				foreach (object key in d.Keys)
				{

					if (key != null)
					{
						var newKey = key.ToString();
						var val = d[key];
						if (val != null)
							dataDict.Add(newKey, val.ToString());
						else
							dataDict.Add(newKey, null);
					}
				}
			}
			else
			{
				dataDict = obj.ToDict();
				tblName = obj.GetType().Name;
			}

			var lst = new List<string>();
			foreach (KeyValuePair<string, string> kv in dataDict)
			{
				if (excludedProperties.Count(__ => __.ToUpper() == kv.Key.ToUpper()) > 0)
					continue;

				lst.Add(string.Format("[{0}] = {1}",
					kv.Key,
					kv.Value == null
						? "NULL" : "'" + kv.Value.ToString().Replace("'", "''") + "'"
					));
			}

			var format = "UPDATE [{0}] SET {1} WHERE {2}";

			if (string.IsNullOrWhiteSpace(condition))
				condition = "1=1";

			return string.Format(format, tblName,
				string.Join(", ", lst), condition
				);
		}
	}
}
