using UnityEngine;

public class UIKillfeedPanel : MonoBehaviour
{
    [SerializeField]
    private UIKillfeedElement _elementPrefab;

    private void Start()
    {
        foreach(var vicitim in FindObjectsOfType<Player>())
            vicitim.Died += (killer) =>
                Instantiate(_elementPrefab, transform).Initialize(vicitim, killer);
    }
}
