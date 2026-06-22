using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace Baloon.SaveSystem
{
    

    [System.Serializable]
    public class SaveEntry
    {
        public string id;
        public string rawJsonData; // The nested JSON data stored as a simple string

        public SaveEntry(string id)
        {
            this.id = id;
        }
    }

    [System.Serializable]
    public class MasterSaveData
    {
        public System.Collections.Generic.List<SaveEntry> entries = new System.Collections.Generic.List<SaveEntry>();
    }

    public class SaveManager : SingletonPersistent<SaveManager> 
    {
        public static UnityAction OnUpdateDataEntry;

        string fileName = "save.json";

        //string cache = "";

        string filePath = null;

        MasterSaveData masterSaveData;// = new MasterSaveData();


        protected override void Awake()
        {
            base.Awake();
            filePath = Path.Combine(Application.persistentDataPath, fileName);

#if UNITY_EDITOR
            Load();
#endif
        } 
            
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
#if UNITY_EDITOR
            //cache = "{\r\n  \"code\": \"PLAYER_SAVE_01\",\r\n  \"position\": {\r\n    \"x\": 12.5,\r\n    \"y\": -4.5,\r\n    \"z\": 87.2\r\n  },\r\n  \"rotation\": {\r\n    \"x\": 0.0,\r\n    \"y\": 0.7071,\r\n    \"z\": 0.0,\r\n    \"w\": 0.7071\r\n  }\r\n}";
#endif

        }

        // Update is called once per frame
        void Update()
        {
#if UNITY_EDITOR
            //if (Input.GetKeyDown(KeyCode.X))
            //{
            //    LoadFile();

            //}
            if (Input.GetKeyDown(KeyCode.X))
            {
                Save();
                //Debug.Log(cache);
            }
            //if (Input.GetKeyDown(KeyCode.V))
            //{
            //    DeleteFile();
            //    //Debug.Log(cache);
            //}
#endif
        }


        void DeleteFile()
        {
            if(File.Exists(filePath)) File.Delete(filePath);
        }

        public void Save()
        {
            

            // Call event to let savable object to update the masterSaveData using CreateOrUpdateDataEntry()
            OnUpdateDataEntry?.Invoke();

            if (masterSaveData == null) return;

            // Create json
            string json = JsonUtility.ToJson(masterSaveData, true);
            File.WriteAllText(filePath, json);
        }

        public void CreateOrUpdateDataEntry(string id, string rawJsonData)
        {
            if(masterSaveData == null) masterSaveData = new MasterSaveData();

            var entry = masterSaveData.entries.Find(e=>e.id == id);
            if (entry == null)
            {
                entry = new SaveEntry(id);
                masterSaveData.entries.Add(entry);
            }
                

            entry.rawJsonData = rawJsonData;
        }

        public void Load()
        {
            // Clear data
            masterSaveData = null;

            if (!File.Exists(filePath)) return;

            // Load file
            var json = File.ReadAllText(filePath);

            // Create entries
            masterSaveData = JsonUtility.FromJson<MasterSaveData>(json);
        }

        public void Delete()
        {
            masterSaveData = null;
            File.Delete(filePath);
        }

        public bool DataEntryExists(string id)
        {
            if (masterSaveData == null) return false;
            
            return masterSaveData.entries.Exists(e=>e.id == id);    
        }

        public string GetRawJsonData(string dataEntryId)
        {
            if(!DataEntryExists(dataEntryId)) return null;

            return masterSaveData.entries.Find(e=>e.id== dataEntryId).rawJsonData;
        }

        public bool SaveFileExists()
        {
            return File.Exists(filePath);
        }
    }
}