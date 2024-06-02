using System.Collections.Generic;
using BouncingBall.UI;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;

namespace BouncingBall;

public interface IDrawable {
    public void Draw(SpriteBatch spriteBatch, Dictionary<FontType, BitmapFont> fonts);

}