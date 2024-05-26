using System;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Input.InputListeners;

namespace BouncingBall.UI;

public abstract class Widget(RuleType managedRule) {

    public readonly RuleType ManagedRule = managedRule;

    public virtual RectangleF Bounds { get; set; }

    public abstract void Draw(SpriteBatch spriteBatch);

    public abstract InputListener[] GetListeners();

    public abstract event EventHandler<UIEventArgs> Updated;
}