using System.Collections.Generic;
using System.Text;
using FortDefense.Data;

namespace FortDefense.Utilities
{
    public static class ResourceFormatting
    {
        public static string FormatCost(IList<ResourceAmount> amounts)
        {
            if (amounts == null || amounts.Count == 0)
            {
                return "Free";
            }

            StringBuilder builder = new StringBuilder();

            for (int index = 0; index < amounts.Count; index++)
            {
                if (index > 0)
                {
                    builder.Append("  ");
                }

                builder.Append(amounts[index].Amount);
                builder.Append(" ");
                builder.Append(GetShortLabel(amounts[index].Type));
            }

            return builder.ToString();
        }

        public static string GetShortLabel(ResourceType resourceType)
        {
            switch (resourceType)
            {
                case ResourceType.Ore:
                    return "Ore";
                case ResourceType.Energy:
                    return "Energy";
                case ResourceType.Alloy:
                    return "Alloy";
                case ResourceType.Ammo:
                    return "Ammo";
                default:
                    return resourceType.ToString();
            }
        }
    }
}
