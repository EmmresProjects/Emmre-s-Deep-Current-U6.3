using UnityEngine;

namespace DeepCurrent
{
    // SCRIPTABLE OBJECT listing everything the player can customize.
    // "Add a new skin or paint?" -> drop the sprite in, add one line to this asset.
    // The Customize screen reads this list automatically.
    [CreateAssetMenu(fileName = "DiverCatalog", menuName = "Deep Current/Diver Catalog")]
    public class DiverCatalog : ScriptableObject
    {
        [System.Serializable]
        public class SkinOption
        {
            public string displayName = "Hull";
            public Sprite sprite;
        }

        [System.Serializable]
        public class PaintOption
        {
            public string displayName = "Paint";
            [ColorUsage(false)] public Color tint = Color.white;
        }

        public SkinOption[] skins = new SkinOption[0];
        public PaintOption[] paints = new PaintOption[0];
    }
}
