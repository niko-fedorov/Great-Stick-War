using UnityEngine;
using UnityEngine.Events;

public interface IDamageable
{
    public event UnityAction Damaged;

    public enum Type
    {
        Default,
        Metal
    }
    public int Health { get; }

    public void Damage(int damage, RaycastHit hit);
}
