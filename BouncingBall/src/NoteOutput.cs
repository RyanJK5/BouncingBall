using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using Melanchall.DryWetMidi.Multimedia;

namespace BouncingBall;

public class NoteOutput {
    
    private IEnumerator<long> _enumerator;
    
    private readonly Dictionary<(TimedEvent, long), TimedEvent> _onOffPairs;
    private readonly OutputDevice _outputDevice;
    private readonly ICollection<TimedEvent> _events;
    private readonly TempoMap _map;


    public NoteOutput(MidiFile file) {
        _events = file.GetTimedEvents();
        _onOffPairs = [];
        _outputDevice = OutputDevice.GetByName("Microsoft GS Wavetable Synth");
        _map = file.GetTempoMap();
        CreateOnOffPairs();
    }

    private void CreateOnOffPairs() {
        List<long> times = [];
        List<TimedEvent> singlePairs = [];

        foreach (var evt in _events) {
            long time = evt.TimeAs<MetricTimeSpan>(_map).TotalMicroseconds;
            times.Add(time);
            if (evt.Event is NoteOnEvent) {
                singlePairs.Add(evt);
            }
            else if (evt.Event is NoteOffEvent off) {
                TimedEvent oEvt = singlePairs.Find(on => ((NoteEvent) on.Event).NoteNumber == off.NoteNumber);
                singlePairs.Remove(oEvt);
                _onOffPairs.TryAdd((oEvt, oEvt.TimeAs<MetricTimeSpan>(_map).TotalMicroseconds), evt);
            }
        }

        _enumerator = times.Distinct().GetEnumerator();
    }

    public void PlayNextNote() {
        if (!_enumerator.MoveNext()) {
            CreateOnOffPairs();
            _enumerator.MoveNext();
        }

        List<NoteOffEvent> offs = [];
        long offDelay = long.MaxValue;
        
        long time = _enumerator.Current;


        foreach (var evt in _events) {
            if (evt.Event is NoteOnEvent && evt.TimeAs<MetricTimeSpan>(_map).TotalMicroseconds == time) {
                _outputDevice.SendEvent(evt.Event);
                if (!_onOffPairs.ContainsKey((evt, time))) {
                    continue;
                }

                TimedEvent timedOff = _onOffPairs[(evt, time)];
                offs.Add((NoteOffEvent) timedOff.Event);
                offDelay = Math.Min(offDelay, timedOff.TimeAs<MetricTimeSpan>(_map).TotalMicroseconds - time);
            }
        }

        var task = new Task(() => { 
            Thread.Sleep((int) (offDelay / 1000));
            foreach (NoteOffEvent evt in offs) {
                _outputDevice.SendEvent(evt);
            }
        });
        task.Start();
    }
}