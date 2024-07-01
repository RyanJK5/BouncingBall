using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;

namespace BouncingBall.UI;

public class RuleSlider(RuleType ruleType, float initialValue = 0) : Slider<RuleChangeEventArgs>(initialValue) {
    
    public readonly RuleType ManagedRule = ruleType;

    protected override void WhenDraw(SpriteBatch spriteBatch, Dictionary<FontType, BitmapFont> fonts) {
        base.WhenDraw(spriteBatch, fonts);
        spriteBatch.DrawString(fonts[FontType.NumberFont], ((int) Value).ToString(), new(Bounds.X, Bounds.Bottom), Color.White);
        spriteBatch.DrawString(
            fonts[FontType.SliderFont], 
            Util.AddSpaces(ManagedRule.ToString()), 
            new(Bounds.X - 5, Bounds.Top - fonts[FontType.SliderFont].LineHeight), 
            Color.White
        );
    }

    protected override RuleChangeEventArgs GetEventArgs() => new(ManagedRule, (int) Value);
}