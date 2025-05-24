using Unity.Netcode;
using UnityEngine;

public struct SerializedArray
{
    public void Serialize<T>(ref BufferSerializer<T> serializer) where T : IReaderWriter
    {

    }
}

public struct GameModeData : INetworkSerializable
{
    [SerializeField]
    private Transform[] _spawnPoints;
    public Transform[] SpawnPoints => _spawnPoints;

    private Vector3[] _spawnPointPositions;
    public Vector3[] SpawnPointsPositions => _spawnPointPositions;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        int length = 0;

        if (serializer.IsWriter)
            length = _spawnPointPositions.Length;

        serializer.SerializeValue(ref length);

        if (serializer.IsReader)
            _spawnPointPositions = new Vector3[length];

        for (int i = 0; i < length; i++)
            serializer.SerializeValue(ref _spawnPointPositions[i]);
    }
}
