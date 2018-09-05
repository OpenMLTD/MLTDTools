using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace MillionDance {
    internal static class ApplicationHelper {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [NotNull]
        public static string GetApplicationTitle() {
            return ApplicationTitle.Value;
        }

        private static readonly Lazy<string> ApplicationTitle = new Lazy<string>(() => {
            var assembly = Assembly.GetAssembly(typeof(ApplicationHelper));
            var titleAttr = assembly.GetCustomAttribute<AssemblyTitleAttribute>();

            return titleAttr?.Title ?? string.Empty;
        });

    }
}
