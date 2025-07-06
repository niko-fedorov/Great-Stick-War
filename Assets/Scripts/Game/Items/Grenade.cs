using UnityEngine;

namespace Game
{
    public class Grenade : Gun
    {
        public new GrenadeData Data => base.Data as GrenadeData;

        public override void Activate(Vector3 origin, Vector3 direction, ActivationType type)
        {
            this.StartTimer(Data.ExplosionTime, () =>
            {
                foreach (var collider in Physics.OverlapSphere(transform.position, Data.DamageRadius))
                {
                    if( collider.TryGetComponent<IDamageable>(out var damageable) && !Physics.Linecast(transform.position, collider.transform.position, out var hit))
                        damageable.Damage(0, hit);
                }
            });
        }
    }
}
