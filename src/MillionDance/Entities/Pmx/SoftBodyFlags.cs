using System;

namespace OpenMLTD.MillionDance.Entities.Pmx {
    [Flags]
    public enum SoftBodyFlags {

        None = 0x0,
        GenerateBendingLinks = 0x1,
        GenerateClusters = 0x2,
        RandomizeConstraints = 0x4

    }
}
