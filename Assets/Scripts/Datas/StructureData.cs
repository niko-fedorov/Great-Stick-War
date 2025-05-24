using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "NewSideData", menuName = "Scriptable Object/Side Data")]
    public class StructureData : ScriptableObject
    {
        #region Variables

        public string Name { get; private set; }
        public Voxel[,,] Data { get; private set; }

        #endregion

        #region Methods

        public void Initialize(string name, Voxel[,,] data)
        {
            Name = name;
            Data = data;
        }

        #endregion
    }
}
