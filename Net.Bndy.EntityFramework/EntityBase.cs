// =================================================================================
// Copyright (c) 2014 http://www.bndy.net.
// Created by Bndy at 3/26/2014 10:36:43
// ---------------------------------------------------------------------------------
// Base class of Entity
// =================================================================================

using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Net.Bndy.Data;
using System.Data;

namespace Net.Bndy.EntityFramework
{
	/// <summary>
	/// Class EntityBase.
	///		The base class of entity.
	/// </summary>
	public abstract class EntityBase
	{
		public static int i;
		#region Properties
		protected abstract string DbConnectionString { get; }
		protected abstract string PrimaryKey { get; }
		public object Id
		{
			get
			{
				var idProperty = this.GetType().GetProperties().FirstOrDefault(
					p => p.Name.Equals(this.PrimaryKey, StringComparison.OrdinalIgnoreCase)
					);
				if (idProperty != null)
				{
					return idProperty.GetValue(this, null);
				}
				return null;
			}
		}
		protected internal IDbFactory Factory
		{
			get
			{
				return DbFactory.GetInstance(this.DbConnectionString);
			}
		}
		#endregion

		#region Methods
		internal protected List<TSelf> Query<TSelf>(string condition) where TSelf : EntityBase
		{
			using (IDbFactory dbFactory = DbFactory.GetInstance(this.DbConnectionString))
			{
				List<TSelf> lst = new List<TSelf>();
				var dt = dbFactory.Select(this.GetType().Name, condition);
				foreach(DataRow dr in dt.Rows)
				{
					lst.Add(dr.ConvertToDict().ConvertTo<TSelf>());
				}
				return lst;
			}
		}
		public virtual TSelf Load<TSelf>(object id) where TSelf : EntityBase
		{
			using (IDbFactory dbFactory = DbFactory.GetInstance(this.DbConnectionString))
			{
				var dt = dbFactory.Select(this.GetType().Name, string.Format("[{0}] = '{1}'", this.PrimaryKey, id));
				var dr = dt.Rows.Cast<DataRow>().FirstOrDefault();
				if (dr != null)
				{
					var dict = dr.ConvertToDict();
					return dict.ConvertTo<TSelf>();
				}
			}

			return default(TSelf);
		}

		public virtual void Save()
		{
			using (IDbFactory dbFactory = DbFactory.GetInstance(this.DbConnectionString))
			{
				dbFactory.InsertObject(this);
			}
		}

		public virtual void Update()
		{
			using (IDbFactory dbFactory = DbFactory.GetInstance(this.DbConnectionString))
			{
				dbFactory.UpdateObject(this, string.Format("[{0}] = '{1}'", this.PrimaryKey, this.Id));
			}
		}
		public virtual void Delete()
		{
			using (IDbFactory dbFactory = DbFactory.GetInstance(this.DbConnectionString))
			{
				dbFactory.Delete(this.GetType().Name, string.Format("[{0}] = '{1}'", this.PrimaryKey, this.Id));
			}
		}
		#endregion
	}
}