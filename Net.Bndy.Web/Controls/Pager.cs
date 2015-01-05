// =================================================================================
// Copyright (c) 2012 http://www.bndy.net.
// Created by Bndy at 12/5/2012 19:03:00
// ---------------------------------------------------------------------------------
// Pagination
//	This is just an interface to view another page via URL.
//	Show the data via some else controls.
// =================================================================================

using System.Text;

namespace Net.Bndy.Web.Controls
{
	public class Pager
	{
		const string UrlParameterName = "page";

		public int RecordCount { get; set; }
		public int PageSize { get; set; }
		public int PageCount { get; private set; }

		public Pager(int recordCount, int pageSize = 30)
		{
			this.RecordCount = recordCount;
			this.PageSize = pageSize;

			this.PageCount = this.RecordCount / this.PageSize
				+ (this.RecordCount % this.PageSize > 0 ? 1 : 0);

			_urlParameters = UrlParameters.Current;
		}


		public string ToHtml()
		{
			int currentPage;
			int.TryParse(_urlParameters.GetValue(UrlParameterName), out currentPage);

			return ToHtml(currentPage);
		}

		private string ToHtml(int currentPage)
		{
			StringBuilder result = new StringBuilder();

			// More than one page, show the pages.
			if (this.PageCount > 1 && this.RecordCount > 0)
			{
				if (currentPage < 1) currentPage = 1;
				if (currentPage > this.PageCount) currentPage = this.PageCount;

				result.Append("<div class=\"box-pager\">");
				result.Append(BuildPageNumber(currentPage));
				result.Append(BuildGotoPage(currentPage));
				result.Append("</div>");
			}

			return result.ToString();
		}

		private string BuildPageNumber(int currentPage)
		{
			StringBuilder result = new StringBuilder();

			int beginPage = 1;
			int endPage = this.PageCount;

			if (this.PageCount > 10)
			{
				// Show 9 page numbers.
				if (currentPage < 6)
				{
					beginPage = 2;
					endPage = 9;
				}
				else if (currentPage > this.PageCount - 4)
				{
					beginPage = this.PageCount - 8;
					endPage = this.PageCount - 1;
				}
				else
				{
					beginPage = currentPage - 4;
					endPage = currentPage + 3;
				}
			}
			else
			{
				beginPage = 2;
				endPage = this.PageCount - 1;
			}

			result.Append("<ul class=\"pager\">");

			// First Page & Previous Page
			if (currentPage == 1)
			{
				result.AppendFormat("<li class=\"disable\"><<</li>");
				result.AppendFormat("<li class=\"current\">1</li>");
			}
			else
			{
				// Previous Page
				_urlParameters.SetValue(UrlParameterName, currentPage - 1);
				result.AppendFormat("<li {1}><a href=\"{0}\"><<</a></li>",
					_urlParameters.CombineWithCurrentUrl(),
					currentPage == 1 ? "class=\"disable\"" : ""
					);

				// First Page
				_urlParameters.SetValue(UrlParameterName, 1);
				result.AppendFormat("<li><a href=\"{0}\">1</a></li>",
					_urlParameters.CombineWithCurrentUrl(),
					currentPage == 1 ? "class=\"current\"" : ""
					);
			}

			// Show MORE text for indicates some pages are hided. 
			if (beginPage > 2)
			{
				result.AppendFormat("<li class=\"more\">...</li>");
			}

			// Page Numbers
			for (int i = beginPage; i <= endPage; i++)
			{
				if (i != currentPage)
				{
					_urlParameters.SetValue(UrlParameterName, i);
					result.AppendFormat("<li {2}><a href=\"{1}\">{0}</a></li>",
						i,
						_urlParameters.CombineWithCurrentUrl(),
						i == currentPage ? "class=\"current\"" : ""
						);
				}
				else
				{
					result.AppendFormat("<li class=\"current\">{0}</li>", i);
				}
			}

			// Show MORE text for indicates some pages are hided. 
			if (endPage < this.PageCount - 1)
			{
				result.AppendFormat("<li class=\"more\">...</li>");
			}

			// Last Page & Next Page
			if (currentPage == this.PageCount)
			{
				result.AppendFormat("<li class=\"current\">{0}</li>", this.PageCount);
				result.AppendFormat("<li class=\"disable\">>></li>");
			}
			else
			{
				// Last Page
				_urlParameters.SetValue(UrlParameterName, this.PageCount);
				result.AppendFormat("<li {2}><a href=\"{0}\">{1}</li>",
					_urlParameters.CombineWithCurrentUrl(),
					this.PageCount,
					currentPage == this.PageCount ? "class=\"current\"" : ""
					);

				// Next Page
				_urlParameters.SetValue(UrlParameterName, currentPage + 1);
				result.AppendFormat("<li><a href=\"{0}\">>></a></li>",
					_urlParameters.CombineWithCurrentUrl(),
					currentPage == this.PageCount ? "class=\"disable\"" : ""
					);
			}

			result.Append("</ul>");

			return result.ToString();
		}

		private string BuildGotoPage(int currentPage)
		{
			_urlParameters.Remove(UrlParameterName);

			string url = _urlParameters.CombineWithCurrentUrl();
			if (url.IndexOf('?') < 0)
				url += "?" + UrlParameterName;
			else
				url += "&" + UrlParameterName;

			StringBuilder js = new StringBuilder();
			js.Append("var pageNum = document.getElementById('pager-txtPageNumber').value;");
			js.Append("if(isNaN(pageNum)){");
			js.Append("alert('Please enter a valid number.');");
			js.Append("}");
			js.Append("else{");
			js.AppendFormat("    location.href = '{0}=' + pageNum;", url);
			js.Append("}");

			return string.Format(@"<div class=""pager"">
                <input type=""text"" value=""{0}"" id=""pager-txtPageNumber"" />
                <input type=""button"" value=""Go"" onclick=""{1}"" />
                </div>",
				currentPage,
				js);
		}

		private UrlParameters _urlParameters;
	}
}
