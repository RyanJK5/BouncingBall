using System;
using System.Collections.Generic;
using BouncingBall;
using Melanchall.DryWetMidi.Core;

Dictionary<RuleType, float> rules = [];
bool defaultRules = false;

foreach (RuleType ruleType in Enum.GetValues<RuleType>()) {
    Console.Write(ruleType + ": ");
    string str = Console.ReadLine();
    if (str == "\\") {
        defaultRules = true;
        break;
    }
    rules[ruleType] = float.Parse(str);
}
Console.Write("MIDI File: ");
using var game = new Simulation(MidiFile.Read(Console.ReadLine()), defaultRules ? new() : new(rules));
game.Run();