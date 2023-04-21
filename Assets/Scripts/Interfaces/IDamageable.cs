using UnityEngine;

public enum SurfaceType
{
    Entity,
    Ground,
    Wood,
    Metal,
    Glass
}

public interface IDamageable
{
    public event System.Action Damaged;

    public float Health { get; }

    public void Damage(float damage, RaycastHit hit);
}
