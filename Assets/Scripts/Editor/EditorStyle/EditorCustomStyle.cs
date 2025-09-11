using UnityEngine;
using UnityEngine.UIElements;

public class EditorCustomStyle
{
    private VisualElement element;

    private EditorCustomStyle(VisualElement e)
    {
        element = e;
    }

    // --- Création ---
    public static EditorCustomStyle Create(VisualElement e)
    {
        return new EditorCustomStyle(e);
    }

    // --- Méthodes pour modifier le style ---
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
    
    public VisualElement Build()
    {
        return element;
    }
}