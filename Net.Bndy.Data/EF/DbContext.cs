// ==========================================================================
// Copyright (c) 2013 http://www.bndy.net
// Created by Bndy at 3/22/2013 20:31:35
// --------------------------------------------------------------------------
// DbContext for Entity Framework
// ==========================================================================

using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace Net.Bndy.Data.EF
{
    public class DbContext : System.Data.Entity.DbContext
    {
        /// <summary>
        /// Pages data.
        /// </summary>
        /// <typeparam name="TEntity">The type of the t entity.</typeparam>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="page">The page.</param>
        /// <param name="condition">The condition.</param>
        /// <param name="sortField">The sort field.</param>
        /// <param name="isAscending">if set to <c>true</c> [is ascending].</param>
        /// <param name="converter">The converter.</param>
        /// <param name="includes">The includes.</param>
        /// <returns>Page&lt;TEntity&gt;.</returns>
        public Page<TEntity> Page<TEntity>(
           int pageSize, int page,
           Expression<Func<TEntity, bool>> condition,
           Expression<Func<TEntity, object>> sortField,
           bool isAscending,
           Func<TEntity, TEntity> converter = null,
           params Expression<Func<TEntity, dynamic>>[] includes
        ) where TEntity : class
        {
            var result = new Page<TEntity>(pageSize, page);

            var set = this.Set<TEntity>();
            var query = set.AsQueryable();

            if (condition != null)
            {
                result.RecordCount = query.Count(condition);
                query = query.Where(condition);
            }
            else
                result.RecordCount = set.Count();

            if (includes != null)
            {
                query = includes.Aggregate(query, (current, include) => current.Include(include));
            }

            if (isAscending)
                query = query.OrderBy(sortField);
            else
                query = query.OrderByDescending(sortField);

            if (converter != null)
                result.Data = query.Skip((page - 1) * pageSize).Take(pageSize).Select(converter).ToList();
            else
                result.Data = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            return result;
        }
    }
}
