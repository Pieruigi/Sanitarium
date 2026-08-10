using Steam;
using Steamworks;
using System;
using UnityEngine;

namespace Baloon
{
    public class GlobalStatsManager : MonoBehaviour
    {
        string engineStartedStat = "log_start_engine";
        string firstGameStat = "log_first_game";

        string waypointReachedStatPrefix = "log_reached_";
        string demoCompletedStat = "log_demo_completed";

        bool owner = false;

         

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            //if(SteamFriends.GetPersonaName() == "Pierlu")
            if(SteamStatsManager.Instance.Initialized)
            {
                Debug.Log(SteamUser.GetSteamID());
                if("76561198052836073".Equals(SteamUser.GetSteamID().ToString()))
                    owner = true;
            }

#if UNITY_EDITOR
            //owner = false;
                        
#endif

            DebugAllGlobalStats();

            UpdateGlobalStatsLog(firstGameStat);

            //DebugAllGlobalStats();


        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnEnable()
        {
            BaloonControlPanel.OnStarted += HandleOnEngineStarted;
            BaloonWaypoint.OnReached += HandleOnReached;
            GameManager.OnDemoCompleted += HandleOnDemoCompleted;
        }

        private void OnDisable()
        {
            BaloonControlPanel.OnStarted -= HandleOnEngineStarted;
            BaloonWaypoint.OnReached -= HandleOnReached;
            GameManager.OnDemoCompleted -= HandleOnDemoCompleted;
        }

        private void HandleOnDemoCompleted()
        {
            UpdateGlobalStatsLog(demoCompletedStat);
        }

        private void HandleOnReached(BaloonWaypoint waypoint)
        {
            if (!waypoint.CompareTag("Log")) return;

            string wName = waypoint.gameObject.name;
            wName = wName.Split("-")[0];

            UpdateGlobalStatsLog(waypointReachedStatPrefix + wName);
        }

        private void HandleOnEngineStarted()
        {
            UpdateGlobalStatsLog(engineStartedStat);

           
        }

        void DebugAllGlobalStats()
        {

            SteamStatsManager.Instance.DebugStat(engineStartedStat);
            SteamStatsManager.Instance.DebugStat(firstGameStat);
            SteamStatsManager.Instance.DebugStat(waypointReachedStatPrefix + "D0");
            SteamStatsManager.Instance.DebugStat(waypointReachedStatPrefix + "C1");
            SteamStatsManager.Instance.DebugStat(waypointReachedStatPrefix + "D1");
            SteamStatsManager.Instance.DebugStat(waypointReachedStatPrefix + "B3");
            SteamStatsManager.Instance.DebugStat(waypointReachedStatPrefix + "A4");
            SteamStatsManager.Instance.DebugStat(waypointReachedStatPrefix + "A3");
            SteamStatsManager.Instance.DebugStat(waypointReachedStatPrefix + "B6");
            SteamStatsManager.Instance.DebugStat(waypointReachedStatPrefix + "B8");
            SteamStatsManager.Instance.DebugStat(waypointReachedStatPrefix + "A7");
        }

        void UpdateGlobalStatsLog(string statName)
        {
            if (owner) return;

            //if (PlayerPrefs.HasKey(statName)) return;

            if (!SteamStatsManager.Instance.TryGetStat(statName, out var value)) return;

            if (value > 0) return; // Already set

            if (SteamStatsManager.Instance.TryIncrementStat(statName, 1))
            {
                //PlayerPrefs.SetInt(statName, 1);
                //PlayerPrefs.Save();
            }

            SteamStatsManager.Instance.DebugStat(statName);
        }
    }

}
