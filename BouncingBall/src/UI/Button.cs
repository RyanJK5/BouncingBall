using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Input.InputListeners;

namespace BouncingBall.UI;

public class Button : Widget, IUpdatable<EventArgs> {

    private readonly Texture2D _texture;

    public event EventHandler<EventArgs> Updated;

    public Button(Texture2D texture) {
        _texture = texture;
        Bounds = texture.Bounds;
    }

    protected override void WhenDraw(SpriteBatch spriteBatch, Dictionary<FontType, BitmapFont> fonts) => spriteBatch.Draw(_texture, Bounds.Position, Color.White);

    public override InputListener[] GetListeners() {
        var listener = new MouseListener();

        listener.MouseDown += (sender, args) => {
            if (Active && Bounds.Contains(args.Position)) {
                Updated?.Invoke(this, new());
            }
        };

        return [ listener ];
    }
}