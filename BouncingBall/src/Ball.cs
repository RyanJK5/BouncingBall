using System;
using System.Drawing.Printing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace BouncingBall;

public class Ball {

    public CircleF Bounds;
    public Vector2 Velocity;

    public float Radius {
        get => Bounds.Radius;
        set => Bounds.Radius = value;
    }

    public Point2 Center {
        get => Bounds.Center;
        set => Bounds.Center = value;
    }

    public Point2 TopLeft {
        get => Bounds.ToRectangleF().TopLeft;
    }
    
    public void Update(GameTime gameTime, Random random, SimulationRules rules) {
        Velocity.Y += gameTime.GetElapsedSeconds() * rules.Gravity;
        
        if (MathF.Abs(Velocity.X) < 1) {
            int sign = random.NextDouble() > 0.5 ? 1 : -1;
            Velocity.X += sign * ((float) (random.NextDouble() * 5) + 1);
        }
        Center += Velocity;
    }

    public bool CollidesWith(Ball ball) => ball != this && (ball.Center - Center).Length() <= ball.Radius + Radius + 2;

    public bool CollidesWithOuter(CircleF outerCircle) =>(Center - outerCircle.Center).Length() >= outerCircle.Radius - Radius;


    public void OnCollision(Ball ball) {
        Vector2 thisCorrection = Velocity * 0.1f;
        Vector2 otherCorrection = ball.Velocity * 0.1f;

        while (CollidesWith(ball)) {
            Center -= thisCorrection;
            ball.Center -= otherCorrection;
        }

        ReflectVelocities(this, ball);
        Console.WriteLine((ball.Center - Center).Length());
    }

    public void OnCollision(CircleF outerCircle) {
        Center -= (Center - outerCircle.Center).NormalizedCopy() * Velocity.Length();
        ReflectVelocityOffOuter(outerCircle, Center);
    }

    private static void ReflectVelocities(Ball a, Ball b) {
        Vector2 _oldVelocity = a.Velocity;

        a.Velocity -= Vector2.Dot(a.Velocity - b.Velocity, a.Center - b.Center) / (a.Center - b.Center).LengthSquared() 
            * (a.Center - b.Center);
        b.Velocity -= Vector2.Dot(b.Velocity - _oldVelocity, b.Center - a.Center) / (b.Center - a.Center).LengthSquared() 
            * (b.Center - a.Center);
    }

    private void ReflectVelocityOffOuter(CircleF outer, Vector2 collisionPoint) {
        Vector2 normal = collisionPoint - (Vector2) outer.Center;
        normal.Normalize();
        Vector2 tangent = normal.PerpendicularCounterClockwise();
        float normalSpeed = -Vector2.Dot(normal, Velocity);
        float tangentSpeed = Vector2.Dot(tangent, Velocity);
        Velocity = new Vector2(
            normalSpeed * normal.X + tangentSpeed * tangent.X, 
            normalSpeed * normal.Y + tangentSpeed * tangent.Y
        );
    }

    public Texture2D GetTexture(GraphicsDevice graphics) {
        int diameter = (int) (Radius * 2);
        var center = new Vector2(Radius, Radius);
        
        var result = new Texture2D(graphics, diameter, diameter);
        Color[] data = new Color[diameter * diameter];
        for (var x = 0; x < diameter; x++) {
            for (var y = 0; y < diameter; y++) {
                var point = new Vector2(x, y);
                if ((point - center).Length() <= Radius) {
                    data[x * diameter + y] = Color.White;
                    continue;
                }
            }
        }
        result.SetData(data);
        return result;
    }
}