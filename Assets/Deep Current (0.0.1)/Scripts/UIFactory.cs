using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

namespace DeepCurrent
{
    // Builder pattern for creating UI panels and buttons.
    public class UIFactory
    {
        public static Sprite WhiteSprite;
        RectTransform buttonRoot;
        Image buttonImage;
        TextMeshProUGUI buttonLabel;
        BtnStyle buttonStyle;

        public static void LoadSprites()
        {
            WhiteSprite = Res.Load<Sprite>("Sprites/UI/ui_white");
        }

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

        public static Image Img(string name, Transform parent, Sprite sprite, Color color, bool sliced = false)
        {
            var rt = Rect(name, parent);
            var img = rt.gameObject.AddComponent<Image>();
            img.sprite = sprite;
            img.color = color;
            if (sliced && sprite != null) img.type = Image.Type.Sliced;
            img.raycastTarget = false;
            return img;
        }

        public static TextMeshProUGUI Text(string name, Transform parent, string text,
            TMP_FontAsset font, float size, Color color, TextAlignmentOptions align = TextAlignmentOptions.TopLeft)
        {
            var rt = Rect(name, parent);
            var tmp = rt.gameObject.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.font = font;
            tmp.fontSize = size;
            tmp.color = color;
            tmp.alignment = align;
            tmp.raycastTarget = false;
            tmp.textWrappingMode = TextWrappingModes.Normal;
            return tmp;
        }

        // Step 1: Make the root and its visible background. Keep it hidden until all parts have been added.
        public UIFactory Panel(string name, Transform parent, BtnStyle style)
        {
            if (buttonRoot != null)
                throw new InvalidOperationException("Finish this button before starting another one.");
            buttonStyle = style;
            buttonRoot = Rect(name, parent);
            buttonRoot.gameObject.SetActive(false);
            buttonImage = buttonRoot.gameObject.AddComponent<Image>();
            buttonImage.sprite = WhiteSprite;
            buttonImage.color = style.bg;
            buttonImage.raycastTarget = true;
            return this;
        }

        // Step 2: Add a real text child, not just a saved setting.
        public UIFactory Kicker(string label, float fontSize)
        {
            if (buttonRoot == null)
                throw new InvalidOperationException("Build the panel before adding its label.");
            buttonLabel = Text("Label", buttonRoot, label, FontLib.ChakraBold,
                fontSize, buttonStyle.fg, TextAlignmentOptions.Center);
            buttonLabel.textWrappingMode = TextWrappingModes.NoWrap;
            Stretch((RectTransform)buttonLabel.transform);
            return this;
        }

        // Step 3: Set the size of the button being built.
        public UIFactory Fixed(float w, float h)
        {
            if (buttonRoot == null)
                throw new InvalidOperationException("Build the panel before setting its size.");
            buttonRoot.sizeDelta = new Vector2(w, h);
            return this;
        }

        // Final Step: Add the click action and return the finished Button.
        public Button MakeButton(UnityAction onClick)
        {
            if (buttonRoot == null || buttonLabel == null)
                throw new InvalidOperationException("Build a panel and label before finishing the button.");
            var btn = buttonRoot.gameObject.AddComponent<Button>();
            btn.targetGraphic = buttonImage;
            var colors = btn.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = buttonStyle.hover;
            colors.pressedColor = new Color(0.8f, 0.8f, 0.8f, 1f);
            colors.selectedColor = Color.white;
            colors.fadeDuration = 0f;
            btn.colors = colors;
            btn.onClick.AddListener(onClick);
            btn.onClick.AddListener(() =>
            {
                var es = UnityEngine.EventSystems.EventSystem.current;
                if (es != null) es.SetSelectedGameObject(null);
            });
            buttonRoot.gameObject.SetActive(true);
            Debug.Log("[Builder] Created " + buttonRoot.gameObject.name + " with Image, Label and Button.");
            buttonRoot = null;
            buttonImage = null;
            buttonLabel = null;
            buttonStyle = null;
            return btn;
        }
        public class BtnStyle
        {
            public Color bg, fg, hover;
        }

        public static readonly BtnStyle Primary = new BtnStyle
        {
            bg = DC.Amber, fg = DC.Abyss, hover = new Color(1.12f, 1.12f, 1.12f, 1f)
        };
        public static readonly BtnStyle Ghost = new BtnStyle
        {
            bg = DC.Panel, fg = DC.Ice, hover = new Color(1.2f, 1.2f, 1.2f, 1f)
        };
        public static readonly BtnStyle Exit = new BtnStyle
        {
            bg = DC.Panel, fg = DC.Coral, hover = new Color(1.2f, 1.2f, 1.2f, 1f)
        };
    }
}
