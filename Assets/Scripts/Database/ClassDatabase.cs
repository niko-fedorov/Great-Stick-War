using System.Linq;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu]
    public class ClassDatabase : ScriptableObject
    {
        [SerializeField]
        private ClassData[] _datas;
        public ClassData[] Datas => _datas;

        public ClassData this[ClassType type]
            => _datas.First(data => data.Type == type);

        //public static ClassData[] GetDatas()
        //    => GameManager.Instance.ClassDatas;

        //public static ClassData GetData(ClassType type)
        //    => GameManager.Instance.GetClassData(type);
    }
}
