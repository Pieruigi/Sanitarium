using UnityEngine;

namespace Baloon.SaveSystem
{
    public interface ISavable
    {
        // Unique identifier for this specific save slot/object (e.g., "Player", "Inventory")
        string SaveID { get; }

        // Converts the internal data class into a JSON string
        string GenerateSaveData();

        // Restores the internal data from a JSON string
        void LoadSaveData(string jsonData);
    }
}