using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Input.InputListeners;

namespace BouncingBall.UI;

public class TabbedContainer : Container, IUpdatable<MouseEventArgs> {

    private readonly Color TabColor = new(80, 80, 80);
    private readonly Color BoundsColor = new(40, 40, 40);

    private Dictionary<RuleCategory, Tab> _tabs;
    private readonly Dictionary<RectangleF, RuleCategory> _boundstoRules;

    public RuleCategory ActiveTab {
        get => _activeTab;
        set {
            _activeTab = value;
            foreach (RuleCategory rule in _tabs.Keys) {
                foreach (Widget widget in _tabs[rule].Widgets) {
                    widget.Active = widget.Bounds.Y >= Bounds.Y && rule == ActiveTab;
                }
            }
        }
    }

    protected override List<IDrawable> _drawables { 
        get => [.. _tabs[ActiveTab].Widgets];
    }

    private RuleCategory _activeTab;
    
    public int TotalTabs => _tabs.Count;

    public event EventHandler<MouseEventArgs> Updated;

    public TabbedContainer(RectangleF bounds, Dictionary<RuleCategory, List<Widget>> tabs) {
        if (tabs.Keys.Count == 0) {
            throw new ArgumentException("pages must have a length greater than 0");
        }
        Bounds = bounds;
        
        _tabs = [];
        foreach (RuleCategory rule in tabs.Keys) {
            _tabs.Add(rule, new Tab() {
                Widgets = tabs[rule]
            });
        }

        _boundstoRules = [];
        Layer = -1;
        ActiveTab = 0;
        CreateScrollListeners();
    }

    private void CreateScrollListeners() {
        foreach (RuleCategory tab in _tabs.Keys) {
            foreach (Widget widget in _tabs[tab].Widgets) {
                if (widget is ScrollBar scrollBar) {
                    scrollBar.Updated += (sender, args) => Scroll(tab, args.Displacement);
                }
            }
        }
    }

    public override void Draw(SpriteBatch spriteBatch, RectangleF region, Dictionary<FontType, BitmapFont> fonts) {
        if (_boundstoRules.Count != TotalTabs) {
            PopulateBoundsToRules(fonts);
        }

        BitmapFont font = fonts[FontType.NumberFont];
        foreach (RectangleF bounds in _boundstoRules.Keys) {
            RuleCategory category = _boundstoRules[bounds];
            string str = Util.AddSpaces(category.ToString());
            Color color = category == ActiveTab ? TabColor : BoundsColor;

            spriteBatch.FillRectangle(bounds, color);
            spriteBatch.DrawString(font, str, new(bounds.X - 2, bounds.Y), Color.White);
        }
        spriteBatch.FillRectangle(Bounds.Intersection(region), BoundsColor);

        DrawElements(spriteBatch, region, fonts);
    }

    protected override void DrawItem(IDrawable drawable, SpriteBatch spriteBatch, RectangleF region, Dictionary<FontType, BitmapFont> fonts) =>
        base.DrawItem(drawable, spriteBatch, Bounds, fonts)
    ;

    public override InputListener[] GetListeners() {
        var listener = new MouseListener();
        listener.MouseDown += (sender, args) => {
            foreach (RectangleF tab in _boundstoRules.Keys) {
                if (tab.Contains(args.Position)) {
                    ActiveTab = _boundstoRules[tab];
                    Updated?.Invoke(this, args);
                    return;
                }
            }
        };

        return [ listener ];
    }

    private void PopulateBoundsToRules(Dictionary<FontType, BitmapFont> fonts) {
        _boundstoRules.Clear();

        BitmapFont font = fonts[FontType.NumberFont];
        float xPos = Bounds.X;

        foreach (RuleCategory category in _tabs.Keys) {
            string str = Util.AddSpaces(category.ToString());
            Size2 strSize = font.MeasureString(str);

            var bounds = new RectangleF(new(xPos, Bounds.Y - strSize.Height), strSize);
            _boundstoRules.Add(bounds, category);

            xPos += strSize.Width + 5;
        }
    }

    private void Scroll(RuleCategory tab, float newScrollPos) {
        float yChange = newScrollPos - _tabs[tab].YDisplacement;
        _tabs[tab] = new Tab() {
            YDisplacement = newScrollPos,
            Widgets = _tabs[tab].Widgets
        };
        foreach (Widget widget in _tabs[tab].Widgets) {
            if (widget is ScrollBar) {
                continue;
            }
            widget.Bounds = new RectangleF(
                widget.Bounds.X,
                widget.Bounds.Y - yChange,
                widget.Bounds.Width,
                widget.Bounds.Height
            );
            widget.Active = widget.Bounds.Y >= Bounds.Y;
        } 
    }

    private struct Tab {
        public float YDisplacement;
        public List<Widget> Widgets;
    }
}