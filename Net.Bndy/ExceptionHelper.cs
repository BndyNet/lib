// ==========================================================================
// Copyright (c) 2013 http://www.bndy.net
// Created by Bndy at 3/9/2013 08:29:05 AM
// --------------------------------------------------------------------------
// System.Exception Extension Class.
// ==========================================================================

using System;

namespace Net.Bndy
{
	public static class ExceptionHelper
	{
		static ExceptionHelper()
		{
			DefaultHandler = delegate(Exception exception)
			{
				throw exception;
			};
		}

		/// <summary>
		/// Sets the default handler.
		/// </summary>
		/// <param name="handler">The instance of ExceptionHandler delegate.</param>
		public static void SetDefaultHandler(ExceptionHandler handler)
		{
			DefaultHandler = handler;
		}

		/// <summary>
		/// Handles the exception which is the instance of System.Exception.
		/// </summary>
		/// <param name="exception">The instance of System.Exception.</param>
		/// <param name="handler">
		/// The handler for exception.
		///		If null, the default handler will be used.
		///		Default Handler is throw exception, you can set it with SetDefaultHandler method.
		/// </param>
		public static void Handle(this Exception exception, ExceptionHandler handler = null)
		{
			if (handler != null)
			{
				handler(exception);
			}
			else
			{
				DefaultHandler(exception);
			}
		}

		private static ExceptionHandler DefaultHandler;
	}

	public delegate void ExceptionHandler(Exception exception);
}
