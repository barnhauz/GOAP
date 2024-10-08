using UnityEngine;

namespace CrashKonijn.Agent.Core
{
    public interface ITarget
    {
        public Vector3 Position { get; }
        public bool IsValid();
        public DestinationPoint DestinationPoint { get; }
    }
}