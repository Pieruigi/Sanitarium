using UnityEngine;

namespace Baloon
{

    public class SurfaceTypeData : MonoBehaviour
    {
        [SerializeField]
        SurfaceType type = SurfaceType.Concrete;

        public SurfaceType Type => type;

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