using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using SharpDX.X3DAudio;

namespace BouncingBall.UI;

public class RuleSlider(RuleType ruleType, float initialValue = 0) : Slider<RuleChangeEventArgs>(false, initialValue) {
    
    public readonly RuleType ManagedRule = ruleType;

    public override void Draw(SpriteBatch spriteBatch, RectangleF region, Dictionary<FontType, BitmapFont> fonts) {
        base.Draw(spriteBatch, region, fonts);

        string valueStr = ((int) Value).ToString();
        spriteBatch.DrawString(
            fonts[FontType.NumberFont], 
            valueStr, 
            new(Bounds.X, Bounds.Bottom), 
            Color.White,
            region.ToRectangle()
        );

        string ruleStr = Util.AddSpaces(ManagedRule.ToString());
        spriteBatch.DrawString(
            fonts[FontType.SliderFont], 
            ruleStr, 
            new(Bounds.X - 5, Bounds.Top - fonts[FontType.SliderFont].LineHeight), 
            Color.White,
            region.ToRectangle()
        );
    }

    protected override RuleChangeEventArgs GetEventArgs() => new(ManagedRule, (int) Value);
}