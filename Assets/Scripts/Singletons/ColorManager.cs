using System.Linq;
using UnityEngine;

public enum ColorType
{
    Green,
    Red
}

[System.Serializable]
public struct Colors
{
    [System.Serializable]
    private struct ColorData
    {
        [SerializeField]
        private ColorType _type;
        public ColorType Type => _type;
        [SerializeField]
        private Color _color;
        public Color Color => _color;
    }
    [SerializeField]
    private ColorData[] _colors;

    public Color this[ColorType type]
        => _colors.First(color => color.Type == type).Color;
}
