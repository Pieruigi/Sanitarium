using UnityEngine;
using UnityEngine.UI;

namespace Baloon.UI
{
    public class UrlButton : MonoBehaviour
    {
        [SerializeField]
        string url;

        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(() => { Application.OpenURL(url); });
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}