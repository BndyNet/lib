// =================================================================================
// Copyright (c) 2016 http://www.bndy.net.
// Created by Bndy at 11/9/2015 7:38:14 PM
// ---------------------------------------------------------------------------------
// Summary
// =================================================================================

using Newtonsoft.Json;
using System;
using System.Web.Mvc;

namespace Net.Bndy.Web.MVC
{
    public class JsonNetResult : System.Web.Mvc.JsonResult
    {
        /// <summary>
        /// Enables processing of the result of an action method by a custom type that inherits from the <see cref="T:System.Web.Mvc.ActionResult" /> class.
        /// </summary>
        /// <param name="context">The context within which the result is executed.</param>
        /// <exception cref="ArgumentNullException">context</exception>
        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            var response = context.HttpContext.Response;

            response.ContentType = !String.IsNullOrEmpty(ContentType)
                ? ContentType
                : "application/json";

            if (ContentEncoding != null)
                response.ContentEncoding = ContentEncoding;

            var serializedObject = JsonConvert.SerializeObject(Data, Formatting.Indented, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            });

            response.Write(serializedObject);
        }
    }
}
