namespace BouncingBall;

using System;
using System.Windows.Forms;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using BouncingBall.UI;

public class BallWindow : Game {
    
    private const float Gravity = 40;
    private const float InitialSpeed = 5;

    private const float Saturation = 1;
    private const float Value = 1;
    private const int HueIncrement = 2;
    private const int RadiusIncrement = 1;
    private const float SpeedFactor = 1.01f;

    private static readonly Random Random = new();
    
    private readonly GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private List<IElement> _ui;

    private List<Ball> _balls;

    private CircleF _outerCircle;

    private int _hue;

    private int WindowWidth => _graphics.PreferredBackBufferWidth;
    private int WindowHeight => _graphics.PreferredBackBufferHeight;


    public BallWindow() {
        _graphics = new GraphicsDeviceManager(this) {
            PreferredBackBufferWidth = 800,
            PreferredBackBufferHeight = 800
        };
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        _hue = 0;

        TextBox box = new()
        {
            Location = new System.Drawing.Point(100, 100),
            Size = new(50, 100),
            Text = "Hello"
        };
    }

    protected override void Initialize() {
        base.Initialize();

        _outerCircle = new CircleF(new(WindowWidth / 2, WindowHeight / 2), 300);

        _balls = [
            new Ball() {
                Bounds = new CircleF(new(_outerCircle.Position.X, _outerCircle.Position.Y - 80), 10),
                Velocity = new Vector2(0, 1) * InitialSpeed
            },
        ];

        _ui = [];
    }

    protected override void LoadContent() => _spriteBatch = new SpriteBatch(GraphicsDevice);

    protected override void Update(GameTime gameTime) {
        base.Update(gameTime);
        _hue = (_hue + HueIncrement) % 360;

        foreach (var ball1 in _balls) {
            ball1.Update(gameTime, Random, new PhysicsRules() { Gravity = Gravity });
            if (ball1.CollidesWithOuter(_outerCircle)) {
                OuterCollisionEffect(ball1);
                continue;
            }
            foreach (var ball2 in _balls) {
                if (ball2.CollidesWith(ball1)) {
                    InnerCollisionEffect(ball1, ball2);
                    ball2.OnCollision(ball1);
                }
            }
        }
    }

    private static void OuterCollisionEffect(Ball ball) {
        ball.Radius += RadiusIncrement; 
    }

    private static void InnerCollisionEffect(Ball ball1, Ball ball2) {
        ball1.Radius += RadiusIncrement;
        ball2.Radius += RadiusIncrement;
    }

    protected override void Draw(GameTime gameTime) {
        Color color = ColorFromHSV(_hue, Saturation, Value);
        GraphicsDevice.Clear(Color.Black);

        _spriteBatch.Begin();

        _spriteBatch.DrawCircle(_outerCircle, 100, color, 5f);
        
        foreach (var ball in _balls) {
            _spriteBatch.Draw(ball.GetTexture(_spriteBatch.GraphicsDevice), ball.TopLeft, color);
        }

        foreach (var ui in _ui) {
        }

        _spriteBatch.End();

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