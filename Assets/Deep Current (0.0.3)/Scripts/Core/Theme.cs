using UnityEngine;

namespace DeepCurrent
{
    // Scriptable Object with the whole color palette.
    // "If you want to change the colors, edit the Theme asset, never the code.
    [CreateAssetMenu(fileName = "Theme", menuName = "Deep Current/Theme")]
    public class Theme : ScriptableObject
    {
        public Color abyss = new Color(0.008f, 0.039f, 0.063f, 1f);
        public Color panel = new Color(0.063f, 0.133f, 0.169f, 1f);
        public Color cyan  = new Color(0.239f, 0.949f, 0.812f, 1f);
        public Color ice   = new Color(0.851f, 0.965f, 0.941f, 1f);
        public Color amber = new Color(1.000f, 0.761f, 0.294f, 1f);
        public Color coral = new Color(1.000f, 0.416f, 0.302f, 1f);
    }
}
