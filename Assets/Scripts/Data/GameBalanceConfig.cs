using System.Collections.Generic;
using UnityEngine;

namespace FortDefense.Data
{
    [CreateAssetMenu(menuName = "Fort Defense/Game Balance", fileName = "GameBalanceConfig")]
    public class GameBalanceConfig : ScriptableObject
    {
        public int PrototypeVersion = 2;
        public int StartingCoreHealth = 25;
        public List<ResourceAmount> StartingResources = new List<ResourceAmount>();
    }
}

