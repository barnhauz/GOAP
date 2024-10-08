using CrashKonijn.Agent.Core;
using UnityEngine;

namespace CrashKonijn.Goap.Runtime
{
    public class PositionTarget : ITarget
    {
        public Vector3 Position { get; private set; }
        public DestinationPoint DestinationPoint { get; }

        public PositionTarget(Vector3 position)
        {
            this.Position = position;
            DestinationPoint = null;
        }

        public PositionTarget(DestinationPoint destinationPoint)
        {
            DestinationPoint = destinationPoint;
            Position = destinationPoint == null ? default : destinationPoint.Position;
        }

        public PositionTarget SetPosition(Vector3 position)
        {
            this.Position = position;
            return this;
        }

        public bool IsValid()
        {
            return true;
        }
    }
}
