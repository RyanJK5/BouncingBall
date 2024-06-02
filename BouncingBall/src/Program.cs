using System;
using BouncingBall;
using Melanchall.DryWetMidi.Core;

Console.Write("MIDI File: ");
using var game = new Simulation(MidiFile.Read(Console.ReadLine()));
game.Run();