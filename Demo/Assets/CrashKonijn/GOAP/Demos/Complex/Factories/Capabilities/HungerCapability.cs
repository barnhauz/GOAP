﻿using CrashKonijn.Goap.Core;
using CrashKonijn.Goap.Demos.Complex.Actions;
using CrashKonijn.Goap.Demos.Complex.Classes.Items;
using CrashKonijn.Goap.Demos.Complex.Factories.Extensions;
using CrashKonijn.Goap.Demos.Complex.Goals;
using CrashKonijn.Goap.Demos.Complex.Interfaces;
using CrashKonijn.Goap.Demos.Complex.Sensors.World;
using CrashKonijn.Goap.Demos.Complex.Targets;
using CrashKonijn.Goap.Demos.Complex.WorldKeys;
using CrashKonijn.Goap.Runtime;

namespace CrashKonijn.Goap.Demos.Complex.Factories.Capabilities
{
    public class HungerCapability : CapabilityFactoryBase
    {
        public override ICapabilityConfig Create()
        {
            var builder = new CapabilityBuilder("HungerCapability");

            // Goals
            builder.AddGoal<FixHungerGoal>()
                .AddCondition<Hunger>(Comparison.SmallerThanOrEqual, 0);
            
            // Actions
            builder.AddAction<EatAction>()
                .SetTarget<Targets.TransformTarget>()
                .AddEffect<Hunger>(EffectType.Decrease)
                .AddCondition<IsHolding<IEatable>>(Comparison.GreaterThanOrEqual, 1)
                .AddCondition<Hunger>(Comparison.GreaterThanOrEqual, 30)
                .SetValidateConditions(false); // We don't need to validate conditions for this action, or it will stop when becoming below 80 hunger
            
            builder.AddAction<GatherItemAction<Apple>>()
                .SetTarget<ClosestSourceTarget<Apple>>()
                .AddEffect<IsHolding<IEatable>>(EffectType.Increase)
                .SetBaseCost(3)
                .SetProperties(new GatherItemAction<Apple>.Props
                {
                    pickupItem = true,
                    timer = 3
                });
            
            builder.AddPickupItemAction<IEatable>();
            
            // Target sensors
            builder.AddClosestItemTargetSensor<IEatable>();
            builder.AddClosestSourceTargetSensor<Apple>();
            
            // World sensors
            builder.AddIsHoldingSensor<IEatable>();
            builder.AddIsInWorldSensor<IEatable>();
            builder.AddWorldSensor<HungerSensor>()
                .SetKey<Hunger>();

            return builder.Build();
        }
    }
}