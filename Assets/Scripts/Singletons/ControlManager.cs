using System;
using UnityEngine;
using UnityEngine.Events;

public class ControlManager : Singleton<ControlManager>
{
    public event UnityAction<KeyCode> AnyKeyPressed;

    [SerializeField]
    private ControlsData _controlsData;
    public ControlsData ControlsData => _controlsData;

    private void Start()
    {
        
    }

    private void Update()
    {
        foreach(KeyCode key in Enum.GetValues(typeof(KeyCode)))
            if(Input.GetKeyDown(key))
                AnyKeyPressed.Invoke(key);
    }
}
