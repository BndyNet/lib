// =================================================================================
// Copyright (c) 2014 http://www.bndy.net
// Created by Bndy at 3/26/2014 15:00:37
// ---------------------------------------------------------------------------------
// Extensions of Object
// =================================================================================

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Net.Bndy
{
	/// <summary>
	/// Extensions of Object.
	/// </summary>
	public static class ObjectExtension
	{
		/// <summary>
		/// Gets dictionary instance that includes all property names and values.
		/// </summary>
		/// <param name="obj">The object.</param>
		/// <returns>Dictionary{System.String System.String}.</returns>
		public static Dictionary<string, string> ToDict(this object obj)
		{
			if (obj == null) return null;

			if (obj is Dictionary<string, string>)
				return obj as Dictionary<string, string>;

			var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
			var properties = obj.GetType().GetProperties();
			foreach (var p in properties)
			{
				var value = p.GetValue(obj, null);
				dict.Add(p.Name, value == null ? null : value.ToString());
			}
			return dict;
		}

		/// <summary>
		/// Gets dictionary instance that includes the property names and values which is not class.
		/// </summary>
		/// <param name="obj">The object.</param>
		/// <returns>Dictionary{System.StringSystem.Object}.</returns>
		public static Dictionary<string, object> GetNativePropertiesDict(this object obj)
		{
			var dict = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
			var properties = obj.GetType().GetProperties(BindingFlags.Public);
			foreach (var p in properties)
			{
				if (!p.PropertyType.IsClass)
					dict.Add(p.Name, p.GetValue(obj, null));
			}
			return dict; ;
		}

		/// <summary>
		/// Formats the object to string.
		/// </summary>
		/// <param name="obj">The object.</param>
		/// <param name="format">The format. Use @PropertyName to cite the value</param>
		/// <returns>System.String.</returns>
		public static string Format2String(this object obj, string format)
		{
			string result = null;

			if (format != null)
			{
				result = format;
				foreach (PropertyInfo pi in obj.GetType().GetProperties())
				{
					result = result.Replace(
						string.Format("@{0}", pi.Name),
						string.Format("{0}", pi.GetValue(obj, null))
						);
				}
			}

			return result;
		}

		/// <summary>
		/// Unions the properties with the specified separator.
		/// </summary>
		/// <param name="obj">The object.</param>
		/// <param name="separator">The separator.</param>
		/// <returns>System.String.</returns>
		public static string UnionPropertiesWith(this object obj, string separator)
		{
			if (obj == null)
				return null;

			List<string> lst=new List<string> ();
			foreach(var kv in obj.ToDict())
			{
				lst.Add(string.Format("{0}={1}", kv.Key, kv.Value));
			}

			return string.Join(separator, lst.ToArray());
		}
	}
}
