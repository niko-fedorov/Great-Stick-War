using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu]
public class UIData : ScriptableObject
{
    #region Variables

    [SerializeField]
    private VisualTreeAsset _actionPanelVisualTreeAsset;
    public VisualTreeAsset ActionPanelVisualTreeAsset => _actionPanelVisualTreeAsset;

    #endregion
}