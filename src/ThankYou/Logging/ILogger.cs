using System;
using JetBrains.Annotations;

namespace OpenMLTD.ThankYou.Logging {
    public interface ILogger {

        void Debug([NotNull] object message);

        void Debug([NotNull] object message, [NotNull] Exception exception);

        void Debug([NotNull] string message);

        void DebugFormat([NotNull] IFormatProvider provider, [NotNull] string format, params object[] args);

        void DebugFormat([NotNull] string format, object arg0);

        void DebugFormat([NotNull] string format, object arg0, object arg1);

        void DebugFormat([NotNull] string format, object arg0, object arg1, object arg2);

        void DebugFormat([NotNull] string format, params object[] args);

        void Info([NotNull] object message);

        void Info([NotNull] object message, [NotNull] Exception exception);

        void Info([NotNull] string message);

        void InfoFormat([NotNull] IFormatProvider provider, [NotNull] string format, params object[] args);

        void InfoFormat([NotNull] string format, object arg0);

        void InfoFormat([NotNull] string format, object arg0, object arg1);

        void InfoFormat([NotNull] string format, object arg0, object arg1, object arg2);

        void InfoFormat([NotNull] string format, params object[] args);

        void Warn([NotNull] object message);

        void Warn([NotNull] object message, [NotNull] Exception exception);

        void Warn([NotNull] string message);

        void WarnFormat([NotNull] IFormatProvider provider, [NotNull] string format, params object[] args);

        void WarnFormat([NotNull] string format, object arg0);

        void WarnFormat([NotNull] string format, object arg0, object arg1);

        void WarnFormat([NotNull] string format, object arg0, object arg1, object arg2);

        void WarnFormat([NotNull] string format, params object[] args);

        void Error([NotNull] object message);

        void Error([NotNull] object message, [NotNull] Exception exception);

        void Error([NotNull] string message);

        void ErrorFormat([NotNull] IFormatProvider provider, [NotNull] string format, params object[] args);

        void ErrorFormat([NotNull] string format, object arg0);

        void ErrorFormat([NotNull] string format, object arg0, object arg1);

        void ErrorFormat([NotNull] string format, object arg0, object arg1, object arg2);

        void ErrorFormat([NotNull] string format, params object[] args);

        void Fatal([NotNull] object message);

        void Fatal([NotNull] object message, [NotNull] Exception exception);

        void Fatal([NotNull] string message);

        void FatalFormat([NotNull] IFormatProvider provider, [NotNull] string format, params object[] args);

        void FatalFormat([NotNull] string format, object arg0);

        void FatalFormat([NotNull] string format, object arg0, object arg1);

        void FatalFormat([NotNull] string format, object arg0, object arg1, object arg2);

        void FatalFormat([NotNull] string format, params object[] args);

    }
}
