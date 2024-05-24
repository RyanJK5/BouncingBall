using System;

namespace BouncingBall;

public class BallEventArgs(SimulationEventType type, SimulationRules rules, params Ball[] balls) : EventArgs {
    
    public readonly SimulationEventType Type = type;

    public readonly SimulationRules Rules = rules;

    public readonly Ball[] Balls = balls;
}