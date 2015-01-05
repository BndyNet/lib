// =================================================================================
// Copyright (c) 2014 http://www.bndy.net
// Created by Bndy at 3/26/2014 15:13:06
// ---------------------------------------------------------------------------------
// Extensions of Dictionary
// =================================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Net.Bndy
{
	public static class DictionaryExtension
	{
		/// <summary>
		/// Converts to an instance of TResult.
		/// </summary>
		/// <typeparam name="TResult">The type of the attribute result.</typeparam>
		/// <param name="dict">The dictionary.</param>
		/// <returns>An instance of TResult.</returns>
		public static TResult ConvertTo<TResult>(this Dictionary<string, object> dict)
			where TResult : class
		{
			TResult result = Activator.CreateInstance<TResult>();

			foreach (KeyValuePair<string, object> item in dict)
			{
				var p = result.GetType().GetProperty(item.Key);
				if (p != null)
				{
					if (!p.PropertyType.IsClass)
						p.SetValue(result, item.Value, null);
					else
					{
						var d = item.Value as Dictionary<string, object>;
						if (d != null)
						{
							//TODO Convert the dict to nested property.
						}
					}
				}
			}

			return result;
		}
		/// <summary>
		/// Formats the keys and values to a string list.
		/// </summary>
		/// <param name="dict">The dictionary.</param>
		/// <param name="formatOfKeyValue">The format of key value pair.</param>
		/// <returns>List{System.String}.</returns>
		public static List<string> ToList(this Dictionary<string, object> dict, string formatOfKeyValue)
		{
			List<string> result = new List<string>();
			foreach (KeyValuePair<string, object> item in dict)
			{
				result.Add(string.Format(formatOfKeyValue, item.Key, item.Value));
			}
			return result;
		}
		/// <summary>
		/// Merges the specified dictionaries.
		/// </summary>
		/// <typeparam name="TKey">The type of the attribute key.</typeparam>
		/// <typeparam name="TValue">The type of the attribute value.</typeparam>
		/// <param name="dict">The dictionary.</param>
		/// <param name="dictionaries">The dictionaries.</param>
		/// <returns>The merged dictionary.</returns>
		public static Dictionary<TKey, TValue> Merge<TKey, TValue>(this Dictionary<TKey, TValue> dict, params Dictionary<TKey, TValue>[] dictionaries)
		{
			Dictionary<TKey, TValue> result = new Dictionary<TKey, TValue>();

			var lst = new List<Dictionary<TKey, TValue>>();
			lst.Add(dict);
			lst.AddRange(dictionaries);

			foreach (Dictionary<TKey, TValue> dic in lst)
			{
				foreach (KeyValuePair<TKey, TValue> kv in dic)
				{
					result[kv.Key] = kv.Value;
				}
			}

			return result;
		}
		/// <summary>
		/// Formats the specified dictionary.
		/// </summary>
		/// <param name="dict">The dictionary.</param>
		/// <param name="format">The format {[key]}.</param>
		/// <returns>System.String.</returns>
		public static string Format(this Dictionary<string, object> dict,
			string format)
		{
			if (dict == null) return "";

			var result = format;
			foreach (Match m in Regex.Matches(format, @"{(?<f>.*?)}"))
			{
				var f = m.Groups["f"].Value;

				result = result.Replace("{" + f + "}",
					dict[f] != null ? dict[f].ToString() : "");
			}
			return result;
		}

		/// <summary>
		/// Tries to get the value.
		/// </summary>
		/// <typeparam name="TKey">The type of the attribute key.</typeparam>
		/// <typeparam name="TValue">The type of the attribute value.</typeparam>
		/// <param name="dict">The dictionary.</param>
		/// <param name="key">The key.</param>
		/// <param name="defaultValue">The default value.</param>
		/// <returns></returns>
		public static TValue TryGet<TKey, TValue>(this IDictionary<TKey, TValue> dict,
			TKey key, TValue defaultValue = default(TValue))
		{
			TValue result = defaultValue;

			if (dict.ContainsKey(key))
				return dict[key];

			return result;
		}
	}
}
