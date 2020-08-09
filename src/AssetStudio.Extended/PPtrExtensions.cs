using System.Text;
using JetBrains.Annotations;

namespace AssetStudio.Extended {
    public static class PPtrExtensions {

        [NotNull]
        public static string GetDebugDescription<T>([NotNull] this PPtr<T> p)
            where T : Object {
            var t = typeof(T);

            var sb = new StringBuilder();

            sb.AppendFormat("PPtr<{0}> {{ FileID: {1}, PathID: {2} }}", t.Name, p.m_FileID.ToString(), p.m_PathID.ToString());

            if (p.IsNull) {
                sb.Append(" (null)");
            }

            return sb.ToString();
        }

    }
}
