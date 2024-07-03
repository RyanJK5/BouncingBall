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

    private readonly SoundHandler _soundHandler;

    private readonly List<Ball> _balls;
    private readonly RawContainer _screen;

    private SpriteBatch? _spriteBatch;
    
    private CircleF _ballContainer;

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
        _fonts = [];
        _rules = new();

        _screen = new() {
            Bounds = new(0, 0, WindowWidth, WindowHeight)
        };

        _soundHandler = new(file);
        // OuterCollision += (sender, args) => _soundHandler.PlayNextNote();
        InnerCollision += (sender, args) => _ballContainer.Radius = Math.Max(1, _ballContainer.Radius + args.Rules[RuleType.ContainerIncrement]);
    }

    protected override void Initialize() {
        base.Initialize();

        RestartSimulation();
        InitializeSliders();

        Texture2D buttonTexture = Content.Load<Texture2D>("img/restartbutton");
        var button = new Button(buttonTexture) {
            Bounds = new(
                _ballContainer.Center.X + _ballContainer.Radius - buttonTexture.Width,
                _ballContainer.Center.Y + _ballContainer.Radius - buttonTexture.Height, 
                buttonTexture.Width, 
                buttonTexture.Height
            )
        };
        button.Updated += (sender, args) => RestartSimulation();
        Components.Add(new InputListenerComponent(this, button.GetListeners()));
        _screen.Add(button);
    }

    private void InitializeBalls() {
        int ballCount = (int) _rules[RuleType.InitialBallCount];
        for (var i = 0; i < ballCount; i++) {
            float theta = i * (MathF.Tau / ballCount);
            float distance = ballCount > 1 ? _rules[RuleType.InitialBallRadius] * ballCount * 0.6f : 0;
            _balls.Add(new Ball() {
                Center = new(
                    distance * MathF.Cos(theta) + _ballContainer.Center.X,
                    distance * MathF.Sin(theta) + _ballContainer.Center.Y 
                ), 
                Radius = _rules[RuleType.InitialBallRadius],
                Velocity = new Vector2(0, 1) * _rules[RuleType.InitialSpeed]
            });
        }
    }

    private void InitializeSliders() {
        var scroll = new ScrollBar(true) {
                Bounds = new RectangleF(
                    WindowWidth - 10, 
                    _ballContainer.Center.Y + _ballContainer.Radius + 25, 
                    10, 
                    WindowHeight - _ballContainer.Center.Y - _ballContainer.Radius - 25
                ),
                KnobWidth = 20
        };
        Components.Add(new InputListenerComponent(this, scroll.GetListeners()));
        
        var dict = new Dictionary<RuleCategory, List<Widget>>() {
            { RuleCategory.Initial, CreateRuleControlList(RuleTypes.InitialConditions()) },
            { RuleCategory.Simulation, CreateRuleControlList(RuleTypes.DynamicConditions()) },
            { RuleCategory.Graphics, []},
            { RuleCategory.Audio, [] }
        };

        var container = new TabbedContainer(
            new(
                0,
                _ballContainer.Center.Y + _ballContainer.Radius + 25,
                WindowWidth,
                WindowHeight - _ballContainer.Center.Y - _ballContainer.Radius - 25
            ),
            dict
        );
        Components.Add(new InputListenerComponent(this, container.GetListeners()));
        _screen.Add(container);
    }

    private List<Widget> CreateRuleControlList(RuleType[] rules) {
        List<Widget> result = [];
        SimulationEventType[] triggers = Enum.GetValues<SimulationEventType>();

        for (var i = 0; i < rules.Length; i++) {
            float yPos = _ballContainer.Center.Y + _ballContainer.Radius + (i + 1) * 60;
            RuleSlider slider = CreateSlider(
                rules[i], 
                RuleTypes.DefaultRange(rules[i]), 
                yPos
            );
            result.Add(slider);

            var bar = new ScrollBar(true) {
                Bounds = new RectangleF(
                    WindowWidth - 10, 
                    _ballContainer.Center.Y + _ballContainer.Radius + 25, 
                    10, 
                    WindowHeight - _ballContainer.Center.Y - _ballContainer.Radius - 25
                ),
                KnobWidth = 20
            };
            result.Add(bar);
            Components.Add(new InputListenerComponent(this, bar.GetListeners()));

            if (RuleTypes.InitialCondition(rules[i])) {
                continue;
            }

            for (var j = 0; j < triggers.Length; j++) {
                result.Add(CreateSwitch(
                    rules[i], 
                    triggers[j], 
                    new Vector2(slider.Bounds.X + 200 + j * 75, yPos))
                );
            }
        }
        return result;
    }

    private RuleSlider CreateSlider(RuleType rule, Range<float> ruleRange, float yPos) {
        var slider = new RuleSlider(rule, _rules[rule]) {
                Bounds = new RectangleF(
                    _ballContainer.Center.X - _ballContainer.Radius, 
                    yPos,
                    100,
                    20
                ),
                KnobWidth = 10,
                MinValue = ruleRange.Min,
                MaxValue = ruleRange.Max
        };
        Components.Add(new InputListenerComponent(this, slider.GetListeners()));
        slider.Updated += (sender, args) => _rules = new SimulationRules(_rules, args.Rule, args.Value);
        return slider;    
    }

    private ToggleSwitch CreateSwitch(RuleType rule, SimulationEventType trigger, Vector2 position) {
        var toggleSwitch = new ToggleSwitch(trigger, rule) {
            Bounds = new RectangleF(position.X, position.Y, 50, 20),
            KnobWidth = 20
        };
        toggleSwitch.Updated += HandleSwitchUpdate;
        Components.Add(new InputListenerComponent(this, toggleSwitch.GetListeners()));
        return toggleSwitch;
    }

    private void HandleSwitchUpdate(object? sender, RuleTriggerUpdateEventArgs args) {
        if (args.State) {
                switch (args.Trigger) {
                    case SimulationEventType.BallOnWall:
                        OuterCollision += GetEventFor(args.Rule);
                        return;
                    case SimulationEventType.BallOnBall:
                        InnerCollision += GetEventFor(args.Rule);
                        return;
                }
            }
            switch (args.Trigger) {
                case SimulationEventType.BallOnWall:
                    OuterCollision -= GetEventFor(args.Rule);
                    return;
                case SimulationEventType.BallOnBall:
                    InnerCollision -= GetEventFor(args.Rule);
                    return;
            }
    }

    private EventHandler<BallEventArgs> GetEventFor(RuleType rule) =>
        rule switch {
            RuleType.SpeedIncrement => (sender, args) => {
                foreach (Ball ball in args.Balls) {
                    ball.Velocity += new Vector2(args.Rules[RuleType.SpeedIncrement]);
                }
            },
            RuleType.RadiusIncrement => (sender, args) => {
                foreach (Ball ball in args.Balls) {
                    ball.Radius += args.Rules[RuleType.RadiusIncrement];
                }
            },
            RuleType.ContainerIncrement => (sender, args) => _ballContainer.Radius = Math.Max(1, _ballContainer.Radius + args.Rules[RuleType.ContainerIncrement]),
            _ => throw new ArgumentException("rule must be dynamic"),
        }
    ;

    private void RestartSimulation() {
        _soundHandler.Restart();
        GraphicsDevice.Clear(Color.Black);

        int outerRadius = (WindowWidth - BorderMargin * 2) / 2;
        _ballContainer = new CircleF(new(WindowWidth / 2, outerRadius + BorderMargin * 4), outerRadius);
        
        _balls.Clear();
        InitializeBalls();
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

            if (ball1.CollidesWithOuter(_ballContainer)) {
                OuterCollision?.Invoke(this, new BallEventArgs(SimulationEventType.BallOnWall, _rules, ball1));
                ball1.OnCollision(_ballContainer);
                continue;
            }
        }
    }

    protected override void Draw(GameTime gameTime) {
        Color color = Util.ColorFromHSV(_hue, Saturation, Value);
        GraphicsDevice.Clear(Color.Black);

        _spriteBatch?.Begin();

        _spriteBatch?.DrawCircle(_ballContainer, 100, color, 5f);
        
        foreach (var ball in _balls) {
            _spriteBatch?.Draw(ball.GetTexture(_spriteBatch.GraphicsDevice), ball.TopLeft, color);
        }

        _screen.Draw(_spriteBatch, _screen.Bounds, _fonts);

        _spriteBatch?.End();

        base.Draw(gameTime);
    }
}