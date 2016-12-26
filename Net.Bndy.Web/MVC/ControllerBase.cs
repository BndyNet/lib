// =================================================================================
// Copyright (c) 2016 http://www.bndy.net.
// Created by Bndy at 11/9/2015 7:55:32 PM
// ---------------------------------------------------------------------------------
// Summary
// =================================================================================

using System;
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

        /// <summary>
        /// Returns an AjaxResult json about OK.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>JsonNetResult.</returns>
        protected JsonNetResult AjaxOK(object data)
        {
            return AjaxResult(AjaxResultStatus.OK, null, data, null);
        }
        /// <summary>
        /// Returns an AjaxResult json about error.
        /// </summary>
        /// <param name="ex">The ex.</param>
        /// <returns>JsonNetResult.</returns>
        protected JsonNetResult AjaxError(Exception ex)
        {
            return AjaxResult(AjaxResultStatus.Error, ex.Message, ex, null);
        }
        /// <summary>
        /// Returns an AjaxResult json about error.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>JsonNetResult.</returns>
        protected JsonNetResult AjaxError(string message)
        {
            return AjaxResult(AjaxResultStatus.Error, message);
        }
        /// <summary>
        /// Returns an AjaxResult json about unauthorized.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>JsonNetResult.</returns>
        protected JsonNetResult AjaxUnauthorized(string message)
        {
            return AjaxResult(AjaxResultStatus.Unauthorized, message);
        }
        protected JsonNetResult AjaxResult(AjaxResultStatus status, string message, object data = null, object extraData = null)
        {
            return new JsonNetResult
            {
                Data = new AjaxResult
                {
                    Data = data,
                    ExtraData = extraData,
                    Message = message,
                    Status = status,
                },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
    }
}
