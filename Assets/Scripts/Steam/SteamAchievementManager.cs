using Steamworks;
using UnityEngine;

namespace Steam
{
    public class SteamAchievementManager : SingletonPersistent<SteamAchievementManager>
    {
        public bool Initialized
        {
            get
            {
                if (!SteamManager.Initialized)
                {
                    Debug.LogWarning("Steam Not Initialized.");
                    return false;
                }
                return true;
            }
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
#if UNITY_EDITOR
            //if (Input.GetKeyDown(KeyCode.X))
            //    UnlockAchievement("alog_start_engine");
#endif
        }



        #region util
        // Reset achievement (per testing)
        void ResetAchievement(string achievementId)
        {
            if (!Initialized) return;

            SteamUserStats.ClearAchievement(achievementId);
            SteamUserStats.StoreStats();
            
        }

        void HardResetAchievements()
        {
            if (!Initialized) return;

            for (uint i = 0; i < SteamUserStats.GetNumAchievements(); i++)
            {
                string achievementId = SteamUserStats.GetAchievementName(i);
                SteamUserStats.ClearAchievement(achievementId);
            }
            SteamUserStats.StoreStats();

            
            SteamUserStats.ResetAllStats(true); // <-- 'true' it's important
            SteamAPI.RunCallbacks();

            Debug.Log("Hard reset completed");
        }

        public void DebugAllAchievements()
        {
            if (!Initialized) return;

            uint numAchievements = SteamUserStats.GetNumAchievements();
            //Debug.Log($"Numero achievement trovati: {numAchievements}");

            for (uint i = 0; i < numAchievements; i++)
            {
                string achievementId = SteamUserStats.GetAchievementName(i);
                bool achieved = SteamUserStats.GetAchievement(achievementId, out achieved);

                Debug.Log($"Achievement [{i}]: {achievementId} - Status: {achieved}");
            }
        }
        #endregion


        public void UnlockAchievement(string achievementId)
        {
            if (!Initialized) return;

            bool success = SteamUserStats.SetAchievement(achievementId);

            if (success)
            {
                SteamUserStats.StoreStats();
                
            }
            else
            {
                Debug.LogError($"Failed unlocking the achievement: {achievementId}");
            }

            DebugAllAchievements();
        }
    }
}