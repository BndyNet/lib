// =================================================================================
// Copyright (c) 2014 http://www.bndy.net.
// Created by Bndy at 5/28/2014 2:56:45 PM
// ---------------------------------------------------------------------------------
// Helper class for Http
// =================================================================================

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace Net.Bndy.Web
{
	public class WebHelper
	{
		/// <summary>
		/// Gets the html using the specified URI.
		/// </summary>
		/// <param name="uri">The URI.</param>
		/// <param name="encoding">The encoding. Default value: utf-8</param>
		/// <param name="proxyHost">The proxy host.</param>
		/// <param name="proxyPort">The proxy port.</param>
		/// <returns>The html string.</returns>
		public static string Get(ref string uri, string encoding = "utf-8", string proxyHost = null, string proxyPort = null)
		{
			HttpWebRequest request = WebRequest.Create(uri) as HttpWebRequest;
			request.Timeout = 3 * 60 * 1000;
			// for getting the actual uri, if redirect.
			request.AllowAutoRedirect = false;

			if (!string.IsNullOrWhiteSpace(proxyHost) && !string.IsNullOrWhiteSpace(proxyPort))
			{
				request.Proxy = new WebProxy(string.Format("{0}:{1}", proxyHost, proxyPort));
			}

			using (WebResponse response = request.GetResponse())
			{
				// set actual uri and re-request the uri for getting the final html.
				if (response.Headers["Location"] != null)
				{
					var url = response.Headers["Location"];
					url = new Uri(new Uri(uri), url).ToString();
					uri = url;
					return Get(ref uri, encoding, proxyHost, proxyPort);
				}
				else
				{
					using (StreamReader sr = new StreamReader(
						response.GetResponseStream(), Encoding.GetEncoding(encoding)))
					{

						return sr.ReadToEnd();
					}
				}
			}
		}

		/// <summary>
		/// Gets the response html by 'POST' request using the specified URI.
		/// </summary>
		/// <param name="uri">The URI.</param>
		/// <param name="formData">The form data.</param>
		/// <param name="encoding">The encoding. Default value: utf-8</param>
		/// <param name="proxyHost">The proxy host.</param>
		/// <param name="proxyPort">The proxy port.</param>
		/// <returns>The response html.</returns>
		public static string Post(string uri, Dictionary<string, string> formData, string encoding = "utf-8", string proxyHost = null, string proxyPort = null)
		{
			WebRequest request = WebRequest.Create(uri);
			request.Method = "POST";
			request.ContentType = "application/x-www-form-urlencoded";
			if (!string.IsNullOrWhiteSpace(proxyHost) && !string.IsNullOrWhiteSpace(proxyPort))
			{
				request.Proxy = new WebProxy(string.Format("{0}:{1}", proxyHost, proxyPort));
			}

			string postData = string.Join("&", new Func<Dictionary<string, string>, List<string>>(__ =>
			{
				List<string> lst = new List<string>();
				if (__ != null)
				{
					foreach (KeyValuePair<string, string> item in __)
					{
						lst.Add(string.Format("{0}={1}", item.Key, item.Value));
					}
				}
				return lst;
			}));
			byte[] postArray = Encoding.GetEncoding(encoding).GetBytes(postData);

			Stream reqStream = request.GetRequestStream();
			reqStream.Write(postArray, 0, postArray.Length);
			reqStream.Close();

			using (WebResponse response = request.GetResponse())
			{
				using (StreamReader sr = new StreamReader(
					response.GetResponseStream(), Encoding.GetEncoding(encoding)))
				{

					return sr.ReadToEnd();
				}
			}
		}

		/// <summary>
		/// Downloads file using the specified URI.
		/// </summary>
		/// <param name="uri">The URI.</param>
		/// <param name="saveAs">The destination folder or file.</param>
		/// <param name="proxyHost">The proxy host.</param>
		/// <param name="proxyPort">The proxy port.</param>
		/// <returns>The full name of the destination file.</returns>
		public static string Download(string uri, string saveAs, string proxyHost = null, string proxyPort = null)
		{
			WebRequest request = HttpWebRequest.Create(uri);
			if (!string.IsNullOrWhiteSpace(proxyHost)
				&& !string.IsNullOrWhiteSpace(proxyPort))
			{
				request.Proxy = new WebProxy(string.Format("{0}:{1}", proxyHost, proxyPort));
			}

			WebResponse response = request.GetResponse();

			string destFileName = null;
			FileInfo fi = new FileInfo(saveAs);
			if (string.IsNullOrEmpty(fi.Extension) 
				|| !Regex.IsMatch(fi.Extension, @"^\.\w{3,4}$"))
			{
				destFileName = Path.Combine(saveAs, Regex.Replace(Path.GetFileName(uri), @"[^\.\w]", "-"));
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

			response.Close();

			return destFileName;
		}
	}
}
