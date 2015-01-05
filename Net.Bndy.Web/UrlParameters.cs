// =================================================================================
// Copyright (c) 2012 http://www.bndy.net.
// Created by Bndy at 12/5/2012 19:03:00
// ---------------------------------------------------------------------------------
// Url parameters helper.
//	Organizes the url parameters.
// =================================================================================

using System.Web;
using System.Collections.Generic;

namespace Net.Bndy.Web
{
	public class UrlParameters
	{
		private HttpContext _httpContext;
		private Dictionary<string, object> _lstParameters;

		public static UrlParameters Current
		{
			get
			{
				UrlParameters instance = new UrlParameters();
				instance._httpContext = HttpContext.Current;
				return instance;
			}
		}

		private UrlParameters()
		{
			_lstParameters = new Dictionary<string, object>();
			_httpContext = HttpContext.Current;

			foreach (string key in _httpContext.Request.QueryString.AllKeys)
			{
				if (!_lstParameters.ContainsKey(key))
				{
					_lstParameters.Add(key, _httpContext.Request.QueryString[key]);
				}
			}
		}

		public void SetValue(string key, object value)
		{
			_lstParameters[key] = value;
		}

		public string GetValue(string key)
		{
			if (_lstParameters.ContainsKey(key) && _lstParameters[key] != null)
				return _lstParameters[key].ToString();

			return string.Empty;
		}

		public void Remove(string key)
		{
			if (_lstParameters.ContainsKey(key))
				_lstParameters.Remove(key);
		}

		public string ToUrlString()
		{
			List<string> lst = new List<string>();
			foreach (KeyValuePair<string, object> kv in _lstParameters)
			{
				if (kv.Value != null)
					lst.Add(string.Format("{0}={1}",
						_httpContext.Server.UrlEncode(kv.Key),
						_httpContext.Server.UrlEncode(kv.Value.ToString())
						));
			}

			return string.Join("&", lst.ToArray());
		}

		public string CombineWithCurrentUrl()
		{
			if (_lstParameters.Count > 0)
				return string.Format("{0}?{1}", _httpContext.Request.FilePath, ToUrlString());

			return _httpContext.Request.FilePath;
		}
	}
}
