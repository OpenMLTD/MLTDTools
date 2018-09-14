using System;

namespace OpenMLTD.MillionDance.Entities.Pmx {
    [Flags]
    public enum MaterialFlags {

        None = 0x0,
        CullNone = 0x1,
        Shadow = 0x2,
        SelfShadowMap = 0x4,
        SelfShadow = 0x8,
        Edge = 0x10,
        VertexColor = 0x20,
        PointDraw = 0x40,
        LineDraw = 0x80

    }
}
