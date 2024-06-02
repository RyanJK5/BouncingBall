namespace BouncingBall;

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using Melanchall.DryWetMidi.Core;
using BouncingBall.UI;
using MonoGame.Extended.Input.InputListeners;
using MonoGame.Extended.BitmapFonts;

#nullable enable

public class Simulation : Game {
    
    private const int BorderMargin = 10;

    private const float Saturation = 1;
    private const float Value = 1;
    private const int HueIncrement = 2;

    private static readonly Random Random = new();

    private readonly Dictionary<FontType, BitmapFont> _fonts;

    private SimulationRules _rules;

    private readonly GraphicsDeviceManager _graphics;


    private readonly List<Ball> _balls;
    private readonly List<IDrawable> _drawables;

    private SpriteBatch? _spriteBatch;
    
    private CircleF _outerCircle;

    private int _hue;

    public event EventHandler<BallEventArgs>? OuterCollision;

    public event EventHandler<BallEventArgs>? InnerCollision;

    private int WindowWidth => _graphics.PreferredBackBufferWidth;

    private int WindowHeight => _graphics.PreferredBackBufferHeight;

    public Simulation(MidiFile file) : base() {
        _graphics = new GraphicsDeviceManager(this) {
            PreferredBackBufferWidth = 450,
            PreferredBackBufferHeight = 1000
        };
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        _hue = 0;
        _balls = [];
        _drawables = [];
        _fonts = [];
        _rules = new();

        SoundHandler output = new(file);
        OuterCollision += (sender, args) => output.PlayNextNote();
        OuterCollision += (sender, args) => {
            foreach (Ball ball in args.Balls) {
                ball.Radius += args.Rules[RuleType.RadiusIncrement];
            }
        };
        InnerCollision += (sender, args) => _outerCircle.Radius += args.Rules[RuleType.OuterRadiusIncrement];
    }

    protected override void Initialize() {
        base.Initialize();

        int outerRadius = (WindowWidth - BorderMargin * 2) / 2;
        _outerCircle = new CircleF(new(WindowWidth / 2, outerRadius + BorderMargin * 4), outerRadius);

        bool oneBall = _rules[RuleType.BallCount] == 1;
        for (var i = 0; i < _rules[RuleType.BallCount]; i++) {
            float theta = (float) Random.NextDouble() * MathF.Tau;
            float distance = (float) Random.NextDouble() * (_outerCircle.Radius - _rules[RuleType.InitialRadius] * 3);
            
            _balls.Add(new Ball() {
                Center = new(
                    (oneBall ? distance * MathF.Sin(theta) : 0) + _outerCircle.Center.Y, 
                    (oneBall ? distance * MathF.Cos(theta) : 0) + _outerCircle.Center.X
                ), 
                Radius = _rules[RuleType.InitialRadius],
                Velocity = new Vector2(0, 1) * _rules[RuleType.InitialSpeed]
            });
        }

        RuleType[] ruleTypes = Enum.GetValues<RuleType>();
        for (var i = 0; i < ruleTypes.Length; i++) {
            Range<float> range = RuleTypes.DefaultRange(ruleTypes[i]);
            var slider = new RuleSlider(ruleTypes[i], _rules[ruleTypes[i]]) {
                Bounds = new RectangleF(
                    _outerCircle.Center.X - _outerCircle.Radius, 
                    _outerCircle.Center.Y + _outerCircle.Radius + 25 + i * 60,
                    100,
                    20
                ),
                KnobWidth = 10,
                MinValue = range.Min,
                MaxValue = range.Max
            };
            Components.Add(new InputListenerComponent(this, slider.GetListeners()));
            slider.Updated += (sender, args) => _rules = new SimulationRules(_rules, args.Rule, args.Value);
            _drawables.Add(slider);
        }
    }

    protected override void LoadContent() {
        foreach (FontType type in Enum.GetValues<FontType>()) {
            _fonts[type] = Content.Load<BitmapFont>("fonts/" + type.ToString().ToLower());
        }

        _spriteBatch = new SpriteBatch(GraphicsDevice);
    } 
        

    protected override void Update(GameTime gameTime) {
        base.Update(gameTime);
        _hue = (_hue + HueIncrement) % 360;

        foreach (var ball1 in _balls) {
            ball1.Update(gameTime, Random, _rules);

            foreach (var ball2 in _balls) {
                if (!ball2.CollidesWith(ball1)) {
                    continue;
                }
                InnerCollision?.Invoke(this, new BallEventArgs(SimulationEventType.BallOnBall, _rules, ball1, ball2));
                ball1.OnCollision(ball2);
            }

            if (ball1.CollidesWithOuter(_outerCircle)) {
                OuterCollision?.Invoke(this, new BallEventArgs(SimulationEventType.BallOnWall, _rules, ball1));
                ball1.OnCollision(_outerCircle);
                continue;
            }
        }
    }

    protected override void Draw(GameTime gameTime) {
        Color color = ColorFromHSV(_hue, Saturation, Value);
        if (_rules[RuleType.Redraw] == 1) {
            GraphicsDevice.Clear(Color.Black);
        }

        _spriteBatch?.Begin();

        _spriteBatch?.DrawCircle(_outerCircle, 100, color, 5f);
        
        foreach (var ball in _balls) {
            _spriteBatch?.Draw(ball.GetTexture(_spriteBatch.GraphicsDevice), ball.TopLeft, color);
            if (_rules[RuleType.Redraw] == 0) {
                _spriteBatch?.DrawCircle(ball.Bounds, 100, Color.White, 2.5f);
            }
        }

        foreach (var drawable in _drawables) {
            drawable.Draw(_spriteBatch, _fonts);
        }

        _spriteBatch?.End();

        base.Draw(gameTime);
    }

    private static Color ColorFromHSV(int h, float s, float v) {
        var rgb = new int[3];

        var baseColor = (h + 60) % 360 / 120;
        var shift = (h + 60) % 360 - (120 * baseColor + 60 );
        var secondaryColor = (baseColor + (shift >= 0 ? 1 : -1) + 3) % 3;
        
        rgb[baseColor] = 255;
        rgb[secondaryColor] = (int) (MathF.Abs(shift) / 60.0f * 255.0f);
        
        for (var i = 0; i < 3; i++) {
            rgb[i] += (int) ((255 - rgb[i]) * (1 - s));
        }
        for (var i = 0; i < 3; i++) {
            rgb[i] -= (int) (rgb[i] * (1 - v));
        }

        return new Color(rgb[0], rgb[1], rgb[2]);
    }
}