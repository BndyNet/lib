// =================================================================================
// Copyright (c) 2012 http://www.bndy.net.
// Created by Bndy at 9/6/2012 3:16:59 PM
// ---------------------------------------------------------------------------------
// Provides the operations for word files.
//      Using the Word 11 COM.
// =================================================================================

using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Office.Interop.Word;

namespace Net.Bndy.IO.Office
{
	public class WordApp : IDisposable
	{
		#region Fields

		private _Application _application;
		private _Document _document;
		private Dictionary<object, _Document> _documents;
		private object _objMissing = Type.Missing;

		#endregion

		#region Constructors

		public WordApp(object fileName)
		{
			_application = new ApplicationClass();
			_document = _application.Documents.Open(
					fileName,
					_objMissing,
					_objMissing,
					_objMissing,
					_objMissing,
					_objMissing,
					_objMissing,
					_objMissing,
					_objMissing,
					_objMissing,
					_objMissing,
					_objMissing,
					_objMissing,
					_objMissing,
					_objMissing,
					_objMissing
					);

			_document.Activate();
		}

		public WordApp(object[] fileNames)
		{
			_application = new ApplicationClass();
			_documents = new Dictionary<object, _Document>();

			_document = null;
			foreach (object file in fileNames)
			{
				_document = _application.Documents.Open(
						file,
						_objMissing,
						_objMissing,
						_objMissing,
						_objMissing,
						_objMissing,
						_objMissing,
						_objMissing,
						_objMissing,
						_objMissing,
						_objMissing,
						_objMissing,
						_objMissing,
						_objMissing,
						_objMissing,
						_objMissing
						);

				_document.Activate();
				_documents.Add(file, _document);

			}
		}

		#endregion

		#region Methods

		/// <summary>
		/// Updates the bookmark content.
		/// </summary>
		/// <param name="bookmark">The bookmark name.</param>
		/// <param name="content">The content needs to be displayed in the bookmark.</param>
		/// <return>An instance of <see cref="Bndy.Net.Office.Interop.Word.Range"/>.</return>
		public Range UpdateBookmark(object bookmark, string content)
		{
			if (_document.Bookmarks.Exists(bookmark.ToString()))
			{
				Bookmark bm = _document.Bookmarks[bookmark];
				Range r = bm.Range;
				bm.Range.Text = content;
				r.End = r.Start + content.Length;
				_document.Bookmarks.Add(bookmark.ToString(), (object)r);

				return r;
			}

			return null;
		}

		/// <summary>
		/// Updates the bookmark for setting the content to image.
		/// </summary>
		/// <param name="bookmark">The bookmark name.</param>
		/// <param name="imgFileName">File name of the image.</param>
		/// <returns>An instance of <see cref="Bndy.Net.Office.Interop.Word.Range"/>.</returns>
		public InlineShape UpdateBookmarkToShape(object bookmark, string imgFileName)
		{
			if (_document.Bookmarks.Exists(bookmark.ToString()))
			{
				object saveWithDoc = true;
				Bookmark bm = _document.Bookmarks[bookmark];
				bm.Select();
				InlineShape shape = _application.Selection.InlineShapes.AddPicture(
						imgFileName,
						_objMissing,
						saveWithDoc,
						_objMissing
						);

				Range r = shape.Range;

				_document.Bookmarks.Add(bookmark.ToString(), (object)r);

				return shape;
			}

			return null;
		}

		/// <summary>
		/// Inserts a table in the specified bookmark.
		/// </summary>
		/// <param name="bookmark">The bookmark name.</param>
		/// <param name="rows">The count of rows.</param>
		/// <param name="columns">The count of columns.</param>
		/// <returns>
		/// An instance of <see cref="Bndy.Net.Office.Interop.Word.Table"/>.
		///     null, if the bookmark does not exist.
		/// </returns>
		public Table InsertTable(object bookmark, int rows, int columns)
		{
			Table result = null;

			if (_document.Bookmarks.Exists(bookmark.ToString()))
			{
				Range r = _document.Bookmarks[bookmark].Range;
				result = _application.Selection.Tables.Add(
						r,
						rows,
						columns,
						_objMissing,
						_objMissing
						);
				r.SetRange(r.Start, result.Range.End);
				_document.Bookmarks.Add(bookmark.ToString(), (object)r);
			}

			return result;
		}

		/// <summary>
		/// Gets the content of the bookmark.
		/// </summary>
		/// <param name="bookmark">The bookmark name.</param>
		/// <returns><see cref="String.Empty"/>, if the bookmark does not exist.</returns>
		public string GetContent(object bookmark)
		{
			string result = string.Empty;

			if (_document.Bookmarks.Exists(bookmark.ToString()))
			{
				result = _document.Bookmarks[bookmark].Range.Text;
			}

			return result;
		}

		/// <summary>
		/// Gets the range of bookmark.
		/// </summary>
		/// <param name="bookmark">The bookmark name.</param>
		/// <returns>An instance of <see cref="Bndy.Net.Office.Interop.Word.Range"/>.</returns>
		public Range GetRange(object bookmark)
		{
			if (_document.Bookmarks.Exists(bookmark.ToString()))
			{
				return _document.Bookmarks[bookmark].Range;
			}

			return null;
		}

		/// <summary>
		/// Deletes the bookmark with content.
		/// </summary>
		/// <param name="bookmark">The bookmark name.</param>
		public void DeleteBookmark(object bookmark)
		{
			if (_document.Bookmarks.Exists(bookmark.ToString()))
			{
				Range r = _document.Bookmarks[bookmark].Range;

				foreach (Table tbl in r.Tables)
				{
					tbl.Delete();
				}

				foreach (Shape shp in r.InlineShapes)
				{
					shp.Delete();
				}

				r.Delete();

				if (_document.Bookmarks.Exists(bookmark.ToString()))
				{
					_document.Bookmarks[bookmark].Delete();
				}
			}
		}

		/// <summary>
		/// Saves the current activated document.
		/// </summary>
		public void Save()
		{
			_document.Save();
		}

		/// <summary>
		/// Saves as a new file(Only supports DOC file).
		/// </summary>
		/// <param name="fileName">Name of the new file.</param>
		public void SaveAs(string fileName)
		{
			object fileFormat = fileName.Substring(fileName.LastIndexOf('.') + 1);
			switch (fileFormat.ToString().ToUpper())
			{
				case "PDF":
					// This function is provided in the Office Com Version 14.
					//fileFormat = WdSaveFormat.wdFormatPDF;
					break;

				case "DOC":
					fileFormat = WdSaveFormat.wdFormatDocument;
					break;

				case "DOCX":
					fileFormat = WdSaveFormat.wdFormatDocument;
					break;
			}

			object objFileName = fileName;
			_document.SaveAs(
					objFileName,
					fileFormat,
					_objMissing,
					_objMissing,
					_objMissing,
					_objMissing,
					_objMissing,
					_objMissing,
					_objMissing,
					_objMissing,
					_objMissing,
					_objMissing,
					_objMissing,
					_objMissing,
					_objMissing,
					_objMissing
					);
		}

		/// <summary>
		/// Merges to a new file.
		///     If the file already has existed, that will be deleted and recreated.
		/// </summary>
		/// <param name="fileName">Name of the new file.</param>
		public void MergeTo(object fileName)
		{
			if (File.Exists(fileName.ToString()))
			{
				File.Delete(fileName.ToString());
			}

			_document = _application.Documents.Add(
					_objMissing,
					_objMissing,
					_objMissing,
					_objMissing
					);
			_document.Activate();

			int count = 0;
			foreach (KeyValuePair<object, _Document> kv in _documents)
			{
				_application.Selection.PageSetup.Orientation = kv.Value.PageSetup.Orientation;
				_application.Selection.PageSetup.PaperSize = kv.Value.PageSetup.PaperSize;
				_application.Selection.PageSetup.PageWidth = kv.Value.PageSetup.PageWidth;
				_application.Selection.PageSetup.PageHeight = kv.Value.PageSetup.PageHeight;

				_application.Selection.InsertFile(
						kv.Key.ToString(),
						_objMissing,
						_objMissing,
						_objMissing,
						_objMissing
						);

				count++;

				if (count != _documents.Count)
				{
					object breakType = WdBreakType.wdSectionBreakNextPage;
					_application.Selection.InsertBreak(breakType);
				}
			}

			_document.SaveAs(
					fileName,
					_objMissing,
					_objMissing,
					_objMissing,
					_objMissing,
					_objMissing,
					_objMissing,
					_objMissing,
					_objMissing,
					_objMissing,
					_objMissing,
					_objMissing,
					_objMissing,
					_objMissing,
					_objMissing,
					_objMissing
					);
		}

		#endregion

		#region Dispose

		private bool _disposed = false;

		~WordApp()
		{
			Dispose(false);
		}

		private void Dispose(bool disposing)
		{
			if (!_disposed)
			{
				if (disposing)
				{
					// Release managed resources
				}

				// Release unmanaged resources
				object saveChanges = false;
				if (_application != null)
				{
					foreach (_Document d in _application.Documents)
					{
						d.Close(saveChanges, _objMissing, _objMissing);
					}
					_application.Quit();
					_application = null;
				}

				this._disposed = true;
			}

		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		#endregion
	}
}
