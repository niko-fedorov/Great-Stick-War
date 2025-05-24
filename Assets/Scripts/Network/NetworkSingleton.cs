using Unity.Netcode;
using UnityEngine;

public class NetworkSingleton<T> : NetworkBehaviour where T : NetworkBehaviour
{
    #region Variables

    private static T _instance;
    public static T Instance
        => _instance ? _instance : FindFirstObjectByType<T>();

    [SerializeField]
    private bool _dontDestroyOnLoad;

    #endregion

    #region Methods

    private void Awake()
    {
        _instance = this as T;

        if (_dontDestroyOnLoad)
            DontDestroyOnLoad(this);

        OnAwake();
    }

    protected virtual void OnAwake() { }

    #endregion
}
