using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public struct EditAction
{
    public Vector3Int[] Voxels { get; private set; }
    public Color[] Colors { get;private set; }

    public EditAction(Vector3 point, Color color)
    {
        Voxels = new Vector3Int[] { Vector3Int.FloorToInt(point) };
        Colors = new Color[] { color };
    }
    public EditAction(Vector3Int index, Color color)
    {
        Voxels = new Vector3Int[] { index };
        Colors = new Color[] { color };
    }

    public EditAction(Vector3[] voxels, Color[] colors)
    {
        Voxels = voxels.Select(voxel => Vector3Int.FloorToInt(voxel)).ToArray();
        Colors = colors.ToArray();
    }

    public EditAction(Vector3Int[] voxels, Color[] colors)
    {
        Voxels = voxels;
        Colors = colors;
    }

    public EditAction(Vector3Int[] voxels, Color color) : this(voxels, new Color[voxels.Length]) { }
}

