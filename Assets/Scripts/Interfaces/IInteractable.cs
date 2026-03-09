using UnityEngine;

namespace SNT.Interfaces
{
    public interface IInteractable
    {
        void StartInteraction();

        void StopInteraction();

        bool IsInteractable();
    }
}
