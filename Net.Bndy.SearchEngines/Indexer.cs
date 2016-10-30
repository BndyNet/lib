// =================================================================================
// Copyright (c) 2015 http://www.bndy.net.
// Created by Bndy at 5/3/2015 11:37:18 AM
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
    public abstract class Indexer<TIndex, TIndexSource> where TIndex : Index, new()
    {
        public abstract TIndex Get(string id);
        public abstract void Upsert(params TIndex[] item);
        public abstract SearchResult<TIndex> Search(string keywords,
            int? pageSize = null,
            int page = 1,
            object condition = null);
        public abstract bool Remove(string id);
        public abstract bool RemoveRange(object predicate);
        public abstract TIndex ParseFromIndexSource(TIndexSource indexSource);
    }

}
