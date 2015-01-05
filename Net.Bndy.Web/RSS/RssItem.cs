// ==========================================================================
// Copyright (c) 2013 http://www.bndy.net.
// Created by Bndy at 3/4/2013 10:48:52
// --------------------------------------------------------------------------
// Item of Channel of Rss.
// ==========================================================================

using System;
using System.Text;
using System.Collections.Generic;

namespace Net.Bndy.Web.RSS
{
	public class RssItem
	{
		public string Guid { get; set; }
		public string Link { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public DateTime? PubDate { get; set; }

		public Dictionary<string, string> ExtraData { get; set; }

		public RssItem()
		{
			this.ExtraData = new Dictionary<string, string>();
		}

		public string ToXmlString()
		{
			StringBuilder sb = new StringBuilder();

			sb.AppendFormat("<item>");
			sb.AppendFormat("<title>{0}</title>", this.Title);
			sb.AppendFormat("<description>{0}</description>", this.Description);
			sb.AppendFormat("<link>{0}</link>", this.Link);
			sb.AppendFormat("<guid>{0}</guid>", this.Guid);

			sb.AppendFormat("<pubDate>{0}</pubDate>",
				this.PubDate.HasValue ?
					this.PubDate.Value.AddHours(-8).ToString("r") : string.Empty);

			foreach (KeyValuePair<string, string> kv in this.ExtraData)
			{
				sb.AppendFormat("<{0}>{1}</{0}>", kv.Key, kv.Value);
			}

			sb.AppendFormat("</item>");

			return sb.ToString();
		}
	}
}
