// =================================================================================
// Copyright (c) 2015 http://www.bndy.net.
// Created by Bndy at 5/2/2015 11:15:21 AM
// ---------------------------------------------------------------------------------
// Summary & Change Logs.
// =================================================================================


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net.Bndy.SearchEngines
{
    public class SearchResult<TIndex> where TIndex : Index
    {
        public List<TIndex> Items { get; set; }
        public int? PageSize { get; set; }
        public int Page { get; set; }
        public int Total { get; set; }

        public SearchResult(int? pageSize, int page)
        {
            PageSize = pageSize;
            Page = page;
            Items = new List<TIndex>();
        }
    }

}
