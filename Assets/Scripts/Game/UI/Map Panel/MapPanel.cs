using System.Linq;
using UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game.UI
{
    public class MapPanel : VisualElement, IInitializeable
    {
        public void Initialize()
        {
            var mapTexture = new Texture2D(WorldGenerator.Instance.WorldSize.x, WorldGenerator.Instance.WorldSize.z);

            var pixels = new Color[mapTexture.width, mapTexture.height];
            for (int x = 0; x < mapTexture.width; x++)
                for (int y = 0; y < mapTexture.height; y++)
                    if (WorldGenerator.Instance.GetVoxel(x, WorldGenerator.Instance.GetHeight(x, y), y, out Voxel voxel))
                        pixels[x, y] = voxel.Color;

            mapTexture.SetPixels(pixels.Cast<Color>().ToArray());
            mapTexture.Apply();

            this.Q<Image>("image_map").sprite = Sprite.Create(mapTexture, new Rect(0, 0, mapTexture.width, mapTexture.height), Vector2.one / 2);

            //PlayerController.Spawned += (player) =>
            //    Instantiate(_playerMarkPrefab, transform).Initialize(player.transform);

            WorldGenerator.Instance.Updated += () =>
            {
                //StartCoroutine(UpdateMap());
            };
            WorldGenerator.Instance.VoxelsUpdated += (voxels) =>
            {
                var pixels = mapTexture.GetPixels();
                for (int i = 0; i < voxels.Length; i++)
                    if (WorldGenerator.Instance.GetVoxel(voxels[i], out Voxel voxel))
                        pixels[i] = voxel.Color;

                mapTexture.Apply();
                mapTexture.SetPixels(pixels);
            };
        }

        //public Vector3 WorldToMapPoint(Vector3 position)
        //    => new Vector3();

        //private IEnumerator UpdateMap()
        //{
        //    var pixels = new Color[_mapTexture.width * _mapTexture.height];
        //    for (int x = 0; x < _mapTexture.width; x++)
        //        for (int y = 0; y < _mapTexture.height; y++)
        //        {
        //            if (WorldGenerator.Instance.GetVoxel(x, WorldGenerator.Instance.GetHeight(x, y), y, out Voxel voxel))
        //                pixels[x / _mapTexture.height + y % _mapTexture.height] = voxel.Color;
        //        }

        //    _mapTexture.SetPixels(pixels);
        //    _mapTexture.Apply();

        //    yield return null;
        //}
    }
}

