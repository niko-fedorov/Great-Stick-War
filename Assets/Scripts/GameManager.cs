using UnityEngine;
using System.Linq;

public enum ActionType
{
    Default,
    Damage,
    Death
}
public enum SideType
{
    Germany,
    Austro_Hungarian
}

public class GameManager : Singleton<GameManager>
{
    [System.Serializable]
    private struct ActionColor
    {
        [SerializeField]
        private ActionType _type;
        public ActionType Type => _type;
        [SerializeField]
        private Color _color;
        public Color Color => _color;
    }
    [SerializeField]
    private ActionColor[] _actionColors;
    public Color GetActionColor(ActionType actionType)
        => _actionColors.First(x => x.Type == actionType).Color;

    [System.Serializable]
    private struct SideData
    {
        [SerializeField]
        private SideType _type;
        public SideType Type => _type;
        [SerializeField]
        private Color _color;
        public Color Color => _color;
    }
    [SerializeField]
    private SideData[] _sideDatas;
    public Color GetSideColor(SideType sideType)
        => _sideDatas.First(x => x.Type == sideType).Color;

    private void Start()
    {
        DontDestroyOnLoad(this);
    }
}
