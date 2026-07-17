using UnityEngine;
using UnityEngine.UI;

namespace Baloon.UI
{
    public class WishlistButton : MonoBehaviour
    {
        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(() => { Application.OpenURL("https://store.steampowered.com/app/4839050/Altitude_Zero/"); });
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
