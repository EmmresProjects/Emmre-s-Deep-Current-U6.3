using UnityEngine;

namespace DeepCurrent
{
    // Builder Pattern: collects the cosmetic parts step by step, then applies them in one call.
    // It knows how to dress a diver. It does not know how the diver moves or shoots.
    public class DiverBuilder
    {
        Sprite skin;
        Color paint = Color.white;

        public DiverBuilder WithSkin(Sprite s)
        {
            skin = s;
            return this;
        }

        public DiverBuilder WithPaint(Color tint)
        {
            paint = tint;
            return this;
        }

        public void ApplyTo(PlayerController player)
        {
            player.ApplyLook(skin, paint);
        }

        // Convenience: build straight from what the player saved last time.
        public static DiverBuilder FromLoadout(DiverCatalog catalog)
        {
            var b = new DiverBuilder();
            if (catalog == null) return b;

            if (catalog.skins != null && catalog.skins.Length > 0)
            {
                int i = catalog.ClampSkinIndex(DiverLoadout.SkinIndex);
                b.WithSkin(catalog.skins[i].sprite);
            }
            if (catalog.paints != null && catalog.paints.Length > 0)
            {
                int i = catalog.ClampPaintIndex(DiverLoadout.PaintIndex);
                b.WithPaint(catalog.paints[i].tint);
            }
            return b;
        }
    }
}
