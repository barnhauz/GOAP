using CrashKonijn.Agent.Core;
using UnityEngine;
using PathfindingExtension;

namespace CrashKonijn.Goap.Runtime
{
    public class TransformTarget : ITarget
    {
        public Transform Transform { get; private set; }

        public Vector3 Position
        {
            get
            {
                if (this.Transform == null)
                    return Vector3.zero;

                return this.Transform.position;
            }
        }

        public DestinationPoint DestinationPoint { get; }

        public TransformTarget(Transform transform)
        {
            this.Transform = transform;
        }

        public TransformTarget SetTransform(Transform transform)
        {
            this.Transform = transform;
            return this;
        }

        public bool IsValid()
        {
            return this.Transform != null;
        }
    }
}
