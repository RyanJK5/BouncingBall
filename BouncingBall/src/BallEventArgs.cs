namespace BouncingBall;

public readonly struct BallEventArgs(SimulationEventType type, SimulationRules rules, params Ball[] balls) {
    
    public readonly SimulationEventType Type = type;

    public readonly SimulationRules Rules = rules;

    public readonly Ball[] Balls = balls;
}