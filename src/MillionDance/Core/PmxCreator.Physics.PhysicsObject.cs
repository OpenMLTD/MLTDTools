using JetBrains.Annotations;
using OpenMLTD.MillionDance.Entities.Pmx;

namespace OpenMLTD.MillionDance.Core {
    partial class PmxCreator {

        partial class Physics {

            public sealed class PhysicsObject {

                internal PhysicsObject([NotNull, ItemNotNull] PmxRigidBody[] bodies, [NotNull, ItemNotNull] PmxJoint[] joints) {
                    Bodies = bodies;
                    Joints = joints;
                }

                [NotNull, ItemNotNull]
                public PmxRigidBody[] Bodies { get; }

                [NotNull, ItemNotNull]
                public PmxJoint[] Joints { get; }

                public void Deconstruct([NotNull, ItemNotNull] out PmxRigidBody[] bodies, [NotNull, ItemNotNull] out PmxJoint[] joints) {
                    bodies = Bodies;
                    joints = Joints;
                }

            }

        }

    }
}
