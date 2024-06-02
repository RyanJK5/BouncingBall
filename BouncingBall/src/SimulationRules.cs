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

    public SimulationRules() : this(Util.CreateEnumDict<RuleType, float>(RuleTypes.DefaultValue)) { }
}