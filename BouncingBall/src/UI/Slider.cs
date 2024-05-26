using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Input.InputListeners;

namespace BouncingBall.UI;

public class Slider(RuleType ruleType, float initialValue=0) : Widget(ruleType) {
    
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

    public Size _knobSize;

    public Color SliderColor = Color.DarkGray;
    public Color KnobColor = Color.LightGray;

    public float MinValue;
    public float MaxValue = 1;

    private float _value = initialValue;

    private bool _dragging;

    public override event EventHandler<UIEventArgs> Updated;

    public override void Draw(SpriteBatch spriteBatch) {
        spriteBatch.FillRectangle(Bounds, SliderColor);
        spriteBatch.FillRectangle(
            new RectangleF(
                _value / (MaxValue - MinValue) * Bounds.Width + Bounds.X, 
                Bounds.Y, 
                _knobSize.Width, 
                _knobSize.Height
            ), 
            KnobColor
        );
    }

    public override InputListener[] GetListeners() {
        MouseListener listener = new();
        listener.MouseDown += (sender, args) => {
            if (!Bounds.Contains(args.Position)) {
                return;
            }
            SetValueFrom(args.Position.X);
        };

        listener.MouseDragStart += (sender, args) => _dragging = Bounds.Contains(args.Position);
        listener.MouseDragEnd += (sender, args) => _dragging = false;

        listener.MouseDrag += (sender, args) => {
            if (!_dragging && !Bounds.Contains(args.Position)) {
                return;
            }
            SetValueFrom(args.Position.X);
        };
        return [ listener ];
    }

    private void SetValueFrom(float xPos) {
        _value = MathF.Max(MinValue, MathF.Min(MaxValue, (xPos - Bounds.X) / Bounds.Width * MaxValue + MinValue));
        Updated.Invoke(this, new UIEventArgs(ManagedRule, _value));
    }
}