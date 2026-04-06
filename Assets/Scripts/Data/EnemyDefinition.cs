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
        public Color SecondaryColor = new Color(0.25f, 0.28f, 0.33f);
        public Color AccentColor = new Color(0.94f, 0.87f, 0.31f);

        [Header("Motion")]
        [Range(0f, 0.5f)] public float HoverAmplitude = 0.1f;
        [Range(0.5f, 12f)] public float HoverFrequency = 3f;
        [Range(0f, 0.4f)] public float BobAmplitude = 0.05f;
        [Range(0.5f, 14f)] public float BobFrequency = 6f;
        [Range(0f, 12f)] public float LeanAmount = 4f;

        [Header("Surface")]
        [Range(0f, 1f)] public float SurfaceGlossiness = 0.25f;
        [Range(0f, 2f)] public float AccentEmission = 0.75f;

        [Header("Death FX")]
        [Range(0.5f, 8f)] public float DebrisBurstForce = 3f;
        [Range(30f, 720f)] public float DebrisSpinSpeed = 220f;
        [Range(0.2f, 1.4f)] public float DebrisLifetime = 0.5f;

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
