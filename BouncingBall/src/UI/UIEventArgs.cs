namespace BouncingBall.UI;

public readonly struct UIEventArgs(RuleType rule, float value) {
    
    public readonly RuleType Rule = rule;

    public readonly float Value = value;

}