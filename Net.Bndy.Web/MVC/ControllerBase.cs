// =================================================================================
// Copyright (c) 2016 http://www.bndy.net.
// Created by Bndy at 11/9/2015 7:55:32 PM
// ---------------------------------------------------------------------------------
// Summary
// =================================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Net.Bndy.Web.MVC
{
    public class ControllerBase : Controller
    {
        /// <summary>
        ///  Creates a JsonNetResult object that serializes the specified object
        ///     to JavaScript Object Notation (JSON) format using the specified content type
        ///     and JSON request behavior.
        /// </summary>
        /// <param name="data">The JavaScript object graph to serialize.</param>
        /// <param name="behavior"> The JSON request behavior.</param>
        /// <returns>The result object that serializes the specified object to JSON format. </returns>
        protected JsonNetResult JsonNet(object data, JsonRequestBehavior behavior = JsonRequestBehavior.AllowGet)
        {
            return new JsonNetResult()
            {
                Data = data,
                JsonRequestBehavior = behavior,
            };
        }
    }
}
