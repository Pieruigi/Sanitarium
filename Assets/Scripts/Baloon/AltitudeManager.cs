using UnityEngine;

namespace Baloon
{
    public enum AltitudeRange { Red, Yellow, Green }

    public class AltitudeManager : Singleton<AltitudeManager>
    {

        [SerializeField]
        float minAltitude = 30;
        public float MinAltitude => minAltitude;

        [SerializeField]
        float maxAltitude = 50;
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

        public void SetAltitude(float minAltitude, float maxAltitude)
        {
            this.minAltitude = minAltitude;
            this.maxAltitude = maxAltitude;
        }

        public AltitudeRange GetCurrentRange()
        {
            var currentAltitude = BaloonController.Instance.Altitude;
            var middleAltitude = (maxAltitude + minAltitude) / 2f;
            var range = (maxAltitude - minAltitude);
            var greenAmount = range * .4f;
            var greenMax = middleAltitude + greenAmount / 2f;
            var greenMin = middleAltitude - greenAmount / 2f;

            if (currentAltitude < minAltitude || currentAltitude > maxAltitude) // Out of range
            {
                return AltitudeRange.Red;
            }
            else // In range
            {
                if (currentAltitude < greenMin || currentAltitude > greenMax)
                {
                    return AltitudeRange.Yellow;
                }
                else
                {
                    return AltitudeRange.Green;
                }
            }
        }
    }
}