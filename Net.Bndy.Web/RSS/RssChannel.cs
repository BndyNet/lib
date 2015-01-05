// ==========================================================================
// Copyright (c) 2013 http://www.bndy.net.
// Created by Bndy at 3/4/2013 10:49:23
// --------------------------------------------------------------------------
// Channel of Rss
// ==========================================================================

using System;
using System.Text;
using System.Collections.Generic;

namespace Net.Bndy.Web.RSS
{
	public class RssChannel
	{
		public string Title { get; set; }
		public string Description { get; set; }
		public string Link { get; set; }
		public DateTime? LastBuildDate { get; set; }
		public DateTime? PubDate { get; set; }
		public int TTL { get; set; }
		public List<RssItem> Items { get; set; }

		public Dictionary<string, string> ExtraData { get; set; }

		public RssChannel()
		{
			this.Items = new List<RssItem>();
			this.ExtraData = new Dictionary<string, string>();
		}

		public string ToXmlString()
		{
			StringBuilder sb = new StringBuilder();

			sb.AppendFormat("<channel>");
			sb.AppendFormat("<title>{0}</title>", this.Title);
			sb.AppendFormat("<description>{0}</description>", this.Description);
			sb.AppendFormat("<link>{0}</link>", this.Link);
			sb.AppendFormat("<ttl>{0}</ttl>", this.TTL);

			sb.AppendFormat("<lastBuildDate>{0}</lastBuildDate>",
				this.LastBuildDate.HasValue ?
					this.LastBuildDate.Value.AddHours(-8).ToString("r") : string.Empty);
			sb.AppendFormat("<pubDate>{0}</pubDate>",
				this.PubDate.HasValue ?
					this.PubDate.Value.AddHours(-8).ToString("r") : string.Empty);

			foreach (KeyValuePair<string, string> kv in this.ExtraData)
			{
				sb.AppendFormat("<{0}>{1}</{0}>", kv.Key, kv.Value);
			}

			foreach (RssItem i in this.Items)
			{
				sb.Append(i.ToXmlString());
			}

			sb.AppendFormat("</channel>");

			return sb.ToString();
		}
	}
}
