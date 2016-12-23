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
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Net.Bndy.Web.MVC
{
    public static class HtmlHelperExtensions
    {
        /// <summary>
        /// Displays the enum.
        /// </summary>
        /// <param name="htmlHelper">The HTML helper.</param>
        /// <param name="enumItem">The enum item.</param>
        /// <returns>System.String.</returns>
        public static HtmlString DisplayEnum(this System.Web.Mvc.HtmlHelper htmlHelper, object enumItem)
        {
            foreach (var item in enumItem.GetType().GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                if ((int)(item.GetValue(null)) == (int)enumItem)
                {
                    var displayAttr = (DisplayAttribute)item.GetCustomAttribute(typeof(DisplayAttribute));
                    var label = displayAttr != null ? displayAttr.Name : enumItem.ToString();
                    return new HtmlString(label);
                }
            }
            return new HtmlString(enumItem.ToString());
        }


        /// <summary>
        /// Renders a drop down list for Enum type.
        /// </summary>
        /// <param name="htmlHelper">The HTML helper.</param>
        /// <param name="itemOrType">The enum type or item.</param>
        /// <param name="htmlAttributes">The HTML attributes.</param>
        /// <param name="caption">The caption.</param>
        /// <returns>HtmlString.</returns>
        public static HtmlString DropDownEnum(this System.Web.Mvc.HtmlHelper htmlHelper, object itemOrType,
            object htmlAttributes = null, string caption = null)
        {
            var attrs = new List<string>();
            if (htmlAttributes != null)
            {
                foreach (var p in htmlAttributes.GetType().GetProperties())
                {
                    attrs.Add(string.Format(@"{0}=""{1}""", p.Name, p.GetValue(htmlAttributes)));
                }
            }

            var html = new StringBuilder();
            html.AppendFormat(@"<select {0}>", string.Join(" ", attrs));

            if (!string.IsNullOrWhiteSpace(caption))
            {
                html.AppendFormat(@"<option value="""" selected>{0}</option>", caption);
            }

            var enumType = itemOrType.GetType().IsValueType ? itemOrType.GetType() : (Type)itemOrType;
            int? selectedValue = null;
            if (itemOrType.GetType().IsValueType)
            {
                selectedValue = (int)itemOrType;
            }
            foreach (var item in enumType.GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                var displayAttr = (DisplayAttribute)(item.GetCustomAttribute(typeof(DisplayAttribute), false));
                var val = (int)item.GetValue(null);
                html.AppendFormat(@"<option value=""{0}"" {2}>{1}</option>", val, displayAttr != null ? displayAttr.Name : item.Name, val == selectedValue ? "selected" : "");
            }
            html.AppendFormat("</select>");

            return new HtmlString(html.ToString());
        }
    }
}
