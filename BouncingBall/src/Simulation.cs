namespace BouncingBall;

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using Melanchall.DryWetMidi.Core;
using BouncingBall.UI;
using MonoGame.Extended.Input.InputListeners;
using System.Data;

#nullable enable

public class Simulation : Game {
    
    private const float Saturation = 1;
    private const float Value = 1;
    private const int HueIncrement = 2;

    private static readonly Random Random = new();

    private SimulationRules _rules;

    private readonly GraphicsDeviceManager _graphics;

    private SpriteBatch? _spriteBatch;

    private readonly List<Ball> _balls;
    private readonly List<Widget> _widgets;

    private CircleF _outerCircle;

    private int _hue;

    public event EventHandler<BallEventArgs>? OuterCollision;

    public event EventHandler<BallEventArgs>? InnerCollision;

    private int WindowWidth => _graphics.PreferredBackBufferWidth;

    private int WindowHeight => _graphics.PreferredBackBufferHeight;

    public Simulation(MidiFile file, SimulationRules rules) : base() {
        _graphics = new GraphicsDeviceManager(this) {
            PreferredBackBufferWidth = 800,
            PreferredBackBufferHeight = 800
        };
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        _hue = 0;
        _rules = rules;
        _balls = [];
        _widgets = [];

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

        _outerCircle = new CircleF(new(WindowWidth / 2, WindowHeight / 2), 300);

        for (var i = 0; i < _rules[RuleType.BallCount]; i++) {
            float theta = (float) Random.NextDouble() * MathF.Tau;
            float distance = (float) Random.NextDouble() * (_outerCircle.Radius - 50);
            _balls.Add(new Ball() {
                Bounds = new CircleF(new(
                    distance * MathF.Sin(theta) + _outerCircle.Center.Y, 
                    distance * MathF.Cos(theta) + _outerCircle.Center.X), 
                    _rules[RuleType.InitialRadius]
                ),
                Velocity = new Vector2(0, 1) * _rules[RuleType.InitialSpeed]
            });
        }
    }

    protected override void LoadContent() {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        var slider = new Slider(RuleType.Gravity, _rules[RuleType.Gravity]) {
            Bounds = new RectangleF(100, 100, 100, 10),
            KnobWidth = 10,
            MinValue = 0f,
            MaxValue = 100f
        };
        Components.Add(new InputListenerComponent(this, slider.GetListeners()));
        slider.Updated += (sender, args) => _rules = new SimulationRules(_rules, args.Rule, args.Value);
        _widgets.Add(slider);
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

        foreach (var widget in _widgets) {
            widget.Draw(_spriteBatch);
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