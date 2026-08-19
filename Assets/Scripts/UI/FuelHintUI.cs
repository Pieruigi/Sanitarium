using System;
using System.Collections;
using UnityEngine;

namespace Baloon.UI
{
    public class FuelHintUI : MonoBehaviour
    {
        [SerializeField]
        BaloonWaypoint waypoint;

        [SerializeField]
        GameObject text;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            text.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnEnable()
        {
            NavigationSystem.OnDestinationReached += HandleOnWaypointReached;
            BasePlatform.OnLanding += HandleOnLanding;
            BaloonPathManager.OnPathSet += HandleOnPathSet;
        }

        private void OnDisable()
        {
            NavigationSystem.OnDestinationReached -= HandleOnWaypointReached;
            BasePlatform.OnLanding -= HandleOnLanding;
            BaloonPathManager.OnPathSet -= HandleOnPathSet;
        }

        private void HandleOnPathSet()
        {
            if (text.activeSelf) text.SetActive(false);
        }

        private void HandleOnLanding(BasePlatform platform)
        {
            HandleOnPathSet();
        }

        private void HandleOnWaypointReached(BaloonWaypoint waypoint)
        {
            if (this.waypoint != waypoint) return;

            StartCoroutine(ShowHint());

            IEnumerator ShowHint()
            {
                yield return new WaitForSeconds(1f);

                text.SetActive(true);

                //yield return new WaitForSeconds(3f);

                //text.SetActive(false);
            }
        }
    }

}
