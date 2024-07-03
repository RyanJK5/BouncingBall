using System;

namespace BouncingBall.UI;

public class ScrollEventArgs(float displacement) : EventArgs {
    public float Displacement = displacement;
}