using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Input.InputListeners;

namespace BouncingBall.UI;

public class ToggleSwitch(SimulationEventType type, RuleType managedRule, bool defaultState=false) : Widget, IUpdatable<RuleTriggerUpdateEventArgs> {
    
    public override RectangleF Bounds { 
        get => _bounds;
        set {
            _bounds = value;
            _knobSize.Height = (int) _bounds.Height;
        }
    }

    public int KnobWidth {
        get => _knobSize.Width;
        set => _knobSize.Width = value;
    }

    private RectangleF _bounds;
    private Size _knobSize;

    public Color OnColor = Color.DarkGray;
    public Color OffColor = new(80, 80, 80);
    public Color KnobColor = Color.LightGray;

    private readonly SimulationEventType _managedEvent = type;
    private readonly RuleType _managedRule = managedRule;

    public bool State = defaultState;

    public event EventHandler<RuleTriggerUpdateEventArgs> Updated;

    protected override void WhenDraw(SpriteBatch spriteBatch, Dictionary<FontType, BitmapFont> fonts) {
        spriteBatch.FillRectangle(Bounds, State ? OnColor : OffColor);
        spriteBatch.FillRectangle(
            new RectangleF(
                Bounds.X + (State ? Bounds.Width - _knobSize.Width : 0), 
                Bounds.Y,
                _knobSize.Width, 
                _knobSize.Height), 
            KnobColor
        );
    }

    public override InputListener[] GetListeners() {
        var listener = new MouseListener();
        listener.MouseDown += (sender, args) => {
            if (!Active || !Bounds.Contains(args.Position)) {
                return;
            }
            State = !State;
            Updated.Invoke(this, new RuleTriggerUpdateEventArgs(_managedRule, _managedEvent, State));
        };
        return [ listener ];
    }
}