using System;
using BouncingBall;
using Melanchall.DryWetMidi.Core;

Console.Write("Ball Count: ");
var count = uint.Parse(Console.ReadLine());
Console.Write("Gravity: ");
var grav = float.Parse(Console.ReadLine());
Console.Write("MIDI File: ");

using var game = new Simulation(MidiFile.Read(Console.ReadLine()), new(ballCount:count, gravity:grav, radiusIncrement:0));
game.Run();