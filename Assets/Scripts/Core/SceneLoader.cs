using UnityEngine.SceneManagement;

namespace FortDefense.Core
{
    public static class SceneLoader
    {
        public static void LoadMainMenu()
        {
            SceneManager.LoadScene(PrototypeNames.MainMenuSceneName);
        }

        public static void LoadBattle()
        {
            SceneManager.LoadScene(PrototypeNames.BattleSceneName);
        }
    }
}

