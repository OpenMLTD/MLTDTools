using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace OpenMLTD.MiriTore {
    public static class ApplicationHelper {

        // https://stackoverflow.com/questions/1348643/how-performant-is-stackframe/1348853#1348853
        [MethodImpl(MethodImplOptions.NoInlining)]
        [NotNull]
        public static string GetCallerMethodName([CallerMemberName] string callerMemberName = null) {
            return callerMemberName;
        }

        [NotNull]
        public static string GetApplicationTitle() {
            return ApplicationTitle.Value;
        }

        [NotNull]
        public static string GetApplicationVersionString() {
            return ApplicationVersion.Value;
        }

        private static readonly Lazy<Assembly> ApplicationAssembly = new Lazy<Assembly>(Assembly.GetEntryAssembly);

        private static readonly Lazy<string> ApplicationTitle = new Lazy<string>(() => {
            var assembly = ApplicationAssembly.Value;
            var attr = assembly.GetCustomAttribute<AssemblyTitleAttribute>();

            return attr?.Title ?? string.Empty;
        });

        private static readonly Lazy<string> ApplicationVersion = new Lazy<string>(() => {
            var assembly = ApplicationAssembly.Value;

            var informationalVersionAttribute = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
            var version = informationalVersionAttribute?.InformationalVersion;

            if (!string.IsNullOrEmpty(version)) {
                return version;
            }

            var fileVersionAttribute = assembly.GetCustomAttribute<AssemblyFileVersionAttribute>();

            version = fileVersionAttribute?.Version;

            if (!string.IsNullOrEmpty(version)) {
                return version;
            }

            var versionAttribute = assembly.GetCustomAttribute<AssemblyVersionAttribute>();

            version = versionAttribute.Version;

            return version ?? string.Empty;
        });

    }
}
