using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class MapGenerator : NetworkBehaviour, IDamageable
{
    struct WorldData
    {

    }

    [SerializeField]
    private Vector3Int _worldSize;

    private struct Voxel : INetworkSerializable
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
            _condition = 1;
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
            => voxel.Color != Color.clear ? true : false;
    }
    private struct VoxelData : INetworkSerializable
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
    //private Net<Voxel> _worldData = new NetworkList<Voxel>();
    private Voxel[,,] _data;//_worldData.Value;

    public float Health => throw new System.NotImplementedException();

    public event System.Action Damaged;
    public event System.Action Died;

    private Mesh _mesh;
    private MeshFilter _filter;
    private MeshCollider _collider;

    private void Start()
    {
        _filter = GetComponent<MeshFilter>();
        _collider = GetComponent<MeshCollider>();

        transform.position -= new Vector3(_worldSize.x / 4f, _worldSize.y / 2f, _worldSize.z / 4f);

        //var data = new Voxel[_worldSize.x, _worldSize.y, _worldSize.z];

        if (IsServer)
        {
            //for (int i = 0; i < data.Length; i++)
            //    data[i] = new Voxel(Random.ColorHSV());

            _data = new Voxel[_worldSize.x, _worldSize.y, _worldSize.z];

            for (int x = 0; x < _worldSize.x; x++)
                for (int y = 0; y < _worldSize.y; y++)
                    for (int z = 0; z < _worldSize.z; z++)
                        _data[x, y, z] = new Voxel(Random.ColorHSV());

            UpdateMesh();
        }
    }

    public void Damage(float damage, RaycastHit hit)
        => DamageClientRpc(damage, hit.point, -hit.normal);

    [ServerRpc]
    public void DamageServerRpc(float damage, Vector3 point, Vector3 direction)
    {
        point = point - transform.position + direction;
        _data[Mathf.FloorToInt(point.x), Mathf.FloorToInt(point.y), Mathf.FloorToInt(point.z)].Condition -= (byte) (damage / 100 * 255);
        //DamageClientRpc
    }

    [ClientRpc]
    public void DamageClientRpc(float damage, Vector3 point, Vector3 direction)
    {
        point = (point - transform.position) * 2 + direction;
        print(point);
        _data[Mathf.FloorToInt(point.x), Mathf.FloorToInt(point.y), Mathf.FloorToInt(point.z)].Condition -= (byte) (damage / 100 * 255);
        UpdateMesh();
    }
    private void UpdateMesh()
    {
        var vertices = new List<Vector3>();
        var colors = new List<Color>();
        List<Vector3> vertices_;

        for (int x = 0; x < _worldSize.x; x++)
            for (int y = 0; y < _worldSize.y; y++)
                for (int z = 0; z < _worldSize.z; z++)
                {
                    if (_data[x, y, z])
                    {
                        vertices_ = new List<Vector3>();

                        if (x != 0 && !_data[x - 1, y, z])
                            vertices_.AddRange(new Vector3[4] { Vector3.forward, new Vector3(0, 1, 1), Vector3.up, Vector3.zero });
                        if (x != _worldSize.x - 1 && !_data[x + 1, y, z])
                            vertices_.AddRange(new Vector3[4] { Vector3.right, new Vector3(1, 1, 0), Vector3.one, new Vector3(1, 0, 1) });
                        if (y == 0 || !_data[x, y - 1, z])
                            vertices_.AddRange(new Vector3[4] { Vector3.up, new Vector3(0, 1, 1), Vector3.one, new Vector3(1, 1, 0) });
                        if (y != _worldSize.y - 1 && !_data[x, y + 1, z])
                            vertices_.AddRange(new Vector3[4] { Vector3.forward, Vector3.zero, Vector3.right, new Vector3(1, 0, 1) });
                        if (z != 0 && !_data[x, y, z - 1])
                            vertices_.AddRange(new Vector3[4] { Vector3.zero, Vector3.up, new Vector3(1, 1, 0), Vector3.right });
                        if (z != _worldSize.z - 1 && !_data[x, y, z + 1])
                            vertices_.AddRange(new Vector3[4] { new Vector3(1, 0, 1), Vector3.one, new Vector3(0, 1, 1), Vector3.forward });

                        vertices.AddRange(vertices_.Select(v => (v + new Vector3(x, y, z)) / 2));
                        colors.AddRange(Enumerable.Repeat(_data[x, y, z].GetColor(), vertices_.Count));
                    }
                }

        _mesh = _filter.mesh = new Mesh();
        _mesh.SetVertices(vertices);
        _mesh.SetTriangles(Enumerable.Repeat(new int[6] { 0, 1, 2, 0, 2, 3 }, vertices.Count / 4).SelectMany(t => t).Select((t, i) => t + i / 6 * 4).ToArray(), 0);
        _mesh.SetColors(colors);
        _collider.sharedMesh = _mesh;
    }
}
