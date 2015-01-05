// =================================================================================
// Copyright (c) 2012 http://www.bndy.net.
// Created by Bndy at 12/15/2012 11:11:59 AM
// ---------------------------------------------------------------------------------
// Summary & Change Logs.
// =================================================================================

using System;
using System.Data;
using System.Collections.Generic;
using System.Data.Common;

namespace Net.Bndy.Data
{
	using Net.Bndy.Data.SqlServer;

	public abstract class DbFactory
	{
		/// <summary>
		/// Gets an derived instance of IDbFactory.
		/// </summary>
		/// <param name="connectionString">The connection string</param>
		/// <param name="databaseType">Database Type</param>
		/// <returns>An instance of IDbFactory</returns>
		public static IDbFactory GetInstance(string connectionString, DbType databaseType = DbType.SqlServer)
		{
			IDbFactory factory = null;
			switch (databaseType)
			{
				case DbType.SqlServer:
					factory = new MsSqlFactory(connectionString);
					break;

				default:
					throw new Exception(databaseType + " is not supported.");
			}

			return factory;
		}
	}
}