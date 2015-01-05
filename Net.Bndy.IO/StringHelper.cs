// =================================================================================
// Copyright (c) 2014 http://www.bndy.net.
// Created by Bndy at 5/28/2014 4:44:51 PM
// ---------------------------------------------------------------------------------
// Summary & Change Logs.
// =================================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Net.Bndy.IO
{
	public static class StringHelper
	{
		/// <summary>
		/// Saves the string to the destination file.
		/// </summary>
		/// <param name="str">The string.</param>
		/// <param name="saveAs">The destination file.</param>
		/// <param name="overwrite">True to overwrite the original file, otherwise to append.</param>
		/// <param name="encoding">The file encoding.</param>
		public static void SaveAs(this string str, string saveAs, bool overwrite = true, Encoding encoding = null)
		{
			var dir = Path.GetDirectoryName(saveAs);
			if (!Directory.Exists(dir))
				Directory.CreateDirectory(dir);
			if (encoding == null) encoding = Encoding.UTF8;
			if(File.Exists(saveAs) && !overwrite)
			{
				File.AppendAllText(saveAs, str, encoding);
			}
			else
			{
				File.WriteAllText(saveAs, str, encoding);
			}
		}
#if NET45
		/// <summary>
		/// Asynchronous to save the string to the destination file.
		/// </summary>
		/// <param name="str">The string.</param>
		/// <param name="saveAs">The destination file.</param>
		/// <param name="encoding">The encoding.</param>
		public static async void SaveAsAsync(this string str, string saveAs, Encoding encoding = null)
		{
			var dir = Path.GetDirectoryName(saveAs);
			if (!Directory.Exists(dir))
				Directory.CreateDirectory(dir);
			if (encoding == null) encoding = Encoding.UTF8;

			using (StreamWriter sw = new StreamWriter(saveAs, false, encoding))
			{
				await sw.WriteAsync(str);
			}
		}
#endif
	}
}
