using System.Diagnostics;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace OpenMLTD.MillionDance.Utilities {
    internal static class UrlOpener {

        public static void OpenUrl([NotNull] string url) {
            var psi = new ProcessStartInfo();

            // A simple check about URLs cannot have common command line separators
            Debug.Assert(url.IndexOf(' ') < 0);
            Debug.Assert(url.IndexOf('\\') < 0);
            Debug.Assert(url.IndexOf('"') < 0);

            // https://stackoverflow.com/a/43232486
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                psi.FileName = url;
                psi.UseShellExecute = true;
            } else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
                psi.FileName = "xdg-open";
                psi.Arguments = url;
            } else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) {
                psi.FileName = "open";
                psi.Arguments = url;
            }

            using (Process.Start(psi)) {
            }
        }

    }
}
