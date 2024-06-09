using System;
using System.Linq;
using MonoGame.Extended;

namespace BouncingBall;

public enum RuleType {
    Gravity, 
    InitialBallCount,
    InitialSpeed,
    InitialBallRadius,
    
    RadiusIncrement,
    ContainerIncrement,
    SpeedIncrement
}

public static class RuleTypes {

    public static float DefaultValue(RuleType rule) =>
        rule switch {
            RuleType.InitialBallCount => 1,
            RuleType.Gravity => 40,
            RuleType.InitialSpeed => 5,
            RuleType.InitialBallRadius => 20,
            RuleType.RadiusIncrement => 0,
            RuleType.ContainerIncrement => 0,
            RuleType.SpeedIncrement => 0,
            _ => throw new  ArgumentException("Must provide non-null ruletype")
        }
    ;

    public static Range<float> DefaultRange(RuleType rule) =>
        rule switch {
            RuleType.InitialBallCount => new(0, 20),
            RuleType.Gravity => new(0, 100),
            RuleType.InitialSpeed => new(0, 50),
            RuleType.InitialBallRadius => new(1, 50),
            RuleType.RadiusIncrement => new(-10, 10),
            RuleType.ContainerIncrement => new(-10, 10),
            RuleType.SpeedIncrement => new(-10, 10),
            _ => throw new ArgumentException("Must provide non-null ruletype"),
        }
    ;

    public static RuleType[] InitialConditions() => Enum.GetValues<RuleType>().Where(InitialCondition).ToArray();

    public static RuleType[] DynamicConditions() => Enum.GetValues<RuleType>().Where(rule => !InitialCondition(rule)).ToArray();

    public static bool InitialCondition(RuleType rule) => 
        rule == RuleType.Gravity || rule == RuleType.InitialBallCount || rule == RuleType.InitialSpeed || rule == RuleType.InitialBallRadius
    ;
}