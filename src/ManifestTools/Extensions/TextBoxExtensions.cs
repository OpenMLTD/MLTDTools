using System.Windows.Forms;
using JetBrains.Annotations;

namespace OpenMLTD.ManifestTools.Extensions {
    internal static class TextBoxExtensions {

        public static void SetPlaceholderText([NotNull] this TextBox textBox, [CanBeNull] string placeholder) {
            if (placeholder == null) {
                placeholder = string.Empty;
            }

            NativeMethods.SendMessage(textBox.Handle, NativeMethods.EM_SETCUEBANNER, 0, placeholder);
        }

    }
}
