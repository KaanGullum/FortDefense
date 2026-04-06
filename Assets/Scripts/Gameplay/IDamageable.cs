using FortDefense.Data;
using UnityEngine;

namespace FortDefense.Gameplay
{
    public interface IDamageable
    {
        void ApplyDamage(float damage, DamageKind damageKind, Vector3 hitPoint);
    }
}

