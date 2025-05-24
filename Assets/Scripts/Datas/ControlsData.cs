using UnityEngine;

[CreateAssetMenu(fileName = "NewControlsData", menuName = "Scriptable Object")]
public class ControlsData : ScriptableObject
{
    [SerializeField]
    private ControlData[] _controls;
    public ControlData[] Controls => _controls;
}
