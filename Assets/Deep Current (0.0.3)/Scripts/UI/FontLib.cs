using UnityEngine;
using TMPro;

namespace DeepCurrent
{
    // Creates the runtime TextMesh Pro font assets from the bundled TTF files.
    // The fonts are built at runtime so the text looks the same as the original HTML version.
    public static class FontLib
    {
        static TMP_FontAsset bungee, chakra, chakraBold;

        public static TMP_FontAsset Bungee
        {
            get
            {
                if (bungee == null) bungee = Create("Fonts/Bungee-Regular");
                return bungee;
            }
        }

        public static TMP_FontAsset Chakra
        {
            get
            {
                if (chakra == null) chakra = Create("Fonts/ChakraPetch-Regular");
                return chakra;
            }
        }

        public static TMP_FontAsset ChakraBold
        {
            get
            {
                if (chakraBold == null) chakraBold = Create("Fonts/ChakraPetch-Bold");
                return chakraBold;
            }
        }

        static TMP_FontAsset Create(string resourcePath)
        {
            Font f = Resources.Load<Font>(resourcePath);
            if (f == null)
            {
                Debug.LogWarning("[DeepCurrent] Font missing: " + resourcePath + ". Falling back to the TMP default.");
                return TMP_Settings.defaultFontAsset;
            }
            var asset = TMP_FontAsset.CreateFontAsset(
                f, 48, 6, UnityEngine.TextCore.LowLevel.GlyphRenderMode.SDFAA, 512, 512,
                AtlasPopulationMode.Dynamic, true);
            asset.name = f.name + " SDF (runtime)";
            return asset;
        }
    }
}
