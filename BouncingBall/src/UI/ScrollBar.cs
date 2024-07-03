namespace BouncingBall.UI;

public class ScrollBar(bool vertical) : Slider<ScrollEventArgs>(vertical) {
    protected override ScrollEventArgs GetEventArgs() => new(Value * (Vertical ? Bounds.Height : Bounds.Width));
}