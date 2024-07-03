using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;

namespace BouncingBall.UI;

public abstract class Container : Widget {
    
    protected virtual List<IDrawable> _drawables { get; set; }

    protected void DrawElements(SpriteBatch spriteBatch, RectangleF region, Dictionary<FontType, BitmapFont> fonts) {
        List<IDrawable> remainingDrawables = [];
        remainingDrawables.AddRange(_drawables);
        int? layer = remainingDrawables.MinBy(drawable => drawable.Layer)?.Layer;
        while (remainingDrawables.Count > 0) {
            for (var i = 0; i < remainingDrawables.Count; i++) {
                var drawable = remainingDrawables[i];
                if (drawable.Layer == layer) {
                    DrawItem(drawable, spriteBatch, region, fonts);
                    remainingDrawables.Remove(drawable);
                    i--;
                }
            }
            layer++;
        }
    }

    protected virtual void DrawItem(IDrawable drawable, SpriteBatch spriteBatch, RectangleF region, Dictionary<FontType, BitmapFont> fonts) =>
        drawable.Draw(spriteBatch, region, fonts)
    ;
}