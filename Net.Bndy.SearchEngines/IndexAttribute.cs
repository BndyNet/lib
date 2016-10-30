// =================================================================================
// Copyright (c) 2015 http://www.bndy.net.
// Created by Bndy at 5/3/2015 9:37:18 AM
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
    [AttributeUsage(AttributeTargets.Property)]
    public class IndexAttribute : Attribute
    {
        public float? Priority { get; set; }
        public IndexMethod Method { get; set; }
    }
}
