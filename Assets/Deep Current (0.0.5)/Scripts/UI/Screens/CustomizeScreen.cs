using System;
using UnityEngine;
using TMPro;

namespace DeepCurrent
{
    public sealed class CustomizeScreen : ICustomizeView
    {
        readonly GameObject root;
        readonly CustomizeOptionsView options;

        public event Action<int> SkinSelected
        {
            add => options.SkinSelected += value;
            remove => options.SkinSelected -= value;
        }

        public event Action<int> PaintSelected
        {
            add => options.PaintSelected += value;
            remove => options.PaintSelected -= value;
        }

        public event Action ApplyRequested;
        public event Action BackRequested;

        public CustomizeScreen(UIManager ui, RectTransform layer)
        {
            root = layer.gameObject;
            var theme = ui.Theme;

            UIWidgets.Dimmer(layer);
            var card = UIWidgets.Dossier(layer, theme.panel, 760f, 560f);

            var heading = UIWidgets.Text("Heading", card, "DIVER BAY", FontLib.Bungee, 40f, theme.cyan);
            UIWidgets.SetTopLeft(heading, 30f, -22f, 700f, 56f);
            var tag = UIWidgets.Text("Tag", card, "Pick a hull and a paint, then APPLY.",
                FontLib.Chakra, 20f, theme.ice);
            UIWidgets.SetTopLeft(tag, 34f, -84f, 692f, 28f);

            var preview = UIWidgets.Img("Preview", card, null, Color.white);
            preview.preserveAspect = true;
            UIWidgets.SetTopLeft(preview, 34f, -130f, 290f, 220f);
            options = new CustomizeOptionsView(ui, card, preview);

            var apply = new UIBuilder().Panel("ApplyBtn", card, ui.Primary)
                .Kicker("APPLY", 24f).Sized(290f, 56f)
                .FinishButton(() => ApplyRequested?.Invoke());
            UIWidgets.SetTopLeft(apply, 430f, -470f, 290f, 56f);

            var back = new UIBuilder().Panel("BackBtn", card, ui.Ghost)
                .Kicker("BACK", 24f).Sized(210f, 56f)
                .FinishButton(() => BackRequested?.Invoke());
            UIWidgets.SetTopLeft(back, 34f, -470f, 210f, 56f);
        }

        public void SetOptions(string[] names, Sprite[] sprites, Color[] paints)
        {
            options.SetOptions(names, sprites, paints);
        }

        public void SetSelection(int skinIndex, int paintIndex)
        {
            options.SetSelection(skinIndex, paintIndex);
        }

        public void Show() => root.SetActive(true);
        public void Hide() => root.SetActive(false);
    }
}
