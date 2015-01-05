// =================================================================================
// Copyright (c) 2012 http://www.bndy.net.
// Created by Bndy at 12/27/2012 9:09:54 AM
// ---------------------------------------------------------------------------------
// Table for displaying  data.
//	Features: Sorts automatically and customizes manually for cell data.
// =================================================================================

using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace Net.Bndy.Web.Controls
{
	public class DataGrid<TItem>
	{
		const string Url_Parameter_Sort_Field = "sort_field";
		const string Url_Parameter_Sort_Direction = "sort_direction";

		public DataGridColumn<TItem>[] Columns { get; private set; }
		public string IconPathAsc { private get; set; }
		public string IconPathDesc { private get; set; }
		public IList<TItem> DataSource { private get; set; }

		private string CurrentSortField
		{
			get
			{
				string result = UrlParameters.Current.GetValue(Url_Parameter_Sort_Field);

				if (string.IsNullOrWhiteSpace(result))
				{
					// Gets default sort field from the column collection.
					foreach (DataGridColumn<TItem> col in this.Columns)
					{
						if (col.IsDefaultSort)
						{
							result = col.BindingFields;
							break;
						}
					}
				}

				return result;
			}
		}
		private string CurrentSortDirection
		{
			get
			{
				string result = UrlParameters.Current.GetValue(Url_Parameter_Sort_Direction);

				if (string.IsNullOrWhiteSpace(result))
				{
					// Gets default sort direction of the default  sort column.
					foreach (DataGridColumn<TItem> col in this.Columns)
					{
						if (col.IsDefaultSort)
						{
							result = col.IsAscSort ? "ASC" : "DESC";
							break;
						}
					}
				}

				return result;
			}
		}

		public DataGrid(IList<TItem> dataSource, params DataGridColumn<TItem>[] columns)
		{
			this.DataSource = dataSource;
			this.Columns = columns;
		}

		public string ToHtml()
		{
			if (this.IconPathAsc == null) this.IconPathAsc = "&uarr;";
			if (this.IconPathDesc == null) this.IconPathDesc = "&darr;";

			SortDataSource();

			return string.Format("<table>{0}{1}</table>",
				GetHeader(),
				GetBody()
				);
		}

		private void SortDataSource()
		{
			if (!string.IsNullOrWhiteSpace(this.CurrentSortDirection) 
				&& !string.IsNullOrWhiteSpace(this.CurrentSortField))
			{
				if ("DESC".Equals(this.CurrentSortDirection, StringComparison.OrdinalIgnoreCase))
				{
					this.DataSource = this.DataSource.OrderByDescending(
						m => m.GetType().GetProperty(this.CurrentSortField).GetValue(m, null)).ToList();
				}
				else
				{
					this.DataSource = this.DataSource.OrderBy(
						m => m.GetType().GetProperty(this.CurrentSortField).GetValue(m, null)).ToList();
				}
			}
		}

		private string GetHeader()
		{
			StringBuilder result = new StringBuilder();

			result.Append("<thead><tr>");
			foreach (DataGridColumn<TItem> col in this.Columns)
			{
				UrlParameters url = UrlParameters.Current;
				if (col.Sortable)
				{
					url.SetValue(Url_Parameter_Sort_Field, col.BindingFields);

					url.SetValue(Url_Parameter_Sort_Direction,
						this.CurrentSortField == col.BindingFields && "ASC".Equals(this.CurrentSortDirection)
							? "DESC" : "ASC"
						);
				}

				result.AppendFormat("<th{1}{2}>{0}</th>",
					col.Sortable
						? string.Format("<a href=\"{1}\">{0}{2}</a>",
								col.HeaderText,
								url.CombineWithCurrentUrl(),
								this.CurrentSortField == col.BindingFields
									? ("ASC".Equals(this.CurrentSortDirection) 
										? this.IconPathAsc : this.IconPathDesc)
									: string.Empty
							)
						: col.HeaderText,
					!string.IsNullOrWhiteSpace(col.HeaderClass)
						? string.Format(" class=\"{0}\"", col.HeaderClass) : string.Empty,
					!string.IsNullOrWhiteSpace(col.HeaderStyle)
						? string.Format(" style=\"{0}\"", col.HeaderStyle) : string.Empty
					);
			}
			result.Append("</tr></thead>");

			return result.ToString();
		}

		private string GetBody()
		{
			StringBuilder result = new StringBuilder();

			result.Append("<tbody>");
			if (this.DataSource != null)
			{
				foreach (TItem item in this.DataSource)
				{
					result.Append("<tr>");
					foreach (DataGridColumn<TItem> col in this.Columns)
					{
						result.AppendFormat("<td{1}{2}>{0}</td>",
							col.GetCellValue(item),
							!string.IsNullOrWhiteSpace(col.CellClass)
								? string.Format(" class=\"{0}\"", col.CellClass) : string.Empty,
							!string.IsNullOrWhiteSpace(col.CellStyle)
								? string.Format(" style=\"{0}\"", col.CellStyle) : string.Empty
							);
					}
					result.Append("</tr>");
				}
			}
			else
			{
				result.AppendFormat("<tr><td colspan=\"{0}\">No Data</td></tr>",
					this.Columns.Length
				);
			}
			result.Append("</tbody>");

			return result.ToString();
		}
	}

	public class DataGridColumn<TItem>
	{
		public string Name { get; set; }
		public bool Sortable { get; set; }
		public bool IsDefaultSort { get; set; }
		public bool IsAscSort { get; set; }

		public string HeaderText { get; set; }
		public string HeaderStyle { get; set; }
		public string HeaderClass { get; set; }

		public string CellStyle { get; set; }
		public string CellClass { get; set; }
		public string CellFormat { get; set; }
		public string BindingFields { get; set; }

		public Func<TItem, string> CellDataHandler;

		public string GetCellValue(TItem item)
		{
			if (this.BindingFields != null && item != null)
			{
				List<object> values = new List<object>();
				object o = null;
				foreach (string field in this.BindingFields.Split(
					new char[] { ',', '|', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries))
				{
					o = item.GetType().GetProperty(field).GetValue(item, null);
					values.Add(o);
				}

				if (!string.IsNullOrWhiteSpace(this.CellFormat))
				{
					return string.Format(this.CellFormat, values);
				}

				if (this.CellDataHandler != null)
				{
					return this.CellDataHandler(item);
				}

				return values.Count > 0 && values[0] != null ? values[0].ToString() : string.Empty;
			}

			if (string.IsNullOrWhiteSpace(this.BindingFields))
			{
				throw new ArgumentNullException("CellField(Column binding field)");
			}

			return string.Empty;
		}
	}
}