using UnityEngine;

namespace DeepCurrent
{
    // SCRIPTABLE OBJECT listing everything the player can customize.
    // Add a new skin or paint? Drop the sprite in and add one line to this asset.
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

        // Keep a saved skin index inside the list even if the asset shrinks later.
        public int ClampSkinIndex(int index)
        {
            if (skins.Length == 0) return 0;
            return Mathf.Clamp(index, 0, skins.Length - 1);
        }

        // Same guard for paints, so a stale saved index never breaks the HUD.
        public int ClampPaintIndex(int index)
        {
            if (paints.Length == 0) return 0;
            return Mathf.Clamp(index, 0, paints.Length - 1);
        }
    }
}
