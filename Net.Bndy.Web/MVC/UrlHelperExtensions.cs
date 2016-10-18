// ==========================================================================
// Copyright (c) 2016 http://www.bndy.net
// Created by Bndy at 10/18/2016 21:00:32
// --------------------------------------------------------------------------
// Extensions for UrlHelper
// ==========================================================================


namespace Net.Bndy.Web.MVC
{
    public static class UrlHelperExtensions
    {
        public static string ToString(this System.Web.Mvc.UrlHelper it)
        {
            return it.RequestContext.HttpContext.Request.Url.ToString();
        }

        public static string SetQueryString(this System.Web.Mvc.UrlHelper it, string key, object value)
        {
            var up = UrlParameters.Current;
            up.SetValue(key, value);
            return up.ToUrlString();
        }

        public static string GetQueryString(this System.Web.Mvc.UrlHelper it, string key)
        {
            var up = UrlParameters.Current;
            return up.GetValue(key);
        }
        public static T GetQueryString<T>(this System.Web.Mvc.UrlHelper it, string key, T defaultValue)
            where T : struct
        {
            var val = GetQueryString(it, key);
            return val.ConvertTo<T>(defaultValue);
        }
    }
}
