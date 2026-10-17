using UnityEngine;

namespace DeepCurrent
{
    // The player's saved customization choice (index into the DiverCatalog).
    // Small static data bag so both the menu and the spawn code can reach it.
    public static class DiverLoadout
    {
        public static int SkinIndex { get; private set; }
        public static int PaintIndex { get; private set; }

        public static void Load()
        {
            SkinIndex = PlayerPrefs.GetInt("deepcurrent_skin", 0);
            PaintIndex = PlayerPrefs.GetInt("deepcurrent_paint", 0);
        }

        public static void Select(int skin, int paint)
        {
            SkinIndex = skin;
            PaintIndex = paint;
        }

        public static void Save()
        {
            PlayerPrefs.SetInt("deepcurrent_skin", SkinIndex);
            PlayerPrefs.SetInt("deepcurrent_paint", PaintIndex);
            PlayerPrefs.Save();
        }
    }
}
