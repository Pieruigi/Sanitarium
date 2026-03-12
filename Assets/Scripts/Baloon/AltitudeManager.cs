using UnityEngine;

namespace Baloon
{
    public class AltitudeManager : Singleton<AltitudeManager>
    {

        [SerializeField]
        float minAltitude = 20;
        public float MinAltitude => minAltitude;

        [SerializeField]
        float maxAltitude = 80;
        public float MaxAltitude => maxAltitude;    
        

        // Instead of having a specific time, we set a sort of difficulty level to better handle it
        //int tolleranceLevel = 0;



        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        //public float GetTolleranceTime()
        //{
        //    switch(tolleranceLevel) 
        //    {
        //        case 0: 
        //    }
        //}

        
    }
}