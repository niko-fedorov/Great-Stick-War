using System.Linq;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu]
    public class SideDatabase : ScriptableObject
    {
        [SerializeField]
        private SideData[] _datas;
        public SideData[] Datas => _datas;

        public SideData this[SideType type]
            => _datas.First(data => data.Type == type);

        //public static SideData[] GetDatas()
        //    => GameManager.Instance.SideDatas;

        //public static SideData GetData(SideType type)
        //    => GameManager.Instance.GetSideData(type);
    }
}