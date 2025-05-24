using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public static class MonoBehaviourExtension
{
    public static Coroutine StartTimer(this MonoBehaviour monoBehaviour, float duration, UnityAction callback)
    {
        IEnumerator Routine()
        {
            yield return new WaitForSeconds(duration);
            callback.Invoke();
        }
        
        return monoBehaviour.StartCoroutine(Routine());
    }
}