// =================================================================================
// Copyright (c) 2013 http://www.bndy.net.
// Created by Bndy at 3/4/2013 10:06:00
// ---------------------------------------------------------------------------------
// Really Simple Syndication / Rich Site Summary
// =================================================================================

using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace Net.Bndy.Web.RSS
{
	public class Rss
	{
		private const string Default_Version = "2.0";	// Default version of Rss

		public string Version { get; set; }
		public List<RssChannel> Channels { get; set; }

		public Rss() : this(Default_Version) { }

		public Rss(params RssChannel[] channels) : this(Default_Version, channels) { }

		public Rss(string version, params RssChannel[] channels)
		{
			this.Version = version;

			if (channels == null)
				this.Channels = new List<RssChannel>();
			else
				this.Channels = channels.ToList();
		}

		public string ToXmlString()
		{
			StringBuilder sb = new StringBuilder();

			sb.AppendFormat("<?xml version=\"1.0\" encoding=\"UTF-8\" ?>");
			sb.AppendFormat("<rss version=\"{0}\">", this.Version);

			foreach (RssChannel c in this.Channels)
			{
				sb.Append(c.ToXmlString());
			}

			sb.AppendFormat("</rss>");

			return sb.ToString();
		}

		public void SaveTo(string fileFullName, bool overwrite = true)
		{
			if (File.Exists(fileFullName) && !overwrite)
				return;

			File.WriteAllText(fileFullName, ToXmlString(), Encoding.UTF8);
		}
	}
}
