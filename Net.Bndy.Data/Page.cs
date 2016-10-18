// ==========================================================================
// Copyright (c) 2013 http://www.bndy.net
// Created by Bndy at 3/22/2013 20:31:35
// --------------------------------------------------------------------------
// Page Model
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Linq;

namespace Net.Bndy.Data
{
    public class Page<T> where T : class
    {
        private IList<T> _data;
        public IList<T> Data
        {
            get
            {
                return _data;
            }
            set
            {
                _data = value;
                if (!this.PageSize.HasValue)
                {
                    this.RecordCount = _data.Count;
                    this.RecordCountOfCurrentPage = this.RecordCount;
                }
            }
        }
        public int? PageSize { get; set; }
        private int _recordCount;

        public int RecordCount
        {
            get
            {
                return _recordCount;
            }
            set
            {
                _recordCount = value;

                if (_recordCount > 0)
                {
                    // set page count
                    if (this.PageSize > 0)
                    {
                        if (_recordCount % this.PageSize == 0)
                        {
                            this.PageCount = _recordCount / this.PageSize.Value;
                        }
                        else
                        {
                            this.PageCount = _recordCount / this.PageSize.Value + 1;
                        }
                    }
                    else
                    {
                        this.PageCount = 1;
                    }
                    // check current page
                    if (this.CurrentPage < 1)
                        this.CurrentPage = 1;
                    else if (this.CurrentPage > this.PageCount)
                        this.CurrentPage = this.PageCount;
                    // set the max item count
                    if (this.CurrentPage == this.PageCount)
                    {
                        this.RecordCountOfCurrentPage = _recordCount;
                    }
                    else
                    {
                        this.RecordCountOfCurrentPage = this.CurrentPage * this.PageSize.Value;
                    }
                    // set start and end page numbers
                    if (this.PageCount <= 10)
                    {
                        this.DisplayPageNumbers.AddRange(MathHelper.Range(this.PageCount));
                    }
                    else
                    {
                        this.DisplayPageNumbers.AddRange(new int[] { 1, 2 });

                        var b = 0;
                        var e = 0;

                        if (this.CurrentPage <= 5)
                        {
                            b = 3;
                            e = b + 4;
                        }
                        else if (this.CurrentPage >= this.PageCount - 5)
                        {
                            e = this.PageCount - 2;
                            b = e - 4;
                        }
                        else
                        {
                            b = this.CurrentPage - 2;
                            e = this.CurrentPage + 2;
                        }

                        if (b > 3)
                            this.DisplayPageNumbers.Add(-1);

                        this.DisplayPageNumbers.AddRange(
                            MathHelper.Range(b, e));

                        if (e < this.PageCount - 3)
                            this.DisplayPageNumbers.Add(-2);

                        this.DisplayPageNumbers.AddRange(new int[] { this.PageCount - 1, this.PageCount });
                    }
                }
            }
        }
        public int CurrentPage { get; set; }

        public int PageCount { get; set; }

        public List<int> DisplayPageNumbers { get; set; }

        public int RecordCountOfCurrentPage { get; set; }
        public object Extra { get; set; }
        public Page(int? pageSize = null,
            int currentPage = 1)
        {
            this.PageSize = pageSize;
            this.CurrentPage = currentPage;
            this.Data = new List<T>();
            this.Extra = new Dictionary<string, string>();
            this.DisplayPageNumbers = new List<int>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Pagination{T}"/> class.
        /// </summary>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="currentPage">The current page.</param>
        /// <param name="query">The query with condition and sort</param>
        public Page(int pageSize, int currentPage, IQueryable<T> query)
            : this(pageSize, currentPage)
        {
            this.RecordCount = query.Count();
            if (currentPage <= 0)
            {
                currentPage = 1;
            }
            this.Data = query.Skip(pageSize * (currentPage - 1)).Take(pageSize).ToList();
        }
        public Page<TDest> Select<TDest>(Func<T, TDest> converter)
            where TDest : class
        {
            var result = new Page<TDest>(this.PageSize, this.CurrentPage);
            result.RecordCount = this.RecordCount;
            result.Extra = this.Extra;
            foreach (var item in this.Data)
            {
                result.Data.Add(converter(item));
            }
            return result;
        }
        public Page<object> Select(Func<T, object> converter)
        {
            return Select<object>(converter);
        }

    }
}
