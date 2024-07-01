using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Input.InputListeners;

namespace BouncingBall.UI;

public class TabbedContainer : Widget, IUpdatable<MouseEventArgs> {

    private readonly Color TabColor = new(80, 80, 80);
    private readonly Color BoundsColor = new(40, 40, 40);

    private readonly Dictionary<RuleCategory, List<Widget>> _widgets;


    public RuleCategory ActiveTab {
        get => _activeTab;
        set {
            _activeTab = value;
            foreach (RuleCategory rule in _widgets.Keys) {
                foreach (Widget widget in _widgets[rule]) {
                    widget.Active = rule == ActiveTab;
                }
            }
        }
    }

    private RuleCategory _activeTab;
    private readonly Dictionary<RectangleF, RuleCategory> _tabs;
    
    public int TotalTabs => _widgets.Count;

    public event EventHandler<MouseEventArgs> Updated;

    public TabbedContainer(RectangleF bounds, Dictionary<RuleCategory, List<Widget>> tabs) {
        if (tabs.Keys.Count == 0) {
            throw new ArgumentException("pages must have a length greater than 0");
        }
        Bounds = bounds;
        _widgets = tabs;
        _tabs = [];
        Layer = -1;
        ActiveTab = 0;
    }

    protected override void WhenDraw(SpriteBatch spriteBatch, Dictionary<FontType, BitmapFont> fonts) {
        if (_tabs.Count != TotalTabs) {
            PopulateTabs(fonts);
        }

        BitmapFont font = fonts[FontType.NumberFont];
        foreach (RectangleF bounds in _tabs.Keys) {
            RuleCategory category = _tabs[bounds];
            string str = Util.AddSpaces(category.ToString());
            Color color = category == ActiveTab ? TabColor : BoundsColor;

            spriteBatch.FillRectangle(bounds, color);
            spriteBatch.DrawString(font, str, new(bounds.X - 2, bounds.Y), Color.White);
        }
        spriteBatch.FillRectangle(Bounds, BoundsColor);
    }

    public override InputListener[] GetListeners() {
        var listener = new MouseListener();
        listener.MouseDown += (sender, args) => {
            foreach (RectangleF tab in _tabs.Keys) {
                if (tab.Contains(args.Position)) {
                    ActiveTab = _tabs[tab];
                    Updated?.Invoke(this, args);
                    return;
                }
            }
        };

        return [ listener ];
    }

    private void PopulateTabs(Dictionary<FontType, BitmapFont> fonts) {
        _tabs.Clear();

        BitmapFont font = fonts[FontType.NumberFont];
        float xPos = Bounds.X;

        foreach (RuleCategory category in _widgets.Keys) {
            string str = Util.AddSpaces(category.ToString());
            Size2 strSize = font.MeasureString(str);

            var bounds = new RectangleF(new(xPos, Bounds.Y - strSize.Height), strSize);
            _tabs.Add(bounds, category);

            xPos += strSize.Width + 5;
        }
    }
}