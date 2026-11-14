using System;

namespace DeepCurrent
{
    public interface ITitleView
    {
        event Action DiveRequested;
        event Action CustomizeRequested;
        event Action QuitRequested;
        void Show(int bestScore);
        void Hide();
    }
}
