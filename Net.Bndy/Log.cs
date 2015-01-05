// ==========================================================================
// Copyright (c) 2013 http://www.bndy.net
// Created by Bndy at 3/5/2013 12:04:55
// --------------------------------------------------------------------------
// Logs for Application
// ==========================================================================

using System;
using System.IO;
using System.Text;

namespace Net.Bndy
{
	[Serializable]
	public class Log
	{
		#region Properties
		public string Source { get; set; }
		public string Message { get; set; }
		public string Category { get; set; }
		public bool ExceptionThrown { get; set; }
		public Exception Exception { get; set; }
		#endregion

		#region Constructors & Methods

		private Log()
		{
			this.Category = LogType.Info.ToString();
			this.Message = string.Empty;
			this.ExceptionThrown = true;
		}

		public void Save()
		{
			this.Handle();
		}

		protected virtual void Handle()
		{
			// Generate the log message.
			StringBuilder logMessage = new StringBuilder();
			logMessage.AppendFormat("============================================================================={0}", Environment.NewLine);
			logMessage.AppendFormat("{0}\t{1}\t{2}\t{3}{4}", System.DateTime.Now.ToString("HH:mm:ss.fffffff"),
				this.Category,
				this.Source,
				this.Message,
				Environment.NewLine
				);
			if (this.Exception != null)
			{
				logMessage.AppendFormat("-----------------------------------------------------------------------------{0}", Environment.NewLine);
				logMessage.AppendFormat("Exception Source: {0}{1}", this.Exception.Source, Environment.NewLine);
				logMessage.AppendFormat("Exception Data: {0}{1}", this.Exception.Data, Environment.NewLine);
				logMessage.AppendFormat("Exception Message: {0}{1}", this.Exception.Message, Environment.NewLine);
				logMessage.AppendFormat("Exception Stack: {0}{1}", this.Exception.StackTrace, Environment.NewLine);
			}
			logMessage.AppendFormat("============================================================================={0}", Environment.NewLine);

			// Default log handler, Save logs to files.
			string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log");
			if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
			string fileName = Path.Combine(filePath,
				string.Format("{0}_{1}.log", this.Category, System.DateTime.Now.ToString("yyyyMMdd")));
			File.AppendAllText(fileName, logMessage.ToString());

			if (this.Exception != null && this.ExceptionThrown)
			{
				throw this.Exception;
			}
		}

		#endregion

		#region Static Members

		public static void Throw(Log log)
		{
			log.Handle();
		}

		/// <summary>
		/// Throws a log with default handler(save to files).
		/// </summary>
		/// <param name="sender">The type of current class.</param>
		/// <param name="message">Log message.</param>
		/// <param name="logCategory">Category used to the log file name.</param>
		/// <param name="exception">The instance of <see cref="System.Exception"/> class.</param>
		/// <param name="throwException">Indicates whether throws the exception when the parameter 'exception' is not NULL.</param>
		public static void Write(Type sender, string message,  
			string logCategory = null, Exception exception = null, bool throwException = false)
		{
			Log log = new Log();

			log.Message = message;
			log.Source = sender.ToString();
			log.Exception = exception;
			log.ExceptionThrown = throwException;

			if (string.IsNullOrWhiteSpace(logCategory))
			{
				log.Category = log.Exception != null ? LogType.Error.ToString() : LogType.Info.ToString();
			}
			else
			{
				log.Category = logCategory;
			}

			log.Handle();
		}

		/// <summary>
		/// Debugs code by writing a log file and the file name starts with 'Debug'.
		/// </summary>
		/// <param name="sender">The type of current class.</param>
		/// <param name="lines">Current line number.</param>
		public static void Debug(Type sender, int lines)
		{
			Write(sender, "LINE #" + lines, LogType.Debug.ToString());
		}

		#endregion
	}

	internal enum LogType
	{
		Info,
		Debug,
		Warning,
		Error
	}
}
