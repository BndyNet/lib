//------------------------------------------------------------------------------
// <copyright file="CSSqlFunction.cs" company="Net.Bndy.Inc">
//     Copyright (c) Net.Bndy Corporation.  All rights reserved. 
//     Created by Bndy
// </copyright>
//------------------------------------------------------------------------------
using System;
using System.Xml;
using System.Xml.Xsl;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.SqlServer.Server;
using System.Xml.XPath;
using System.Collections;
using System.Security.Cryptography;

public partial class UserDefinedFunctions
{
	#region Scalar-Valued Functions
	[SqlFunction]
	public static SqlChars _RegexReplace(SqlChars input, string pattern, string replacement)
	{
		return new SqlChars(Regex.Replace(new string(input.Value), pattern, replacement).ToCharArray());
	}

	[SqlFunction]
	public static SqlChars _RegexMatch(SqlChars input, string pattern, string groupName)
	{
		string result = string.Empty;
		string source = new string(input.Value);

		if (string.IsNullOrEmpty(groupName))
			result = Regex.Match(source, pattern, RegexOptions.None).Value;
		else
		{
			MatchCollection matches = Regex.Matches(source, pattern, RegexOptions.None);
			if (matches.Count > 0)
				result = matches[0].Groups[groupName].Value;
		}

		return new SqlChars(result.ToCharArray());
	}

	[SqlFunction]
	public static string _RegexParse(SqlChars input, string pattern)
	{
		string text = new string(input.Value);
		Regex reg = new Regex(pattern);
		string[] groupNames = reg.GetGroupNames();
		List<string> lstRow = new List<string>();
		foreach (Match m in reg.Matches(text))
		{
			lstRow.Add("<row>");
			foreach (string gName in groupNames)
			{
				string key = gName;
				string value = m.Groups[key].Value;
				lstRow.Add(string.Format("<{0}>{1}</{0}>", key, value));
			}
			lstRow.Add("</row>");
		}

		return string.Join(Environment.NewLine, lstRow.ToArray());
	}

	[SqlFunction]
	public static SqlChars _RegexParseFromWeb(string url, string pattern)
	{
		WebRequest request = HttpWebRequest.Create(url);

		using (StreamReader reader = new StreamReader(request.GetResponse().GetResponseStream()))
		{
			string s = reader.ReadToEnd();

			return new SqlChars(Regex.Match(s, pattern).Value.ToCharArray());
		}
	}

	[SqlFunction]
	public static SqlChars _RegexParseFromFile(string file, string pattern, string groupName)
	{
		string text = File.ReadAllText(file);

		return _RegexMatch(new SqlChars(text.ToCharArray()), pattern, groupName);
	}
	[SqlFunction]
	public static SqlInt32 _Find(SqlChars input, SqlChars pattern, SqlBoolean ignoreCase)
	{
		if (pattern.IsNull || input.IsNull)
			return SqlInt32.Null;

		string p = new string(pattern.Value);
		bool ignore = ignoreCase.IsTrue;

		return new SqlInt32(Regex.Matches(new string(input.Value), p,
			ignore ? RegexOptions.IgnoreCase : RegexOptions.None).Count);
	}
	[SqlFunction]
	public static string _WebDownload(string url, string saveAs)
	{
		try
		{
			WebRequest request = HttpWebRequest.Create(url);
			WebResponse response = request.GetResponse();

			string destFileName = null;
			FileInfo fi = new FileInfo(saveAs);
			if (string.IsNullOrEmpty(fi.Extension))
			{
				destFileName = Path.Combine(saveAs, Regex.Replace(Path.GetFileName(url), @"[^\.\w]", "-"));
			}
			else
			{
				destFileName = saveAs;
			}

			fi = new FileInfo(destFileName);
			if (!fi.Directory.Exists) fi.Directory.Create();

			using (FileStream fs = File.Create(destFileName))
			using (Stream s = response.GetResponseStream())
			{
				byte[] buffer = new byte[1024];
				int count = s.Read(buffer, 0, 1024);
				while (count > 0)
				{
					fs.Write(buffer, 0, count);
					count = s.Read(buffer, 0, 1024);
				}
				s.Close();
				fs.Close();
			}

			return destFileName;
		}
		catch
		{
			return string.Empty;
		}
	}
	[SqlFunction]
	public static string _WebDownloadWithProxy(string url, string saveAs, string proxy)
	{
		try
		{
			WebRequest request = HttpWebRequest.Create(url);
			request.Proxy = new WebProxy(proxy);
			WebResponse response = request.GetResponse();

			string destFileName = null;
			FileInfo fi = new FileInfo(saveAs);
			if (string.IsNullOrEmpty(fi.Extension))
			{
				destFileName = Path.Combine(saveAs, Regex.Replace(Path.GetFileName(url), @"[^\.\w]", "-"));
			}
			else
			{
				destFileName = saveAs;
			}

			fi = new FileInfo(destFileName);
			if (!fi.Directory.Exists) fi.Directory.Create();

			using (FileStream fs = File.Create(destFileName))
			using (Stream s = response.GetResponseStream())
			{
				byte[] buffer = new byte[1024];
				int count = s.Read(buffer, 0, 1024);
				while (count > 0)
				{
					fs.Write(buffer, 0, count);
					count = s.Read(buffer, 0, 1024);
				}
				s.Close();
				fs.Close();
			}

			return destFileName;
		}
		catch
		{
			return string.Empty;
		}
	}
	[SqlFunction]
	public static SqlChars _Download(SqlChars html, string urlPattern, string urlFormat, string localPath)
	{
		List<string> result = new List<string>();
		if (html != SqlChars.Null)
		{
			string h = new string(html.Value);
			Regex reg = new Regex(urlPattern, RegexOptions.IgnoreCase);
			foreach (Match m in reg.Matches(h))
			{
				foreach (string gName in reg.GetGroupNames())
				{
					string url = null;
					string fileName = null;

					try
					{
						if (gName.StartsWith("url", StringComparison.OrdinalIgnoreCase))
						{
							url = m.Groups[gName].Value;
							fileName = url.Replace("/", "_").Replace("\\", "_");
						}
						if (!string.IsNullOrEmpty(url) && !string.IsNullOrEmpty(urlFormat))
						{
							url = string.Format(urlFormat, url);
						}

						if (!string.IsNullOrEmpty(url))
						{
							// donwload file
							DirectoryInfo di = new DirectoryInfo(localPath);
							if (!di.Exists)
							{
								di.Create();
							}
							string destFile = Path.Combine(di.FullName, fileName);

							if (!File.Exists(destFile))
							{
								_WebDownload(url, destFile);
								result.Add(string.Format("{0} ====> {1}, {2}", url, "OK", destFile));
							}
							else
								result.Add(string.Format("{0} ====> {1}, {2}", url, "Exists", destFile));
						}
					}
					catch (Exception ex)
					{
						result.Add(string.Format("{0} ====> Error: {1}", url, ex.Message));
					}
				}
			}
		}

		return new SqlChars(string.Join(Environment.NewLine, result.ToArray()).ToCharArray());
	}

	[SqlFunction]
	public static SqlChars _StringSplit(SqlChars input, string separator, int index)
	{
		string[] str = new string(input.Value).Split(new string[] { separator }, StringSplitOptions.None);

		if (index < 0)
			return new SqlChars(string.Join(",", str).ToCharArray());
		else if (str.Length > index)
			return new SqlChars(str[index].ToCharArray());

		return SqlChars.Null;
	}

	[SqlFunction]
	public static SqlChars _WebSourceCode(string url)
	{
		WebRequest request = HttpWebRequest.Create(url);

		using (StreamReader reader = new StreamReader(request.GetResponse().GetResponseStream()))
		{
			string s = reader.ReadToEnd();
			return new SqlChars(s.ToCharArray());
		}
	}

	[SqlFunction]
	public static SqlChars _StripHtmlTags(SqlChars input)
	{
		return _RegexReplace(input, @"<.*?>", string.Empty);
	}

	[SqlFunction]
	public static SqlBoolean _IsMatch(string input, string pattern)
	{
		if (Regex.IsMatch(input, pattern))
		{
			return SqlBoolean.True;
		}

		return SqlBoolean.False;
	}

	[SqlFunction]
	public static SqlChars _Iff(SqlChars string1, SqlChars string2, SqlChars trueValue, SqlChars falseValue)
	{
		if (string.Equals(string1, string2))
			return trueValue;
		else
			return falseValue;
	}

	[SqlFunction]
	public static SqlInt64 _LastIndexOf(string input, string value)
	{
		int i = input.LastIndexOf(value);

		return new SqlInt64(i);
	}

	[SqlFunction]
	public static SqlChars _CombineUrl(SqlChars baseUrl, SqlChars relativeUrl)
	{
		if (!string.IsNullOrEmpty(new string(baseUrl.Value)))
		{
			Uri u = new Uri(new Uri(new string(baseUrl.Value)), new string(relativeUrl.Value));
			return new SqlChars(u.ToString().ToCharArray());
		}
		return relativeUrl;
	}
	[SqlFunction]
	public static SqlChars _FillUrlAndAppendBlankTargetToATag(SqlChars html, SqlChars baseUrl)
	{
		string content = new string(html.Value);
		Regex reg = new Regex(@"href=\s*['""](?<url>.*?['""\s])", RegexOptions.IgnoreCase);
		if (!string.IsNullOrEmpty(content))
		{
			content = reg.Replace(content, (Match m) =>
			{
				if (m != null)
				{
					string oldUrl = m.Groups["url"].Value;
					string newUrl = new string(_CombineUrl(baseUrl, new SqlChars(oldUrl.ToCharArray())).Value);

					return m.Value.Replace(oldUrl, newUrl) + @" target=""_blank""";
				}
				return "";
			});
		}

		return new SqlChars(content.ToCharArray());
	}

	#region IO Operations
	[SqlFunction]
	public static SqlChars _MoveFile(SqlChars sourceFile, SqlChars targetDirectory)
	{
		string srcFile = new string(sourceFile.Value);
		string tgtDir = new string(targetDirectory.Value);

		if (File.Exists(srcFile))
		{
			if (!Directory.Exists(tgtDir)) Directory.CreateDirectory(tgtDir);
			string destFile = Path.Combine(tgtDir, new FileInfo(srcFile).Name);
			File.Move(srcFile, destFile);
			return new SqlChars(destFile.ToCharArray());
		}
		else
		{
			return SqlChars.Null;
		}
	}
	[SqlFunction]
	public static SqlChars _MoveFiles(SqlChars sourceDirectory, SqlChars targetDirectory)
	{
		List<string> files = new List<string>();

		string sourceDir = new string(sourceDirectory.Value);
		string targetDir = new string(targetDirectory.Value);

		if (Directory.Exists(sourceDir))
		{
			if (!Directory.Exists(targetDir))
			{
				Directory.CreateDirectory(targetDir);
			}

			foreach (string fileName in Directory.GetFiles(sourceDir))
			{
				string targetFile = Path.GetFileName(fileName);
				targetFile = Path.Combine(targetDir, targetFile);

				File.Move(fileName, targetFile);
				files.Add(targetFile);
			}

			foreach (string dirName in Directory.GetDirectories(sourceDir))
			{
				string targetDirName = new DirectoryInfo(dirName).Name;
				SqlChars r = _MoveFiles(
					new SqlChars(dirName.ToCharArray()),
					new SqlChars(Path.Combine(sourceDir, targetDirName).ToCharArray())
					);

				files.AddRange(
					new string(r.Value).Split(
						new string[] { Environment.NewLine },
						StringSplitOptions.RemoveEmptyEntries));
			}

			// delete empty directories
			if (Directory.GetDirectories(sourceDir).Length == 0
				&& Directory.GetFiles(sourceDir).Length == 0)
			{
				Directory.Delete(sourceDir);
			}
		}

		return new SqlChars(string.Join(Environment.NewLine, files.ToArray()).ToCharArray());
	}
	[SqlFunction]
	public static string _Save2File(SqlChars text, string destinationFile)
	{
		string content = new string(text.Value);
		FileInfo fi = new FileInfo(destinationFile);
		if (!Directory.Exists(fi.Directory.FullName))
			Directory.CreateDirectory(fi.Directory.FullName);

		File.AppendAllText(destinationFile, content, Encoding.UTF8);

		return destinationFile;

	}
	[SqlFunction]
	public static SqlChars _ReadFile(string file)
	{
		string content = string.Empty;
		if (File.Exists(file))
		{
			content = File.ReadAllText(file);
		}
		return new SqlChars(content.ToCharArray());
	}
	[SqlFunction]
	public static SqlBoolean _FileExists(SqlChars fileFullName)
	{
		string file = new string(fileFullName.Value);
		if (File.Exists(file))
			return SqlBoolean.True;
		else
			return SqlBoolean.False;
	}
	[SqlFunction]
	public static string _RenameFile(string sourceFile, string newName)
	{
		FileInfo fi = new FileInfo(sourceFile);
		if (fi.Exists)
		{
			string targetFile = Path.Combine(fi.Directory.FullName, newName);

			if (string.IsNullOrEmpty(new FileInfo(targetFile).Extension))
			{
				targetFile += fi.Extension;
			}

			fi.MoveTo(targetFile);

			return targetFile;
		}

		return string.Empty;
	}
	[SqlFunction]
	public static string _TransformXmlFile(string xmlFile, string xslFile, string resultFile)
	{
		string result = null;
		if (File.Exists(xmlFile) && File.Exists(xslFile))
		{
			XslCompiledTransform xsl = new XslCompiledTransform();
			xsl.Load(xslFile);
			xsl.Transform(xmlFile, resultFile);
			if (File.Exists(resultFile))
				result = resultFile;
		}

		return result;
	}
	[SqlFunction]
	public static string _TransformXmlContent(SqlChars xmlContent, string xslFile, string resultFile)
	{
		string result = null;
		if (xmlContent != SqlChars.Null)
		{
			XslCompiledTransform xsl = new XslCompiledTransform();
			xsl.Load(xslFile);

			XmlDocument xml = new XmlDocument();
			xml.LoadXml(new string(xmlContent.Value).Replace("&", "&amp;"));

			XPathNavigator xmlNav = xml.CreateNavigator();
			using (XmlWriter w = XmlWriter.Create(resultFile))
			{
				xsl.Transform(xmlNav, w);
				w.Close();
			}

			result = resultFile;
		}

		return result;
	}
	[SqlFunction]
	public static SqlChars _ReadXmlFile(string xmlFile, string xslFile)
	{
		SqlChars result = SqlChars.Null;
		if (File.Exists(xmlFile) && File.Exists(xslFile))
		{
			XslCompiledTransform xsl = new XslCompiledTransform();
			xsl.Load(xslFile);

			StringBuilder sb = new StringBuilder();
			using (XmlWriter writer = XmlWriter.Create(sb, xsl.OutputSettings))
			{
				xsl.Transform(xmlFile, writer);
			}

			if (sb.Length > 0)
				result = new SqlChars(sb.ToString().ToCharArray());
		}

		return result;
	}
	[SqlFunction]
	public static SqlChars _ReplaceFileContent(SqlChars fileFullName, SqlChars pattern, SqlChars replacement,
		SqlChars replacementParam1, SqlChars replacementParam2, SqlChars replacementParam3)
	{
		try
		{
			string file = new string(fileFullName.Value);
			string p1 = new string(pattern.Value);
			string p2 = new string(replacement.Value);

			p2 = string.Format(p2,
				new string(replacementParam1.IsNull ? "".ToCharArray() : replacementParam1.Value),
				new string(replacementParam2.IsNull ? "".ToCharArray() : replacementParam2.Value),
				new string(replacementParam3.IsNull ? "".ToCharArray() : replacementParam3.Value)
				);
			if (File.Exists(file))
			{
				string content = File.ReadAllText(file);
				Regex reg = new Regex(p1);
				if (reg.IsMatch(content))
				{
					content = reg.Replace(content, p2);
					File.WriteAllText(file, content);
					return new SqlChars(content.ToCharArray());
				}
				return new SqlChars("".ToCharArray());
			}

			return new SqlChars("! Error - No Files Found".ToCharArray());
		}
		catch (Exception ex)
		{
			return new SqlChars(ex.Message + ex.StackTrace);
		}
	}
	#endregion

	[SqlFunction]
	public static SqlDateTime _ConvertToDateTime(SqlChars input)
	{
		DateTime dt = DateTime.MinValue;
		if (DateTime.TryParse(new string(input.Value), out dt))
		{
			return new SqlDateTime(dt);
		}
		return SqlDateTime.Null;
	}
	[SqlFunction]
	public static SqlChars _MD5(SqlChars input)
	{
		if (input != SqlChars.Null)
		{
			string s = new string(input.Value);
			MD5CryptoServiceProvider provider = new MD5CryptoServiceProvider();

			byte[] hash = null;
			string result = null;
			if (!File.Exists(s))
			{
				hash = provider.ComputeHash(System.Text.Encoding.UTF8.GetBytes(s));
			}
			else
			{
				using (FileStream fs = File.Open(s, FileMode.Open))
				{
					hash = provider.ComputeHash(fs);
				}
			}
			result = Convert.ToBase64String(hash);
			return new SqlChars(result.ToCharArray());
		}

		return SqlChars.Null;
	}
	[SqlFunction]
	public static SqlChars _SHA1(SqlChars input)
	{
		if (input != SqlChars.Null)
		{
			string s = new string(input.Value);
			SHA1CryptoServiceProvider provider = new SHA1CryptoServiceProvider();

			byte[] hash = null;
			string result = null;
			if (!File.Exists(s))
			{
				hash = provider.ComputeHash(System.Text.Encoding.UTF8.GetBytes(s));
			}
			else
			{
				using (FileStream fs = File.Open(s, FileMode.Open))
				{
					hash = provider.ComputeHash(fs);
				}
			}
			result = Convert.ToBase64String(hash);
			return new SqlChars(result.ToCharArray());
		}

		return SqlChars.Null;

	}
	#endregion Scalar-Valued Functions
	public class RegexGroupMatch
	{
		public int RowId { get; set; }
		public string Key { get; set; }
		public string Value { get; set; }
	}

	#region Table-Valued Functions
	[SqlFunction(FillRowMethodName = "FillRowForRegexMatched",
	TableDefinition = "[RowId] INT, [Key] NVARCHAR(MAX), [Value] NVARCHAR(MAX)")]
	public static IEnumerable _RegexMatches(SqlChars input, string pattern)
	{
		ArrayList result = new ArrayList();
		Regex re = new Regex(pattern, RegexOptions.IgnoreCase);
		int index = 0;
		foreach (Match m in re.Matches(new string(input.Value)))
		{
			foreach (string gName in re.GetGroupNames())
			{
				if (gName == "0") continue;
				RegexGroupMatch gItem = new RegexGroupMatch();
				gItem.RowId = index;
				gItem.Key = gName;
				gItem.Value = m.Groups[gName].Value;
				result.Add(gItem);
			}
			index++;
		}

		return result;
	}
	public static void FillRowForRegexMatched(object rowObject,
		out SqlInt32 RowId,
		out SqlChars Key,
		out SqlChars Value)
	{
		RegexGroupMatch groupMatch = rowObject as RegexGroupMatch;
		RowId = groupMatch.RowId;
		Key = new SqlChars(groupMatch.Key);
		Value = new SqlChars(groupMatch.Value);
	}
	#endregion Table-Valued Functions
}
