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
        public static UnityAction OnUpdateMasterSaveData;

        string fileName = "save.json";

        //string cache = "";

        string filePath = null;

        MasterSaveData masterSaveData = new MasterSaveData();


        protected override void Awake()
        {
            base.Awake();
            filePath = Path.Combine(Application.persistentDataPath, fileName);
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
            if (Input.GetKeyDown(KeyCode.X))
            {
                LoadFile();
                
            }
            if (Input.GetKeyDown(KeyCode.C))
            {
                SaveFile();
                //Debug.Log(cache);
            }
            if (Input.GetKeyDown(KeyCode.V))
            {
                DeleteFile();
                //Debug.Log(cache);
            }
#endif
        }

        void LoadFile()
        {
            masterSaveData.entries.Clear();
            var cache = "";
            if (File.Exists(filePath))
            {
                cache = File.ReadAllText(filePath);
            }
                
        }


        void DeleteFile()
        {
            if(File.Exists(filePath)) File.Delete(filePath);
        }

        public void Save()
        {
         
            // Call event to let savable object to update the masterSaveData using CreateOrUpdateDataEntry()
            OnUpdateMasterSaveData?.Invoke();

            // Create json
            string json = JsonUtility.ToJson(masterSaveData, true);
            File.WriteAllText(filePath, json);
        }

        public void CreateOrUpdateDataEntry(string id, string rawJsonData)
        {
            var entry = masterSaveData.entries.Find(e=>e.id == id);
            if (entry == null)
                masterSaveData.entries.Add(new SaveEntry(id));

            entry.rawJsonData = rawJsonData;
        }

        
    }
}