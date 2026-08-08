using Steamworks;
using UnityEngine;


public class GlobalStatsReader : MonoBehaviour
{
    private CallResult<GlobalStatsReceived_t> m_GlobalStatsReceived;

    private void Awake()
    {
        if (!SteamManager.Initialized) return;

        // Initialize the CallResult callback wrapper
        m_GlobalStatsReceived = CallResult<GlobalStatsReceived_t>.Create(OnGlobalStatsReceived);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ReadGlobalStats();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ReadGlobalStats()
    {
        if (!SteamManager.Initialized) return;

        SteamAPICall_t handle = SteamUserStats.RequestGlobalStats(0);
        m_GlobalStatsReceived.Set(handle);
    }

    private void OnGlobalStatsReceived(GlobalStatsReceived_t pCallback, bool bIOFailure)
    {
        if (bIOFailure || pCallback.m_eResult != EResult.k_EResultOK)
        {
            Debug.LogError("Failed to receive global stats. Result: " + pCallback.m_eResult);
            return;
        }

        Debug.Log("Global stats updated successfully!");

        // Read specific global statistics
        ReadExampleStats();
    }

    private void ReadExampleStats()
    {
        // Reading a long/int global stat
        if (SteamUserStats.GetGlobalStat("log_start_engine", out long totalGames))
        {
            Debug.Log($"First engine start: {totalGames}");
        }

        // Reading a double/float global stat
        if (SteamUserStats.GetGlobalStat("log_first_game", out long totalDistance))
        {
            Debug.Log($"First game start: {totalDistance}");
        }

        // Reading a double/float global stat
        if (SteamUserStats.GetGlobalStat("log_reached_D0", out long d0))
        {
            Debug.Log($"Reached D0: {d0}");
        }

        // Reading a double/float global stat
        if (SteamUserStats.GetGlobalStat("log_reached_C1", out long c1))
        {
            Debug.Log($"Reached C1: {c1}");
        }

        // Reading a double/float global stat
        if (SteamUserStats.GetGlobalStat("log_reached_D1", out long d1))
        {
            Debug.Log($"Reached D1: {d1}");
        }

        // Reading a double/float global stat
        if (SteamUserStats.GetGlobalStat("log_reached_B3", out long b3))
        {
            Debug.Log($"Reached B3: {b3}");
        }

        // Reading a double/float global stat
        if (SteamUserStats.GetGlobalStat("log_reached_A4", out long a4))
        {
            Debug.Log($"Reached A4: {a4}");
        }

        // Reading a double/float global stat
        if (SteamUserStats.GetGlobalStat("log_reached_A3", out long a3))
        {
            Debug.Log($"Reached A3: {a3}");
        }

        // Reading a double/float global stat
        if (SteamUserStats.GetGlobalStat("log_reached_B6", out long b6))
        {
            Debug.Log($"Reached B6: {b6}");
        }

        // Reading a double/float global stat
        if (SteamUserStats.GetGlobalStat("log_reached_B8", out long b8))
        {
            Debug.Log($"Reached B8: {b8}");
        }

        // Reading a double/float global stat
        if (SteamUserStats.GetGlobalStat("log_reached_A7", out long a7))
        {
            Debug.Log($"Reached A7: {a7}");
        }

        // Reading a double/float global stat
        if (SteamUserStats.GetGlobalStat("log_demo_completed", out long dc))
        {
            Debug.Log($"Demo completed: {dc}");
        }
    }
}
