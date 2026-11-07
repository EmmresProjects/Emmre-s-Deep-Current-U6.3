using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

namespace DeepCurrent
{
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
