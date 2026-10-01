using UnityEngine;
using TMPro;

namespace DeepCurrent
{
    // Things to Remember: 
    //  To apply just like the original JS version, the font assets must be created at runtime. 
    //  This creates runtime TMP font assets from the bundled TTFs.
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
                Debug.LogWarning("[DEEPCURRENT] Font missing: " + resourcePath + " — falling back to TMP default.");
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
