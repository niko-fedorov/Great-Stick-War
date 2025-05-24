using Unity.Netcode;
using UnityEngine;

namespace Game
{
    public struct MapData : INetworkSerializable
    {
        #region Variables

        [SerializeField]
        private string _name;
        public string Name => _name;

        [SerializeField]
        private Sprite _icon;
        public Sprite Icon => _icon;

        [SerializeField]
        private Vector2Int _worldPoint;
        public Vector2Int WorldPoint => _worldPoint;

        [SerializeField]
        private GameModeData _gameModeDatas;
        public GameModeData GameModeDatas => _gameModeDatas;

        [SerializeField]
        private Voxel[,,] _worldData;
        public Voxel[,,] WorldData => _worldData;

        #endregion

        #region Methods

        public MapData(string name, Sprite icon, Vector2Int worldPoint, GameModeData gameModeData, Voxel[,,] worlData)
        {
            _name = name;
            _icon = icon;
            _worldPoint = worldPoint;
            _gameModeDatas = gameModeData;
            _worldData = worlData;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            //serializer.SerializeValue(ref _name);
            //serializer.SerializeValue(ref _icon);
            //serializer.SerializeValue(ref _worldPoint);
            //serializer.SerializeValue(ref _gameModeDatas);
            //serializer.SerializeValue(ref _worldData);
        }

        #endregion
    }
}