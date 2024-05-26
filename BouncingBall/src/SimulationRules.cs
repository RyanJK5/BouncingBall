using System.Collections.Generic;

namespace BouncingBall;

public readonly struct SimulationRules(Dictionary<RuleType, float> rules) {

    private readonly Dictionary<RuleType, float> _ruleLookup = rules;

    public float this[RuleType type] {
        get => _ruleLookup[type];
    }

    public SimulationRules(SimulationRules rules) : this(rules._ruleLookup) { }


    public SimulationRules(SimulationRules rules, Dictionary<RuleType, float> rulesToChange) : this(rules) {
        foreach (RuleType key in rulesToChange.Keys) {
            _ruleLookup[key] = rulesToChange[key];
        }
    }

    public SimulationRules(SimulationRules rules, RuleType ruleToChange, float newValue) : 
        this(rules, new Dictionary<RuleType, float> {{ ruleToChange, newValue }}) { }

    public SimulationRules(uint ballCount = 1, float initialSpeed = 5, float initialRadius = 20, float gravity = 40, float radiusIncrement = 0, 
        float outerRadiusIncrement = 0, float speedIncrement = 0, bool redraw = true) :
            this(new Dictionary<RuleType, float> { 
                { RuleType.BallCount, ballCount },
                { RuleType.InitialSpeed, initialSpeed },
                { RuleType.InitialRadius, initialRadius },
                { RuleType.Gravity, gravity },
                { RuleType.RadiusIncrement, radiusIncrement },
                { RuleType.OuterRadiusIncrement, outerRadiusIncrement },
                { RuleType.SpeedIncrement, speedIncrement},
                { RuleType.Redraw, redraw ? 1 : 0}
            }) {}

    public SimulationRules() : this(ballCount:1) { }
}