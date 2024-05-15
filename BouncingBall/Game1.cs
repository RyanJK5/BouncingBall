namespace BouncingBall;

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

public class Game1 : Game {
    
    private const float Gravity = 100;
    private const float InitialSpeed = 20;

    private const float Saturation = 1;
    private const float Value = 1;
    private const int HueIncrement = 2;

    private static readonly Random Random = new();
    
    private readonly GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Texture2D _circleTexture;

    private int _hue;

    private CircleF _outerCircle;

    private CircleF _ball;
    private Vector2 _circVelocity;
    

    private int WindowWidth => _graphics.PreferredBackBufferWidth;
    private int WindowHeight => _graphics.PreferredBackBufferHeight;


    public Game1() {
        _graphics = new GraphicsDeviceManager(this) {
            PreferredBackBufferWidth = 800,
            PreferredBackBufferHeight = 800
        };
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        _hue = 0;
        _circVelocity = new Vector2((float) Random.NextDouble(), (float) Random.NextDouble()) * InitialSpeed;
    }

    protected override void Initialize() {
        base.Initialize();
        _outerCircle = new CircleF(new(WindowWidth / 2, WindowHeight / 2), 300);
        _ball = new CircleF(_outerCircle.Position, 10);
        _circleTexture = GetCircleTexture((int) _ball.Radius);
    }

    protected override void LoadContent() {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
    }

    protected override void Update(GameTime gameTime) {
        base.Update(gameTime);
        UpdateCirclePos(gameTime);
        _hue = (_hue + HueIncrement) % 360;
    }

    private void UpdateCirclePos(GameTime gameTime) {
        _circVelocity.Y += gameTime.GetElapsedSeconds() * Gravity;
        
        _ball.Center += _circVelocity;
        float circDistance = (_ball.Center - _outerCircle.Center).Length();
        if (circDistance >= _outerCircle.Radius - _ball.Radius) {
            
            _ball.Center -= (_ball.Center - _outerCircle.Center).NormalizedCopy() * _circVelocity.Length();
            ReflectVelocity(_ball.Center);

            _ball.Radius++;
            _circleTexture = GetCircleTexture((int) _ball.Radius);
            
        }
    }

    protected override void Draw(GameTime gameTime) {
        Color color = ColorFromHSV(_hue, Saturation, Value);
        GraphicsDevice.Clear(Color.Black);

        _spriteBatch.Begin();
        _spriteBatch.DrawCircle(_outerCircle, 100, color, 5f);
        _spriteBatch.Draw(_circleTexture, _ball.ToRectangleF().Position, color);
        _spriteBatch.End();

        base.Draw(gameTime);
    }

    private void ReflectVelocity(Vector2 collisionPoint) {
        Vector2 normal = collisionPoint - (Vector2) _outerCircle.Center;
        normal.Normalize();
        Vector2 tangent = normal.PerpendicularCounterClockwise();
        float normalSpeed = -Vector2.Dot(normal, _circVelocity);
        float tangentSpeed = Vector2.Dot(tangent, _circVelocity);
        _circVelocity = new Vector2(normalSpeed * normal.X + tangentSpeed * tangent.X, normalSpeed * normal.Y + tangentSpeed * tangent.Y);
    }

    private Texture2D GetCircleTexture(int radius) {
        int diameter = radius * 2;
        var center = new Vector2(radius, radius);
        
        var result = new Texture2D(_spriteBatch.GraphicsDevice, diameter, diameter);
        Color[] data = new Color[diameter * diameter];
        for (var x = 0; x < diameter; x++) {
            for (var y = 0; y < diameter; y++) {
                var point = new Vector2(x, y);
                if ((point - center).Length() <= radius) {
                    data[x * diameter + y] = Color.White;
                    continue;
                }
            }
        }
        result.SetData(data);
        return result;
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