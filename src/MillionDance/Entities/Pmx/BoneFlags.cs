using System;

namespace MillionDance.Entities.Pmx {
    [Flags]
    public enum BoneFlags {

        None = 0x0,
        ToBone = 0x1,
        Rotation = 0x2,
        Translation = 0x4,
        Visible = 0x8,
        Enabled = 0x10,
        // ReSharper disable once InconsistentNaming
        IK = 0x20,
        Unknown = 0x40,
        AppendLocal = 0x80,
        AppendRotation = 0x100,
        AppendTranslation = 0x200,
        FixedAxis = 0x400,
        LocalFrame = 0x800,
        PhysicsAfterDeformation = 0x1000,
        ExternalParent = 0x2000

    }
}
