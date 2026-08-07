using Steamworks;
using UnityEngine;

namespace Steam
{
    public class SteamStatsManager : SingletonPersistent<SteamStatsManager>
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
            //{
            //    DebugStat("test");
            //    SteamUserStats.SetStat("test", 1);
            //    SteamUserStats.StoreStats();
            //    //TrySetStat("test", 1);
            //    DebugStat("test");
            //}
            if (Input.GetKeyDown(KeyCode.R))
            {
                ResetAllStats();
            }
#endif
        }

        #region util

        void ResetAllStats()
        {
            if (!Initialized) return;

            SteamUserStats.ResetAllStats(false);
            SteamUserStats.StoreStats();
            Debug.Log("Stats reset completed.");
        }

        public void DebugStat(string name)
        {
#if UNITY_EDITOR
            TryGetStat(name, out int currentValue);
#endif
        }

        #endregion



        bool TryGetStat(string statName, out int value)
        {
            value = 0;

            if (!Initialized) return false;

            bool success = SteamUserStats.GetStat(statName, out value);
            if (!success)
                Debug.LogError($"Stat '{statName}' not found.");
            else
                Debug.Log($"Stat '{statName}':{value}");
            
            return success;
        }

        bool TrySetStat(string statName, int value)
        {
            if (!Initialized) return false;

            bool success = SteamUserStats.SetStat(statName, value);
            if (success)
            {
                success = SteamUserStats.StoreStats();
                Debug.Log($"Stat '{statName}' updated to {value}");
            }
            else
            {
                Debug.LogError($"Failed to update stat '{statName}' to {value}");
            }
           
            return success;
        }

        public bool TryIncrementStat(string name, int amount = 1)
        {
            if (!Initialized) return false;

            bool success = TryGetStat(name, out int currentValue);

            if (success)
                success = TrySetStat(name, currentValue+amount);
            
            return success;



        }
       
       
    }
}