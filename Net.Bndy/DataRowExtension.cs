// =================================================================================
// Copyright (c) 2014 http://www.bndy.net
// Created by Bndy at 3/26/2014 15:58:57
// ---------------------------------------------------------------------------------
// Summary & Change Logs.
// =================================================================================

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Net.Bndy
{
	public static class DataRowExtension
	{
		/// <summary>
		/// Converts to dictionary.
		/// </summary>
		/// <param name="dr">The instance of DataRow.</param>
		/// <returns>Dictionary{System.StringSystem.Object}.</returns>
		public static Dictionary<string, object> ConvertToDict(this DataRow dr)
		{
			var dict = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
			var columns = dr.Table.Columns;
			foreach (DataColumn dc in columns)
			{
				dict.Add(dc.ColumnName, dr[dc.ColumnName]);
			}
			return dict;
		}
	}
}
