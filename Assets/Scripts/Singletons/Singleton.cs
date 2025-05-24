using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    #region Variables

    private static T _instance;
    public static T Instance => _instance ? _instance : FindFirstObjectByType<T>();

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

    public void StartTimer(float duration, UnityAction callback)
        => StartCoroutine(Timer(duration, callback));
    private IEnumerator Timer(float duration, UnityAction callback)
    {
        yield return new WaitForSeconds(duration);
        callback.Invoke();
    }

    #endregion
}
