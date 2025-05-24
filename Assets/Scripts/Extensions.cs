using UnityEngine;

public static class Extensions
{
    public static Color Inverse(this Color color)
        => new Color(1 - color.r, 1 - color.g, 1 - color.b, 1);
}

