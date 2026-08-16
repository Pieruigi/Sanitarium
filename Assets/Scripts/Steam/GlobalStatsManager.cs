using Steam;
using Steamworks;
using System;
using UnityEngine;

namespace Baloon
{
    public class GlobalStatsManager : MonoBehaviour
    {
        string engineStartedStat = "v2_log_start_engine";
        string firstGameStat = "v2_log_first_game";

        string waypointReachedStatPrefix = "v2_log_reached_";
        string demoCompletedStat = "v2_log_demo_completed";

        string blooderStatPrefix = "v2_blooder_sealed_";
        string waypointLeftStatPrefix = "v2_log_left_";

        bool owner = false;

         

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            //if(SteamFriends.GetPersonaName() == "Pierlu")
            //if(SteamStatsManager.Instance.Initialized)
            //{
            //    Debug.Log(SteamUser.GetSteamID());
            //    if("76561198052836073".Equals(SteamUser.GetSteamID().ToString()))
            //        owner = true;
            //}

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
            BlooderController.OnSealed += HandleOnBlooderSelead;
            //BaloonWaypoint.OnLeft += HandleOnLeft;

        }

        private void OnDisable()
        {
            BaloonControlPanel.OnStarted -= HandleOnEngineStarted;
            BaloonWaypoint.OnReached -= HandleOnReached;
            GameManager.OnDemoCompleted -= HandleOnDemoCompleted;
            BlooderController.OnSealed -= HandleOnBlooderSelead;
            //BaloonWaypoint.OnLeft -= HandleOnLeft;
        }

        private void HandleOnLeft(BaloonWaypoint waypoint)
        {
            if (!waypoint.CompareTag("Log")) return;

            string wName = waypoint.gameObject.name;
            wName = wName.Split("-")[0];

            UpdateGlobalStatsLog(waypointLeftStatPrefix + wName);
        }

        private void HandleOnBlooderSelead(BlooderController blooderController)
        {
            string s = blooderController.transform.parent.gameObject.name;
            UpdateGlobalStatsLog(blooderStatPrefix + s);
        }

        private void HandleOnDemoCompleted()
        {
            UpdateGlobalStatsLog(demoCompletedStat);
        }

        private void HandleOnReached(BaloonWaypoint waypoint)
        {
            if (!waypoint.CompareTag("Log")) return;

            var g = waypoint.gameObject;
            if(!g.name.Contains("-"))
                g = waypoint.transform.parent.gameObject;

            string wName = g.name;
            wName = wName.Split("-")[0];

            //if("B3".Equals(wName))
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
            SteamStatsManager.Instance.DebugStat(demoCompletedStat);
            SteamStatsManager.Instance.DebugStat(waypointReachedStatPrefix + "D1");
            SteamStatsManager.Instance.DebugStat(waypointReachedStatPrefix + "B3");
            SteamStatsManager.Instance.DebugStat(waypointReachedStatPrefix + "A7");
            SteamStatsManager.Instance.DebugStat(blooderStatPrefix + "D1");
            SteamStatsManager.Instance.DebugStat(blooderStatPrefix + "A7");
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
