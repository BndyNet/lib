// =================================================================================
// Copyright (c) 2012 http://www.bndy.net.
// Created by Bndy at 12/8/2012 11:54:02 AM
// ---------------------------------------------------------------------------------
// Summary & Change Logs.
// =================================================================================

using System.IO;

namespace Net.Bndy.IO
{
	public static class FileHelper
	{
		public static string[] ImageExtensionNames = new string[] { ".jpg", ".jpeg", ".ico", ".png", ".gif", ".bmp", ".tif" };

		/// <summary>
		/// Copies files to destination directory.
		/// </summary>
		/// <param name="directoryInfo">The directory information.</param>
		/// <param name="destDirectory">The dest directory.</param>
		/// <param name="searchPattern">The search pattern. If not specified, to copy all files.</param>
		/// <param name="overwrite">if set to <c>true</c> [overwrite].</param>
		public static void CopyFilesTo(this DirectoryInfo directoryInfo,
			string destDirectory, string searchPattern = null, bool overwrite = false)
		{
			if (searchPattern == null)
				searchPattern = "*.*";

			foreach (FileInfo file in directoryInfo.GetFiles(searchPattern, SearchOption.AllDirectories))
			{
				var relativeName = file.FullName.Replace(directoryInfo.FullName + "\\", "");
				var destName = Path.Combine(destDirectory, relativeName);

				FileInfo fi = new FileInfo(destName);
				if (!fi.Directory.Exists)
					fi.Directory.Create();

				file.CopyTo(destName, overwrite);
			}
		}

		/// <summary>
		/// Determines whether the specified file name is an image file.
		/// </summary>
		/// <param name="fileName">Name of the file.</param>
		/// <returns><c>true</c> if the specified file name is an image file; otherwise, <c>false</c>.</returns>
		public static bool IsImageFile(string fileName)
		{
			foreach (var ext in ImageExtensionNames)
			{
				if (fileName.ToLower().IndexOf(ext) > 0)
				{
					return true;
				}
			}
			return false;
		}
	}
}
