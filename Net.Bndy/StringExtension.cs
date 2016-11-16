// =================================================================================
// Copyright (c) 2013 http://www.bndy.net
// Created by Bndy at 3/28/2013 16:25:46
// ---------------------------------------------------------------------------------
// Extensions of String
// =================================================================================

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Net.Bndy
{
	public static class StringExtension
	{

		/// <summary>
		/// Deserializes the specified string.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="str">The string.</param>
		/// <returns>T.</returns>
		public static T Deserialize<T>(this string str)
		{
			return JsonConvert.DeserializeObject<T>(str);
		}

		/// <summary>
		/// Determines whether the specified string is email.
		/// </summary>
		/// <param name="str">The string.</param>
		/// <returns><c>true</c> if the specified string is email; otherwise, <c>false</c>.</returns>
		public static bool IsEmail(this string str)
		{
			var pattern = @"^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$";
			return Regex.IsMatch(str, pattern);
		}
		/// <summary>
		/// Determines whether the specified string is URL.
		/// </summary>
		/// <param name="str">The string.</param>
		/// <returns><c>true</c> if the specified string is URL; otherwise, <c>false</c>.</returns>
		public static bool IsUrl(this string str)
		{
			var pattern = @"[a-zA-z]+://[^\s]*";
			return Regex.IsMatch(str, pattern);
		}
		/// <summary>
		/// Determines whether the specified string is ip.
		/// </summary>
		/// <param name="str">The string.</param>
		/// <returns><c>true</c> if the specified string is ip; otherwise, <c>false</c>.</returns>
		public static bool IsIP(this string str)
		{
			var pattern = @"d+\.\d+\.\d+\.\d+";
			return Regex.IsMatch(str, pattern);
		}
		/// <summary>
		/// Determines whether this string is date.
		/// Example: 1900-1-1 or 1/1/1900
		/// </summary>
		/// <param name="str">The string.</param>
		/// <returns><c>true</c> if is date, otherwise, <c>false</c>.</returns>
		public static bool IsDate(this string str)
		{
			var pattern = @"^\d{4}-\d{1,2}-\d{1,2}|\d{1,2}/\d{1,2}/\d{4}$";
			return Regex.IsMatch(str, pattern);
		}
		/// <summary>
		/// Converts to the specified type.
		/// </summary>
		/// <typeparam name="TResult">The type of result which is value type.</typeparam>
		/// <param name="str">The string.</param>
		/// <param name="defaultValue">The default value.</param>
		/// <returns>The defaultValue, if errors occurred.</returns>
		public static TResult ConvertTo<TResult>(this string str, TResult defaultValue)
			where TResult : struct
		{
			try
			{
				var value = Convert.ChangeType(str, typeof(TResult));
				return (TResult)value;
			}
			catch
			{
				return defaultValue;
			}
		}
		/// <summary>
		/// Determines whether the specified string is Null or White Space.
		/// </summary>
		/// <param name="str">The string.</param>
		/// <returns><c>true</c> if Null or White Space; otherwise, <c>false</c>.</returns>
		public static bool IsNullOrWhiteSpace(this string str)
		{
			return string.IsNullOrWhiteSpace(str);
		}
		/// <summary>
		/// Parse the string using regex expression.
		/// </summary>
		/// <param name="str">The string.</param>
		/// <param name="pattern">The pattern.</param>
		/// <returns>List{Dictionary{System.StringSystem.String}}.</returns>
		public static List<Dictionary<string, string>> RegexMatches(this string str, string pattern)
		{
			List<Dictionary<string, string>> result = new List<Dictionary<string, string>>();

			Regex reg = new Regex(pattern);
			foreach (Match m in reg.Matches(str))
			{
				Dictionary<string, string> dic = new Dictionary<string, string>();
				foreach (string groupName in reg.GetGroupNames())
				{
					dic.Add(groupName, m.Groups[groupName].Value);

				}
				result.Add(dic);
			}


			return result;
		}
		/// <summary>
		/// Tabs the specified string.
		/// </summary>
		/// <param name="str">The string.</param>
		/// <param name="indents">The amount of indents. like 2 or -2</param>
		/// <returns>System.String.</returns>
		public static string Tab(this string str, int indents)
		{
			if (indents == 0) return str;

			var amount = Math.Abs(indents);

			if (indents > 0)
				return Regex.Replace(str, @"(?<line>\r\n|\r|\n|^)",
					string.Format("${{line}}{0}", "".PadLeft(amount, '\t')));
			else
				return Regex.Replace(str, @"(?<line>\r\n|\r|\n|^)( {" + amount * 4 + "}|\t{" + amount + "})",
					@"${line}");
		}
		/// <summary>
		/// Formats the specified format.
		/// </summary>
		/// <param name="format">The format.</param>
		/// <param name="args">The arguments. The first argument cannot be the string type.</param>
		/// <returns>System.String.</returns>
		public static string Format(this string format, params object[] args)
		{
			return string.Format(format, args.ToArray());
		}

		/// <summary>
		/// Gets the PinYin
		/// </summary>
		/// <param name="str">The Chinese Characters.</param>
		/// <returns>PinYin</returns>
		public static string GetPinYin(this string str)
		{
			return PinYinConverter.Get(str);
		}

		/// <summary>
		/// Gets the first letter of PinYin
		/// </summary>
		/// <param name="str">The Chinese Characters</param>
		/// <returns>Letters</returns>
		public static string GetFirstLetterOfPinYin(this string str)
		{
			return PinYinConverter.GetFirst(str);
		}
	}
}
