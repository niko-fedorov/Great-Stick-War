using UnityEngine;

public enum ControlActionType
{

}

[System.Serializable]
public struct ControlData
{
    [SerializeField]
    private string _name;
    public string Name => _name;

    [SerializeField]
    private ControlActionType _type;
    public ControlActionType Type => _type;

    [SerializeField]
    private KeyCode _key;
    public KeyCode Key => _key;
}

