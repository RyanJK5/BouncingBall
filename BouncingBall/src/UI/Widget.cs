using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Input.InputListeners;

namespace BouncingBall.UI;

public abstract class Widget<T> : IDrawable where T : EventArgs {

    public virtual RectangleF Bounds { get; set; }

    public abstract InputListener[] GetListeners();

    public abstract void Draw(SpriteBatch spriteBatch, Dictionary<FontType, BitmapFont> fonts);

    public abstract event EventHandler<T> Updated;
}