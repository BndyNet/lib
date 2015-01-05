// =================================================================================
// Copyright (c) 2014 http://www.bndy.net.
// Created by Bndy at 3/26/2014 17:19:08
// ---------------------------------------------------------------------------------
// The class of EntitySet
// =================================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Net.Bndy.EntityFramework
{
	public static class EntitySet<TEntity> where TEntity : EntityBase
	{
		private static TEntity _emptyEntity;
		static EntitySet()
		{
			_emptyEntity = System.Activator.CreateInstance<TEntity>();
		}
		public static TEntity Get(string condition)
		{
			return _emptyEntity.Query<TEntity>(condition).FirstOrDefault();
		}
		public static List<TEntity> GetList(string condition)
		{
			return _emptyEntity.Query<TEntity>(condition);
		}
		public static int GetCount(string condition = null)
		{
			return _emptyEntity.Factory.Count(typeof(TEntity).Name, condition);
		}
		public static int Delete(string condition)
		{
			return _emptyEntity.Factory.Delete(typeof(TEntity).Name, condition);
		}
	}
}
