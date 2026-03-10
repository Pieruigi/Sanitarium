using StarterAssets;
using UnityEngine;

namespace Baloon
{
    public class InputManager : Singleton<InputManager>
    {


        StarterAssetsInputs input;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            input = FindFirstObjectByType<StarterAssetsInputs>();
        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}
