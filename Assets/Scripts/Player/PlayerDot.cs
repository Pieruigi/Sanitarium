using UnityEngine;
using UnityEngine.UI;

namespace SNT.UI
{
    public class PlayerDot : Singleton<PlayerDot>
    {
        [SerializeField]
        Image dot;



        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            //ShowDot(true);
        }

        // Update is called once per frame
        void Update()
        {
            if(!dot.enabled) return;

            var pos = Input.mousePosition;
            dot.rectTransform.position = pos;
        }

        public void ShowDot(bool value)
        {
            if (dot.enabled == value) return;

            dot.enabled = value;
        }
    }
}