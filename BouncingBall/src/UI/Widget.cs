using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Input.InputListeners;

namespace BouncingBall.UI;

public abstract class Widget : IDrawable {

    public bool Active = true;

    public virtual RectangleF Bounds { get; set; }

    public virtual int Layer { get; set; }

    public abstract InputListener[] GetListeners();

    protected abstract void WhenDraw(SpriteBatch spriteBatch, Dictionary<FontType, BitmapFont> fonts);

    public void Draw(SpriteBatch spriteBatch, Dictionary<FontType, BitmapFont> fonts) {
        if (Active) {
            WhenDraw(spriteBatch, fonts);
        }
    }
}