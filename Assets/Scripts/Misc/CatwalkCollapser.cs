using Baloon.SaveSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Baloon
{
    public class CatwalkCollapser : MonoBehaviour
    {
        [SerializeField]
        float duration = 26f;

        [SerializeField]
        float inTime = 2f; 

        [SerializeField]
        float outTime = 1f;

        [SerializeField]
        List<Rigidbody> rigidbodies;

        [SerializeField]
        AudioSource audioSource;

        bool triggered = false;

        [SerializeField]
        string saveId;

        class Data
        {
            [System.Serializable]
            public class Element
            {
                public Vector3 position;
                public Quaternion rotation;
            }

            public Data()
            {
                elements = new List<Element>();
            }

            public bool triggered;

            public List<Element> elements;
        }

        private void Awake()
        {
            foreach(var rigidbody in rigidbodies)
                rigidbody.isKinematic = true;
            
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            string rawData = SaveManager.Instance.GetRawJsonData(saveId);
            if (!string.IsNullOrEmpty(rawData))
            {
                var data = JsonUtility.FromJson<Data>(rawData);
                triggered = data.triggered;

                if (triggered)
                {
                    for(int i=0; i<data.elements.Count; i++)
                    {
                        var element = data.elements[i];
                        var rb = rigidbodies[i];
                        rb.isKinematic = true;
                        rb.transform.position = element.position;
                        rb.transform.rotation = element.rotation;
                    }
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
#if UNITY_EDITOR

            //if (Input.GetKeyDown(KeyCode.X))
            //{
            //    Play();
            //}
#endif
        }

        private void OnEnable()
        {
            SaveManager.OnUpdateDataEntry += HandleOnUpdateDataEntry;
        }

        private void OnDisable()
        {
            SaveManager.OnUpdateDataEntry -= HandleOnUpdateDataEntry;
        }

        private void HandleOnUpdateDataEntry()
        {
            var data = new Data();
            data.triggered = triggered;

            if (triggered)
            {
                foreach(var rb in rigidbodies)
                {
                    Data.Element element = new Data.Element();
                    element.position = rb.transform.position;
                    element.rotation = rb.transform.rotation;
                    data.elements.Add(element);
                }
            }

            SaveManager.Instance.CreateOrUpdateDataEntry(saveId, JsonUtility.ToJson(data));
        }

        public void Play()
        {
            if(triggered) return;
            triggered = true;

            // Start camera shake
            CameraShake.Instance.PlayCatwalkCollapseShake(duration);

            // Start destroying the catwalk
            StartCoroutine(DoCollapse());

            IEnumerator DoCollapse()
            {

                audioSource.Play();

                var time = duration - inTime - outTime;

                yield return new WaitForSeconds(inTime);

                float step = time / (float)rigidbodies.Count;
                int count = 0;

                do
                {
                    yield return new WaitForSeconds(step);

                   
                    var r = rigidbodies[count];
                    r.GetComponent<MeshCollider>().convex = true;
                    r.isKinematic = false;


                    count++;

                }
                while (count < rigidbodies.Count);
                
                yield return new WaitForSeconds(outTime);

                audioSource.Stop();

            }
        }
    }


}