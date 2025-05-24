using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

namespace Game
{
    public struct Voxel : INetworkSerializable
    {
        private byte _condition;
        public byte Condition
        {
            get => _condition;
            set
            {
                _condition = value;
                if (_condition == 0)
                    _color = Color.clear;
            }
        }
        private Color _color;
        public Color Color => _color;

        public Voxel(Color color)
        {
            _condition = (color == Color.clear) ? (byte)0 : (byte)1;
            _color = color;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref _condition);
            serializer.SerializeValue(ref _color);
        }

        public Color GetColor()
            => Color * Condition;

        public static implicit operator bool(Voxel voxel)
            => voxel.Color == Color.clear ? false : true;
    }
    public struct VoxelData : INetworkSerializable
    {
        private Voxel[,,] _data;
        public Voxel[,,] Data { get => _data; }
        public void SetData(Voxel[,,] data)
            => _data = data;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            Vector3Int length = Vector3Int.zero;

            if (!serializer.IsReader)
                length = new Vector3Int(_data.GetLength(0), _data.GetLength(1), _data.GetLength(2));

            serializer.SerializeValue(ref length);

            if (serializer.IsReader)
                _data = new Voxel[length.x, length.y, length.z];

            for (int x = 0; x < length.x; x++)
                for (int y = 0; y < length.y; y++)
                    for (int z = 0; z < length.z; z++)
                        ;// serializer.SerializeValue(ref _data[x, y, z]);
        }
    }

    public class WorldGenerator : Singleton<WorldGenerator>
    {
        public event UnityAction Updated = delegate { };
        public event UnityAction<Vector3Int[]> VoxelsUpdated;

        private NetworkVariable<Voxel[]> _networkData = new NetworkVariable<Voxel[]>();
        private Voxel[,,] _data;
        public Voxel[,,] Data => _data;
        public Vector3Int WorldSize => new Vector3Int(_data.GetLength(0), _data.GetLength(1), _data.GetLength(2));

        [SerializeField]
        private int _chunkSize;
        public int ChunkSize => _chunkSize;
        [SerializeField]
        private Chunk _chunkPrefab;

        private Chunk[] _chunks;

        private void Start()
        {
            _data = new Voxel[64, 1, 64];

            _chunks = new Chunk[WorldSize.x / _chunkSize * WorldSize.z / _chunkSize];
            for (int i = 0; i < _chunks.Length; i++)
            {
                _chunks[i] = Instantiate(_chunkPrefab, transform);
                _chunks[i].transform.localPosition = new Vector3(i / (WorldSize.z / _chunkSize), 0, i % (WorldSize.z / _chunkSize)) * _chunkSize;
            }
            //_networkData.Value = _data;
        }

        public void LoadMap(MapData map)
        {
            _data = map.WorldData;
        }

        public int GetHeight(int x, int z)
        {
            int height = 0;
            for (int y = 0; y < _data.GetLength(1); y++)
                if (_data[x, y, z])
                    height = y;

            return height;
        }

        public Chunk GetChunk(int voxelX, int voxelZ)
            => _chunks[voxelX / _chunkSize * WorldSize.z / _chunkSize + voxelZ / _chunkSize];
        public Chunk GetChunk(Vector3Int voxel)
            => GetChunk(voxel.x, voxel.z);
        public Chunk GetChunk(Vector3 point)
            => GetChunk(Vector3Int.FloorToInt(point));
        private Chunk GetChunk(int face)
            => _chunks
            .Where((chunk, index) => face
            < _chunks.Take(index + 1)
            .Sum(chunk_ => chunk_.FaceCount))
            .First();

        private int GetChunkIndex(Chunk chunk)
            => Array.IndexOf(_chunks.ToArray(), chunk);
        private int GetChunkIndex(Vector3Int voxel)
            => Array.IndexOf(_chunks.ToArray(), GetChunk(voxel));
        private int GetChunkIndex(int face)
            => GetChunkIndex(GetChunk(face));

        public bool GetVoxel(int x, int y, int z)
            => x >= 0 && y >= 0 && z >= 0
            && x < WorldSize.x && y < WorldSize.y && z < WorldSize.z;
        public bool GetVoxel(Vector3Int index)
            => GetVoxel(index.x, index.y, index.z);
        public bool GetVoxel(Vector3 point)
            => GetVoxel(Vector3Int.FloorToInt(point));
        public bool GetVoxel(int x, int y, int z, out Voxel voxel)
        {
            voxel = new Voxel();

            if (GetVoxel(x, y, z))
            {
                voxel = _data[x, y, z];
                return true;
            }

            return false;
        }
        public bool GetVoxel(Vector3Int index, out Voxel voxel)
            => GetVoxel(index.x, index.y, index.z, out voxel);
        public bool GetVoxel(Vector3 point, out Voxel voxel)
            => GetVoxel(Vector3Int.FloorToInt(point), out voxel);

        public Voxel[] GetVoxels(Vector3Int[] indices)
        {
            var voxels = new Voxel[indices.Length];

            for (int i = 0; i < voxels.Length; i++)
                voxels[i] = _data[indices[i].x, indices[i].y, indices[i].z];

            return voxels;
        }

        public Color GetColor(int x, int y, int z)
            => _data[x, y, z].Color;
        public Color GetColor(Vector3Int index)
            => GetColor(index.x, index.y, index.z);
        public Color GetColor(Vector3 point)
            => GetColor(Vector3Int.FloorToInt(point));

        private void SetColor(int x, int y, int z, Color color)
        {
            if (GetVoxel(x, y, z))
                _data[x, y, z] = new Voxel(color);
        }
        private void SetColor(Vector3Int index, Color color)
            => SetColor(index.x, index.y, index.z, color);
        private void SetColor(Vector3 point, Color color)
            => SetColor(Vector3Int.FloorToInt(point), color);

        public void SetVoxel(int x, int y, int z, Color color)
        {
            SetColor(x, y, z, color);

            UpdateChunk(GetChunk(x, z));

            Updated.Invoke();
        }
        public void SetVoxel(Vector3Int index, Color color)
            => SetVoxel(index.x, index.y, index.z, color);
        public void SetVoxel(Vector3 point, Color color)
            => SetVoxel(Vector3Int.FloorToInt(point), color);

        public void SetVoxels(Vector3Int[] indices, Color[] colors)
        {
            if (indices.Length != colors.Length)
                throw new ArgumentOutOfRangeException("The sizes of the index and color arrays must match.");

            for (int i = 0; i < indices.Length; i++)
                SetColor(indices[i], colors[i]);

            foreach (var chunk in indices.Select(index => GetChunk(index)))
                chunk.UpdateMesh();

            Updated.Invoke();
        }

        public int GetFace(Vector3Int voxel, Vector3 normal)
            => GetChunk(voxel)
            .GetFace(voxel - GetChunk(voxel).Point, normal)
            + _chunks
            .Take(GetChunkIndex(voxel))
            .Sum(chunk => chunk.FaceCount);

        public void SetFaceColor(int face, Color color)
            => GetChunk(face)
            .SetFaceColor(face - _chunks
                .Take(GetChunkIndex(face)).Sum(chunk => chunk.FaceCount), color);
        public void SetFaceColor(Vector3Int voxel, Vector3 normal, Color color)
        {
            if (voxel.y < 0)
            {

            }
            else
                GetChunk(voxel).SetFaceColor(voxel, normal, color);
        }

        public void SetInverseFaceColor(int face)
            => GetChunk(face)
            .SetInverseFaceColor(face - _chunks
                .Take(GetChunkIndex(face)).Sum(chunk => chunk.FaceCount));

        public void ResetFaceColor(int face)
            => GetChunk(face)
            .ResetFaceColor(face - _chunks
                .Take(GetChunkIndex(face))
                .Sum(chunk => chunk.FaceCount));

        struct Child
        {
            public int ParentMin { get; private set; }
            public int ParentMax { get; private set; }
            public int Y { get; private set; }
            public int DirectionY { get; private set; }

            public Child(int parentMin, int parentMax, int y, int directionY)
            {
                ParentMin = parentMin;
                ParentMax = parentMax;
                Y = y;
                DirectionY = directionY;
            }
        }
        public int[] GetPlane(Vector3Int voxelIndex, Vector3 normal)
        {
            var directions = new Vector3Int[]
            {
            Vector3Int.right,
            Vector3Int.up,
            Vector3Int.forward
            }.OrderBy(direction => Vector3.Dot(direction, normal)).ToArray();

            var plane = new List<Vector3Int>();

            Vector3Int GetIndex(int x, int y)
                => x * directions[0] + y * directions[1] + Vector3Int.Scale(voxelIndex, directions[2]);
            bool CheckVoxel(int x, int y)
            {
                var index = GetIndex(x, y);

                return (index.y == -1
                   || (GetVoxel(index, out Voxel voxel)
                   && voxel))
                   && GetVoxel(index + normal, out Voxel voxel_)
                   && !voxel_
                   && !plane.Contains(index);
            }
            void AddVoxel(int x, int y)
                => plane.Add(GetIndex(x, y));

            var children = new Stack<Child>();
            children.Push(
                new Child
                (
                (int)(voxelIndex * directions[0]).magnitude,
                (int)(voxelIndex * directions[0]).magnitude,
                (int)(voxelIndex * directions[1]).magnitude,
                1
                ));
            children.Push(
                new Child
                (
                (int)(voxelIndex * directions[0]).magnitude,
                (int)(voxelIndex * directions[0]).magnitude,
                (int)(voxelIndex * directions[1]).magnitude - 1,
                -1
                ));

            Child child;

            while (children.Count != 0)
            {
                child = children.Pop();

                int min, max;
                min = max = child.ParentMin;

                if (CheckVoxel(child.ParentMin, child.Y))
                {
                    while (CheckVoxel(min - 1, child.Y))
                    {
                        min--;
                        AddVoxel(min, child.Y);
                    }
                    if (min < child.ParentMin)
                        children.Push(
                            new Child
                            (
                            min,
                            child.ParentMin - 1,
                            child.Y - child.DirectionY,
                            -child.DirectionY
                            ));
                }



                while (max <= child.ParentMax)
                {
                    if (CheckVoxel(child.ParentMin, child.Y))
                        AddVoxel(max, child.Y);

                    while (CheckVoxel(max + 1, child.Y))
                    {
                        max++;
                        AddVoxel(max, child.Y);
                    }
                    if (max > child.ParentMax)
                        children.Push(
                            new Child
                            (
                                child.ParentMax + 1,
                                max,
                                child.Y - child.DirectionY,
                                -child.DirectionY
                            ));

                    if (max > min)
                        children.Push(
                            new Child
                            (
                                min,
                                max,
                                child.Y + child.DirectionY,
                                child.DirectionY
                            ));

                    max += 2;
                    while (max < child.ParentMax && !CheckVoxel(max, child.Y))
                        max++;

                    min = max;
                }
            }

            return plane
                .Select(voxel => GetFace(voxel, normal))
                .OrderBy(face => face)
                .ToArray();
        }

        private int[][] GetChunkPlanes(int[] plane)
        {
            var chunkPlanes = new int[_chunks.Length][];

            int faceCount = 0;

            for (int i = 0; i < _chunks.Length; i++)
            {
                chunkPlanes[i] = plane
                    .SkipWhile(face => face < faceCount)
                    .TakeWhile(face => face < faceCount + _chunks[i].FaceCount)
                    .Select(face => face - faceCount)
                    .ToArray();
                faceCount += _chunks[i].FaceCount;
            }

            return chunkPlanes;
        }
        public void SetPlaneColor(Vector3Int[] plane, Vector3 normal, Color color)
        {
            foreach (var chunk in plane.GroupBy(voxel => GetChunk(voxel)).Select(group => group.Key))
                chunk.SetPlaneColor(plane.Where(voxel => GetChunk(voxel) == chunk).Select(voxel => voxel - chunk.Point).ToArray(), normal, color);

            Updated.Invoke();
        }
        public void SetPlaneColor(int[] plane, Color color)
        {
            var chunkPlanes = GetChunkPlanes(plane);
            for (int i = 0; i < chunkPlanes.Length; i++)
                _chunks[i].SetPlaneColor(chunkPlanes[i], color);
        }

        public void SetInversePlaneColor(int[] plane)
        {
            var chunkPlanes = GetChunkPlanes(plane);
            for (int i = 0; i < chunkPlanes.Length; i++)
                _chunks[i].SetInversePlaneColor(chunkPlanes[i]);
        }

        public void ResetPlaneColor(Vector3Int[] plane, Vector3 normal)
        {
            foreach (var chunk in plane.GroupBy(voxel => GetChunk(voxel)).Select(group => group.Key))
                chunk.ResetPlaneColor(plane.Where(voxel => GetChunk(voxel) == chunk).Select(voxel => voxel - chunk.Point).ToArray(), normal);
        }
        public void ResetPlaneColor(int[] plane)
        {
            var chunkPlanes = GetChunkPlanes(plane);
            for (int i = 0; i < chunkPlanes.Length; i++)
                _chunks[i].ResetPlaneColor(chunkPlanes[i]);
        }

        public void SetPlaneVoxel(Vector3Int[] plane, Color color)
        {
            foreach (var voxel in plane)
                SetColor(voxel, color);

            print(plane.Count(voxel => GetChunk(voxel)));
            foreach (var chunk in plane.Select(voxel => GetChunk(voxel)))
                chunk.UpdateMesh();

            Updated.Invoke();
        }
        public void SetPlaneVoxel(Vector3Int[] plane, Vector3 normal, Color color)
        {
            foreach (var voxel in plane)
                SetColor(voxel + normal, color);

            foreach (var chunk in plane.Select(voxel => GetChunk(voxel)))
                chunk.UpdateMesh();

            Updated.Invoke();
        }
        public Vector3Int[] SetPlaneVoxel(int[] plane, Vector3 normal, Color color)
        {
            var voxels = new List<Vector3Int>();

            var chunkPlanes = GetChunkPlanes(plane);
            for (int i = 0; i < chunkPlanes.Length; i++)
                voxels.AddRange(_chunks[i].SetPlaneVoxel(chunkPlanes[i], color));

            Updated.Invoke();

            return voxels.ToArray();
        }

        public void Damage(float damage, RaycastHit hit)
            => DamageServerRpc(damage, hit.point, -hit.normal);
        [ServerRpc]
        public void DamageServerRpc(float damage, Vector3 point, Vector3 direction)
        {
            point = point - transform.position + direction;
            //_networkData.Value[Mathf.FloorToInt(point.x), Mathf.FloorToInt(point.y), Mathf.FloorToInt(point.z)].Condition -= (byte)(damage / 100 * 255);
            //DamageClientRpc();
        }
        [ClientRpc]
        public void DamageClientRpc(float damage, Vector3 point, Vector3 direction)
        {
            point = (point - transform.position) * 2 + direction;
            _data[Mathf.FloorToInt(point.x), Mathf.FloorToInt(point.y), Mathf.FloorToInt(point.z)].Condition -= (byte)(damage / 100 * 255);
        }

        private void UpdateChunk(Chunk chunk)
            => chunk.UpdateMesh();
    }
}
