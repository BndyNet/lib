// =================================================================================
// Copyright (c) 2014 http://www.bndy.net
// Created by Bndy at 7/15/2014 2:29:07 PM
// ---------------------------------------------------------------------------------
// Summary & Change Logs.
// =================================================================================

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Net.Bndy
{
	public static class CollectionExtension
	{
		/// <summary>
		/// Formats the items.
		/// </summary>
		/// <param name="source">The source.</param>
		/// <param name="itemFormat">The item format. 
		/// @#: get the Index begins with 1,
		///	@PropertyName: get the public property value.
		///	If the item is an Array like int[], string[], etc.
		///		Please using {0} to format current item.
		/// </param>
		/// <returns>System.String.</returns>
		public static string FormatItems(this IEnumerable source, string itemFormat)
		{
			StringBuilder sb = new StringBuilder();

			int idx = 0;
			IEnumerator enumerator = source.GetEnumerator();
			while (enumerator.MoveNext())
			{
				idx++;
				string item = itemFormat.Replace("@#", idx.ToString());
				object obj = enumerator.Current;
				if (obj.GetType().IsArray)
				{
					List<object> lst = new List<object>();
					foreach (object o in (obj as Array))
					{
						lst.Add(o);
					}
					item = string.Format(item, lst.ToArray());
				}

				else if (obj.GetType().IsClass)
				{
					foreach (PropertyInfo pi in obj.GetType().GetProperties())
					{
						item = item.Replace(
							string.Format("@{0}", pi.Name),
							(pi.GetValue(obj, null) ?? "").ToString());
					}
				}

				sb.Append(item);
			}

			return sb.ToString();
		}
	}
}
