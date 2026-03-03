using UnityEngine;

namespace Baloon
{
    public class BoilerController : Singleton<BoilerController>
    {
        [SerializeField]
        [Range(0, 1f)]
        float power = 0;
        public float Power => power;

        float[] maxPowers = new float[] { 1f, 1.5f };

        int version = 0;

        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
#if UNITY_EDITOR
            //if (Input.GetKey(KeyCode.X))
            //{
            //    power = maxPowers[version];
            //}
            //else
            //{
            //    power = 0;
            //}
#endif
        }


    }

}
