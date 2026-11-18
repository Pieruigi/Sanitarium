using Baloon.SaveSystem;
using StarterAssets;
using Steam;
using System;
using UnityEngine;

namespace Baloon
{
    public class BaloonAchievements : MonoBehaviour
    {

        bool firstTakeOff = false;

        bool refueled = false;

        

        class Data
        {
            public bool refueled;
        }

        string saveId = "ach_data";

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            if (!SteamAchievementManager.Instance) Destroy(gameObject);
        }

        // Update is called once per frame
        void Update()
        {
            var rawData = SaveManager.Instance.GetRawJsonData(saveId);
            if (!string.IsNullOrEmpty(rawData))
            {
                var data = JsonUtility.FromJson<Data>(rawData);
                refueled = data.refueled;
            }
        }

        private void OnEnable()
        {
            BaloonPathManager.OnPathSet += HandleOnPathSet;
            FirstPersonController.OnDead += HandleOnPlayerDead;
            BoilerController.OnGasRefueled += HandleOnRefueled;
            BaloonBoilerHealth.OnDamageTaken += HandleOnDamageTaken;
            SaveManager.OnUpdateDataEntry += HandleOnUpdateDataEntry;
            VoidmawController.OnGotRidOf += HandleOnVoidmawGotRidOf;
            BigVoidmawController.OnGotRidOf += HandleOnBigVoidmawGotRidOf;
            CarrionFlyController.OnGotRidOf += HandleOnHornetGotRidOf;
        }

        private void OnDisable()
        {
            BaloonPathManager.OnPathSet -= HandleOnPathSet;
            FirstPersonController.OnDead -= HandleOnPlayerDead;
            BoilerController.OnGasRefueled -= HandleOnRefueled;
            BaloonBoilerHealth.OnDamageTaken -= HandleOnDamageTaken;
            SaveManager.OnUpdateDataEntry -= HandleOnUpdateDataEntry;
            VoidmawController.OnGotRidOf -= HandleOnVoidmawGotRidOf;
            BigVoidmawController.OnGotRidOf -= HandleOnBigVoidmawGotRidOf;
            CarrionFlyController.OnGotRidOf -= HandleOnHornetGotRidOf;
        }

        private void HandleOnHornetGotRidOf()
        {
            SteamAchievementManager.Instance.UnlockAchievement("HORNET");
        }

        private void HandleOnBigVoidmawGotRidOf()
        {
            SteamAchievementManager.Instance.UnlockAchievement("TOO_HEAVY");
        }

        private void HandleOnVoidmawGotRidOf()
        {
            SteamAchievementManager.Instance.UnlockAchievement("HIT_VOIDMAW");
        }

        private void HandleOnUpdateDataEntry()
        {
            var data = new Data();
            data.refueled = refueled;
            SaveManager.Instance.CreateOrUpdateDataEntry(saveId, JsonUtility.ToJson(data));
        }

        private void HandleOnDamageTaken(float oldHealth, float newHealth)
        {
            if (newHealth <= 1.0f - 3 * BaloonBoilerHealth.DamageStep)
                SteamAchievementManager.Instance.UnlockAchievement("DAMAGER");
        }

        private void HandleOnRefueled()
        {
            refueled = true; 
        }

        private void HandleOnPlayerDead(PlayerDeadType deadType)
        {
            switch (deadType)
            {
                case PlayerDeadType.GameEndingDrown:
                    string achName = "COMPLETED";
                    if (SettingsManager.Instance.GameMode == 0)
                        achName += "_NO_DIE";

                    SteamAchievementManager.Instance.UnlockAchievement(achName);

                    if(!refueled)
                        SteamAchievementManager.Instance.UnlockAchievement("COMPLETE_NO_GAS");
                    break;

                case PlayerDeadType.KillerWind:
                    SteamAchievementManager.Instance.UnlockAchievement("TENTACLE");
                    break;
            }
        }

        private void HandleOnPathSet()
        {
            if (firstTakeOff) return;
            firstTakeOff = true;

            SteamAchievementManager.Instance.UnlockAchievement("TAKE_OFF");
        }

        
    }
}