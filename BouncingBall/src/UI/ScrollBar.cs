using System;
using MonoGame.Extended.Input.InputListeners;

namespace BouncingBall.UI;

public class ScrollBar : Slider<MouseEventArgs> {
    public ScrollBar(bool vertical, float initialValue = 0) : base(vertical, initialValue) {
    }

    protected override MouseEventArgs GetEventArgs() {
        throw new NotImplementedException();
    }
}