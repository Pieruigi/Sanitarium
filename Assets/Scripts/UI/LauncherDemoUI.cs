using System.Collections;
using UnityEngine;

namespace Baloon.UI
{
    public class LauncherDemoUI : Singleton<LauncherDemoUI>
    {
        [SerializeField]
        GameObject root;

        protected override void Awake()
        {
            base.Awake();
            
#if !DEMO
            Destroy(gameObject);
#endif
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            root.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void ShowMessage()
        {
            if (root.activeSelf) return;

            StartCoroutine(ShowAndHide());

            IEnumerator ShowAndHide()
            {
                root.SetActive(true);

                yield return new WaitForSeconds(2f);

                root.SetActive(false);
            }
        }
    }
}