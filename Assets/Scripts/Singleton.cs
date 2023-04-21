using Unity.Netcode;
using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance { get; private set; }

    private void Awake()
    {
        Instance = this as T;

        Initialize();
    }

    protected virtual void Initialize() { }
}
