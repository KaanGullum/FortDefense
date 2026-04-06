using System;
using FortDefense.Data;
using UnityEngine;

namespace FortDefense.Gameplay
{
    public class CoreBase : MonoBehaviour, IDamageable
    {
        public event Action<int, int> HealthChanged;
        public event Action Destroyed;

        public int MaxHealth { get; private set; }
        public int CurrentHealth { get; private set; }

        public void Initialize(int maxHealth)
        {
            MaxHealth = maxHealth;
            CurrentHealth = maxHealth;
            HealthChanged?.Invoke(CurrentHealth, MaxHealth);
        }

        public void ApplyDamage(float damage, DamageKind damageKind, Vector3 hitPoint)
        {
            ApplyCoreDamage(Mathf.CeilToInt(damage));
        }

        public void ApplyCoreDamage(int amount)
        {
            if (amount <= 0 || CurrentHealth <= 0)
            {
                return;
            }

            CurrentHealth = Mathf.Max(0, CurrentHealth - amount);
            HealthChanged?.Invoke(CurrentHealth, MaxHealth);

            if (CurrentHealth <= 0)
            {
                Destroyed?.Invoke();
            }
        }
    }
}

