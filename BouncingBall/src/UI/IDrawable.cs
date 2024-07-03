using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;

namespace BouncingBall.UI;

public interface IDrawable {
    public void Draw(SpriteBatch spriteBatch, RectangleF region, Dictionary<FontType, BitmapFont> fonts);

    public int Layer { get; set; }
}