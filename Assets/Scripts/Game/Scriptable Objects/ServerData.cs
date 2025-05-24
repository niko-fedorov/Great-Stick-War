using Unity.Netcode;

namespace Game
{
    public struct ServerData : INetworkSerializable
    {
        private MapData _mapData;
        public MapData MapData => _mapData;
        private SideType[] _sides;
        public SideType[] Sides => _sides;

        public ServerData(MapData map, SideType[] sides)
        {
            _mapData = map;
            _sides = sides;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref _mapData);

            int length = 0;

            if (serializer.IsWriter)
                length = _sides.Length;

            serializer.SerializeValue(ref length);

            if (serializer.IsReader)
                _sides = new SideType[length];

            for (int i = 0; i < length; i++)
                serializer.SerializeValue(ref _sides[i]);
        }
    }

}
