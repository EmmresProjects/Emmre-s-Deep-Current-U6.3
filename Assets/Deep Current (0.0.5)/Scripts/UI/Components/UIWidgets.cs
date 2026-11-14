using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace DeepCurrent
{
    // Small static helpers that place UI widgets without repeating RectTransform code.
    public static class UIWidgets
    {
        const float OverlayAlpha = 0.8f;

        public static Sprite WhiteSprite;

        public static RectTransform Rect(string name, Transform parent)
        {
            var go = new GameObject(name, typeof(RectTransform));
            var rt = (RectTransform)go.transform;
            rt.SetParent(parent, false);
            return rt;
        }

        public static RectTransform Stretch(RectTransform rt)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
            return rt;
        }

        public static Image Img(string name, Transform parent, Sprite sprite, Color color)
        {
            var rt = Rect(name, parent);
            var img = rt.gameObject.AddComponent<Image>();
            img.sprite = sprite;
            img.color = color;
            img.raycastTarget = false;
            return img;
        }

        public static TextMeshProUGUI Text(string name, Transform parent, string text,
            TMP_FontAsset font, float size, Color color,
            TextAlignmentOptions align = TextAlignmentOptions.TopLeft)
        {
            var rt = Rect(name, parent);
            var tmp = rt.gameObject.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.font = font;
            tmp.fontSize = size;
            tmp.color = color;
            tmp.alignment = align;
            tmp.raycastTarget = false;
            return tmp;
        }

        // The dark see-through layer that covers the game behind a popup screen.
        // It blocks clicks from reaching the game underneath.
        public static void Dimmer(RectTransform layer)
        {
            var dim = layer.gameObject.AddComponent<Image>();
            dim.sprite = WhiteSprite;
            dim.color = new Color(0f, 0f, 0f, OverlayAlpha);
            dim.raycastTarget = true;
        }

        // The centered card that holds the content of a popup screen.
        public static RectTransform Dossier(RectTransform layer, Color color, float width, float height)
        {
            var card = Img("Dossier", layer, WhiteSprite, color);
            card.raycastTarget = true;
            var rt = (RectTransform)card.transform;
            rt.sizeDelta = new Vector2(width, height);
            return rt;
        }

        // Pin a widget to a top-left anchored box (the layout style used by all screens).
        public static void SetTopLeft(Component c, float x, float y, float w, float h)
        {
            var rt = (RectTransform)c.transform;
            rt.anchorMin = rt.anchorMax = new Vector2(0f, 1f);
            rt.pivot = new Vector2(0f, 1f);
            rt.anchoredPosition = new Vector2(x, y);
            rt.sizeDelta = new Vector2(w, h);
        }
    }
}
