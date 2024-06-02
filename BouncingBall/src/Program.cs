using BouncingBall;
using Melanchall.DryWetMidi.Core;

using var game = new Simulation(MidiFile.Read("D:/Downloads/fallen_down.mid"));
game.Run();