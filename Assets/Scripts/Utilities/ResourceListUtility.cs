using System.Collections.Generic;
using FortDefense.Data;
using UnityEngine;

namespace FortDefense.Utilities
{
    public static class ResourceListUtility
    {
        public static List<ResourceAmount> ScaleCosts(IList<ResourceAmount> amounts, float multiplier)
        {
            return Scale(amounts, multiplier, ScaleMode.Ceil);
        }

        public static List<ResourceAmount> ScaleProduction(IList<ResourceAmount> amounts, float multiplier)
        {
            return Scale(amounts, multiplier, ScaleMode.Round);
        }

        public static List<ResourceAmount> ScaleRefunds(IList<ResourceAmount> amounts, float multiplier)
        {
            return Scale(amounts, multiplier, ScaleMode.Floor);
        }

        public static void MergeInto(IDictionary<ResourceType, int> ledger, IList<ResourceAmount> amounts)
        {
            if (amounts == null)
            {
                return;
            }

            for (int index = 0; index < amounts.Count; index++)
            {
                if (!ledger.ContainsKey(amounts[index].Type))
                {
                    ledger[amounts[index].Type] = 0;
                }

                ledger[amounts[index].Type] += amounts[index].Amount;
            }
        }

        public static List<ResourceAmount> ToList(IDictionary<ResourceType, int> ledger)
        {
            List<ResourceAmount> amounts = new List<ResourceAmount>();

            foreach (KeyValuePair<ResourceType, int> pair in ledger)
            {
                if (pair.Value <= 0)
                {
                    continue;
                }

                amounts.Add(new ResourceAmount(pair.Key, pair.Value));
            }

            return amounts;
        }

        private static List<ResourceAmount> Scale(IList<ResourceAmount> amounts, float multiplier, ScaleMode scaleMode)
        {
            List<ResourceAmount> scaled = new List<ResourceAmount>();
            if (amounts == null)
            {
                return scaled;
            }

            for (int index = 0; index < amounts.Count; index++)
            {
                float scaledAmount = amounts[index].Amount * multiplier;
                int finalAmount;
                switch (scaleMode)
                {
                    case ScaleMode.Floor:
                        finalAmount = Mathf.FloorToInt(scaledAmount);
                        break;
                    case ScaleMode.Ceil:
                        finalAmount = Mathf.CeilToInt(scaledAmount);
                        break;
                    default:
                        finalAmount = Mathf.RoundToInt(scaledAmount);
                        break;
                }

                if (finalAmount > 0)
                {
                    scaled.Add(new ResourceAmount(amounts[index].Type, finalAmount));
                }
            }

            return scaled;
        }

        private enum ScaleMode
        {
            Floor,
            Round,
            Ceil
        }
    }
}
