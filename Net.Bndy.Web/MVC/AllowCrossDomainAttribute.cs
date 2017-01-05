// =================================================================================
// Copyright (c) 2016 http://www.bndy.net.
// Created by Bndy at 1/4/2015 7:55:32 PM
// ---------------------------------------------------------------------------------
// Summary
// =================================================================================

using System.Web.Mvc;

namespace Net.Bndy.Web.MVC
{
    /// <summary>
    /// Indicates whether enable crossing domain for Action.
    /// </summary>
    /// <seealso cref="System.Web.Mvc.ActionFilterAttribute" />
    public class AllowCrossDomainAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Gets or sets the origin. e.x. http://www.bndy.net
        /// </summary>
        /// <value>The origin.</value>
        public string Origin { get; set; }
        /// <summary>
        /// Gets or sets the headers.
        /// </summary>
        /// <value>The headers.</value>
        public string Headers { get; set; }
        /// <summary>
        /// Gets or sets the methods (GET, POST, PUT, DELETE).
        /// </summary>
        /// <value>The methods.</value>
        public string Methods { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AllowCrossDomainAttribute"/> class.
        /// </summary>
        /// <param name="origins">The origins.</param>
        /// <param name="methods">The methods.</param>
        /// <param name="headers">The headers.</param>
        public AllowCrossDomainAttribute(string origin = null, string methods = null, string headers = null)
        {
            this.Origin = origin ?? "*";
            this.Methods = methods;
            this.Headers = headers;
        }

        /// <summary>
        /// Called by the ASP.NET MVC framework before the action method executes.
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            filterContext.RequestContext.HttpContext.Response.Headers.Add("Access-Control-Allow-Origin", this.Origin);

            if (!string.IsNullOrWhiteSpace(this.Methods))
            {
                filterContext.RequestContext.HttpContext.Response.Headers.Add("Access-Control-Allow-Methods", this.Methods);
            }

            if (!string.IsNullOrWhiteSpace(this.Headers))
            {
                filterContext.RequestContext.HttpContext.Response.Headers.Add("Access-Control-Allow-Headers", this.Headers);
            }

            base.OnActionExecuting(filterContext);
        }
    }
}
