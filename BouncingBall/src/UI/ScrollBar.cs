using System;
using MonoGame.Extended.Input.InputListeners;

namespace BouncingBall.UI;

public class ScrollBar(bool vertical) : Slider<MouseEventArgs>(vertical) {
    protected override MouseEventArgs GetEventArgs() {
        throw new NotImplementedException();
    }
}