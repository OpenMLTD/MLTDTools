using System;
using System.Reflection;
using JetBrains.Annotations;
using log4net;

namespace OpenMLTD.AllStarsTheater.Logging {
    public static class IdolLog {

        #region Logging methods
        #region Debug
        public static void Debug([NotNull] object message) {
            if (LoggingEnabled) {
                _logger?.Debug(message);
            }
        }

        public static void Debug([NotNull] object message, [NotNull] Exception exception) {
            if (LoggingEnabled) {
                _logger?.Debug(message, exception);
            }
        }

        public static void Debug([NotNull] string message) {
            if (LoggingEnabled) {
                _logger?.Debug(message);
            }
        }

        [StringFormatMethod("format")]
        public static void DebugFormat([NotNull] IFormatProvider provider, [NotNull] string format, params object[] args) {
            if (LoggingEnabled) {
                _logger?.DebugFormat(provider, format, args);
            }
        }

        [StringFormatMethod("format")]
        public static void DebugFormat([NotNull] string format, object arg0) {
            if (LoggingEnabled) {
                _logger?.DebugFormat(format, arg0);
            }
        }

        [StringFormatMethod("format")]
        public static void DebugFormat([NotNull] string format, object arg0, object arg1) {
            if (LoggingEnabled) {
                _logger?.DebugFormat(format, arg0, arg1);
            }
        }

        [StringFormatMethod("format")]
        public static void DebugFormat([NotNull] string format, object arg0, object arg1, object arg2) {
            if (LoggingEnabled) {
                _logger?.DebugFormat(format, arg0, arg1, arg2);
            }
        }

        [StringFormatMethod("format")]
        public static void DebugFormat([NotNull] string format, params object[] args) {
            if (LoggingEnabled) {
                _logger?.DebugFormat(format, args);
            }
        }
        #endregion

        #region Info
        public static void Info([NotNull] object message) {
            if (LoggingEnabled) {
                _logger?.Info(message);
            }
        }

        public static void Info([NotNull] object message, [NotNull] Exception exception) {
            if (LoggingEnabled) {
                _logger?.Info(message, exception);
            }
        }

        public static void Info([NotNull] string message) {
            if (LoggingEnabled) {
                _logger?.Info(message);
            }
        }

        [StringFormatMethod("format")]
        public static void InfoFormat([NotNull] IFormatProvider provider, [NotNull] string format, params object[] args) {
            if (LoggingEnabled) {
                _logger?.InfoFormat(provider, format, args);
            }
        }

        [StringFormatMethod("format")]
        public static void InfoFormat([NotNull] string format, object arg0) {
            if (LoggingEnabled) {
                _logger?.InfoFormat(format, arg0);
            }
        }

        [StringFormatMethod("format")]
        public static void InfoFormat([NotNull] string format, object arg0, object arg1) {
            if (LoggingEnabled) {
                _logger?.InfoFormat(format, arg0, arg1);
            }
        }

        [StringFormatMethod("format")]
        public static void InfoFormat([NotNull] string format, object arg0, object arg1, object arg2) {
            if (LoggingEnabled) {
                _logger?.InfoFormat(format, arg0, arg1, arg2);
            }
        }

        [StringFormatMethod("format")]
        public static void InfoFormat([NotNull] string format, params object[] args) {
            if (LoggingEnabled) {
                _logger?.InfoFormat(format, args);
            }
        }
        #endregion

        #region Warn
        public static void Warn([NotNull] object message) {
            if (LoggingEnabled) {
                _logger?.Warn(message);
            }
        }

        public static void Warn([NotNull] object message, [NotNull] Exception exception) {
            if (LoggingEnabled) {
                _logger?.Warn(message, exception);
            }
        }

        public static void Warn([NotNull] string message) {
            if (LoggingEnabled) {
                _logger?.Warn(message);
            }
        }

        [StringFormatMethod("format")]
        public static void WarnFormat([NotNull] IFormatProvider provider, [NotNull] string format, params object[] args) {
            if (LoggingEnabled) {
                _logger?.WarnFormat(provider, format, args);
            }
        }

        [StringFormatMethod("format")]
        public static void WarnFormat([NotNull] string format, object arg0) {
            if (LoggingEnabled) {
                _logger?.WarnFormat(format, arg0);
            }
        }

        [StringFormatMethod("format")]
        public static void WarnFormat([NotNull] string format, object arg0, object arg1) {
            if (LoggingEnabled) {
                _logger?.WarnFormat(format, arg0, arg1);
            }
        }

        [StringFormatMethod("format")]
        public static void WarnFormat([NotNull] string format, object arg0, object arg1, object arg2) {
            if (LoggingEnabled) {
                _logger?.WarnFormat(format, arg0, arg1, arg2);
            }
        }

        [StringFormatMethod("format")]
        public static void WarnFormat([NotNull] string format, params object[] args) {
            if (LoggingEnabled) {
                _logger?.WarnFormat(format, args);
            }
        }
        #endregion

        #region Error
        public static void Error([NotNull] object message) {
            if (LoggingEnabled) {
                _logger?.Error(message);
            }
        }

        public static void Error([NotNull] object message, [NotNull] Exception exception) {
            if (LoggingEnabled) {
                _logger?.Error(message, exception);
            }
        }

        public static void Error([NotNull] string message) {
            if (LoggingEnabled) {
                _logger?.Error(message);
            }
        }

        [StringFormatMethod("format")]
        public static void ErrorFormat([NotNull] IFormatProvider provider, [NotNull] string format, params object[] args) {
            if (LoggingEnabled) {
                _logger?.ErrorFormat(provider, format, args);
            }
        }

        [StringFormatMethod("format")]
        public static void ErrorFormat([NotNull] string format, object arg0) {
            if (LoggingEnabled) {
                _logger?.ErrorFormat(format, arg0);
            }
        }

        [StringFormatMethod("format")]
        public static void ErrorFormat([NotNull] string format, object arg0, object arg1) {
            if (LoggingEnabled) {
                _logger?.ErrorFormat(format, arg0, arg1);
            }
        }

        [StringFormatMethod("format")]
        public static void ErrorFormat([NotNull] string format, object arg0, object arg1, object arg2) {
            if (LoggingEnabled) {
                _logger?.ErrorFormat(format, arg0, arg1, arg2);
            }
        }

        [StringFormatMethod("format")]
        public static void ErrorFormat([NotNull] string format, params object[] args) {
            if (LoggingEnabled) {
                _logger?.ErrorFormat(format, args);
            }
        }
        #endregion

        #region Fatal
        public static void Fatal([NotNull] object message) {
            if (LoggingEnabled) {
                _logger?.Fatal(message);
            }
        }

        public static void Fatal([NotNull] object message, [NotNull] Exception exception) {
            if (LoggingEnabled) {
                _logger?.Fatal(message, exception);
            }
        }

        public static void Fatal([NotNull] string message) {
            if (LoggingEnabled) {
                _logger?.Fatal(message);
            }
        }

        [StringFormatMethod("format")]
        public static void FatalFormat([NotNull] IFormatProvider provider, [NotNull] string format, params object[] args) {
            if (LoggingEnabled) {
                _logger?.FatalFormat(provider, format, args);
            }
        }

        [StringFormatMethod("format")]
        public static void FatalFormat([NotNull] string format, object arg0) {
            if (LoggingEnabled) {
                _logger?.FatalFormat(format, arg0);
            }
        }

        [StringFormatMethod("format")]
        public static void FatalFormat([NotNull] string format, object arg0, object arg1) {
            if (LoggingEnabled) {
                _logger?.FatalFormat(format, arg0, arg1);
            }
        }

        [StringFormatMethod("format")]
        public static void FatalFormat([NotNull] string format, object arg0, object arg1, object arg2) {
            if (LoggingEnabled) {
                _logger?.FatalFormat(format, arg0, arg1, arg2);
            }
        }

        [StringFormatMethod("format")]
        public static void FatalFormat([NotNull] string format, params object[] args) {
            if (LoggingEnabled) {
                _logger?.FatalFormat(format, args);
            }
        }
        #endregion
        #endregion

        public static bool LoggingEnabled { get; set; } = true;

        public static void Initialize([NotNull] string loggerName) {
            _logger = LogManager.GetLogger(Assembly.GetEntryAssembly(), loggerName);
        }

        [CanBeNull]
        private static ILog _logger;

    }
}
