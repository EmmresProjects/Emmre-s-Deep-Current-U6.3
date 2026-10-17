using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace DeepCurrent
{
    // Driver Bay: pick a hull skin and a hull paint.
    // The options come from the DiverCatalog asset. The choice is applied with DiverBuilder.
    public class CustomizeScreen
    {
        const float HullColumnX = 400f;
        const float HullRowHeight = 56f;
        const float HullButtonHeight = 48f;
        const float HullButtonWidth = 320f;
        const float PaintStartX = 34f;
        const float PaintStep = 52f;
        const float SwatchSize = 44f;
        const float PaintRowY = -402f;

        readonly GameObject root;
        readonly Image preview;
        readonly Image[] skinBgs;
        readonly TextMeshProUGUI[] skinLabels;
        readonly Outline[] paintMarks;
        int selSkin, selPaint;

        public CustomizeScreen(UIManager ui, RectTransform layer)
        {
            root = layer.gameObject;
            var t = ui.Theme;
            var catalog = GameRoot.Instance.diverCatalog;

            UIWidgets.Dimmer(layer);
            var card = UIWidgets.Dossier(layer, t.panel, 760f, 560f);

            var heading = UIWidgets.Text("Heading", card, "DIVER BAY", FontLib.Bungee, 40f, t.cyan);
            UIWidgets.SetTopLeft(heading, 30f, -22f, 700f, 56f);
            var tag = UIWidgets.Text("Tag", card, "Pick a hull and a paint, then APPLY.",
                FontLib.Chakra, 20f, t.ice);
            UIWidgets.SetTopLeft(tag, 34f, -84f, 692f, 28f);

            // Live preview of the current selection (left column).
            preview = UIWidgets.Img("Preview", card, null, Color.white);
            preview.preserveAspect = true;
            UIWidgets.SetTopLeft(preview, 34f, -130f, 290f, 220f);

            // HULL column (right side): one button per catalog skin.
            var hullLabel = UIWidgets.Text("HullLbl", card, "HULL", FontLib.ChakraBold, 20f, t.amber);
            UIWidgets.SetTopLeft(hullLabel, HullColumnX, -124f, HullButtonWidth, 26f);

            skinBgs = new Image[catalog.skins.Length];
            skinLabels = new TextMeshProUGUI[catalog.skins.Length];
            for (int i = 0; i < catalog.skins.Length; i++)
            {
                BuildSkinButton(ui, card, catalog, i);
            }

            // PAINT row (bottom left): one swatch per catalog paint.
            var paintLabel = UIWidgets.Text("PaintLbl", card, "PAINT", FontLib.ChakraBold, 20f, t.amber);
            UIWidgets.SetTopLeft(paintLabel, PaintStartX, -368f, 300f, 26f);

            paintMarks = new Outline[catalog.paints.Length];
            for (int i = 0; i < catalog.paints.Length; i++)
            {
                BuildPaintSwatch(t, card, catalog, i);
            }

            var apply = new UIBuilder().Panel("ApplyBtn", card, ui.Primary)
                .Kicker("APPLY", 24f).Sized(290f, 56f)
                .FinishButton(ApplyAndBack);
            UIWidgets.SetTopLeft(apply, 430f, -470f, 290f, 56f);

            var back = new UIBuilder().Panel("BackBtn", card, ui.Ghost)
                .Kicker("BACK", 24f).Sized(210f, 56f)
                .FinishButton(() => UIManager.Instance.ShowTitle(GameRoot.Instance.Scores.Best));
            UIWidgets.SetTopLeft(back, 34f, -470f, 210f, 56f);
        }

        void BuildSkinButton(UIManager ui, RectTransform card, DiverCatalog catalog, int index)
        {
            var t = ui.Theme;
            float y = -156f - index * HullRowHeight;

            var builder = new UIBuilder().Panel("Skin" + index, card, ui.Ghost)
                .Kicker("", 20f).Sized(HullButtonWidth, HullButtonHeight);
            var btn = builder.FinishButton(() => { selSkin = index; RefreshSelection(); });
            UIWidgets.SetTopLeft(btn, HullColumnX, y, HullButtonWidth, HullButtonHeight);

            skinBgs[index] = btn.targetGraphic as Image;
            var icon = UIWidgets.Img("Icon", btn.transform, catalog.skins[index].sprite, Color.white);
            icon.preserveAspect = true;
            UIWidgets.SetTopLeft(icon, 8f, -4f, 64f, 40f);

            skinLabels[index] = UIWidgets.Text("Name", btn.transform, catalog.skins[index].displayName,
                FontLib.ChakraBold, 20f, t.ice);
            UIWidgets.SetTopLeft(skinLabels[index], 80f, -10f, 230f, 28f);
        }

        void BuildPaintSwatch(Theme t, RectTransform card, DiverCatalog catalog, int index)
        {
            var sw = UIWidgets.Img("Paint" + index, card, UIWidgets.WhiteSprite, catalog.paints[index].tint);
            sw.raycastTarget = true;
            UIWidgets.SetTopLeft(sw, PaintStartX + index * PaintStep, PaintRowY, SwatchSize, SwatchSize);

            var mark = sw.gameObject.AddComponent<Outline>();
            mark.effectColor = t.ice;
            mark.effectDistance = new Vector2(3f, 3f);
            mark.enabled = false;
            paintMarks[index] = mark;

            var click = sw.gameObject.AddComponent<Button>();
            click.targetGraphic = sw;
            click.onClick.AddListener(() => { selPaint = index; RefreshSelection(); });
        }

        void ApplyAndBack()
        {
            DiverLoadout.Select(selSkin, selPaint);
            DiverLoadout.Save();
            DiverBuilder.FromLoadout(GameRoot.Instance.diverCatalog).ApplyTo(GameRoot.Instance.Player);
            UIManager.Instance.ShowTitle(GameRoot.Instance.Scores.Best);
        }

        void RefreshSelection()
        {
            var catalog = GameRoot.Instance.diverCatalog;
            var t = UIManager.Instance.Theme;

            if (catalog.skins.Length > 0)
            {
                preview.sprite = catalog.skins[selSkin].sprite;
                for (int i = 0; i < skinBgs.Length; i++)
                {
                    bool on = i == selSkin;
                    skinBgs[i].color = on ? t.cyan : t.panel;
                    skinLabels[i].color = on ? t.abyss : t.ice;
                }
            }
            if (catalog.paints.Length > 0)
            {
                preview.color = catalog.paints[selPaint].tint;
                for (int i = 0; i < paintMarks.Length; i++)
                    paintMarks[i].enabled = i == selPaint;
            }
        }

        public void Show()
        {
            var catalog = GameRoot.Instance.diverCatalog;
            selSkin = Mathf.Clamp(DiverLoadout.SkinIndex, 0, Mathf.Max(0, catalog.skins.Length - 1));
            selPaint = Mathf.Clamp(DiverLoadout.PaintIndex, 0, Mathf.Max(0, catalog.paints.Length - 1));
            RefreshSelection();
            root.SetActive(true);
        }

        public void Hide() => root.SetActive(false);
    }
}
