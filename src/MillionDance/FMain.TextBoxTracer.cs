using System.Diagnostics;
using JetBrains.Annotations;

namespace OpenMLTD.MillionDance {
    partial class FMain {

        internal sealed class TextBoxTracer : TraceListener {

            public TextBoxTracer([NotNull] FMain form) {
                _form = form;
            }

            public override void Write(string message) {
                WriteLine(message);
            }

            public override void WriteLine(string message) {
                if (message != null) {
                    _form.Log(message);
                }
            }

            [NotNull]
            private readonly FMain _form;

        }

    }
}
