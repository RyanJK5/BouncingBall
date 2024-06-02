using System;

namespace BouncingBall.UI;

public class RuleChangeEventArgs(RuleType rule, float value) : EventArgs {
    
    public readonly RuleType Rule = rule;

    public readonly float Value = value;

}