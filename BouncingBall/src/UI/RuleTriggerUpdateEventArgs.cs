using System;

namespace BouncingBall.UI;

public class RuleTriggerUpdateEventArgs(RuleType rule, SimulationEventType trigger, bool state) : EventArgs {
    public readonly RuleType Rule = rule;
    public readonly SimulationEventType Trigger = trigger;
    public readonly bool State = state;
}