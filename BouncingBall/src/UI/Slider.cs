using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Input.InputListeners;

namespace BouncingBall.UI;

public abstract class Slider<T>(bool vertical, float initialValue=0) : Widget, IUpdatable<T> where T : EventArgs {
    
    public override RectangleF Bounds { 
        get => _bounds;
        set {
            _bounds = value;
            if (_vertical) {
                _knobSize.Width = (int) _bounds.Width;
                return;
            }
            _knobSize.Height = (int) _bounds.Height;
        }
    }

    public int KnobWidth {
        get => _vertical ? _knobSize.Height : _knobSize.Width;
        set {
            if (_vertical) {
                _knobSize.Height = value;
                return;
            }
            _knobSize.Width = value;
        }
    }

    private RectangleF _bounds;

    private Size _knobSize;

    public Color SliderColor = Color.DarkGray;
    public Color KnobColor = Color.LightGray;

    public float MinValue;
    public float MaxValue = 1;

    protected float Value = initialValue;

    private bool _dragging;
    private bool _vertical = vertical;

    public event EventHandler<T> Updated;

    protected override void WhenDraw(SpriteBatch spriteBatch, Dictionary<FontType, BitmapFont> fonts) {
        spriteBatch.FillRectangle(Bounds, SliderColor);
        spriteBatch.FillRectangle(
            new RectangleF(
                KnobX, 
                KnobY, 
                _knobSize.Width, 
                _knobSize.Height
            ), 
            KnobColor
        );
    }

    public override InputListener[] GetListeners() {
        var listener = new MouseListener();
        listener.MouseDown += (sender, args) => {
            if (!Bounds.Contains(args.Position) || !Active) {
                return;
            }
            SetValueFrom(args.Position);
        };

        listener.MouseDragStart += (sender, args) => _dragging = Bounds.Contains(args.Position);
        listener.MouseDragEnd += (sender, args) => _dragging = false;

        listener.MouseDrag += (sender, args) => {
            if (!Active || (!_dragging && !Bounds.Contains(args.Position))) {
                return;
            }
            SetValueFrom(args.Position);
        };
        return [ listener ];
    }

    private void SetValueFrom(Point2 pos) {
        float progression = _vertical ? (pos.Y - Bounds.Y) / (Bounds.Height - KnobWidth) : (pos.X - Bounds.X) / (Bounds.Width - KnobWidth);
        Value = Util.Clamp(MinValue, MaxValue, progression * (MaxValue - MinValue) + MinValue);
        Updated?.Invoke(this, GetEventArgs());
    }

    protected abstract T GetEventArgs();

    private float KnobX => _vertical ? Bounds.X : (Value - MinValue) / (MaxValue - MinValue) * (Bounds.Width - KnobWidth) + Bounds.X;

    private float KnobY => !_vertical ? Bounds.Y : (Value - MinValue) / (MaxValue - MinValue) * (Bounds.Height - KnobWidth) + Bounds.Y;
}