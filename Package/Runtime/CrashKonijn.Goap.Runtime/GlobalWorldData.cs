﻿using System;
using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Core;

namespace CrashKonijn.Goap.Runtime
{
    public class GlobalWorldData : WorldDataBase, IGlobalWorldData
    {
        public override (bool Exists, int Value) GetWorldValue(Type worldKey)
        {
            if (!this.States.ContainsKey(worldKey))
                return (false, 0);
            
            return (true, this.States[worldKey]);
        }

        public override ITarget GetTargetValue(Type targetKey)
        {
            if (!this.Targets.ContainsKey(targetKey))
                return null;
            
            return this.Targets[targetKey];
        }
    }
}