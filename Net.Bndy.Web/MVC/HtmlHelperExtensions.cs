// ==========================================================================
// Copyright (c) 2013 http://www.bndy.net.
// Created by Bndy at 3/6/2013 09:22:17 AM
// --------------------------------------------------------------------------
// HtmlHelper Extensions for System.Web.Mvc.HtmlHelper
// ==========================================================================

using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Net.Bndy.Web.MVC
{
	public static class HtmlHelperExtensions
	{
		/// <summary>
		/// Displays a drop down list according to the specific Enum type.
		///		Each enum item should declare the <see cref="Net.Bndy.DisplayAttribute"/> attribute so that the exact information is been display in drop down list.
		/// </summary>
		/// <typeparam name="TEnum">The type of Enum</typeparam>
		/// <param name="htmlHelper">The instance of <see cref="System.Web.Mvc.HtmlHelper"/></param>
		/// <param name="defaultValue">Default value of drop down list</param>
		/// <param name="htmlAttributes">Html attributes of drop down list</param>
		/// <returns></returns>
		public static string DisplayEnum<TEnum>(this System.Web.Mvc.HtmlHelper htmlHelper,
			Nullable<TEnum> defaultValue = null,
			object htmlAttributes = null)
			where TEnum : struct
		{
			StringBuilder result = new StringBuilder();
			DisplayAttribute attr = null;
			List<string> lstAttr = new List<string>();

			if (htmlAttributes != null)
			{
				foreach (var item in htmlAttributes.GetType().GetProperties())
				{
					lstAttr.Add(string.Format("{0}=\"{1}\"", item.Name, item.GetValue(htmlAttributes, null)));
				}
			}

			result.AppendFormat("<select id=\"{0}\" name=\"{0}\" {1}>", typeof(TEnum), string.Join(" ", lstAttr.ToArray()));
			if (defaultValue == null)
				result.AppendFormat("<option value=\"\">Select one...</option>");

			foreach (var item in typeof(TEnum).GetFields(BindingFlags.Public | BindingFlags.Static))
			{
				object[] attrs = item.GetCustomAttributes(typeof(DisplayAttribute), false);
				string title = string.Empty;
				string description = string.Empty;
				string value = string.Empty;

				if (attrs != null && attrs.Count() > 0)
				{
					attr = (DisplayAttribute)attrs[0];
					title = attr.Title;
					description = attr.Description;
				}
				else
				{
					title = item.Name;
					description = item.Name;
				}
				value = ((int)item.GetValue(null)).ToString();

				result.AppendFormat("<option value=\"{0}\" title=\"{1}\"{3}>{2}</option>",
					value, description, title,
					defaultValue != null && defaultValue.Value.ToString() == item.Name ? " selected=\"selected\"" : string.Empty);

			}

			result.AppendFormat("</select>");

			return result.ToString();
		}
	}
}
