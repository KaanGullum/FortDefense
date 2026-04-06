using UnityEngine;

namespace FortDefense.Data
{
    [CreateAssetMenu(menuName = "Fort Defense/Enemy Definition", fileName = "EnemyDefinition")]
    public class EnemyDefinition : ScriptableObject
    {
        [Header("Identity")]
        public string Id = "enemy";
        public string DisplayName = "Enemy";
        public EnemyVisualStyle VisualStyle = EnemyVisualStyle.Runner;

        [Header("Stats")]
        public float MaxHealth = 30f;
        public float MoveSpeed = 3f;
        public int CoreDamage = 1;
        public float Scale = 1f;

        [Header("Damage Modifiers")]
        [Range(0.1f, 2f)] public float LightDamageMultiplier = 1f;
        [Range(0.1f, 2f)] public float HeavyDamageMultiplier = 1f;
        [Range(0.1f, 2f)] public float ExplosiveDamageMultiplier = 1f;

        [Header("Visuals")]
        public Color PrimaryColor = Color.white;

        public float GetDamageMultiplier(DamageKind damageKind)
        {
            switch (damageKind)
            {
                case DamageKind.Light:
                    return LightDamageMultiplier;
                case DamageKind.Heavy:
                    return HeavyDamageMultiplier;
                case DamageKind.Explosive:
                    return ExplosiveDamageMultiplier;
                default:
                    return 1f;
            }
        }
    }
}

