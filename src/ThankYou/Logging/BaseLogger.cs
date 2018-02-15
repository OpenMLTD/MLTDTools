using System;

namespace OpenMLTD.ThankYou.Logging {
    public abstract class BaseLogger : ILogger {

        public abstract void Debug(object message);

        public void Debug(object message, Exception exception) {
            var stackTrace = exception.StackTrace;
            var info = message + Environment.NewLine + stackTrace;
            Debug(info);
        }

        public void Debug(string message) {
            Debug((object)message);
        }

        public void DebugFormat(IFormatProvider provider, string format, params object[] args) {
            var message = string.Format(provider, format, args);
            Debug(message);
        }

        public void DebugFormat(string format, object arg0) {
            var message = string.Format(format, arg0);
            Debug(message);
        }

        public void DebugFormat(string format, object arg0, object arg1) {
            var message = string.Format(format, arg0, arg1);
            Debug(message);
        }

        public void DebugFormat(string format, object arg0, object arg1, object arg2) {
            var message = string.Format(format, arg0, arg1, arg2);
            Debug(message);
        }

        public void DebugFormat(string format, params object[] args) {
            var message = string.Format(format, args);
            Debug(message);
        }

        public abstract void Info(object message);

        public void Info(object message, Exception exception) {
            var stackTrace = exception.StackTrace;
            var info = message + Environment.NewLine + stackTrace;
            Info(info);
        }

        public void Info(string message) {
            Info((object)message);
        }

        public void InfoFormat(IFormatProvider provider, string format, params object[] args) {
            var message = string.Format(provider, format, args);
            Info(message);
        }

        public void InfoFormat(string format, object arg0) {
            var message = string.Format(format, arg0);
            Info(message);
        }

        public void InfoFormat(string format, object arg0, object arg1) {
            var message = string.Format(format, arg0, arg1);
            Info(message);
        }

        public void InfoFormat(string format, object arg0, object arg1, object arg2) {
            var message = string.Format(format, arg0, arg1, arg2);
            Info(message);
        }

        public void InfoFormat(string format, params object[] args) {
            var message = string.Format(format, args);
            Info(message);
        }

        public abstract void Warn(object message);

        public void Warn(object message, Exception exception) {
            var stackTrace = exception.StackTrace;
            var info = message + Environment.NewLine + stackTrace;
            Warn(info);
        }

        public void Warn(string message) {
            Warn((object)message);
        }

        public void WarnFormat(IFormatProvider provider, string format, params object[] args) {
            var message = string.Format(provider, format, args);
            Warn(message);
        }

        public void WarnFormat(string format, object arg0) {
            var message = string.Format(format, arg0);
            Warn(message);
        }

        public void WarnFormat(string format, object arg0, object arg1) {
            var message = string.Format(format, arg0, arg1);
            Warn(message);
        }

        public void WarnFormat(string format, object arg0, object arg1, object arg2) {
            var message = string.Format(format, arg0, arg1, arg2);
            Warn(message);
        }

        public void WarnFormat(string format, params object[] args) {
            var message = string.Format(format, args);
            Warn(message);
        }

        public abstract void Error(object message);

        public void Error(object message, Exception exception) {
            var stackTrace = exception.StackTrace;
            var info = message + Environment.NewLine + stackTrace;
            Error(info);
        }

        public void Error(string message) {
            Error((object)message);
        }

        public void ErrorFormat(IFormatProvider provider, string format, params object[] args) {
            var message = string.Format(provider, format, args);
            Error(message);
        }

        public void ErrorFormat(string format, object arg0) {
            var message = string.Format(format, arg0);
            Error(message);
        }

        public void ErrorFormat(string format, object arg0, object arg1) {
            var message = string.Format(format, arg0, arg1);
            Error(message);
        }

        public void ErrorFormat(string format, object arg0, object arg1, object arg2) {
            var message = string.Format(format, arg0, arg1, arg2);
            Error(message);
        }

        public void ErrorFormat(string format, params object[] args) {
            var message = string.Format(format, args);
            Error(message);
        }

        public abstract void Fatal(object message);

        public void Fatal(object message, Exception exception) {
            var stackTrace = exception.StackTrace;
            var info = message + Environment.NewLine + stackTrace;
            Fatal(info);
        }

        public void Fatal(string message) {
            Fatal((object)message);
        }

        public void FatalFormat(IFormatProvider provider, string format, params object[] args) {
            var message = string.Format(provider, format, args);
            Fatal(message);
        }

        public void FatalFormat(string format, object arg0) {
            var message = string.Format(format, arg0);
            Fatal(message);
        }

        public void FatalFormat(string format, object arg0, object arg1) {
            var message = string.Format(format, arg0, arg1);
            Fatal(message);
        }

        public void FatalFormat(string format, object arg0, object arg1, object arg2) {
            var message = string.Format(format, arg0, arg1, arg2);
            Fatal(message);
        }

        public void FatalFormat(string format, params object[] args) {
            var message = string.Format(format, args);
            Fatal(message);
        }

    }
}
