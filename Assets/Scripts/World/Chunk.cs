using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Game
{
    public class Chunk : MonoBehaviour, IDamageable
    {
        #region Variables

        public int Health => throw new NotImplementedException();

        public event UnityAction Damaged;

        public int FaceCount => _mesh.vertexCount / 4;

        private Voxel[,,] _data => WorldGenerator.Instance.Data;
        private Vector3Int _worldSize => WorldGenerator.Instance.WorldSize;
        private int _chunkSize => WorldGenerator.Instance.ChunkSize;
        public Vector3Int Point => Vector3Int.FloorToInt(transform.position);

        private ChunkGrid _grid;

        private Mesh _mesh;
        private MeshCollider _collider;

        #endregion

        #region Methods

        public void Damage(int damage, RaycastHit hit)
        {
            
        }

        void Start()
        {
            _mesh = GetComponent<MeshFilter>().mesh = new Mesh();
            _collider = GetComponent<MeshCollider>();

            _grid = GetComponentInChildren<ChunkGrid>();

            UpdateMesh();
        }

        public Vector3Int GetVoxelIndex(int face)
            => Point + Vector3Int.FloorToInt(
               _mesh.vertices[face * 4] -
               Vector3.Cross(_mesh.vertices[face * 4 + 2] - _mesh.vertices[face * 4 + 1],
               _mesh.vertices[face * 4] - _mesh.vertices[face * 4 + 1]) / 2);

        private bool GetVoxel(int x, int y, int z)
            => true;
        private bool GetVoxel(int x, int y, int z, out Voxel voxel)
        {
            voxel = _data[x + Point.x, y + Point.y, z + Point.z];
            return true;
        }
        private Voxel GetVoxel(Vector3Int index)
            => index.y == -1 ? new Voxel() : _data[index.x, index.y, index.z];
        public Voxel GetVoxel(int face)
            => GetVoxel(GetVoxelIndex(face));

        public int GetFace(Vector3Int voxel, Vector3 normal)
            => _mesh.vertices
            .Select((vertice, index) => (vertice, index))
            .First(pair => pair.vertice == voxel + Vector3Int.CeilToInt(normal / 2)
            && pair.index % 4 == 0
            && normal == Vector3.Cross(_mesh.vertices[pair.index + 2] - _mesh.vertices[pair.index + 1],
            _mesh.vertices[pair.index] - _mesh.vertices[pair.index + 1]))
            .index / 4;

        public void SetFaceColor(int index, Color color)
        {
            var colors = _mesh.colors;

            for (int i = 0; i < 4; i++)
                colors[index * 4 + i] = color;

            _mesh.SetColors(colors);
        }
        public void SetFaceColor(Vector3Int voxelIndex, Vector3 normal, Color color)
            => SetFaceColor(GetFace(voxelIndex, normal), color);

        public void SetInverseFaceColor(int face)
            => SetFaceColor(face, GetVoxel(face).Color.Inverse());

        public void ResetFaceColor(int face)
            => SetFaceColor(face, GetVoxel(face).Color);

        public void SetPlaneColor(Vector3Int[] plane, Vector3 normal, Color color)
        {
            var colors = _mesh.colors;

            foreach (var voxel in plane)
            {
                var face = GetFace(voxel, normal);
                for (int i = 0; i < 4; i++)
                    colors[face * 4 + i] = color;
            }

            _mesh.SetColors(colors.ToArray());
        }
        public void SetPlaneColor(int[] plane, Color color)
        {
            var colors = _mesh.colors;

            foreach (var face in plane)
                for (int i = 0; i < 4; i++)
                    colors[face * 4 + i] = color;

            _mesh.SetColors(colors.ToArray());
        }

        public void SetInversePlaneColor(int[] plane)
        {
            var colors = _mesh.colors;

            foreach (var face in plane)
                for (int i = 0; i < 4; i++)
                    colors[face * 4 + i] = GetVoxel(face).Color.Inverse();

            _mesh.SetColors(colors.ToArray());
        }

        public void ResetPlaneColor(Vector3Int[] plane, Vector3 normal)
        {
            var colors = _mesh.colors;

            foreach (var voxel in plane)
            {
                var face = GetFace(voxel, normal);
                var color = GetVoxel(voxel).Color;
                for (int i = 0; i < 4; i++)
                    colors[face * 4 + i] = color;
            }

            _mesh.SetColors(colors.ToArray());
        }
        public void ResetPlaneColor(int[] plane)
        {
            var colors = _mesh.colors;

            foreach (var face in plane)
            {
                var color = GetVoxel(face).Color;
                for (int i = 0; i < 4; i++)
                    colors[face * 4 + i] = color;
            }

            _mesh.SetColors(colors.ToArray());
        }

        public Vector3Int[] SetPlaneVoxel(int[] plane, Color color)
        {
            return null;
        }

        public void UpdateMesh()
        {
            var vertices = new List<Vector3>();
            var colors = new List<Color>();

            for (int x = 0; x < _chunkSize; x++)
                for (int y = 0; y < _worldSize.y; y++)
                    for (int z = 0; z < _chunkSize; z++)
                    {
                        var vertices_ = new List<Vector3>();

                        if (GetVoxel(x, y, z, out var voxel))
                        {
                            if (x == 0 || !GetVoxel(x - 1, y, z))
                                vertices_.AddRange(new Vector3[4] { Vector3.zero, Vector3.forward, Vector3.forward + Vector3.up, Vector3.up });
                            if (x == _worldSize.x - 1 || !GetVoxel(x + 1, y, z))
                                vertices_.AddRange(new Vector3[4] { Vector3.right, Vector3.right + Vector3.up, Vector3.one, Vector3.right + Vector3.forward });
                            if (y == 0 || !GetVoxel(x, y - 1, z))
                                vertices_.AddRange(new Vector3[4] { Vector3.zero, Vector3.right, Vector3.forward + Vector3.right, Vector3.forward });
                            if (y == _worldSize.y - 1 || !GetVoxel(x, y + 1, z))
                                vertices_.AddRange(new Vector3[4] { Vector3.up, Vector3.up + Vector3.forward, Vector3.one, Vector3.up + Vector3.right });
                            if (z == 0 || !GetVoxel(x, y, z - 1))
                                vertices_.AddRange(new Vector3[4] { Vector3.zero, Vector3.up, Vector3.right + Vector3.up, Vector3.right });
                            if (z == _worldSize.z - 1 || !GetVoxel(x, y, z + 1))
                                vertices_.AddRange(new Vector3[4] { Vector3.forward, Vector3.forward + Vector3.right, Vector3.one, Vector3.up });

                            colors.AddRange(Enumerable.Repeat(voxel.Color, vertices_.Count));
                        }
                        else if (y == 0)
                        {
                            vertices_.AddRange(new Vector3[4] { Vector3.zero, Vector3.forward, Vector3.forward + Vector3.right, Vector3.right });
                            colors.AddRange(Enumerable.Repeat(Color.clear, 4));
                        }

                        vertices.AddRange(vertices_.Select(vertice => vertice + new Vector3(x, y, z)));
                    }

            _mesh.Clear();
            _mesh.SetVertices(vertices);
            _mesh.SetTriangles(
                Enumerable.Repeat(new int[6] { 0, 1, 2, 0, 2, 3 }, vertices.Count / 4)
                .SelectMany(triangle => triangle).Select((triangle, index) => triangle + index / 6 * 4).ToArray(), 0);
            _mesh.SetColors(colors);

            _collider.sharedMesh = _mesh;
        }
    }

    #endregion
}