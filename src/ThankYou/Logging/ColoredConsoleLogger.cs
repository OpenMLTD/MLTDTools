using System;
using JetBrains.Annotations;

namespace OpenMLTD.ThankYou.Logging {
    public sealed class ColoredConsoleLogger : BaseLogger {

        public override void Debug(object message) {
            SetColor(DebugForeColor, DebugBackColor);
            WriteMessage(message.ToString(), "DEBUG");
            ResetColor();
        }

        public override void Info(object message) {
            SetColor(InfoForeColor, InfoBackColor);
            WriteMessage(message.ToString(), "INFO");
            ResetColor();
        }

        public override void Warn(object message) {
            SetColor(WarnForeColor, WarnBackColor);
            WriteMessage(message.ToString(), "WARN");
            ResetColor();
        }

        public override void Error(object message) {
            SetColor(ErrorForeColor, ErrorBackColor);
            WriteMessage(message.ToString(), "ERROR");
            ResetColor();
        }

        public override void Fatal(object message) {
            SetColor(FatalForeColor, FatalBackColor);
            WriteMessage(message.ToString(), "FATAL");
            ResetColor();
        }

        private static void WriteMessage([NotNull] string message, [CanBeNull] string levelPrefix) {
            var now = DateTime.Now;
            var secondString = now.ToString("yyyy-MM-dd HH:mm:ss");
            Console.Write(secondString);
            if (levelPrefix != null) {
                Console.Write(" [{0}]", levelPrefix);
            }
            Console.Write(" : ");
            Console.WriteLine(message);
        }

        private void SetColor(ConsoleColor foreColor, ConsoleColor backColor) {
            _originalForeColor = Console.ForegroundColor;
            _originalBackColor = Console.BackgroundColor;
            Console.ForegroundColor = foreColor;
            Console.BackgroundColor = backColor;
        }

        private void ResetColor() {
            Console.ForegroundColor = _originalForeColor;
            Console.BackgroundColor = _originalBackColor;
        }

        private ConsoleColor _originalForeColor;
        private ConsoleColor _originalBackColor;

        public ConsoleColor DebugForeColor { get; set; } = ConsoleColor.Gray;
        public ConsoleColor DebugBackColor { get; set; } = ConsoleColor.Black;
        public ConsoleColor InfoForeColor { get; set; } = ConsoleColor.White;
        public ConsoleColor InfoBackColor { get; set; } = ConsoleColor.Black;
        public ConsoleColor WarnForeColor { get; set; } = ConsoleColor.Yellow;
        public ConsoleColor WarnBackColor { get; set; } = ConsoleColor.Black;
        public ConsoleColor ErrorForeColor { get; set; } = ConsoleColor.Red;
        public ConsoleColor ErrorBackColor { get; set; } = ConsoleColor.Black;
        public ConsoleColor FatalForeColor { get; set; } = ConsoleColor.White;
        public ConsoleColor FatalBackColor { get; set; } = ConsoleColor.DarkRed;

    }
}
