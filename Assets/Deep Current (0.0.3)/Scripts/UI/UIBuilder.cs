using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

namespace DeepCurrent
{
    // The colors of one button, taken from the Theme asset by UIManager.
    public struct ButtonStyle
    {
        public Color bg, fg, hover;

        public ButtonStyle(Color bg, Color fg, Color hover)
        {
            this.bg = bg;
            this.fg = fg;
            this.hover = hover;
        }
    }

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

    // Builder Pattern for buttons: collect the parts step by step, then finish at the end.
    public class UIBuilder
    {
        RectTransform root;
        Image image;
        TextMeshProUGUI label;
        ButtonStyle style;

        // Step 1: the root and its background. Hidden until the button is finished.
        public UIBuilder Panel(string name, Transform parent, ButtonStyle s)
        {
            root = UIWidgets.Rect(name, parent);
            root.gameObject.SetActive(false);
            image = root.gameObject.AddComponent<Image>();
            image.sprite = UIWidgets.WhiteSprite;
            image.color = s.bg;
            image.raycastTarget = true;
            style = s;
            return this;
        }

        // Step 2: the centered label.
        public UIBuilder Kicker(string text, float size)
        {
            label = UIWidgets.Text("Label", root, text, FontLib.ChakraBold, size, style.fg,
                TextAlignmentOptions.Center);
            label.textWrappingMode = TextWrappingModes.NoWrap;
            UIWidgets.Stretch((RectTransform)label.transform);
            return this;
        }

        // Step 3: a fixed size.
        public UIBuilder Sized(float w, float h)
        {
            root.sizeDelta = new Vector2(w, h);
            return this;
        }

        // Final step: wire the click and show the button.
        public Button FinishButton(UnityAction onClick)
        {
            var btn = root.gameObject.AddComponent<Button>();
            btn.targetGraphic = image;
            var colors = btn.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = style.hover;
            colors.pressedColor = new Color(0.8f, 0.8f, 0.8f, 1f);
            colors.selectedColor = Color.white;
            colors.fadeDuration = 0f;
            btn.colors = colors;
            btn.onClick.AddListener(onClick);
            // Clear the keyboard selection after a click so the highlight does not stick.
            btn.onClick.AddListener(() =>
            {
                var es = UnityEngine.EventSystems.EventSystem.current;
                if (es != null) es.SetSelectedGameObject(null);
            });
            root.gameObject.SetActive(true);
            return btn;
        }
    }
}
