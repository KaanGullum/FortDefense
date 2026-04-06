using System;
using System.Collections.Generic;
using FortDefense.Data;
using UnityEngine;

namespace FortDefense.Gameplay
{
    public class ResourceBank : MonoBehaviour
    {
        public event Action ResourcesChanged;

        private readonly Dictionary<ResourceType, int> _resources = new Dictionary<ResourceType, int>();

        public void Initialize(IEnumerable<ResourceAmount> startingResources)
        {
            _resources.Clear();

            Array values = Enum.GetValues(typeof(ResourceType));
            for (int index = 0; index < values.Length; index++)
            {
                _resources[(ResourceType)values.GetValue(index)] = 0;
            }

            if (startingResources != null)
            {
                foreach (ResourceAmount amount in startingResources)
                {
                    Add(amount.Type, amount.Amount, false);
                }
            }

            ResourcesChanged?.Invoke();
        }

        public int GetAmount(ResourceType resourceType)
        {
            int amount;
            return _resources.TryGetValue(resourceType, out amount) ? amount : 0;
        }

        public bool CanAfford(IList<ResourceAmount> cost)
        {
            if (cost == null)
            {
                return true;
            }

            for (int index = 0; index < cost.Count; index++)
            {
                if (GetAmount(cost[index].Type) < cost[index].Amount)
                {
                    return false;
                }
            }

            return true;
        }

        public bool TrySpend(IList<ResourceAmount> cost)
        {
            if (!CanAfford(cost))
            {
                return false;
            }

            if (cost != null)
            {
                for (int index = 0; index < cost.Count; index++)
                {
                    _resources[cost[index].Type] -= cost[index].Amount;
                }
            }

            ResourcesChanged?.Invoke();
            return true;
        }

        public void AddRange(IList<ResourceAmount> producedResources)
        {
            if (producedResources == null)
            {
                return;
            }

            for (int index = 0; index < producedResources.Count; index++)
            {
                Add(producedResources[index].Type, producedResources[index].Amount, false);
            }

            ResourcesChanged?.Invoke();
        }

        public void Add(ResourceType resourceType, int amount)
        {
            Add(resourceType, amount, true);
        }

        private void Add(ResourceType resourceType, int amount, bool notify)
        {
            if (!_resources.ContainsKey(resourceType))
            {
                _resources[resourceType] = 0;
            }

            _resources[resourceType] += amount;

            if (notify)
            {
                ResourcesChanged?.Invoke();
            }
        }
    }
}

