using UnityEngine;

namespace DeepCurrent
{
    // The colors of one button, taken from the Theme asset by UIManager.
    public struct ButtonStyle
    {
        public Color bg, fg, hover;

        public ButtonStyle(Color bg, Color fg, Color hover)
        {
            this.bg = bg;
            this.fg = fg;
            this.hover = hover;
        }
    }
}
