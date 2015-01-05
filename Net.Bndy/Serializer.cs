// =================================================================================
// Copyright (c) 2013 http://www.bndy.net
// Created by Bndy at 10/8/2013 09:04:53
// ---------------------------------------------------------------------------------
// 
// =================================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Net.Bndy
{
	public class Serializer
	{
		/// <summary>
		/// Converts an object to xml string.
		/// </summary>
		/// <param name="obj">The object.</param>
		/// <returns>System.String.</returns>
		public static string ToXmlString(object obj)
		{
			List<string> lst = new List<string>();
			string typeName = obj.GetType().Name;

			lst.Add(string.Format("<{0}>", typeName));

			foreach (PropertyInfo pi in obj.GetType().GetProperties())
			{
				string name = pi.Name;
				object value = pi.GetValue(obj, null);

				if (value.GetType().IsValueType || value.GetType() == typeof(string))
				{
					lst.Add(string.Format("<{0}>{1}</{0}>", name, value));
				}
				else
				{
					lst.Add(ToXmlString(value));
				}
			}
			lst.Add(string.Format("</{0}>", typeName));

			return string.Join(Environment.NewLine, lst.ToArray());
		}

		/// <summary>
		/// Converts an object to Json format string.
		/// </summary>
		/// <param name="obj">The object.</param>
		/// <returns>System.String.</returns>
		public static string ToJson(object obj)
		{
			List<string> lst = new List<string>();
			string typeName = obj.GetType().Name;

			lst.Add(string.Format("{{\"{0}\": {{", typeName));

			foreach (PropertyInfo pi in obj.GetType().GetProperties())
			{
				string name = pi.Name;
				object value = pi.GetValue(obj, null);

				if (value.GetType().IsValueType || value.GetType() == typeof(string))
				{
					lst.Add(string.Format("{{\"{0}\": \"{1}\"}}", name, value));
				}
				else
				{
					lst.Add(ToXmlString(value));
				}
			}
			lst.Add("}");

			return string.Join(Environment.NewLine, lst.ToArray());
		}
	}
}
