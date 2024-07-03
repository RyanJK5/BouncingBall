using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Input.InputListeners;

namespace BouncingBall.UI;

public class RawContainer : Container {

    public RawContainer() {
        _drawables = [];
    }

    public override InputListener[] GetListeners() => [];

    public override void Draw(SpriteBatch spriteBatch, RectangleF region, Dictionary<FontType, BitmapFont> fonts) => DrawElements(spriteBatch, region, fonts);

    public void Add(IDrawable drawable) => _drawables.Add(drawable);

    public void Remove(IDrawable drawable) => _drawables.Remove(drawable);

    public void Clear() => _drawables.Clear();
}