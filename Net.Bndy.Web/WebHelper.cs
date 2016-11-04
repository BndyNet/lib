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
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

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

            List<string> lst = new List<string>();
            foreach (KeyValuePair<string, string> item in formData)
            {
                lst.Add(string.Format("{0}={1}", item.Key, item.Value));
            }
            var postData = string.Join("&", lst.ToArray());
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

        #region Agent
        /// <summary>
        /// Response the specified url and replace all links to agent url.
        /// </summary>
        /// <param name="requestUrl">The request URL.</param>
        /// <param name="agentUrl">The agent URL.</param>
        public static void Agent(string requestUrl, string agentUrl)
        {
            if (!string.IsNullOrWhiteSpace(requestUrl) && requestUrl != "/")
            {
                var request = (HttpWebRequest)WebRequest.Create(requestUrl);
                request.Method = "GET";
                request.Timeout = 10000;
                request.UserAgent = "Mozilla/5.0";
                request.Headers.Add("Accept-Encoding", "gzip, deflate");

                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    // the following code will lead to the breakdown of response
                    //foreach (var headerKey in resp.Headers.AllKeys)
                    //{
                    //    HttpContext.Current.Response.AppendHeader(headerKey, resp.Headers[headerKey]);
                    //}
                    HttpContext.Current.Response.ContentType = response.ContentType;

                    using (System.IO.Stream responseStream = response.GetResponseStream())
                    {
                        // write the response stream to MemoryStream
                        var ms = new MemoryStream();
                        var buffer = new byte[1024];
                        var contentBytes = new List<byte>();
                        var length = 0;
                        do
                        {
                            length = responseStream.Read(buffer, 0, buffer.Length);
                            contentBytes.AddRange(buffer.Take(length));
                            ms.Write(buffer.Take(length).ToArray(), 0, length);
                        } while (length > 0);
                        ms.Flush();

                        var contentType = response.ContentType.ToLower();

                        if (contentType.StartsWith("text") ||
                            contentType.IndexOf("javascript") > 0
                            )
                        {
                            // plain text and replace all links in html, css

                            // get the charset from response header
                            var characterSet = !string.IsNullOrWhiteSpace(response.CharacterSet) && response.CharacterSet != "ISO-8859-1"
                                ? response.CharacterSet : "gbk";
                            if (response.ContentType != null)
                            {
                                var codeCharacterSet = Regex.Match(response.ContentType,
                                    @"(?<=charset=)[\w-]*", RegexOptions.IgnoreCase).Value;
                                if (!string.IsNullOrWhiteSpace(codeCharacterSet))
                                {
                                    characterSet = codeCharacterSet;
                                }
                            }

                            Func<string, string> decodeResponse = (charset) =>
                            {
                                var encoding = Encoding.GetEncoding(charset);
                                if (response.ContentEncoding.ToLower() == "gzip")
                                {
                                    ms.Position = 0;
                                    return new StreamReader(new GZipStream(ms, CompressionMode.Decompress), encoding).ReadToEnd();
                                }
                                else if (response.ContentEncoding.ToLower() == "deflate")
                                {
                                    ms.Position = 0;
                                    return new StreamReader(new DeflateStream(ms, CompressionMode.Decompress), encoding).ReadToEnd();
                                }
                                else
                                {
                                    ms.Position = 0;
                                    return new StreamReader(ms, encoding).ReadToEnd();
                                }
                            };

                            // decoding using the default charset
                            var textContent = decodeResponse(characterSet);


                            if (contentType.StartsWith("text/html"))
                            {
                                // get the actual encoding from html
                                var charset = Regex.Match(textContent,
                                    @"(?<=charset=)[\w\d-]+", RegexOptions.IgnoreCase).Value.ToLower();
                                // redecoding when the actual encoding is not UTF-8
                                if (!string.IsNullOrWhiteSpace(charset) && charset != "utf-8")
                                {
                                    textContent = decodeResponse(charset);
                                }
                            }

                            if (contentType.StartsWith("text/html") || contentType.StartsWith("text/css"))
                            {
                                textContent = AgentReplaceLinksInHtmlOrCss(textContent, requestUrl, agentUrl);
                            }

                            HttpContext.Current.Response.Write(textContent);
                        }
                        else
                        {
                            // other files, like image, font and so on.
                            HttpContext.Current.Response.BinaryWrite(ms.ToArray());
                        }
                    }
                }
            }
        }

        private static string AgentReplaceLinksInHtmlOrCss(string code, string sourceUrl, string destUrl = null)
        {
            var patterns = new string[] {
                // html
                @"(?<begin>src=[""'])(?<url>[^'""]+?)(?<end>[""'])",
                @"(?<begin>href=[""'])(?<url>[^'""]+?)(?<end>[""'])",
                @"(?<begin>action=[""'])(?<url>[^'""]+?)(?<end>[""'])",           
                // css            
                @"(?<begin>url\(['""]?)(?<url>[^'""]+?)(?<end>['""]?\))",
            };
            foreach (var pattern in patterns)
            {
                foreach (Match m in Regex.Matches(code, pattern))
                {
                    var url = m.Groups["url"].Value;
                    if (!string.IsNullOrWhiteSpace(url) && !url.StartsWith(destUrl))
                    {
                        try
                        {
                            var newUrl = new Uri(new Uri(sourceUrl), url).ToString();
                            newUrl = destUrl + "?url=" + newUrl;
                            code = code.Replace(m.Value,
                                string.Format("{0}{1}{2}", m.Groups["begin"].Value, newUrl, m.Groups["end"].Value));
                        }
                        catch { }
                    }
                }
            }
            return code;
        }
        #endregion
    }
}
