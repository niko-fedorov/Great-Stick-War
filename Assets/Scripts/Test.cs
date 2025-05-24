using Unity.Netcode;
using UnityEngine;

public class Test : MonoBehaviour
{
    private void Start()
    {
        NetworkManager.Singleton.StartHost();
    }
}
