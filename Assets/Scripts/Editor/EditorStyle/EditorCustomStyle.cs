using UnityEngine;
using UnityEngine.UIElements;

namespace OverBang.GameName.Editor.EditorStyle
{
    public class EditorCustomStyle
    {
        private VisualElement element;

        private EditorCustomStyle(VisualElement e)
        {
            element = e;
        }

        public static EditorCustomStyle Create(VisualElement e)
        {
            return new EditorCustomStyle(e);
        }

        public EditorCustomStyle Bold()
        {
            element.style.unityFontStyleAndWeight = FontStyle.Bold;
            return this;
        }

        public EditorCustomStyle Italic()
        {
            element.style.unityFontStyleAndWeight = FontStyle.Italic;
            return this;
        }

        public EditorCustomStyle FontSize(int size)
        {
            element.style.fontSize = size;
            return this;
        }

        public EditorCustomStyle TextColor(Color color)
        {
            element.style.color = color;
            return this;
        }

        public EditorCustomStyle AlignCenter()
        {
            element.style.unityTextAlign = TextAnchor.MiddleCenter;
            return this;
        }

        public EditorCustomStyle Overflow(Overflow overflow)
        {
            element.style.overflow = overflow;
            return this;
        }

        public EditorCustomStyle AlignLeft()
        {
            element.style.unityTextAlign = TextAnchor.MiddleLeft;
            return this;
        }

        public EditorCustomStyle AlignRight()
        {
            element.style.unityTextAlign = TextAnchor.MiddleRight;
            return this;
        }

        public EditorCustomStyle BackgroundColor(Color color)
        {
            element.style.backgroundColor = color;
            return this;
        }

        public EditorCustomStyle Padding(int left, int right, int top, int bottom)
        {
            element.style.paddingLeft = left;
            element.style.paddingRight = right;
            element.style.paddingTop = top;
            element.style.paddingBottom = bottom;
            return this;
        }

        public EditorCustomStyle Margin(int left, int right, int top, int bottom)
        {
            element.style.marginLeft = left;
            element.style.marginRight = right;
            element.style.marginTop = top;
            element.style.marginBottom = bottom;
            return this;
        }

        #region Flexbox

        public EditorCustomStyle FlexGrow(float value = 1f)
        {
            element.style.flexGrow = value;
            return this;
        }

        public EditorCustomStyle FlexShrink(float value = 1f)
        {
            element.style.flexShrink = value;
            return this;
        }

        public EditorCustomStyle FlexDirection(FlexDirection direction)
        {
            element.style.flexDirection = direction;
            return this;
        }

        public EditorCustomStyle AlignItems(Align align)
        {
            element.style.alignItems = align;
            return this;
        }

        public EditorCustomStyle JustifyContent(Justify justify)
        {
            element.style.justifyContent = justify;
            return this;
        }


        #endregion Flexbox

        #region Size

        public EditorCustomStyle Width(float width)
        {
            element.style.width = width;
            return this;
        }

        public EditorCustomStyle Height(float height)
        {
            element.style.height = height;
            return this;
        }

        public EditorCustomStyle MinWidth(float minWidth)
        {
            element.style.minWidth = minWidth;
            return this;
        }

        public EditorCustomStyle MinHeight(float minHeight)
        {
            element.style.minHeight = minHeight;
            return this;
        }


        #endregion Size


        #region Border & Radius

        public EditorCustomStyle BorderColor(Color color)
        {
            element.style.borderTopColor = color;
            element.style.borderRightColor = color;
            element.style.borderBottomColor = color;
            element.style.borderLeftColor = color;
            return this;
        }

        public EditorCustomStyle BorderWidth(float width)
        {
            element.style.borderTopWidth = width;
            element.style.borderRightWidth = width;
            element.style.borderBottomWidth = width;
            element.style.borderLeftWidth = width;
            return this;
        }

        public EditorCustomStyle BorderRadius(float radius)
        {
            element.style.borderTopLeftRadius = radius;
            element.style.borderTopRightRadius = radius;
            element.style.borderBottomLeftRadius = radius;
            element.style.borderBottomRightRadius = radius;
            return this;
        }

        #endregion Border & Radius

        public VisualElement Build()
        {
            return element;
        }
    }
}