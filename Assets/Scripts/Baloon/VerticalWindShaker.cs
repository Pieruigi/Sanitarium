using UnityEngine;

namespace Baloon
{
    public class VerticalWindShaker : MonoBehaviour
    {
        VerticalWind wind;

        bool shaking = false;

        private void Awake()
        {
            wind = GetComponent<VerticalWind>();
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        
        void LateUpdate()
        {
            if(wind.Running && !shaking)
            {
                shaking = true;
                float randDuration = Random.Range(3.2f * 2f, 4.0f * 2f);
                CameraShake.Instance.PlayVerticalWindShake(randDuration, () => { shaking = false; }, () => { shaking = false; });
                //BaloonShaker.Instance.VerticalWindShake(randDuration*.85f);
            }
        }
    }
}