using UnityEngine;
using UnityEngine.UI;

namespace SteppeBastion.UI
{
    /// <summary>Builds uGUI elements in the game's style. Used at runtime by the build menu and by the editor scene builder.</summary>
    public static class UIFactory
    {
        public static readonly Color Ink = new(0.17f, 0.11f, 0.07f);
        public static readonly Color Cream = new(1f, 0.96f, 0.86f);
        public static readonly Color Gold = new(1f, 0.85f, 0.4f);
        public static readonly Color Shade = new(0.08f, 0.05f, 0.03f, 0.6f);

        private static Font font;
        public static Font Font => font ??= Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        public static RectTransform Rect(Transform parent, string name, Vector2 anchor, Vector2 position, Vector2 size)
        {
            var rt = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
            rt.SetParent(parent, false);
            rt.anchorMin = rt.anchorMax = anchor;
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = position;
            rt.sizeDelta = size;
            return rt;
        }

        public static RectTransform Stretch(Transform parent, string name)
        {
            RectTransform rt = Rect(parent, name, Vector2.zero, Vector2.zero, Vector2.zero);
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            return rt;
        }

        public static Image Image(Transform parent, string name, Sprite sprite, Color color, Vector2 anchor, Vector2 position, Vector2 size)
        {
            var image = Rect(parent, name, anchor, position, size).gameObject.AddComponent<Image>();
            image.sprite = sprite;
            image.color = color;
            image.preserveAspect = sprite != null;
            return image;
        }

        public static Text Text(Transform parent, string name, string value, int fontSize, Color color, TextAnchor alignment,
            Vector2 anchor, Vector2 position, Vector2 size)
        {
            var text = Rect(parent, name, anchor, position, size).gameObject.AddComponent<Text>();
            text.font = Font;
            text.text = value;
            text.fontSize = fontSize;
            text.fontStyle = FontStyle.Bold;
            text.color = color;
            text.alignment = alignment;
            text.raycastTarget = false;
            text.horizontalOverflow = HorizontalWrapMode.Overflow;
            var outline = text.gameObject.AddComponent<Outline>();
            outline.effectColor = Ink;
            outline.effectDistance = new Vector2(2f, -2f);
            return text;
        }

        public static Button Button(Transform parent, string name, Sprite background, string label, int fontSize,
            Vector2 anchor, Vector2 position, Vector2 size)
        {
            Image image = Image(parent, name, background, Color.white, anchor, position, size);
            image.preserveAspect = false;
            var button = image.gameObject.AddComponent<Button>();
            ColorBlock colors = button.colors;
            colors.highlightedColor = new Color(1f, 0.95f, 0.8f);
            colors.pressedColor = new Color(0.85f, 0.8f, 0.7f);
            colors.disabledColor = new Color(0.55f, 0.55f, 0.55f, 0.9f);
            button.colors = colors;
            if (!string.IsNullOrEmpty(label))
                Text(image.transform, "Label", label, fontSize, Cream, TextAnchor.MiddleCenter, new Vector2(0.5f, 0.5f), Vector2.zero, size);
            return button;
        }

        /// <summary>Dark round button with a gold ring, as used by the build menu and the HUD.</summary>
        public static Button RoundButton(Transform parent, string name, Vector2 anchor, Vector2 position, float size,
            Sprite circle, Sprite ring)
        {
            Image background = Image(parent, name, circle, new Color(0.22f, 0.14f, 0.09f, 0.92f),
                anchor, position, Vector2.one * size);
            Image border = Image(background.transform, "Ring", ring, Gold,
                new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.one * size);
            border.raycastTarget = false;
            var button = background.gameObject.AddComponent<Button>();
            ColorBlock colors = button.colors;
            colors.disabledColor = new Color(0.55f, 0.55f, 0.55f, 0.9f);
            button.colors = colors;
            return button;
        }

        public static Slider Slider(Transform parent, string name, Sprite knob, Vector2 anchor, Vector2 position, Vector2 size)
        {
            RectTransform rt = Rect(parent, name, anchor, position, size);
            var back = Stretch(rt, "Background").gameObject.AddComponent<Image>();
            back.color = Ink;

            RectTransform fillArea = Stretch(rt, "Fill Area");
            fillArea.offsetMin = new Vector2(6f, 6f);
            fillArea.offsetMax = new Vector2(-6f, -6f);
            RectTransform fill = Stretch(fillArea, "Fill");
            fill.gameObject.AddComponent<Image>().color = Gold;

            RectTransform slideArea = Stretch(rt, "Handle Slide Area");
            slideArea.offsetMin = new Vector2(size.y / 2f, 0f);
            slideArea.offsetMax = new Vector2(-size.y / 2f, 0f);
            RectTransform handle = Rect(slideArea, "Handle", new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(size.y * 1.6f, 0f));
            handle.anchorMin = new Vector2(0f, 0f);
            handle.anchorMax = new Vector2(0f, 1f);
            handle.sizeDelta = new Vector2(size.y * 1.6f, size.y * 0.6f);
            var handleImage = handle.gameObject.AddComponent<Image>();
            handleImage.sprite = knob;
            handleImage.color = Cream;

            var slider = rt.gameObject.AddComponent<Slider>();
            slider.fillRect = fill;
            slider.handleRect = handle;
            slider.targetGraphic = handleImage;
            slider.direction = UnityEngine.UI.Slider.Direction.LeftToRight;
            slider.minValue = 0f;
            slider.maxValue = 1f;
            return slider;
        }

        public static Toggle Toggle(Transform parent, string name, Vector2 anchor, Vector2 position, float size)
        {
            RectTransform rt = Rect(parent, name, anchor, position, Vector2.one * size);
            var box = rt.gameObject.AddComponent<Image>();
            box.color = Ink;
            var check = Image(rt, "Checkmark", null, Gold, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.one * (size * 0.6f));
            var toggle = rt.gameObject.AddComponent<Toggle>();
            toggle.targetGraphic = box;
            toggle.graphic = check;
            return toggle;
        }

        /// <summary>Full-screen dark overlay with a centered panel, for pause and result screens.</summary>
        public static RectTransform Modal(Transform parent, string name, Sprite panel, Vector2 size)
        {
            RectTransform root = Stretch(parent, name);
            root.gameObject.AddComponent<Image>().color = Shade;
            Image box = Image(root, "Panel", panel, Color.white, new Vector2(0.5f, 0.5f), Vector2.zero, size);
            box.preserveAspect = false;
            return root;
        }
    }
}
