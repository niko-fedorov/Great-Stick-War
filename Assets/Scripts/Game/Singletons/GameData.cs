using UnityEngine;
using System.Linq;

namespace Game
{
    public class GameData : Singleton<GameData>
    {
        #region Variables

        [System.Serializable]
        public struct ActionColor
        {
            public enum Types
            {
                Default,
                Damage,
                Death
            }
            [SerializeField]
            private Types _type;
            public Types Type => _type;

            [SerializeField]
            private Color _color;
            public Color Color => _color;
        }
        [SerializeField]
        private ActionColor[] _actionColors;
        public Color GetActionColor(ActionColor.Types type)
            => _actionColors.First(x => x.Type == type).Color;

        [SerializeField]
        private SideDatabase _sideDatabase;
        public SideData[] SideDatas => _sideDatabase.Datas;
        public SideData GetSideData(SideType type)
            => _sideDatabase[type];

        [SerializeField]
        private ClassDatabase _classDatabase;
        public ClassData[] ClassDatas => _classDatabase.Datas;
        public ClassData GetClassData(ClassType type)
            => ClassDatas.First(classData => classData.Type == type);
        #endregion
    }
}
