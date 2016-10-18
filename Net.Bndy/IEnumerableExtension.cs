// =================================================================================
// Copyright (c) 2013 http://www.bndy.net
// Created by Bndy at 3/28/2013 16:25:46
// ---------------------------------------------------------------------------------
// Extensions of IEnumerable
// =================================================================================

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Net.Bndy
{
	public static class IEnumerableExtension
	{
		/// <summary>
		/// Converts the list to tree.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="it">It.</param>
		/// <param name="root">The root.</param>
		/// <param name="setChildren">The set children.</param>
		/// <returns>IEnumerable&lt;T&gt;.</returns>
		public static IEnumerable<T> ToTree<T>(this IEnumerable<T> it,IEnumerable<T> root, Func<T, IEnumerable<T>> setChildren)
			where T: class
		{
			foreach(var item in root)
			{
				ToTree(it, setChildren(item), setChildren);
			}
			return root;
		}
	}
}
