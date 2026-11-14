using System;

namespace DeepCurrent
{
    public interface ICustomizeView
    {
        event Action<int> SkinSelected;
        event Action<int> PaintSelected;
        event Action ApplyRequested;
        event Action BackRequested;
        void SetOptions(string[] skinNames, UnityEngine.Sprite[] skinSprites, UnityEngine.Color[] paintColors);
        void SetSelection(int skinIndex, int paintIndex);
        void Show();
        void Hide();
    }
}
