using System;

namespace BouncingBall.UI;

public interface IUpdatable<T> where T : EventArgs {
    public event EventHandler<T> Updated;    
}