namespace OpenMLTD.MillionDance.Entities.Pmx {
    public sealed class SoftBodyConfig {

        internal SoftBodyConfig() {
        }

        public int AeroModel { get; internal set; }

        public float VCF { get; internal set; } = 1;

        public float DP { get; internal set; }

        public float DG { get; internal set; }

        public float LF { get; internal set; }

        public float PR { get; internal set; }

        public float VC { get; internal set; }

        public float DF { get; internal set; } = 0.2f;

        public float MT { get; internal set; }

        public float CHR { get; internal set; } = 1;

        public float KHR { get; internal set; } = 0.1f;

        public float SHR { get; internal set; } = 1;

        public float AHR { get; internal set; } = 0.7f;

        public float SRHR_CL { get; internal set; } = 0.1f;

        public float SKHR_CL { get; internal set; } = 1;

        public float SSHR_CL { get; internal set; } = 0.5f;

        public float SR_SPLT_CL { get; internal set; } = 0.5f;

        public float SK_SPLT_CL { get; internal set; } = 0.5f;

        public float SS_SPLT_CL { get; internal set; } = 0.5f;

        public int V_IT { get; internal set; }

        public int P_IT { get; internal set; } = 1;

        public int D_IT { get; internal set; }

        public int C_IT { get; internal set; } = 4;

    }
}
