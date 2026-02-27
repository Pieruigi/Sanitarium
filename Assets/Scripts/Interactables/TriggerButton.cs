using SNT.Interfaces;
using System.Collections;
using UnityEngine;

namespace SNT
{
    public class TriggerButton : MonoBehaviour, IInteractable
    {
        public delegate void TriggeredDelegate(TriggerButton triggerButton);
        public static TriggeredDelegate OnTriggered;

        bool triggered = false;

        float cooldown = .5f;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void StartInteraction()
        {
            if (triggered) return;

            triggered = true;

            StartCoroutine(Cooldown());

            OnTriggered?.Invoke(this);

            IEnumerator Cooldown()
            {
                yield return new WaitForSeconds(cooldown);
                triggered = false;
            }
        }

        public void StopInteraction()
        {
            // Nothing to do here since the button is a trigger, so it doesn't have an "off" state
        }

        public bool IsInteractable()
        {
            return !triggered;
        }
    }
}