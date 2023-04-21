using UnityEngine;

public interface IInteractable
{
    public string ActionText { get; }
    public string Name { get; }

    public Color TextColor { get; }

    public bool IsInteractable { get; }
}
