// =================================================================================
// Copyright (c) 2015 http://www.bndy.net.
// Created by Bndy at 5/3/2015 11:32:23 AM
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

    public abstract class Index
    {
        internal const string NameOfId = "Id";
        internal static Dictionary<string, Dictionary<string, IndexAttribute>> AllFieldIndexAttributes { get; set; }
        public abstract string Id { get; set; }
        public Dictionary<string, string> MatchedSnippets { get; set; }
        public Dictionary<string, IndexAttribute> FieldIndexAttributes
        {
            get
            {
                var thisType = this.GetType().ToString();
                if (AllFieldIndexAttributes == null)
                {
                    AllFieldIndexAttributes = new Dictionary<string, Dictionary<string, IndexAttribute>>();
                }

                if (!AllFieldIndexAttributes.ContainsKey(thisType))
                {
                    AllFieldIndexAttributes[thisType] = new Dictionary<string, IndexAttribute>();

                    foreach (var p in this.GetType().GetProperties())
                    {
                        var attr = p.GetCustomAttributes(typeof(IndexAttribute), true).FirstOrDefault() as IndexAttribute;
                        if (attr != null)
                        {
                            AllFieldIndexAttributes[thisType][p.Name] = attr;
                        }
                    }
                }

                return AllFieldIndexAttributes[thisType];
            }
        }
        public IndexAttribute GetFieldIndexAttribute(string propertyName)
        {
            return FieldIndexAttributes.ContainsKey(propertyName) ? FieldIndexAttributes[propertyName] : null;
        }
        public Index()
        {
            this.MatchedSnippets = new Dictionary<string, string>();
        }
    }

    public enum IndexMethod
    {
        ANALYZED,
        NOT_ANALYZED,
        NOT_INDEX,
    }
}
