using System.Linq;
using UnityEngine;

namespace Game
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
    public class ChunkGrid : MonoBehaviour
    {
        private Vector3Int _worldSize => WorldGenerator.Instance.WorldSize;
        private int _chunkSize => WorldGenerator.Instance.ChunkSize;

        public Mesh Mesh { get; private set; }

        private void Start()
        {
            Mesh = GetComponent<MeshFilter>().mesh = new Mesh();

            Mesh.SetVertices(
                Enumerable.Repeat(new Vector3[4] { Vector3.zero, Vector3.forward, Vector3.forward + Vector3.right, Vector3.right }, _chunkSize * _chunkSize)
                .SelectMany(vertices => vertices)
                .Select((vertice, index) => vertice + new Vector3(index / 4 / _chunkSize, 0, index / 4 % _chunkSize))
                .ToArray());
            Mesh.SetTriangles(
                Enumerable.Repeat(new int[6] { 0, 1, 2, 0, 2, 3 }, Mesh.vertexCount / 4)
                .SelectMany(triangle => triangle).Select((triangle, index) => triangle + index / 6 * 4).ToArray(), 0);
            Mesh.SetColors(
                Enumerable.Repeat(Color.clear, Mesh.vertexCount)
                .ToArray());
            Mesh.SetUVs(0,
                Enumerable.Repeat(new Vector2[4] { Vector2.zero, Vector2.up, Vector2.up + Vector2.right, Vector2.right }, Mesh.vertexCount / 4)
                .SelectMany(uvs => uvs).ToArray());

            GetComponent<MeshCollider>().sharedMesh = Mesh;
        }

        public void SetFaceColor(int index, Color color)
        {
            var colors = Mesh.colors;

            for (int i = 0; i < 4; i++)
                colors[index * 4 + i] = color;

            Mesh.SetColors(colors);
        }
        public void ResetFaceColor(int index)
            => SetFaceColor(index, Color.clear);

        public void SetPlaneColor(int[] plane, Color color)
        {
            var colors = Mesh.colors;

            foreach (var face in plane)
                for (int i = 0; i < 4; i++)
                    colors[face * 4 + i] = color;

            Mesh.SetColors(colors);
        }
        public void ResetPlaneColor(int[] plane)
        {
            var colors = Mesh.colors;

            foreach (var face in plane)
                for (int i = 0; i < 4; i++)
                    colors[face * 4 + i] = Color.clear;

            Mesh.SetColors(colors);
        }
    }
}
