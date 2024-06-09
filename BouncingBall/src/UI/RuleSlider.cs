using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Input.InputListeners;

namespace BouncingBall.UI;

public class RuleSlider(RuleType ruleType, float initialValue=0) : Widget, IUpdatable<RuleChangeEventArgs> {
    
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

    public readonly RuleType ManagedRule = ruleType;

    private RectangleF _bounds;

    public Size _knobSize;

    public Color SliderColor = Color.DarkGray;
    public Color KnobColor = Color.LightGray;

    public float MinValue;
    public float MaxValue = 1;

    private float _value = initialValue;

    private bool _dragging;

    public event EventHandler<RuleChangeEventArgs> Updated;

    protected override void WhenDraw(SpriteBatch spriteBatch, Dictionary<FontType, BitmapFont> fonts) {
        spriteBatch.FillRectangle(Bounds, SliderColor);
        spriteBatch.FillRectangle(
            new RectangleF(
                KnobX, 
                Bounds.Y, 
                _knobSize.Width, 
                _knobSize.Height
            ), 
            KnobColor
        );
        
        string str = _value.ToString();
        int dotindex = str.IndexOf('.');
        str = dotindex > 0 ? str[..dotindex] : str;
        spriteBatch.DrawString(fonts[FontType.NumberFont], str, new(_bounds.X, _bounds.Bottom), Color.White);
        
        spriteBatch.DrawString(
            fonts[FontType.SliderFont], 
            Util.AddSpaces(ManagedRule.ToString()), 
            new(_bounds.X - 5, _bounds.Top - fonts[FontType.SliderFont].LineHeight), 
            Color.White
        );
    }

    public override InputListener[] GetListeners() {
        var listener = new MouseListener();
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
        _value = Util.Clamp(MinValue, MaxValue, (xPos - Bounds.X) / Bounds.Width * (MaxValue - MinValue) + MinValue);
        Updated?.Invoke(this, new RuleChangeEventArgs(ManagedRule, _value));
    }

    private float KnobX => (_value - MinValue) / (MaxValue - MinValue) * Bounds.Width + Bounds.X;
}