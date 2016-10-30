// =================================================================================
// Copyright (c) 2015 http://www.bndy.net.
// Created by Bndy at 5/2/2015 10:12:22 AM
// ---------------------------------------------------------------------------------
// Summary & Change Logs.
// =================================================================================

using System;
using System.Collections.Generic;
using System.Linq;

using Lucene.Net.Analysis;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Analysis.PanGu;
using PanGu;
using PanGu.HighLight;

namespace Net.Bndy.SearchEngines.Lucene
{
    public class LuceneIndexer<TIndex> : Indexer<TIndex, Document>
        where TIndex : Index, new()
    {
        public Directory IndexDirectory { get; private set; }
        public Analyzer Analyzer
        {
            get
            {
                return new PanGuAnalyzer();
            }
        }

        public LuceneIndexer(string indexDirectory)
        {
            IndexDirectory = new SimpleFSDirectory(new System.IO.DirectoryInfo(indexDirectory));
        }

        private IndexWriter GetWriter()
        {
            return new IndexWriter(IndexDirectory, Analyzer,
                !IndexReader.IndexExists(IndexDirectory), IndexWriter.MaxFieldLength.UNLIMITED);
        }
        private IndexSearcher GetSearcher()
        {
            return new IndexSearcher(IndexDirectory);
        }

        public override TIndex ParseFromIndexSource(Document indexSource)
        {
            var result = default(TIndex);

            if (indexSource != null)
            {
                result = new TIndex();
                foreach (var p in typeof(TIndex).GetProperties())
                {
                    var fld = indexSource.GetField(p.Name);
                    if (fld == null) continue;

                    var val = fld.StringValue;
                    if (!string.IsNullOrWhiteSpace(val))
                    {
                        object oVal = val;

                        if (p.PropertyType == typeof(int))
                        {
                            oVal = int.Parse(val);
                        }
                        if (p.PropertyType == typeof(long))
                        {
                            oVal = long.Parse(val);
                        }
                        if (p.PropertyType == typeof(short))
                        {
                            oVal = short.Parse(val);
                        }
                        if (p.PropertyType == typeof(DateTime))
                        {
                            oVal = DateTime.Parse(val);
                        }
                        if (p.PropertyType == typeof(float))
                        {
                            oVal = float.Parse(val);
                        }
                        if (p.PropertyType == typeof(double))
                        {
                            oVal = double.Parse(val);
                        }
                        if (p.PropertyType.IsEnum)
                        {
                            oVal = Enum.Parse(p.PropertyType, val);
                        }

                        if (p.PropertyType.IsGenericType && p.PropertyType.IsValueType)
                        {
                            //e.g. int?
                            // for NET45+
                            //p.SetValue(result, Convert.ChangeType(val, p.PropertyType.GenericTypeArguments[0]), null);
                            p.SetValue(result, Convert.ChangeType(val, p.PropertyType.GetGenericArguments()[0]), null);

                        }
                        else
                        {
                            p.SetValue(result, oVal, null);
                        }
                    }
                }
            }

            return result;
        }


        public override bool Remove(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return false;

            using (IndexWriter iw = GetWriter())
            {
                iw.DeleteDocuments(new Term(Index.NameOfId, id));
                iw.Commit();
                iw.Optimize();

                return iw.HasDeletions();
            }
        }

        public override bool RemoveRange(object predicate)
        {
            if (predicate == null)
                return false;

            using (IndexWriter iw = GetWriter())
            {
                List<Term> terms = new List<Term>();
                foreach (var kv in predicate.ToDict())
                {
                    terms.Add(new Term(kv.Key, kv.Value));
                }
                iw.DeleteDocuments(terms.ToArray());
                iw.Commit();
                iw.Optimize();

                return iw.HasDeletions();
            }
        }

        public override SearchResult<TIndex> Search(string keywords,
            int? pageSize = null,
            int page = 1,
            object condition = null)
        {
            keywords = keywords.Trim();
            var result = new SearchResult<TIndex>(pageSize, page);

            if (!string.IsNullOrWhiteSpace(keywords))
            {
                using (var searcher = GetSearcher())
                {
                    var parser = new MultiFieldQueryParser(global::Lucene.Net.Util.Version.LUCENE_30,
                        new TIndex().FieldIndexAttributes.Select(__ => __.Key).ToArray(),
                        Analyzer);
                    Query query = null;
                    try
                    {
                        query = parser.Parse(keywords);
                    }
                    catch (ParseException)
                    {
                        query = parser.Parse(QueryParser.Escape(keywords));
                    }

                    if (condition != null)
                    {
                        var bq = new BooleanQuery();
                        bq.Add(query, Occur.MUST);
                        foreach (var item in condition.ToDict())
                        {
                            if (item.Key == null || item.Value == null)
                                continue;

                            bq.Add(new TermQuery(new Term(item.Key, item.Value)), Occur.MUST);
                        }
                        query = bq;
                    }

                    var maxContentLength = 180;

                    #region highlight
                    SimpleHTMLFormatter formatter = new SimpleHTMLFormatter("<span class=\"search-highlight\">", "</span>");
                    Highlighter highlighter = new Highlighter(formatter, new Segment());
                    highlighter.FragmentSize = maxContentLength;
                    #endregion

                    var collector = TopScoreDocCollector.Create(100, false);
                    searcher.Search(query, collector);

                    var hits = pageSize.HasValue ? collector.TopDocs((page - 1) * pageSize.Value, pageSize.Value).ScoreDocs : collector.TopDocs().ScoreDocs;
                    result.Total = collector.TotalHits;

                    foreach (var d in hits)
                    {
                        var doc = searcher.Doc(d.Doc);
                        var index = ParseFromIndexSource(doc);
                        if (index != null)
                        {
                            foreach (var field in doc.GetFields())
                            {
                                var matchedText = highlighter.GetBestFragment(keywords, field.StringValue);
                                if (!string.IsNullOrWhiteSpace(matchedText))
                                {
                                    index.MatchedSnippets[field.Name] = matchedText;
                                }
                            }

                            result.Items.Add(index);
                        }
                    }
                }
            }

            return result;
        }

        public override TIndex Get(string id)
        {
            if (id == null) return null;

            using (var searcher = GetSearcher())
            {
                var collector = TopScoreDocCollector.Create(1, false);
                searcher.Search(new TermQuery(new Term(Index.NameOfId, id)), collector);
                var top = collector.TopDocs(0, 1);
                if (top != null && top.ScoreDocs.Any())
                {
                    var doc = searcher.Doc(top.ScoreDocs.First().Doc);
                    var index = ParseFromIndexSource(doc);

                    return index;
                }
            }

            return null;
        }
        public override void Upsert(params TIndex[] items)
        {
            using (IndexWriter iw = GetWriter())
            {
                iw.UseCompoundFile = false;
                foreach (var index in items)
                {
                    if (index == null || string.IsNullOrWhiteSpace(index.Id))
                        continue;

                    var doc = new Document();

                    doc.Add(new Field(Index.NameOfId, index.Id, Field.Store.YES, Field.Index.NOT_ANALYZED));

                    var properties = index.GetType().GetProperties();
                    foreach (var p in properties)
                    {
                        var indexAttr = index.GetFieldIndexAttribute(p.Name);
                        if (indexAttr != null)
                        {
                            var val = p.GetValue(index, null);
                            var field = new Field(p.Name, val != null ? val.ToString() : "", Field.Store.YES,
                               indexAttr.Method == IndexMethod.ANALYZED ? Field.Index.ANALYZED
                                   : (indexAttr.Method == IndexMethod.NOT_ANALYZED ? Field.Index.NOT_ANALYZED
                                       : (indexAttr.Method == IndexMethod.NOT_INDEX ? Field.Index.NO : Field.Index.ANALYZED)
                                   )
                               );
                            if (indexAttr.Priority.HasValue)
                            {
                                field.Boost = indexAttr.Priority.Value;
                            }
                            doc.Add(field);
                        }
                    }

                    if (Get(index.Id) != null)
                    {
                        iw.UpdateDocument(new Term(Index.NameOfId, index.Id), doc);
                    }
                    else
                    {
                        iw.AddDocument(doc);
                    }
                }
                iw.Optimize();
            }
        }
    }
}
