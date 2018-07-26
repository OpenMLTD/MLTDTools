using OpenTK;

namespace MillionDanceView {
    internal static class EntryPoint {

        private static void Main() {
            const int updateRate = 0;
            const int drawRate = 0;

            using (var window = new RenderForm()) {
                window.VSync = VSyncMode.Adaptive;
                window.Run(updateRate, drawRate);
            }
        }

    }
}
