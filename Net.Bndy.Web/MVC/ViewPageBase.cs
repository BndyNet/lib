// ==========================================================================
// Copyright (c) 2016 http://www.bndy.net
// Created by Bndy at 10/18/2016 20:31:35
// --------------------------------------------------------------------------
// The base view page for MVC
// ==========================================================================


namespace Net.Bndy.Web.MVC
{
    public abstract class ViewPageBase : System.Web.Mvc.WebViewPage
    {

    }
    public abstract class ViewPageBase<TModel> : System.Web.Mvc.WebViewPage<TModel>
    {

    }
}
