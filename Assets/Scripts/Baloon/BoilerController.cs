using UnityEngine;

namespace Baloon
{
    public class BoilerController : Singleton<BoilerController>
    {

        float power = 0;
        public float Power => power;

        float[] maxPowers = new float[] { .7f, 1f };

        int version = 0;

        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
#if UNITY_EDITOR
            if (Input.GetKey(KeyCode.X))
            {
                power = maxPowers[version];
            }
            else
            {
                power = 0;
            }
#endif
        }


    }

}
