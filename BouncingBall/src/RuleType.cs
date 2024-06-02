using System;
using System.Collections.Generic;
using System.Data;
using MonoGame.Extended;

namespace BouncingBall;

public enum RuleType {
    Gravity, 
    InitialSpeed,
    InitialRadius,
    BallCount,
    RadiusIncrement,
    OuterRadiusIncrement,
    SpeedIncrement,
    Redraw
}

public static class RuleTypes {

    public static float DefaultValue(RuleType rule) =>
        rule switch {
            RuleType.BallCount => 1,
            RuleType.Gravity => 40,
            RuleType.InitialSpeed => 5,
            RuleType.InitialRadius => 20,
            RuleType.RadiusIncrement => 0,
            RuleType.OuterRadiusIncrement => 0,
            RuleType.SpeedIncrement => 0,
            RuleType.Redraw => 1,
            _ => throw new  ArgumentException("Must provide non-null ruletype")
        }
    ;

    public static Range<float> DefaultRange(RuleType rule) =>
        rule switch {
            RuleType.BallCount => new(0, 20),
            RuleType.Gravity => new(0, 100),
            RuleType.InitialSpeed => new(0, 50),
            RuleType.InitialRadius => new(0, 50),
            RuleType.RadiusIncrement => new(-10, 10),
            RuleType.OuterRadiusIncrement => new(-10, 10),
            RuleType.SpeedIncrement => new(-10, 10),
            RuleType.Redraw => new(0, 1),
            _ => throw new ArgumentException("Must provide non-null ruletype"),
        }
    ;
}