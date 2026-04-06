using UnityEngine;

namespace FortDefense.Utilities
{
    public static class PrimitiveFactory
    {
        public static GameObject CreatePrimitive(
            string name,
            PrimitiveType primitiveType,
            Transform parent,
            Vector3 localPosition,
            Vector3 localScale,
            Color color,
            float glossiness,
            float emissionIntensity,
            bool enableCollider = false)
        {
            return CreatePrimitive(name, primitiveType, parent, localPosition, Quaternion.identity, localScale, color, glossiness, emissionIntensity, enableCollider);
        }

        public static GameObject CreatePrimitive(
            string name,
            PrimitiveType primitiveType,
            Transform parent,
            Vector3 localPosition,
            Vector3 localScale,
            Color color,
            bool enableCollider = false)
        {
            return CreatePrimitive(name, primitiveType, parent, localPosition, Quaternion.identity, localScale, color, 0.08f, 0f, enableCollider);
        }

        public static GameObject CreatePrimitive(
            string name,
            PrimitiveType primitiveType,
            Transform parent,
            Vector3 localPosition,
            Quaternion localRotation,
            Vector3 localScale,
            Color color,
            float glossiness,
            float emissionIntensity,
            bool enableCollider = false)
        {
            GameObject instance = GameObject.CreatePrimitive(primitiveType);
            instance.name = name;
            instance.transform.SetParent(parent, false);
            instance.transform.localPosition = localPosition;
            instance.transform.localRotation = localRotation;
            instance.transform.localScale = localScale;

            Renderer renderer = instance.GetComponent<Renderer>();
            if (renderer != null)
            {
                Material material = renderer.material;
                material.color = color;

                if (material.HasProperty("_Glossiness"))
                {
                    material.SetFloat("_Glossiness", glossiness);
                }

                if (material.HasProperty("_EmissionColor"))
                {
                    Color emissionColor = color * emissionIntensity;
                    material.SetColor("_EmissionColor", emissionColor);
                    if (emissionIntensity > 0.001f)
                    {
                        material.EnableKeyword("_EMISSION");
                    }
                    else
                    {
                        material.DisableKeyword("_EMISSION");
                    }
                }
            }

            Collider collider = instance.GetComponent<Collider>();
            if (collider != null)
            {
                collider.enabled = enableCollider;
            }

            return instance;
        }

        public static GameObject CreatePrimitive(
            string name,
            PrimitiveType primitiveType,
            Transform parent,
            Vector3 localPosition,
            Quaternion localRotation,
            Vector3 localScale,
            Color color,
            bool enableCollider = false)
        {
            return CreatePrimitive(name, primitiveType, parent, localPosition, localRotation, localScale, color, 0.08f, 0f, enableCollider);
        }
    }
}
