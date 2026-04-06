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
            bool enableCollider = false)
        {
            return CreatePrimitive(name, primitiveType, parent, localPosition, Quaternion.identity, localScale, color, enableCollider);
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
            GameObject instance = GameObject.CreatePrimitive(primitiveType);
            instance.name = name;
            instance.transform.SetParent(parent, false);
            instance.transform.localPosition = localPosition;
            instance.transform.localRotation = localRotation;
            instance.transform.localScale = localScale;

            Renderer renderer = instance.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = color;
                renderer.material.SetFloat("_Glossiness", 0.08f);
            }

            Collider collider = instance.GetComponent<Collider>();
            if (collider != null)
            {
                collider.enabled = enableCollider;
            }

            return instance;
        }
    }
}

