using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace DeepCurrent
{
    // Builds the hull and paint choices and keeps the preview in sync with them.
    public sealed class CustomizeOptionsView
    {
        const float HullColumnX = 400f;
        const float HullRowStart = -150f;
        const float HullRowHeight = 42f;
        const float HullButtonHeight = 36f;
        const float HullButtonWidth = 320f;
        const float PaintStartX = 34f;
        const float PaintStep = 46f;
        const float SwatchSize = 36f;
        const float PaintRowY = -402f;

        readonly UIManager ui;
        readonly RectTransform card;
        readonly Image preview;
        Image[] skinBgs = new Image[0];
        TextMeshProUGUI[] skinLabels = new TextMeshProUGUI[0];
        Outline[] paintMarks = new Outline[0];
        string[] skinNames = new string[0];
        Sprite[] skinSprites = new Sprite[0];
        Color[] paintColors = new Color[0];
        int selectedSkin;
        int selectedPaint;

        public event Action<int> SkinSelected;
        public event Action<int> PaintSelected;

        public CustomizeOptionsView(UIManager uiManager, RectTransform parent, Image previewImage)
        {
            ui = uiManager;
            card = parent;
            preview = previewImage;

            var theme = ui.Theme;
            var hullLabel = UIWidgets.Text("HullLbl", card, "HULL", FontLib.ChakraBold, 20f, theme.amber);
            UIWidgets.SetTopLeft(hullLabel, HullColumnX, -124f, HullButtonWidth, 26f);
            var paintLabel = UIWidgets.Text("PaintLbl", card, "PAINT", FontLib.ChakraBold, 20f, theme.amber);
            UIWidgets.SetTopLeft(paintLabel, PaintStartX, -368f, 300f, 26f);
        }

        public void SetOptions(string[] names, Sprite[] sprites, Color[] paints)
        {
            skinNames = names ?? new string[0];
            skinSprites = sprites ?? new Sprite[0];
            paintColors = paints ?? new Color[0];
            skinBgs = new Image[skinNames.Length];
            skinLabels = new TextMeshProUGUI[skinNames.Length];
            paintMarks = new Outline[paintColors.Length];

            for (int i = 0; i < skinNames.Length; i++)
                BuildSkinButton(i);
            for (int i = 0; i < paintColors.Length; i++)
                BuildPaintSwatch(i);

            RefreshSelection();
        }

        public void SetSelection(int skinIndex, int paintIndex)
        {
            selectedSkin = skinNames.Length > 0 ? Mathf.Clamp(skinIndex, 0, skinNames.Length - 1) : 0;
            selectedPaint = paintColors.Length > 0 ? Mathf.Clamp(paintIndex, 0, paintColors.Length - 1) : 0;
            RefreshSelection();
        }

        void BuildSkinButton(int index)
        {
            var theme = ui.Theme;
            float y = HullRowStart - index * HullRowHeight;
            var button = new UIBuilder().Panel("Skin" + index, card, ui.Ghost)
                .Kicker("", 20f).Sized(HullButtonWidth, HullButtonHeight)
                .FinishButton(() => SkinSelected?.Invoke(index));
            UIWidgets.SetTopLeft(button, HullColumnX, y, HullButtonWidth, HullButtonHeight);

            skinBgs[index] = button.targetGraphic as Image;
            var icon = UIWidgets.Img("Icon", button.transform, skinSprites[index], Color.white);
            icon.preserveAspect = true;
            UIWidgets.SetTopLeft(icon, 6f, -3f, 52f, 30f);

            skinLabels[index] = UIWidgets.Text("Name", button.transform, skinNames[index],
                FontLib.ChakraBold, 16f, theme.ice);
            UIWidgets.SetTopLeft(skinLabels[index], 66f, -8f, 246f, 24f);
        }

        void BuildPaintSwatch(int index)
        {
            var swatch = UIWidgets.Img("Paint" + index, card, UIWidgets.WhiteSprite, paintColors[index]);
            swatch.raycastTarget = true;
            UIWidgets.SetTopLeft(swatch, PaintStartX + index * PaintStep, PaintRowY, SwatchSize, SwatchSize);

            var outline = swatch.gameObject.AddComponent<Outline>();
            outline.effectColor = ui.Theme.ice;
            outline.effectDistance = new Vector2(3f, 3f);
            outline.enabled = false;
            paintMarks[index] = outline;

            var button = swatch.gameObject.AddComponent<Button>();
            button.targetGraphic = swatch;
            button.onClick.AddListener(() => PaintSelected?.Invoke(index));
        }

        void RefreshSelection()
        {
            if (skinSprites.Length > 0)
            {
                preview.sprite = skinSprites[selectedSkin];
                for (int i = 0; i < skinBgs.Length; i++)
                {
                    bool isSelected = i == selectedSkin;
                    skinBgs[i].color = isSelected ? ui.Theme.cyan : ui.Theme.panel;
                    skinLabels[i].color = isSelected ? ui.Theme.abyss : ui.Theme.ice;
                }
            }

            if (paintColors.Length == 0) return;

            preview.color = paintColors[selectedPaint];
            for (int i = 0; i < paintMarks.Length; i++)
                paintMarks[i].enabled = i == selectedPaint;
        }
    }
}
