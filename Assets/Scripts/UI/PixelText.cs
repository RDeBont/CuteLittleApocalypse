using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Tekst in het pixelfont apocalypse_font_8x8 (solid of outline), direct vanuit de PNG.
/// Het font heeft alleen hoofdletters, cijfers en ! , . : ? (kleine letters worden hoofdletters,
/// andere tekens een spatie). Elke letter is 8 pixels breed; posities in canvas-pixels (320x180).
/// Op een canvas met een grotere referentieresolutie (bv. 1920x1080) schaalt de tekst mee (x6).
/// </summary>
public class PixelText : MonoBehaviour
{
    public enum Align { Left, Center, Right }

    private const int GlyphSize = 8;
    private const int Columns = 8;      // 8 tekens per rij in de PNG, tekencode - 32 = vakje
    private const int FirstChar = 32;
    private const int LastChar = 90;
    private const float DesignHeight = 180f;   // ontwerpresolutie van alle UI-art

    // Per font-PNG de losse letters, zodat niet elke tekst ze opnieuw maakt
    private static readonly Dictionary<Texture2D, Sprite[]> glyphCache = new Dictionary<Texture2D, Sprite[]>();

    private Texture2D font;
    private string text = string.Empty;
    private Align align = Align.Left;
    private bool middle;
    private bool wrap = true;
    private Color color = Color.white;
    private int lineHeight = 10;

    private readonly List<Image> glyphs = new List<Image>();
    private bool dirty;
    private float builtWidth = -1f;
    private float scale = -1f;
    private float sizeMultiplier = 1f;

    public string Text
    {
        get => text;
        set
        {
            string next = value ?? string.Empty;
            if (next == text) return;
            text = next;
            dirty = true;
        }
    }

    // Groter of kleiner dan de standaardschaal van het canvas; blijft altijd hele pixels (min. 1)
    public float SizeMultiplier
    {
        get => sizeMultiplier;
        set
        {
            if (Mathf.Approximately(value, sizeMultiplier)) return;
            sizeMultiplier = value;
            scale = -1f;
            dirty = true;
        }
    }

    public Color Color
    {
        get => color;
        set
        {
            if (value == color) return;
            color = value;
            foreach (Image glyph in glyphs)
                if (glyph != null) glyph.color = color;
        }
    }

    // Legt een tekstvak precies over een bestaand UI-element (bv. een oude TextMeshPro-tekst).
    public static PixelText Attach(RectTransform target, Texture2D font, Align align, Color color)
    {
        GameObject go = new GameObject("PixelText", typeof(RectTransform));
        go.layer = target.gameObject.layer;

        RectTransform rect = (RectTransform)go.transform;
        rect.SetParent(target, false);
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        PixelText pixelText = go.AddComponent<PixelText>();
        pixelText.font = font;
        pixelText.align = align;
        pixelText.color = color;
        pixelText.middle = true;
        pixelText.wrap = false;   // oude tekstvakken zijn vaak smaller dan de pixeltekst
        pixelText.dirty = true;
        return pixelText;
    }

    // Maakt een tekstvak op een vaste plek (pixels vanaf linksboven) in een ouder van 320x180.
    public static PixelText Create(Transform parent, string name, Rect pixels, Texture2D font, Align align,
        Color color, bool middle = false, int lineHeight = 10)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        go.layer = parent.gameObject.layer;

        RectTransform rect = (RectTransform)go.transform;
        rect.SetParent(parent, false);
        rect.anchorMin = new Vector2(pixels.xMin / 320f, 1f - pixels.yMax / 180f);
        rect.anchorMax = new Vector2(pixels.xMax / 320f, 1f - pixels.yMin / 180f);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        PixelText pixelText = go.AddComponent<PixelText>();
        pixelText.font = font;
        pixelText.align = align;
        pixelText.color = color;
        pixelText.middle = middle;
        pixelText.lineHeight = lineHeight;
        pixelText.dirty = true;
        return pixelText;
    }

    // Opbouwen pas als de breedte bekend is (canvas is in Awake nog niet altijd op maat)
    private void LateUpdate()
    {
        float width = ((RectTransform)transform).rect.width;
        if (width <= 0f) return;
        if (scale < 0f) scale = GetCanvasScale();
        if (!dirty && Mathf.Approximately(width, builtWidth)) return;

        Rebuild(width);
        dirty = false;
        builtWidth = width;
    }

    private void Rebuild(float width)
    {
        foreach (Image glyph in glyphs)
            if (glyph != null) Destroy(glyph.gameObject);
        glyphs.Clear();

        if (font == null || string.IsNullOrEmpty(text)) return;

        Sprite[] sprites = GetGlyphs(font);
        int cell = font.width / Columns;
        float advance = GlyphSize * scale;
        float line = lineHeight * scale;
        float size = cell * scale;
        float padLeft = (cell - GlyphSize) / 2f * scale;          // outline-font: letter staat midden in een groter vak
        float padTop = (cell - GlyphSize) * 3f / 8f * scale;

        List<string> lines = Wrap(text.ToUpperInvariant(), wrap ? Mathf.Max(1, Mathf.FloorToInt(width / advance)) : int.MaxValue);
        float height = ((RectTransform)transform).rect.height;
        float top = middle ? Mathf.Round((height - lines.Count * line) / 2f) : 0f;

        for (int l = 0; l < lines.Count; l++)
        {
            string row = lines[l];
            float rowWidth = row.Length * advance;
            float left = align == Align.Left ? 0f
                : align == Align.Right ? width - rowWidth
                : Mathf.Round((width - rowWidth) / 2f);

            for (int i = 0; i < row.Length; i++)
            {
                Sprite sprite = GetSprite(sprites, row[i]);
                if (sprite == null) continue;   // spatie of onbekend teken

                GameObject go = new GameObject("Letter", typeof(RectTransform));
                go.layer = gameObject.layer;
                RectTransform rect = (RectTransform)go.transform;
                rect.SetParent(transform, false);
                rect.anchorMin = rect.anchorMax = new Vector2(0f, 1f);
                rect.pivot = new Vector2(0f, 1f);
                rect.sizeDelta = new Vector2(size, size);
                rect.anchoredPosition = new Vector2(left + i * advance - padLeft, -(top + l * line) + padTop);

                Image image = go.AddComponent<Image>();
                image.sprite = sprite;
                image.color = color;
                image.raycastTarget = false;
                glyphs.Add(image);
            }
        }
    }

    // Hoeveel canvas-eenheden één pixel van de 320x180-art is (1 bij 320x180, 6 bij 1920x1080)
    private float GetCanvasScale()
    {
        Canvas canvas = GetComponentInParent<Canvas>();
        float canvasScale = 1f;

        CanvasScaler scaler = canvas != null ? canvas.rootCanvas.GetComponent<CanvasScaler>() : null;
        if (scaler != null && scaler.uiScaleMode == CanvasScaler.ScaleMode.ScaleWithScreenSize)
            canvasScale = scaler.referenceResolution.y / DesignHeight;

        return Mathf.Max(1f, Mathf.Round(canvasScale * sizeMultiplier));
    }

    // Woorden op een nieuwe regel zetten als ze niet meer passen; \n blijft een harde regelovergang.
    private static List<string> Wrap(string value, int maxChars)
    {
        List<string> lines = new List<string>();
        foreach (string paragraph in value.Replace("\r", "").Split('\n'))
        {
            string current = string.Empty;
            foreach (string word in paragraph.Split(' '))
            {
                string next = current.Length == 0 ? word : current + " " + word;
                if (next.Length > maxChars && current.Length > 0)
                {
                    lines.Add(current);
                    current = word;
                }
                else current = next;
            }
            lines.Add(current);
        }
        return lines;
    }

    private static Sprite GetSprite(Sprite[] sprites, char c)
    {
        if (c < FirstChar || c > LastChar) return null;
        return sprites[c - FirstChar];
    }

    private static Sprite[] GetGlyphs(Texture2D font)
    {
        if (glyphCache.TryGetValue(font, out Sprite[] cached) && cached != null) return cached;

        int cell = font.width / Columns;
        Sprite[] sprites = new Sprite[LastChar - FirstChar + 1];
        for (int c = FirstChar + 1; c <= LastChar; c++)   // spatie (32) blijft leeg
        {
            int index = c - FirstChar;
            int x = index % Columns * cell;
            int yFromTop = index / Columns * cell;
            if (yFromTop + cell > font.height) continue;

            // In de PNG telt y vanaf boven, in Unity vanaf onder
            Rect rect = new Rect(x, font.height - yFromTop - cell, cell, cell);
            Sprite sprite = Sprite.Create(font, rect, new Vector2(0f, 1f), 1f, 0, SpriteMeshType.FullRect);
            sprite.name = "Letter_" + (char)c;
            sprites[index] = sprite;
        }

        glyphCache[font] = sprites;
        return sprites;
    }
}
