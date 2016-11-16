// =================================================================================
// Copyright (c) 2016 http://www.bndy.net.
// Created by Bndy at 11/10/2016 9:27:19 PM
// ---------------------------------------------------------------------------------
// About Logs
// =================================================================================

using System;
using System.IO;

namespace Net.Bndy.Log
{
    /// <summary>
    /// Class Logger.
    /// </summary>
    public class Logger
    {
        #region Static Members
        private static bool _initialized = false;
        public static void Init(string configFile = null)
        {
            if (string.IsNullOrWhiteSpace(configFile))
            {
                configFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log4net.config");
            }
            log4net.Config.XmlConfigurator.ConfigureAndWatch(new FileInfo(configFile));
            _initialized = true;
        }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>log4net.ILog.</returns>
        public static Logger GetLogger(string name, Func<object> getContextObject = null)
        {
            if (!_initialized)
                throw new Exception("The logger has not been initialized.");

            return new Logger(name) { GetContextObject = getContextObject };
        }
        #endregion Static Members

        #region Instance Members
        public string Name { get; private set; }
        public bool IsDebugEnabled { get; private set; }
        public Func<object> GetContextObject { get; private set; }

        private log4net.ILog _logger
        {
            get
            {
                SetContext();
                return log4net.LogManager.GetLogger(this.Name);
            }
        }

        private Logger(string name)
        {
            this.Name = name;
        }

        public void Debug(string message)
        {
            _logger.Debug(message);
        }
        public void DebugFormat(string format, params object[] args)
        {
            _logger.DebugFormat(format, args);

        }
        public void Info(string message)
        {
            _logger.Info(message);
        }
        public void InfoFormat(string format, params object[] args)
        {
            _logger.InfoFormat(format, args);
        }
        public void Warn(string message)
        {
            _logger.Warn(message);
        }
        public void WarnFormat(string format, params object[] args)
        {
            _logger.WarnFormat(format, args);
        }

        public void Error(string message, Exception ex = null)
        {
            _logger.Error(message, ex);
        }
        public void ErrorFormat(string format, params object[] args)
        {
            _logger.ErrorFormat(format, args);
        }
        public void Fatal(string message, Exception ex = null)
        {
            _logger.Fatal(message, ex);
        }
        public void FatalFormat(string format, params object[] args)
        {
            _logger.FatalFormat(format, args);
        }



        private void SetContext()
        {
            if (this.GetContextObject != null)
            {
                var ndc = this.GetContextObject();
                if (ndc != null)
                {
                    foreach (var kv in ndc.ToDict())
                    {
                        log4net.MDC.Set(kv.Key, kv.Value);
                    }
                }
            }
        }
        #endregion Instance Members
    }
}
